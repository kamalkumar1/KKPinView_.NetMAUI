using System.Collections.ObjectModel;
using System.Linq;
using KKPinView.Constants;
using KKPinView.Debug;
using KKPinView.Helpers;
using KKPinView.Security;
using KKPinView.Storage;
using KKPinView.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace KKPinView.Views;

public sealed partial class KKPinViews : ContentView
{
    private readonly ObservableCollection<PinDigitField> _pinFields = new();
    private readonly KKPinLockoutManager _lockoutManager;
    private readonly KKPinViewsViewModel _viewModel;
    private readonly System.Windows.Input.ICommand _focusFirstEmptyCommand;
    private string _currentPin = string.Empty;
    /// <summary>Index of the PIN field that had focus when keyboard was last closed; -1 means use first field.</summary>
    private int _lastFocusedIndex = -1;

    public KKPinViewsViewModel ViewModel => _viewModel;

    public KKPinViews()
    {
        _focusFirstEmptyCommand = new Command(FocusFirstEmptyField);
        InitializeComponent();
        _lockoutManager = new KKPinLockoutManager();

        // Create and set ViewModel
        _viewModel = new KKPinViewsViewModel();
        BindingContext = _viewModel;

        InitializePinFields();
        UpdateUI();
        Loaded -= OnPageLoaded;
        Loaded += OnPageLoaded;
    }

