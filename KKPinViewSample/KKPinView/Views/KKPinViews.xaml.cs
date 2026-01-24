using System.Collections.ObjectModel;
using KKPinView.Constants;
using KKPinView.Debug;
using KKPinView.Security;
using KKPinView.Storage;
using KKPinView.ViewModels;

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

        // Wire up ViewModel events
        _viewModel.NumberPressed += OnViewModelNumberPressed;
        _viewModel.DeletePressed += OnViewModelDeletePressed;

        InitializePinFields();
        SetupInputMethod();
        UpdateUI();

        // Set up keypad commands after initialization
        Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (Keypad != null)
        {
            Keypad.NumberCommand = _viewModel.NumberCommand;
            Keypad.DeleteCommand = _viewModel.DeleteCommand;
        }

        // Ensure input method is set up after page is fully loaded
        SetupInputMethod();
    }

    private void SetupInputMethod()
    {
        // Show/hide keypad based on input method
        bool useKeypad = KKPinviewConstant.InputMethod == PinInputMethod.NumericKeypad;

        if (Keypad != null)
        {
            Keypad.IsVisible = useKeypad;
        }

        // Set editable state on PIN fields based on input method
        bool isEditable = KKPinviewConstant.InputMethod == PinInputMethod.SystemKeyboard;

        // Remove old handlers first to avoid duplicates
        foreach (var field in _pinFields)
        {
            field.DigitChanged -= OnPinFieldDigitChanged;
            field.DigitCompleted -= OnPinFieldCompleted;

            field.IsEditable = isEditable;

            if (isEditable)
            {
                field.DigitChanged += OnPinFieldDigitChanged;
                field.DigitCompleted += OnPinFieldCompleted;
            }
        }

        // Focus first field if using keyboard (with a small delay to ensure UI is ready)
        if (isEditable && _pinFields.Count > 0)
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                _pinFields[0].FocusEntry();
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

        for (int i = 0; i < KKPinviewConstant.TotalDigits; i++)
        {
            var field = new PinDigitField();
            _pinFields.Add(field);
            PinFieldsContainer.Children.Add(field);
        }
    }

    private void OnViewModelNumberPressed(object? sender, string number)
    {
        OnNumberPressed(number);
    }

    private void OnViewModelDeletePressed(object? sender, EventArgs e)
    {
        OnDeletePressed();
    }

    private void OnNumberPressed(string number)
    {
        KKPinViewDebug.LogVerbose($"Number pressed: {number}");

        if (!_viewModel.IsKeypadEnabled || _currentPin.Length >= KKPinviewConstant.TotalDigits)
        {
            KKPinViewDebug.LogVerbose("Keypad disabled or PIN already complete");
            return;
        }

        _currentPin += number;
        KKPinViewDebug.LogVerbose($"Current PIN length: {_currentPin.Length}");
        UpdatePinFields();

        if (_currentPin.Length == KKPinviewConstant.TotalDigits)
        {
            KKPinViewDebug.Log("PIN entry complete, validating...");
            ValidatePIN();
        }
    }

    private void OnDeletePressed()
    {
        if (_currentPin.Length > 0)
        {
            _currentPin = _currentPin.Substring(0, _currentPin.Length - 1);
            UpdatePinFields();
            ClearMessages();
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
    }

    // Event handlers for keyboard input in PIN fields (when InputMethod is SystemKeyboard)
    private void OnPinFieldDigitChanged(object? sender, string digit)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _pinFields.IndexOf(field);
        if (fieldIndex < 0) return;

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
            else if (_currentPin.Length == KKPinviewConstant.TotalDigits)
            {
                ValidatePIN();
            }
        }
        else
        {
            // Digit deleted (empty) - move focus backward
            if (fieldIndex > 0)
            {
                _pinFields[fieldIndex - 1].FocusEntry();
            }
            else
            {
                // Already at first field, keep focus here
                _pinFields[0].FocusEntry();
            }
        }
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
        else if (_currentPin.Length == KKPinviewConstant.TotalDigits)
        {
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
            ShowErrorMessage(error ?? KKPinviewConstant.InvalidPinError);
            _viewModel.OnSubmit?.Invoke(false);

            // Clear after showing error
            Task.Delay(1500).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ClearPin();
                });
            });
        }

        UpdateUI();
    }

    private async void ShowErrorMessage(string message)
    {
        _viewModel.ErrorMessage = message;
        _viewModel.HasError = true;
        _viewModel.HasSuccess = false;

        // Animate error message appearance - fade and scale simultaneously
        if (ErrorMessageLabel != null)
        {
            ErrorMessageLabel.Opacity = 0;
            ErrorMessageLabel.Scale = 0.3; // Start from smaller scale
            // Run fade and scale animations simultaneously
            await Task.WhenAll(
                ErrorMessageLabel.FadeToAsync(1, 300, Easing.CubicOut),
                ErrorMessageLabel.ScaleToAsync(1, 400, Easing.SpringOut)
            );
        }
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
        _currentPin = string.Empty;
        UpdatePinFields();
        ClearMessages();
    }

    private void UpdateUI()
    {
        _viewModel.IsKeypadEnabled = !_lockoutManager.IsLockedOut;
        _viewModel.KeypadOpacity = _viewModel.IsKeypadEnabled ? 1.0 : 0.5;

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
