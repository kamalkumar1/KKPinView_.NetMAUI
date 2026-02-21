using System.Collections.ObjectModel;
using KKPinView.Constants;
using KKPinView.Debug;
using KKPinView.Security;
using KKPinView.Storage;
using KKPinView.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace KKPinView.Views;

public partial class KKPinViews : ContentView
{
    private readonly ObservableCollection<PinDigitField> _pinFields = new();
    private readonly KKPinLockoutManager _lockoutManager;
    private readonly KKPinViewsViewModel _viewModel;
    private string _currentPin = string.Empty;

    public KKPinViewsViewModel ViewModel => _viewModel;

    public KKPinViews()
    {
        InitializeComponent();
        _lockoutManager = new KKPinLockoutManager();

        // Create and set ViewModel
        _viewModel = new KKPinViewsViewModel();
        BindingContext = _viewModel;

        InitializePinFields();
        SetupPinFields();
        UpdateUI();

        Loaded += OnPageLoaded;
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
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(200), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _pinFields[0].FocusEntry();
                });
            });
        }
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
        if (KKPinviewConstant.FieldShapeType == PinFieldShapeType.RoundedRectangle)
            field.CornerRadius = KKPinviewConstant.FieldCornerRadius;
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
        // Set border color to invalid color if PIN is invalid
        if (_viewModel.IsPinInvalid)
        {
            _pinFields[i].BorderColor = KKPinviewConstant.InvalidPinBorderColor;
        }
        else
        {
            // Reset to default - let UpdateAppearance handle it based on IsFilled
            // Set to a sentinel value that will trigger default behavior
            if (_pinFields[i].IsFilled)
            {
                _pinFields[i].BorderColor = KKPinviewConstant.DigitFieldFilledColor;
            }
            else
            {
                _pinFields[i].BorderColor = Colors.Gray;
            }
        }
    }
}

private void OnRootTapped(object? sender, TappedEventArgs e)
{
    foreach (var f in _pinFields) f.UnfocusEntry();
}

// Event handlers for system keyboard input in PIN fields
private void OnPinFieldDigitChanged(object? sender, string digit)
{
    if (sender is not PinDigitField field) return;

    int fieldIndex = _pinFields.IndexOf(field);
    if (fieldIndex < 0) return;

    // Reset invalid state when user starts typing
    _viewModel.IsPinInvalid = false;

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

    if (isValid)
    {
        KKPinViewDebug.Log("PIN validation successful");
        ShowSuccessMessage(KKPinviewConstant.SetupSuccessMessage);
        _viewModel.OnSubmit?.Invoke(true);

        // Clear after a delay
        Task.Delay(500).ContinueWith(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ClearPin();
            });
        });
    }
    else
    {
        var error = _lockoutManager.GetErrorMessage();
        KKPinViewDebug.LogWarning($"PIN validation failed: {error}");

        _viewModel.OnSubmit?.Invoke(false);

        // Close keyboard when PIN does not match
        foreach (var f in _pinFields) f.UnfocusEntry();

        // Set invalid state to show red borders
        _viewModel.IsPinInvalid = true;
        UpdatePinFields();
        if (!_lockoutManager.IsLockedOut)
        {
            ShowErrorMessage(error ?? KKPinviewConstant.LockedOutError);
        }
        else
        {
            UpdateUI();
        }

        // Clear after showing error and allow user to re-enter (use ClearDigitSilently to avoid event cascade)
        Task.Delay(1500).ContinueWith(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ClearPin();
                if (!_lockoutManager.IsLockedOut)
                    _pinFields[0].FocusEntry();
            });
        });
    }
}

private async void ShowErrorMessage(string message)
{
    // Calculate height based on message length
    double calculatedHeight = CalculateMessageHeight(message, _viewModel.SubtitleFontSize);
    _viewModel.ErrorMessage = message;
    _viewModel.HasError = true;
    _viewModel.HasSuccess = false;


    // Animate error message appearance - fade and scale simultaneously
    if (ErrorMessageLabel != null)
    {
        ErrorMessageLabel.Opacity = 0;
        ErrorMessageLabel.Scale = 0.3;
        ErrorMessageLabel.HeightRequest = 0;

        // Animate height from 0 to calculated height and content (fade + scale) simultaneously
        var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, 0, calculatedHeight, Easing.CubicOut);
        var heightTaskCompletionSource = new TaskCompletionSource<bool>();
        heightAnimation.Commit(ErrorMessageLabel, "height", 16, 300, Easing.CubicOut, (v, c) => heightTaskCompletionSource.SetResult(true));
        await Task.WhenAll(
            heightTaskCompletionSource.Task,
            ErrorMessageLabel.FadeToAsync(1, 300, Easing.CubicOut),
            ErrorMessageLabel.ScaleToAsync(1, 400, Easing.SpringOut)
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
    _viewModel.SuccessMessage = message;
    _viewModel.HasSuccess = true;
    _viewModel.HasError = false;

    // Animate label height and appearance
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

private async void ClearMessages()
{
    // Animate fade out before clearing
    if (ErrorMessageLabel != null && _viewModel.HasError)
    {
        await ErrorMessageLabel.FadeToAsync(0, 200);
        // Reset height (binding will handle visibility)
        ErrorMessageLabel.HeightRequest = 0;
    }

    if (SuccessMessageLabel != null && SuccessMessageLabel.HeightRequest > 0)
    {
        // Animate height back to 0 and fade out simultaneously
        var currentHeight = SuccessMessageLabel.HeightRequest;
        var heightAnimation = new Animation(v => SuccessMessageLabel.HeightRequest = v, currentHeight, 0, Easing.CubicIn);
        var heightTaskCompletionSource = new TaskCompletionSource<bool>();
        heightAnimation.Commit(SuccessMessageLabel, "height", 16, 200, Easing.CubicIn, (v, c) => heightTaskCompletionSource.SetResult(true));

        await Task.WhenAll(
            heightTaskCompletionSource.Task,
            SuccessMessageLabel.FadeToAsync(0, 200)
        );
    }

    _viewModel.HasError = false;
    _viewModel.HasSuccessMessage = false;
    _viewModel.ErrorMessage = string.Empty;
    _viewModel.SuccessMessage = string.Empty;
}

private void ClearPin()
{
    foreach (var f in _pinFields) f.ClearDigitSilently();
    _currentPin = string.Empty;
    _viewModel.IsPinInvalid = false;
    UpdateBorderColors();
    if (!_lockoutManager.IsLockedOut)
    {
        ClearMessages();
    }
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