    /// <summary>Focus the first empty PIN field so the next digit goes there. Used by TapCommand; delete/backspace focus is set explicitly in OnPinFieldDigitDeleted.</summary>
    private void FocusFirstEmptyField()
    {
        if (_pinFields.Count == 0) return;
        var digits = _pinFields.Select(f => f.Digit).ToList();
        int idx = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, _pinFields.Count);
        _pinFields[idx].FocusEntry();
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        SetupPinFields();
    }

    private void SetupPinFields()
    {
        foreach (var field in _pinFields)
        {
            field.DigitChanged -= OnPinFieldDigitChanged;
            field.DigitCompleted -= OnPinFieldCompleted;
            field.DigitDeleted -= OnPinFieldDigitDeleted;

            field.DigitChanged += OnPinFieldDigitChanged;
            field.DigitCompleted += OnPinFieldCompleted;
            field.DigitDeleted += OnPinFieldDigitDeleted;
        }

        if (_pinFields.Count > 0)
        {
            OnCreationCompleted?.Invoke();
        }
    }

    /// <summary>
    /// Callback invoked when the PIN view is fully created and ready. Use this to call <see cref="ShowKeyboard"/> or perform other setup.
    /// </summary>
    public Action? OnCreationCompleted { get; set; }

    /// <summary>
    /// Focuses the first PIN field so the keyboard appears. Call from <see cref="OnCreationCompleted"/> or when you want to show the keyboard.
    /// </summary>
    public void ShowKeyboard()
    {
        if (_pinFields.Count == 0) return;
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() => _pinFields[0].FocusEntry());
        });
    }

    /// <summary>
    /// Callback invoked when "Forgot PIN?" is tapped
    /// </summary>
    public Action? OnForgotPin
    {
        get => _viewModel.OnForgotPin;
        set => _viewModel.OnForgotPin = value;
    }

    /// <summary>
    /// Callback invoked when PIN is submitted. The boolean parameter indicates if the PIN is valid.
    /// </summary>
    public Action<bool>? OnSubmit
    {
        get => _viewModel.OnSubmit;
        set => _viewModel.OnSubmit = value;
    }

    private void InitializePinFields()
    {
        PinFieldsContainer.Children.Clear();
        _pinFields.Clear();
        _currentPin = string.Empty;

        for (int i = 0; i < KKPinviewConstant.TotalPinTextFields; i++)
        {
            var field = new PinDigitField { FieldShapeType = KKPinviewConstant.FieldShapeType };
            if (KKPinviewConstant.FieldShapeType == KKPinFieldShapeType.RoundedRectangle)
                field.CornerRadius = KKPinviewConstant.FieldCornerRadius;
            field.TapCommand = _focusFirstEmptyCommand;
            _pinFields.Add(field);
            PinFieldsContainer.Children.Add(field);
        }
    }

    private void UpdatePinFields()
    {
        for (int i = 0; i < _pinFields.Count; i++)
        {
            _pinFields[i].IsFilled = i < _currentPin.Length;
            if (i < _currentPin.Length)
            {
                _pinFields[i].Digit = _currentPin[i].ToString();
            }
            else
            {
                _pinFields[i].Digit = string.Empty;
            }
        }

        // Update border colors after setting filled state
        UpdateBorderColors();
    }

    private void UpdateBorderColors()
    {
        for (int i = 0; i < _pinFields.Count; i++)
        {
            var targetColor = _viewModel.IsPinInvalid
                ? KKPinviewConstant.InvalidPinBorderColor
                : (_pinFields[i].IsFilled
                    ? KKPinviewConstant.DigitFieldFilledColor
                    : KKPinviewConstant.DigitFieldEmptyBorderColor);
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
            int idx = _lastFocusedIndex;
            if (idx < 0 || idx >= _pinFields.Count) idx = 0;
            _pinFields[idx].FocusEntry();
        }
    }

    // Event handlers for system keyboard input in PIN fields
    private void OnPinFieldDigitChanged(object? sender, string digit)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _pinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // Reset invalid state and error message when user starts typing
        _viewModel.IsPinInvalid = false;
        _viewModel.HasError = false;
        _viewModel.ErrorMessage = string.Empty;

        // Rebuild PIN from all fields
        _currentPin = string.Empty;
        foreach (var pinField in _pinFields)
        {
            if (!string.IsNullOrEmpty(pinField.Digit))
            {
                _currentPin += pinField.Digit;
            }
        }

        UpdatePinFields();
        ClearMessages();

        // Handle focus movement based on input
        if (!string.IsNullOrEmpty(digit))
        {
            // Digit entered - move to next field or validate if complete
            if (fieldIndex < _pinFields.Count - 1)
            {
                _pinFields[fieldIndex + 1].FocusEntry();
            }
            else if (_currentPin.Length == KKPinviewConstant.TotalPinTextFields)
            {
                // Dismiss keyboard when PIN is complete
                field.UnfocusEntry();
                ValidatePIN();
            }
        }
        else
        {
            // Digit deleted - focus movement handled by OnPinFieldDigitDeleted when using keyboard
            // (DigitChanged with empty fires when field had content; DigitDeleted handles both cases)
        }
    }

    private void OnPinFieldDigitDeleted(object? sender, EventArgs e)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _pinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // Use _currentPin to detect: had digit (TextChanged) vs was empty (KeyPress backspace)
        bool currentFieldHadDigit = _currentPin.Length > fieldIndex;
        int fieldToFocus;

        if (currentFieldHadDigit)
        {
            // Field had digit - already cleared by TextChanged; just move focus backward
            fieldToFocus = fieldIndex > 0 ? fieldIndex - 1 : 0;
        }
        else if (fieldIndex > 0)
        {
            // Field was empty - clear previous field and move focus there
            _pinFields[fieldIndex - 1].ClearDigitSilently();
            fieldToFocus = fieldIndex - 1;
        }
        else
        {
            fieldToFocus = 0;
        }

        // Rebuild PIN from all fields
        _currentPin = string.Empty;
        foreach (var pinField in _pinFields)
        {
            if (!string.IsNullOrEmpty(pinField.Digit))
                _currentPin += pinField.Digit;
        }
        // Reset invalid state and error when user deletes
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
        if (sender is not PinDigitField field) return;

        int fieldIndex = _pinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // Move to next field or validate
        if (fieldIndex < _pinFields.Count - 1)
        {
            _pinFields[fieldIndex + 1].FocusEntry();
        }
        else if (_currentPin.Length == KKPinviewConstant.TotalPinTextFields)
        {
            // Dismiss keyboard when PIN is complete
            field.UnfocusEntry();
            ValidatePIN();
        }
    }

    private void ValidatePIN()
    {
        KKPinViewDebug.LogPin("Validating PIN", _currentPin);

        var isValid = _lockoutManager.ValidatePIN(_currentPin);

        try
        {
            if (isValid)
            {
                KKPinViewDebug.Log("PIN validation successful");
                ShowSuccessMessage(KKPinviewConstant.SetupSuccessMessage);

                Task.Delay(500).ContinueWith(_ =>
                {
                    MainThread.BeginInvokeOnMainThread(() => ClearPin());
                });
            }
            else
            {
                var error = _lockoutManager.GetErrorMessage();
                KKPinViewDebug.LogWarning($"PIN validation failed: {error}");

                //  foreach (var f in _pinFields) f.UnfocusEntry();

                _viewModel.IsPinInvalid = true;
                UpdatePinFields();
                //  ClearMessages();
                // ClearPin();
                //_pinFields[0].FocusEntry();

                if (!_lockoutManager.IsLockedOut)
                    ShowErrorMessage(error ?? KKPinviewConstant.LockedOutError);
                else
                    UpdateUI();

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
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("ValidatePIN error", ex);
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
        double height = Math.Max(CalculateMessageHeight(message, _viewModel.ErrorMessageFontSize), KKPinviewConstant.ErrorMessageLabelHeight);
        _viewModel.ErrorMessage = message;
        _viewModel.HasError = true;
        _viewModel.HasSuccess = false;

        if (ErrorMessageLabel != null)
        {
            ErrorMessageLabel.Opacity = 0;
            ErrorMessageLabel.Scale = 1;
            ErrorMessageLabel.HeightRequest = 0;

            var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, 0, height, Easing.CubicOut);
            var heightTaskCompletionSource = new TaskCompletionSource<bool>();
            heightAnimation.Commit(ErrorMessageLabel, "height", 16, 300, Easing.CubicOut, (v, c) => heightTaskCompletionSource.SetResult(true));
            await Task.WhenAll(
                heightTaskCompletionSource.Task,
                ErrorMessageLabel.FadeToAsync(1, 300, Easing.CubicOut)
            );
        }
    }

    /// <summary>
    /// Calculates the height needed for a message based on its length and font size
    /// </summary>
    private double CalculateMessageHeight(string message, double fontSize)
    {
        if (string.IsNullOrEmpty(message))
        {
            return KKPinviewConstant.ErrorMessageLabelHeight;
        }

        // Estimate characters per line based on typical screen width (assuming ~300px available width)
        // This is a rough estimate - adjust based on your actual layout
        double estimatedWidth = 300; // Approximate available width for error message
        double averageCharWidth = fontSize * 0.6; // Rough estimate: character width is about 60% of font size
        int charsPerLine = (int)(estimatedWidth / averageCharWidth);

        // Calculate number of lines needed
        int numberOfLines = (int)Math.Ceiling((double)message.Length / charsPerLine);

        // Minimum 1 line, add some padding
        numberOfLines = Math.Max(1, numberOfLines);

        // Calculate height: line height is typically 1.2-1.5x font size, add some padding
        double lineHeight = fontSize * 1.3;
        double calculatedHeight = (numberOfLines * lineHeight) + 8; // Add 8px padding

        // Ensure minimum height
        return Math.Max(calculatedHeight, KKPinviewConstant.ErrorMessageLabelHeight);
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

            // Animate height from 0 to 50 and content (fade + scale) simultaneously
            var heightAnimation = new Animation(v => SuccessMessageLabel.HeightRequest = v, 0, KKPinviewConstant.SuccessMessageLabelHeight, Easing.CubicOut);
            var heightTaskCompletionSource = new TaskCompletionSource<bool>();
            heightAnimation.Commit(SuccessMessageLabel, "height", 16, 300, Easing.CubicOut, (v, c) => heightTaskCompletionSource.SetResult(true));

            await Task.WhenAll(
                heightTaskCompletionSource.Task,
                SuccessMessageLabel.FadeToAsync(1, 300, Easing.CubicOut),
                SuccessMessageLabel.ScaleToAsync(1, 400, Easing.SpringOut)
            );
        }
    }

    private async Task ClearMessagesAsync()
    {
        if (ErrorMessageLabel != null && ErrorMessageLabel.HeightRequest > 0)
        {
            var currentHeight = ErrorMessageLabel.HeightRequest;
            var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, currentHeight, 0, Easing.CubicIn);
            var heightTaskCompletionSource = new TaskCompletionSource<bool>();
            heightAnimation.Commit(ErrorMessageLabel, "height", 16, 200, Easing.CubicIn, (v, c) => heightTaskCompletionSource.SetResult(true));
            await Task.WhenAll(
                heightTaskCompletionSource.Task,
                ErrorMessageLabel.FadeToAsync(0, 200)
            );
            ErrorMessageLabel.Scale = 1;
            ErrorMessageLabel.HeightRequest = 0;
        }

        if (SuccessMessageLabel != null && SuccessMessageLabel.HeightRequest > 0)
        {
            var currentHeight = SuccessMessageLabel.HeightRequest;
            var heightAnimation = new Animation(v => SuccessMessageLabel.HeightRequest = v, currentHeight, 0, Easing.CubicIn);
            var heightTaskCompletionSource = new TaskCompletionSource<bool>();
            heightAnimation.Commit(SuccessMessageLabel, "height", 16, 200, Easing.CubicIn, (v, c) => heightTaskCompletionSource.SetResult(true));
            await Task.WhenAll(
                heightTaskCompletionSource.Task,
                SuccessMessageLabel.FadeToAsync(0, 200)
            );
            SuccessMessageLabel.Scale = 1;
            SuccessMessageLabel.HeightRequest = 0;
        }

        _viewModel.HasError = false;
        _viewModel.HasSuccessMessage = false;
        _viewModel.ErrorMessage = string.Empty;
        _viewModel.SuccessMessage = string.Empty;
    }

    private void ClearMessages()
    {
        _ = ClearMessagesAsync();
    }

    private async Task ClearPinAsync()
    {
        foreach (var f in _pinFields) f.ClearDigitSilently();
        _currentPin = string.Empty;
        _viewModel.IsPinInvalid = false;
        UpdateBorderColors();
        if (!_lockoutManager.IsLockedOut)
        {
            await ClearMessagesAsync();
        }
    }

    private void ClearPin()
    {
        _ = ClearPinAsync();
    }

    private void UpdateUI()
    {
        if (_lockoutManager.IsLockedOut)
        {
            var error = _lockoutManager.GetErrorMessage();
            ShowErrorMessage(error ?? KKPinviewConstant.LockedOutError);
        }
    }

    private void OnForgotPinClicked(object? sender, EventArgs e)
    {
        _viewModel.OnForgotPin?.Invoke();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler != null)
        {
            _lockoutManager.CheckLockoutStatus();
            UpdateUI();
        }
    }
}
