using System.Collections.ObjectModel;
using System.Linq;
using KKOTPView.Configuration;
using KKOTPView.ViewModels;
using KKPinView.Constants;
using KKPinView.Debug;
using KKPinView.Helpers;
using KKPinView.Security;
using KKPinView.Storage;
using Microsoft.Maui.ApplicationModel;

namespace KKOTPView.Views;

/// <summary>
/// OTP entry view with fluent API for customization.
/// Use instance methods to configure, or set app-wide defaults via <see cref="OTPConfig.Configure"/>.
/// </summary>
/// <example>
/// <code>
/// var otpView = new KKOTPView()
///     .Length(6)
///     .Secure(false)
///     .FieldColors(filled: Colors.Green, invalid: Colors.Red)
///     .ResendText("Resend code")
///     .OnSubmit(isValid => HandleValidation(isValid));
/// </code>
/// </example>
public sealed partial class KKOTPView : ContentView, IDisposable
{
    private bool _disposed;
    private bool _fieldsInitialized;
    private readonly ObservableCollection<OTPPinDigitField> _pinFields = new();
    private readonly KKPinLockoutManager _lockoutManager;
    private readonly OTPConfiguration _config;
    private readonly OTPViewModel _viewModel;
    private readonly System.Windows.Input.ICommand _focusFirstEmptyCommand;
    private string _currentPin = string.Empty;
    private int _lastFocusedIndex = -1;

    /// <summary>Gets the ViewModel for binding.</summary>
    public OTPViewModel ViewModel => _viewModel;

    /// <summary>Creates a new OTP view with default or app-configured settings.</summary>
    public KKOTPView()
    {
        _config = OTPConfig.GetDefaults();
        _viewModel = new OTPViewModel(_config);
        _focusFirstEmptyCommand = new Command(FocusFirstEmptyField);

        InitializeComponent();
        BindingContext = _viewModel;

        _lockoutManager = new KKPinLockoutManager();

        Loaded -= OnPageLoaded;
        Loaded += OnPageLoaded;
    }

    #region Fluent API

    /// <summary>Sets the number of OTP digits (4, 6, 8). Must be called before the view is loaded.</summary>
    public KKOTPView Length(int length)
    {
        _config.Length = length;
        return this;
    }

    /// <summary>When true, digits are masked. When false (default for OTP), digits are visible.</summary>
    public KKOTPView Secure(bool isSecure = true)
    {
        _config.IsSecure = isSecure;
        return this;
    }

    /// <summary>Sets the view background color.</summary>
    public KKOTPView BackgroundColor(Color color)
    {
        _config.BackgroundColor = color;
        return this;
    }

    /// <summary>Sets label colors (text, error, success).</summary>
    public KKOTPView LabelColors(Color? textColor = null, Color? errorColor = null, Color? successColor = null)
    {
        if (textColor.HasValue) _config.TextColor = textColor.Value;
        if (errorColor.HasValue) _config.ErrorTextColor = errorColor.Value;
        if (successColor.HasValue) _config.SuccessTextColor = successColor.Value;
        return this;
    }

    /// <summary>Sets digit field border colors (filled, empty, invalid).</summary>
    public KKOTPView FieldColors(Color? filled = null, Color? empty = null, Color? invalid = null)
    {
        if (filled.HasValue) _config.FilledBorderColor = filled.Value;
        if (empty.HasValue) _config.EmptyBorderColor = empty.Value;
        if (invalid.HasValue) _config.InvalidBorderColor = invalid.Value;
        return this;
    }

    /// <summary>Sets digit field dimensions and spacing.</summary>
    public KKOTPView FieldSize(double width = 50, double height = 50, double spacing = 15)
    {
        _config.FieldWidth = width;
        _config.FieldHeight = height;
        _config.FieldSpacing = spacing;
        return this;
    }

    /// <summary>Sets digit field shape and corner radius.</summary>
    public KKOTPView FieldShape(KKPinFieldShapeType shape, double cornerRadius = 10)
    {
        _config.FieldShapeType = shape;
        _config.FieldCornerRadius = cornerRadius;
        return this;
    }

