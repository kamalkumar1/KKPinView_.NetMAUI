using System.Collections.ObjectModel;
using System.Windows.Input;
using KKPinView.Constants;
using KKPinView.Security;
using KKPinView.Storage;
using KKPinView.ViewModels;

namespace KKPinView.Views;

public partial class KKPINSetUPView : ContentView
{
    private readonly ObservableCollection<PinDigitField> _enterPinFields = new();
    private readonly ObservableCollection<PinDigitField> _confirmPinFields = new();
    private readonly KKPINSetUPViewModel _viewModel;
    private string _currentPin = string.Empty;
    private string _confirmPin = string.Empty;
    private bool _isConfirmingPin = false;

    public KKPINSetUPViewModel ViewModel => _viewModel;

    public KKPINSetUPView()
    {
        InitializeComponent();

        // Create and set ViewModel
        _viewModel = new KKPINSetUPViewModel();
        BindingContext = _viewModel;

        // Wire up ViewModel events
        _viewModel.NumberPressed += OnViewModelNumberPressed;
        _viewModel.DeletePressed += OnViewModelDeletePressed;

        // Initialize PIN fields based on TotalDigits constant
        InitializePinFields();
        InitializeConfirmPinFields();

        // Set up input method visibility
        SetupInputMethod();

        // Set commands on keypad after initialization
        Loaded += OnPageLoaded;
    }

    /// <summary>
    /// Callback invoked when PIN setup is successful
    /// </summary>
    public Action? OnSetupSuccess
    {
        get => _viewModel.OnSetupSuccess;
        set => _viewModel.OnSetupSuccess = value;
    }