    /// <summary>Sets digit field font (size, attributes, family).</summary>
    public KKOTPView DigitFont(double fontSize = 16, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        _config.DigitFontSize = fontSize;
        _config.DigitFontAttributes = attributes;
        if (fontFamily != null) _config.DigitFontFamily = fontFamily;
        return this;
    }

    /// <summary>Sets label font for title, success/error messages, and resend button (size, attributes, family).</summary>
    public KKOTPView LabelFont(double titleSize = 18, double errorSize = 24, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        _config.TitleFontSize = titleSize;
        _config.ErrorMessageFontSize = errorSize;
        _config.TitleFontAttributes = attributes;
        _config.ErrorMessageFontAttributes = attributes;
        if (fontFamily != null)
        {
            _config.TitleFontFamily = fontFamily;
            _config.ErrorMessageFontFamily = fontFamily;
        }
        return this;
    }

    /// <summary>Sets the "Resend OTP" button text.</summary>
    public KKOTPView ResendText(string text)
    {
        _config.ResendText = text;
        return this;
    }

    /// <summary>Sets font for "Resend OTP" button (when enabled).</summary>
    public KKOTPView ResendButtonFont(double fontSize = 16, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        _config.ResendButtonFontSize = fontSize;
        _config.ResendButtonFontAttributes = attributes;
        if (fontFamily != null) _config.ResendButtonFontFamily = fontFamily;
        return this;
    }

    /// <summary>Sets font for countdown text ("Resend in Xs").</summary>
    public KKOTPView ResendCountdownFont(double fontSize = 16, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        _config.ResendCountdownFontSize = fontSize;
        _config.ResendCountdownFontAttributes = attributes;
        if (fontFamily != null) _config.ResendCountdownFontFamily = fontFamily;
        return this;
    }

    /// <summary>Shows or hides the resend button.</summary>
    public KKOTPView ShowResendButton(bool show = true)
    {
        _config.ShowResendButton = show;
        return this;
    }

    /// <summary>Sets custom message strings.</summary>
    public KKOTPView Messages(string? successMessage = null, string? invalidErrorMessage = null)
    {
        if (successMessage != null) _config.SuccessMessage = successMessage;
        if (invalidErrorMessage != null) _config.InvalidErrorMessage = invalidErrorMessage;
        return this;
    }

    /// <summary>Sets custom validator. When set, used instead of PIN storage. Return true if OTP is valid.</summary>
    public KKOTPView OnValidate(Func<string, bool> validator)
    {
        _config.CustomValidator = validator;
        return this;
    }

    /// <summary>Sets callback when OTP is submitted. Parameter indicates if validation succeeded.</summary>
    public KKOTPView OnSubmit(Action<bool> callback)
    {
        _viewModel.OnSubmit = callback;
        return this;
    }

    /// <summary>Sets callback when resend button is tapped.</summary>
    public KKOTPView OnResend(Action callback)
    {
        _viewModel.OnForgotPin = callback;
        return this;
    }

    /// <summary>Sets resend cooldown in seconds (30-60 typical).</summary>
    public KKOTPView ResendCooldown(int seconds)
    {
        _config.ResendCooldownSeconds = seconds;
        return this;
    }

    /// <summary>When true (default), paste of full OTP auto-fills all fields.</summary>
    public KKOTPView EnablePaste(bool enable = true)
    {
        _config.EnablePasteSupport = enable;
        return this;
    }

    /// <summary>When true, countdown starts automatically when view loads.</summary>
    public KKOTPView AutoStartCountdown(bool enable = true)
    {
        _config.AutoStartCountdown = enable;
        return this;
    }

    /// <summary>When true, enables iOS SMS one-time-code autocomplete.</summary>
    public KKOTPView EnableAutoReadSMS(bool enable = true)
    {
        _config.EnableAutoReadSMS = enable;
        return this;
    }

    #endregion

    #region Callbacks (non-fluent setters for XAML)

    /// <summary>Callback invoked when the view is fully created and ready.</summary>
    public Action? OnCreationCompleted { get; set; }

    /// <summary>Callback invoked when resend button is tapped.</summary>
    public Action? OnForgotPin
    {
        get => _viewModel.OnForgotPin;
        set => _viewModel.OnForgotPin = value;
    }

    /// <summary>Callback invoked when OTP is submitted. Parameter indicates if valid.</summary>
    public Action<bool>? OnSubmit
    {
        get => _viewModel.OnSubmit;
        set => _viewModel.OnSubmit = value;
    }

    #endregion

    #region Public methods

    /// <summary>Focuses the first digit field so the keyboard appears.</summary>
    public void ShowKeyboard()
    {
        if (_pinFields.Count == 0) return;
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() => _pinFields[0].FocusEntry());
        });
    }

    /// <summary>Starts the resend countdown. Call after sending OTP (e.g. from OnResend callback).</summary>
    public void StartCountdown()
    {
        _viewModel.StartCountdown();
    }

    /// <summary>Clears OTP fields. Call when resend is triggered to reset input.</summary>
    public void ClearOTP()
    {
        _ = ClearPinAsync();
    }

    #endregion

    #region Private helpers

    private void FocusFirstEmptyField()
    {
        if (_pinFields.Count == 0) return;
        var digits = _pinFields.Select(f => f.Digit).ToList();
        int idx = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, _pinFields.Count);
        _pinFields[idx].FocusEntry();
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (!_fieldsInitialized)
        {
            InitializePinFields();
            _fieldsInitialized = true;
            UpdateUI();
        }
        SetupPinFields();

        if (_pinFields.Count > 0)
        {
            if (_config.AutoStartCountdown && _config.ShowResendButton)
                _viewModel.StartCountdown();
            OnCreationCompleted?.Invoke();
        }
    }

    private void SetupPinFields()
    {
        foreach (var field in _pinFields)
        {
            field.DigitChanged -= OnPinFieldDigitChanged;
            field.DigitCompleted -= OnPinFieldCompleted;
            field.DigitDeleted -= OnPinFieldDigitDeleted;
            field.PasteReceived -= OnPinFieldPasteReceived;

            field.DigitChanged += OnPinFieldDigitChanged;
            field.DigitCompleted += OnPinFieldCompleted;
            field.DigitDeleted += OnPinFieldDigitDeleted;
            field.PasteReceived += OnPinFieldPasteReceived;
        }
    }

    private void OnPinFieldPasteReceived(object? sender, string pastedDigits)
    {
        if (!_config.EnablePasteSupport || string.IsNullOrEmpty(pastedDigits)) return;

        var digits = new string(pastedDigits.Where(char.IsDigit).ToArray());
        if (digits.Length == 0) return;

        var toApply = digits.Length > _config.Length ? digits.Substring(0, _config.Length) : digits;

        for (int i = 0; i < _pinFields.Count; i++)
        {
            var field = _pinFields[i];
            field.ClearDigitSilently();
            if (i < toApply.Length)
            {
                field.Digit = toApply[i].ToString();
                field.IsFilled = true;
            }
        }

        _currentPin = toApply;
        _viewModel.IsPinInvalid = false;
        _viewModel.HasError = false;
        _viewModel.ErrorMessage = string.Empty;
        UpdateBorderColors();
        ClearMessages();

        if (_currentPin.Length == _config.Length)
        {
            _pinFields[^1].UnfocusEntry();
            ValidateOTP();
        }
        else if (toApply.Length < _pinFields.Count)
        {
            _pinFields[toApply.Length].FocusEntry();
        }
    }

    private void InitializePinFields()
    {
        PinFieldsContainer.Children.Clear();
        _pinFields.Clear();
        _currentPin = string.Empty;

        for (int i = 0; i < _config.Length; i++)
        {
            var field = new OTPPinDigitField
            {
                FieldShapeType = _config.FieldShapeType,
                IsSecure = _config.IsSecure,
                FieldWidth = _config.FieldWidth,
                FieldHeight = _config.FieldHeight,
                FontSize = _config.DigitFontSize,
                DigitFontAttributes = _config.DigitFontAttributes,
                TextColor = _config.TextColor,
                BackgroundColor = _config.DigitFieldBackgroundColor,
                BorderColor = _config.EmptyBorderColor,
                CornerRadius = _config.FieldCornerRadius,
                MaxLength = _config.EnablePasteSupport ? _config.Length : 1
            };
            if (_config.FieldShapeType == KKPinFieldShapeType.RoundedRectangle)
                field.CornerRadius = _config.FieldCornerRadius;
            field.TapCommand = _focusFirstEmptyCommand;
            _pinFields.Add(field);
            PinFieldsContainer.Children.Add(field);
        }

        if (_config.EnableAutoReadSMS)
            SetupAutoReadSMS();
    }

    private void SetupAutoReadSMS()
    {
        if (_pinFields.Count == 0) return;
        _pinFields[0].EnableOneTimeCodeContentType();
    }

    private void UpdatePinFields()
    {
        for (int i = 0; i < _pinFields.Count; i++)
        {
            _pinFields[i].IsFilled = i < _currentPin.Length;
            _pinFields[i].Digit = i < _currentPin.Length ? _currentPin[i].ToString() : string.Empty;
        }
        UpdateBorderColors();
    }

    private void UpdateBorderColors()
    {
        for (int i = 0; i < _pinFields.Count; i++)
        {
            var targetColor = _viewModel.IsPinInvalid
                ? _config.InvalidBorderColor
                : (_pinFields[i].IsFilled ? _config.FilledBorderColor : _config.EmptyBorderColor);
            _pinFields[i].BorderColor = targetColor;
        }
    }

    private void OnRootTapped(object? sender, TappedEventArgs e)
    {
        bool keyboardOpen = _pinFields.Any(f => f.IsEntryFocused);
        if (keyboardOpen)
        {
            for (int i = 0; i < _pinFields.Count; i++)
                if (_pinFields[i].IsEntryFocused) { _lastFocusedIndex = i; break; }
        }
        else if (_pinFields.Count > 0)
        {
            int idx = _lastFocusedIndex >= 0 && _lastFocusedIndex < _pinFields.Count ? _lastFocusedIndex : 0;
            _pinFields[idx].FocusEntry();
        }
    }

    private void OnPinFieldDigitChanged(object? sender, string digit)
    {
        if (sender is not OTPPinDigitField field) return;
        int fieldIndex = _pinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        _viewModel.IsPinInvalid = false;
        _viewModel.HasError = false;
        _viewModel.ErrorMessage = string.Empty;

        _currentPin = string.Concat(_pinFields.Select(f => f.Digit ?? string.Empty));
        UpdatePinFields();
        ClearMessages();

        if (!string.IsNullOrEmpty(digit))
        {
            if (fieldIndex < _pinFields.Count - 1)
                _pinFields[fieldIndex + 1].FocusEntry();
            else if (_currentPin.Length == _config.Length)
            {
                field.UnfocusEntry();
                ValidateOTP();
            }
        }
    }

    private void OnPinFieldDigitDeleted(object? sender, EventArgs e)
    {
        if (sender is not OTPPinDigitField field) return;
        int fieldIndex = _pinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        bool currentFieldHadDigit = _currentPin.Length > fieldIndex;
        int fieldToFocus;

        if (currentFieldHadDigit)
            fieldToFocus = fieldIndex > 0 ? fieldIndex - 1 : 0;
        else if (fieldIndex > 0)
        {
            _pinFields[fieldIndex - 1].ClearDigitSilently();
            fieldToFocus = fieldIndex - 1;
        }
        else
            fieldToFocus = 0;

        _currentPin = string.Concat(_pinFields.Select(f => f.Digit ?? string.Empty));
        _viewModel.IsPinInvalid = false;
        _viewModel.HasError = false;
        _viewModel.ErrorMessage = string.Empty;
        UpdatePinFields();
        UpdateBorderColors();
        ClearMessages();
        _pinFields[fieldToFocus].FocusEntry();
    }

    private void OnPinFieldCompleted(object? sender, EventArgs e)
    {
        if (sender is not OTPPinDigitField field) return;
        int fieldIndex = _pinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        if (fieldIndex < _pinFields.Count - 1)
            _pinFields[fieldIndex + 1].FocusEntry();
        else if (_currentPin.Length == _config.Length)
        {
            field.UnfocusEntry();
            ValidateOTP();
        }
    }

    private void ValidateOTP()
    {
        KKPinViewDebug.LogPin("Validating OTP", _currentPin);

        bool isValid;
        if (_config.CustomValidator != null)
        {
            isValid = _config.CustomValidator(_currentPin);
        }
        else
        {
            isValid = _lockoutManager.ValidatePIN(_currentPin);
        }

        try
        {
            if (isValid)
            {
                KKPinViewDebug.Log("OTP validation successful");
                ShowSuccessMessage(_config.SuccessMessage);
                Task.Delay(500).ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() => ClearPin());
                });
            }
            else
            {
                string error = _config.CustomValidator != null
                    ? _config.InvalidErrorMessage
                    : (_lockoutManager.GetErrorMessage() ?? _config.InvalidErrorMessage);

                KKPinViewDebug.LogWarning($"OTP validation failed: {error}");
                _viewModel.IsPinInvalid = true;
                UpdatePinFields();

                if (_config.CustomValidator != null)
                    ShowErrorMessage(error);
                else if (!_lockoutManager.IsLockedOut)
                    ShowErrorMessage(error);
                else
                    UpdateUI();

                if (_config.CustomValidator == null)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1500);
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await ClearPinAsync();
                            if (!_lockoutManager.IsLockedOut && _pinFields.Count > 0)
                            {
                                _lastFocusedIndex = 0;
                                await Task.Delay(100);
                                _pinFields[0].FocusEntry();
                            }
                        });
                    });
                }
                else
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1500);
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await ClearPinAsync();
                            if (_pinFields.Count > 0)
                            {
                                _lastFocusedIndex = 0;
                                await Task.Delay(100);
                                _pinFields[0].FocusEntry();
                            }
                        });
                    });
                }
            }
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("ValidateOTP error", ex);
            isValid = false;
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() => _viewModel.OnSubmit?.Invoke(isValid));
        }
    }

    private async void ShowErrorMessage(string message)
    {
        SuccessMessageLabel.HeightRequest = 0;
        double height = Math.Max(CalculateMessageHeight(message, _config.ErrorMessageFontSize), _config.ErrorMessageLabelHeight);
        _viewModel.ErrorMessage = message;
        _viewModel.HasError = true;
        _viewModel.HasSuccess = false;

        if (ErrorMessageLabel != null)
        {
            ErrorMessageLabel.Opacity = 0;
            ErrorMessageLabel.Scale = 1;
            ErrorMessageLabel.HeightRequest = 0;
            var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, 0, height, Easing.CubicOut);
            var tcs = new TaskCompletionSource<bool>();
            heightAnimation.Commit(ErrorMessageLabel, "height", 16, 300, Easing.CubicOut, (_, _) => tcs.SetResult(true));
            await Task.WhenAll(tcs.Task, ErrorMessageLabel.FadeToAsync(1, 300, Easing.CubicOut));
        }
    }

    private double CalculateMessageHeight(string message, double fontSize)
    {
        if (string.IsNullOrEmpty(message)) return _config.ErrorMessageLabelHeight;
        double estimatedWidth = 300;
        double averageCharWidth = fontSize * 0.6;
        int charsPerLine = (int)(estimatedWidth / averageCharWidth);
        int numberOfLines = Math.Max(1, (int)Math.Ceiling((double)message.Length / charsPerLine));
        double lineHeight = fontSize * 1.3;
        return Math.Max((numberOfLines * lineHeight) + 8, _config.ErrorMessageLabelHeight);
    }

    private async void ShowSuccessMessage(string message)
    {
        ErrorMessageLabel.HeightRequest = 0;
        _viewModel.SuccessMessage = message;
        _viewModel.HasSuccess = true;
        _viewModel.HasError = false;

        if (SuccessMessageLabel != null)
        {
            SuccessMessageLabel.Opacity = 0;
            SuccessMessageLabel.Scale = 0.3;
            SuccessMessageLabel.HeightRequest = 0;
            var heightAnimation = new Animation(v => SuccessMessageLabel.HeightRequest = v, 0, _config.SuccessMessageLabelHeight, Easing.CubicOut);
            var tcs = new TaskCompletionSource<bool>();
            heightAnimation.Commit(SuccessMessageLabel, "height", 16, 300, Easing.CubicOut, (_, _) => tcs.SetResult(true));
            await Task.WhenAll(tcs.Task, SuccessMessageLabel.FadeToAsync(1, 300, Easing.CubicOut), SuccessMessageLabel.ScaleToAsync(1, 400, Easing.SpringOut));
        }
    }

    private async Task ClearMessagesAsync()
    {
        if (ErrorMessageLabel != null && ErrorMessageLabel.HeightRequest > 0)
        {
            var currentHeight = ErrorMessageLabel.HeightRequest;
            var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, currentHeight, 0, Easing.CubicIn);
            var tcs = new TaskCompletionSource<bool>();
            heightAnimation.Commit(ErrorMessageLabel, "height", 16, 200, Easing.CubicIn, (_, _) => tcs.SetResult(true));
            await Task.WhenAll(tcs.Task, ErrorMessageLabel.FadeToAsync(0, 200));
            ErrorMessageLabel.Scale = 1;
            ErrorMessageLabel.HeightRequest = 0;
        }
        if (SuccessMessageLabel != null && SuccessMessageLabel.HeightRequest > 0)
        {
            var currentHeight = SuccessMessageLabel.HeightRequest;
            var heightAnimation = new Animation(v => SuccessMessageLabel.HeightRequest = v, currentHeight, 0, Easing.CubicIn);
            var tcs = new TaskCompletionSource<bool>();
            heightAnimation.Commit(SuccessMessageLabel, "height", 16, 200, Easing.CubicIn, (_, _) => tcs.SetResult(true));
            await Task.WhenAll(tcs.Task, SuccessMessageLabel.FadeToAsync(0, 200));
            SuccessMessageLabel.Scale = 1;
            SuccessMessageLabel.HeightRequest = 0;
        }
        _viewModel.HasError = false;
        _viewModel.HasSuccessMessage = false;
        _viewModel.ErrorMessage = string.Empty;
        _viewModel.SuccessMessage = string.Empty;
    }

    private void ClearMessages() => _ = ClearMessagesAsync();

    private async Task ClearPinAsync()
    {
        foreach (var f in _pinFields) f.ClearDigitSilently();
        _currentPin = string.Empty;
        _viewModel.IsPinInvalid = false;
        UpdateBorderColors();
        if (_config.CustomValidator == null && !_lockoutManager.IsLockedOut)
            await ClearMessagesAsync();
        else if (_config.CustomValidator != null)
            await ClearMessagesAsync();
    }

    private void ClearPin() => _ = ClearPinAsync();

    private void UpdateUI()
    {
        if (_config.CustomValidator == null && _lockoutManager.IsLockedOut)
        {
            var error = _lockoutManager.GetErrorMessage();
            ShowErrorMessage(error ?? _config.InvalidErrorMessage);
        }
    }

    private void OnForgotPinClicked(object? sender, EventArgs e)
    {
        if (!_viewModel.IsResendEnabled) return;

        _viewModel.OnForgotPin?.Invoke();
        _ = ClearPinAsync();
        _viewModel.StartCountdown();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null && _config.CustomValidator == null)
        {
            _lockoutManager.CheckLockoutStatus();
            UpdateUI();
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Loaded -= OnPageLoaded;
        foreach (var f in _pinFields)
        {
            f.DigitChanged -= OnPinFieldDigitChanged;
            f.DigitCompleted -= OnPinFieldCompleted;
            f.DigitDeleted -= OnPinFieldDigitDeleted;
            f.PasteReceived -= OnPinFieldPasteReceived;
            f.ClearDigitSilently();
        }
        _currentPin = string.Empty;
        _viewModel.IsPinInvalid = false;
        _viewModel.HasError = false;
        _viewModel.HasSuccessMessage = false;
        _viewModel.ErrorMessage = string.Empty;
        _viewModel.SuccessMessage = string.Empty;
        _viewModel.Dispose();
    }

    #endregion
}