    /// <summary>
    /// Callback invoked when PIN setup fails. The string parameter contains the error message.
    /// </summary>
    public Action<string>? OnSetupFailed
    {
        get => _viewModel.OnSetupFailed;
        set => _viewModel.OnSetupFailed = value;
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
        bool useKeypad = _viewModel.InputMethod == PinInputMethod.NumericKeypad;

        if (Keypad != null)
        {
            Keypad.IsVisible = useKeypad;
        }

        // Set editable state on PIN fields based on input method
        bool isEditable = _viewModel.InputMethod == PinInputMethod.SystemKeyboard;

        // Remove old handlers first to avoid duplicates
        foreach (var field in _enterPinFields)
        {
            field.DigitChanged -= OnEnterPinFieldDigitChanged;
            field.DigitCompleted -= OnEnterPinFieldCompleted;

            field.IsEditable = isEditable;

            if (isEditable)
            {
                field.DigitChanged += OnEnterPinFieldDigitChanged;
                field.DigitCompleted += OnEnterPinFieldCompleted;
            }
        }

        foreach (var field in _confirmPinFields)
        {
            field.DigitChanged -= OnConfirmPinFieldDigitChanged;
            field.DigitCompleted -= OnConfirmPinFieldCompleted;

            field.IsEditable = isEditable;

            if (isEditable)
            {
                field.DigitChanged += OnConfirmPinFieldDigitChanged;
                field.DigitCompleted += OnConfirmPinFieldCompleted;
            }
        }

        // Focus first field if using keyboard (with a small delay to ensure UI is ready)
        if (isEditable && _enterPinFields.Count > 0)
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                _enterPinFields[0].FocusEntry();
            });
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
        if (_isConfirmingPin)
        {
            // Entering confirm PIN
            if (_confirmPin.Length >= _viewModel.MaxPinLength)
                return;

            _confirmPin += number;
            UpdateConfirmPinFields();

            // Clear any previous messages when user starts typing again
            ClearMessages();

            // If confirm PIN is complete, validate it
            if (_confirmPin.Length == _viewModel.MaxPinLength)
            {
                ValidatePinMatch();
            }
        }
        else
        {
            // Entering first PIN
            if (_currentPin.Length >= _viewModel.MaxPinLength)
                return;

            _currentPin += number;
            UpdatePinFields();

            // If first PIN is complete, switch to confirm mode
            if (_currentPin.Length == _viewModel.MaxPinLength)
            {
                _isConfirmingPin = true;
                _viewModel.ShowConfirmPin = true;
            }
        }
    }

    private void OnDeletePressed()
    {
        if (_isConfirmingPin)
        {
            // Deleting from confirm PIN
            if (_confirmPin.Length > 0)
            {
                _confirmPin = _confirmPin.Substring(0, _confirmPin.Length - 1);
                UpdateConfirmPinFields();
                // Clear messages when user deletes
                ClearMessages();
            }
            else
            {
                // If confirm PIN is empty, go back to entering first PIN
                // Keep confirm fields visible even when empty
                _isConfirmingPin = false;
                _confirmPin = string.Empty;
                UpdateConfirmPinFields(); // Clear confirm fields visually
                ClearMessages();
                
                // Refocus the last field of enter PIN for continued input
                if (_enterPinFields.Count > 0 && _currentPin.Length > 0)
                {
                    int lastFieldIndex = Math.Min(_currentPin.Length, _enterPinFields.Count - 1);
                    _enterPinFields[lastFieldIndex].FocusEntry();
                }
                else if (_enterPinFields.Count > 0)
                {
                    _enterPinFields[0].FocusEntry();
                }
            }
        }
        else
        {
            // Deleting from first PIN
            if (_currentPin.Length > 0)
            {
                _currentPin = _currentPin.Substring(0, _currentPin.Length - 1);
                UpdatePinFields();
                // Clear messages when user deletes
                ClearMessages();
                
                // Refocus the current field after deletion
                if (_enterPinFields.Count > 0)
                {
                    int currentFieldIndex = Math.Min(_currentPin.Length, _enterPinFields.Count - 1);
                    _enterPinFields[currentFieldIndex].FocusEntry();
                }
            }
        }
    }

    private void UpdatePinFields()
    {
        // Update all fields - Entry is bound to Digit, so it will display automatically
        for (int i = 0; i < _enterPinFields.Count; i++)
        {
            bool shouldBeFilled = i < _currentPin.Length;
            _enterPinFields[i].IsFilled = shouldBeFilled;

            // Update digit - Entry will display it (read-only for keypad, editable for keyboard)
            if (shouldBeFilled)
            {
                _enterPinFields[i].Digit = _currentPin[i].ToString();
            }
            else
            {
                _enterPinFields[i].Digit = string.Empty;
            }
        }
    }

    private void UpdateConfirmPinFields()
    {
        // Update all fields - Entry is bound to Digit, so it will display automatically
        for (int i = 0; i < _confirmPinFields.Count; i++)
        {
            bool shouldBeFilled = i < _confirmPin.Length;
            _confirmPinFields[i].IsFilled = shouldBeFilled;

            // Update digit - Entry will display it (read-only for keypad, editable for keyboard)
            if (shouldBeFilled)
            {
                _confirmPinFields[i].Digit = _confirmPin[i].ToString();
            }
            else
            {
                _confirmPinFields[i].Digit = string.Empty;
            }
        }
    }

    private void InitializePinFields()
    {
        // Clear existing fields
        EnterPinFieldsContainer.Children.Clear();
        _enterPinFields.Clear();

        // Create PIN digit fields based on TotalDigits constant
        for (int i = 0; i < KKPinviewConstant.TotalDigits; i++)
        {
            var field = new PinDigitField
            {
                // Apply constants for field appearance
                CornerRadius = KKPinviewConstant.FieldCornerRadius,
                UseRoundShape = KKPinviewConstant.UseRoundFields,
                FieldWidth = KKPinviewConstant.FieldWidth,
                FieldHeight = KKPinviewConstant.FieldHeight,
                BackgroundColor = KKPinviewConstant.DigitFieldBackgroundColor,
                TextColor = KKPinviewConstant.TextColor
            };

            _enterPinFields.Add(field);
            EnterPinFieldsContainer.Children.Add(field);
        }
    }

    private void InitializeConfirmPinFields()
    {
        // Clear existing fields
        ConfirmPinFieldsContainer.Children.Clear();
        _confirmPinFields.Clear();

        // Create confirm PIN digit fields based on TotalDigits constant
        for (int i = 0; i < KKPinviewConstant.TotalDigits; i++)
        {
            var field = new PinDigitField
            {
                // Apply constants for field appearance
                CornerRadius = KKPinviewConstant.FieldCornerRadius,
                UseRoundShape = KKPinviewConstant.UseRoundFields,
                FieldWidth = KKPinviewConstant.FieldWidth,
                FieldHeight = KKPinviewConstant.FieldHeight,
                BackgroundColor = KKPinviewConstant.DigitFieldBackgroundColor,
                TextColor = KKPinviewConstant.TextColor
            };

            _confirmPinFields.Add(field);
            ConfirmPinFieldsContainer.Children.Add(field);
        }
    }

    private void ValidatePinMatch()
    {
        // Clear previous messages
        ClearMessages();

        if (_currentPin == _confirmPin)
        {
            // PINs match - save the PIN and reset lockout
            bool saved = KKPinStorage.SavePIN(_currentPin);
            if (saved)
            {
                // Reset lockout manager when PIN is successfully set up
                var lockoutManager = new KKPinLockoutManager();
                lockoutManager.ResetFailedAttempts();
                ShowSuccessMessage(KKPinviewConstant.SetupSuccessMessage);

                // Invoke success callback
                _viewModel.OnSetupSuccess?.Invoke();
            }
            else
            {
                string errorMessage = "Failed to save PIN. Please try again.";
                ShowErrorMessage(errorMessage);

                // Invoke failure callback
                _viewModel.OnSetupFailed?.Invoke(errorMessage);
            }
        }
        else
        {
            // PINs don't match - show error message
            ShowErrorMessage(KKPinviewConstant.PinMismatchError);

            // Clear confirm PIN fields to allow re-entry
            _confirmPin = string.Empty;
            UpdateConfirmPinFields();

            // Invoke failure callback
            _viewModel.OnSetupFailed?.Invoke(KKPinviewConstant.PinMismatchError);
        }
    }

    private async void ShowErrorMessage(string message)
    {
        _viewModel.ErrorMessage = message;
        _viewModel.HasError = true;
        _viewModel.HasSuccessMessage = false;

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
        _viewModel.HasError = false;

        // Step 1: Animate keypad down first (chained animation)
        if (Keypad != null)
        {
            await Keypad.TranslateToAsync(0, 40, 300, Easing.CubicOut);
        }

        // Step 2: Then show success message animation after keypad animation completes
        if (SuccessMessageLabel != null)
        {
            SuccessMessageLabel.Opacity = 0;
            SuccessMessageLabel.Scale = 0.3; // Start from smaller scale
            _viewModel.HasSuccessMessage = true;
            
            // Run fade and scale animations simultaneously
            await Task.WhenAll(
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

        if (SuccessMessageLabel != null && _viewModel.HasSuccessMessage)
        {
            await SuccessMessageLabel.FadeToAsync(0, 200);
        }

        _viewModel.HasError = false;
        _viewModel.HasSuccessMessage = false;
        _viewModel.ErrorMessage = string.Empty;
        _viewModel.SuccessMessage = string.Empty;
    }

    // Event handlers for keyboard input in PIN fields (when InputMethod is SystemKeyboard)
    private void OnEnterPinFieldDigitChanged(object? sender, string digit)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _enterPinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // Rebuild PIN from all fields
        _currentPin = string.Empty;
        foreach (var pinField in _enterPinFields)
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
            // Digit entered - move to next field or switch to confirm mode
            if (fieldIndex < _enterPinFields.Count - 1)
            {
                _enterPinFields[fieldIndex + 1].FocusEntry();
            }
            else if (_currentPin.Length == _viewModel.MaxPinLength)
            {
                _isConfirmingPin = true;
                _viewModel.ShowConfirmPin = true;
                if (_confirmPinFields.Count > 0)
                {
                    _confirmPinFields[0].FocusEntry();
                }
            }
        }
        else
        {
            // Digit deleted (empty) - move focus backward
            if (fieldIndex > 0)
            {
                _enterPinFields[fieldIndex - 1].FocusEntry();
            }
            else
            {
                // Already at first field, keep focus here
                _enterPinFields[0].FocusEntry();
            }
        }
    }

    private void OnEnterPinFieldCompleted(object? sender, EventArgs e)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _enterPinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // Move to next field
        if (fieldIndex < _enterPinFields.Count - 1)
        {
            _enterPinFields[fieldIndex + 1].FocusEntry();
        }
        else if (_currentPin.Length == _viewModel.MaxPinLength)
        {
            _isConfirmingPin = true;
            _viewModel.ShowConfirmPin = true;
            if (_confirmPinFields.Count > 0)
            {
                _confirmPinFields[0].FocusEntry();
            }
        }
    }

    private void OnConfirmPinFieldDigitChanged(object? sender, string digit)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _confirmPinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // Rebuild confirm PIN from all fields
        _confirmPin = string.Empty;
        foreach (var pinField in _confirmPinFields)
        {
            if (!string.IsNullOrEmpty(pinField.Digit))
            {
                _confirmPin += pinField.Digit;
            }
        }

        UpdateConfirmPinFields();
        ClearMessages();

        // Handle focus movement based on input
        if (!string.IsNullOrEmpty(digit))
        {
            // Digit entered - move to next field or validate if complete
            if (fieldIndex < _confirmPinFields.Count - 1)
            {
                _confirmPinFields[fieldIndex + 1].FocusEntry();
            }
            else if (_confirmPin.Length == _viewModel.MaxPinLength)
            {
                ValidatePinMatch();
            }
        }
        else
        {
            // Digit deleted (empty) - move focus backward
            if (fieldIndex > 0)
            {
                _confirmPinFields[fieldIndex - 1].FocusEntry();
            }
            else if (fieldIndex == 0 && _confirmPin.Length == 0)
            {
                // If first confirm field is empty, go back to last enter PIN field
                _isConfirmingPin = false;
                if (_enterPinFields.Count > 0)
                {
                    int lastFieldIndex = Math.Max(0, _currentPin.Length - 1);
                    _enterPinFields[lastFieldIndex].FocusEntry();
                }
            }
            else
            {
                // Already at first field, keep focus here
                _confirmPinFields[0].FocusEntry();
            }
        }
    }

    private void OnConfirmPinFieldCompleted(object? sender, EventArgs e)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _confirmPinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // Move to next field or validate
        if (fieldIndex < _confirmPinFields.Count - 1)
        {
            _confirmPinFields[fieldIndex + 1].FocusEntry();
        }
        else if (_confirmPin.Length == _viewModel.MaxPinLength)
        {
            ValidatePinMatch();
        }
    }
}
