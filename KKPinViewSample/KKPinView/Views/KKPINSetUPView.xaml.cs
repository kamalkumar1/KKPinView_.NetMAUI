using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using KKPinView.Constants;
using KKPinView.Security;
using KKPinView.Storage;
using KKPinView.ViewModels;
using Microsoft.Maui.ApplicationModel;

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

        // Set keypad visibility to false by default
        if (Keypad != null)
        {
            Keypad.IsVisible = false;
        }


        // Set up input method visibility and initialize UI elements after page is loaded
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
        // Initialize UI elements after page is fully loaded
        if (SuccessMessageLabel != null)
        {
            SuccessMessageLabel.HeightRequest = 0;
        }

        // Wire up ViewModel events
        _viewModel.NumberPressed += OnViewModelNumberPressed;
        _viewModel.DeletePressed += OnViewModelDeletePressed;

        // Initialize PIN fields based on TotalDigits constant
        InitializePinFields();
        InitializeConfirmPinFields();

        //if (KKPinviewConstant.InputMethod == PinInputMethod.s && Keypad != null)
        //{
        Keypad.NumberCommand = _viewModel.NumberCommand;
        Keypad.DeleteCommand = _viewModel.DeleteCommand;
        // }

        // Ensure input method is set up AFTER fields are initialized
        // Add a small delay to ensure fields are fully loaded in the visual tree
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(50), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SetupInputMethod();
            });
        });
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
            field.DigitDeleted -= OnEnterPinFieldDeleted;

            // Set IsEditable - this will trigger UpdateEditableState via property changed handler
            field.IsEditable = isEditable;

            if (isEditable)
            {
                field.DigitChanged += OnEnterPinFieldDigitChanged;
                field.DigitCompleted += OnEnterPinFieldCompleted;
                field.DigitDeleted += OnEnterPinFieldDeleted;
            }
        }

        foreach (var field in _confirmPinFields)
        {
            field.DigitChanged -= OnConfirmPinFieldDigitChanged;
            field.DigitCompleted -= OnConfirmPinFieldCompleted;
            field.DigitDeleted -= OnConfirmPinFieldDeleted;

            // Set IsEditable - this will trigger UpdateEditableState via property changed handler
            field.IsEditable = isEditable;

            if (isEditable)
            {
                field.DigitChanged += OnConfirmPinFieldDigitChanged;
                field.DigitCompleted += OnConfirmPinFieldCompleted;
                field.DigitDeleted += OnConfirmPinFieldDeleted;
            }
        }

        // Focus first field if using keyboard (with a small delay to ensure UI is ready)
        if (isEditable && _enterPinFields.Count > 0)
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(200), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _enterPinFields[0].FocusEntry();
                });
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

            // Clear any previous messages when user starts typing again
            ClearMessages();

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
        try
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UpdatePinFields Exception: {ex.Message}");
        }

    }

    private void UpdateConfirmPinFields()
    {
        try
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateConfirmPinFields Exception: {ex.Message}");
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

                // Add delay before triggering success callback
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                {
                    _viewModel.OnSetupSuccess?.Invoke();
                });
            }
            else
            {
                string errorMessage = "Failed to save PIN. Please try again.";
                ShowErrorMessage(errorMessage);

                // Add delay before triggering failure callback
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                {
                    _viewModel.OnSetupFailed?.Invoke(errorMessage);
                });
            }
        }
        else
        {
            try
            {
                // Brief delay before showing error to improve UX
                // PINs don't match - show error message
                ShowErrorMessage(KKPinviewConstant.PinMismatchError);

                // Clear both Enter PIN and Confirm PIN fields asynchronously to avoid blocking UI
                // Add delay to ensure text input processing is complete before clearing fields
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(150), () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        try
                        {
                            // _currentPin = string.Empty;
                            // _confirmPin = string.Empty;
                            // UpdatePinFields();
                            // UpdateConfirmPinFields();

                            // Reset to enter PIN mode (but keep confirm section visible)
                            _isConfirmingPin = false;

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error clearing PIN fields: {ex.Message}");
                        }
                    });
                });

                // Add delay before triggering failure callback
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                {
                    _viewModel.OnSetupFailed?.Invoke(KKPinviewConstant.PinMismatchError);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ValidatePinMatch Exception: {ex.Message}");
                // Ignore any exceptions from delay
            }

        }
    }

    private async void ShowErrorMessage(string message)
    {
        SuccessMessageLabel.HeightRequest = 0;
        _viewModel.ErrorMessage = message;
        _viewModel.HasError = true;
        _viewModel.HasSuccessMessage = false;

        // Animate error message appearance - fade and scale simultaneously
        if (ErrorMessageLabel != null)
        {
            ErrorMessageLabel.Opacity = 0;
            ErrorMessageLabel.Scale = 0.3; // Start from smaller scale
            ErrorMessageLabel.HeightRequest = 0;
            var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, 0, KKPinviewConstant.ErrorMessageLabelHeight, Easing.CubicOut);
            var heightTaskCompletionSource = new TaskCompletionSource<bool>();
            heightAnimation.Commit(ErrorMessageLabel, "height", 16, 300, Easing.CubicOut, (v, c) => heightTaskCompletionSource.SetResult(true));
            // Run fade and scale animations simultaneously
            await Task.WhenAll(
                heightTaskCompletionSource.Task,
                ErrorMessageLabel.FadeToAsync(1, 300, Easing.CubicOut),
                ErrorMessageLabel.ScaleToAsync(1, 400, Easing.SpringOut)
            );
        }
    }

    private async void ShowSuccessMessage(string message)
    {
        ErrorMessageLabel.HeightRequest = 0;
        _viewModel.SuccessMessage = message;
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
            var currentHeight = ErrorMessageLabel.HeightRequest;
            var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, currentHeight, 0, Easing.CubicIn);
            var heightTaskCompletionSource = new TaskCompletionSource<bool>();
            heightAnimation.Commit(ErrorMessageLabel, "height", 16, 200, Easing.CubicIn, (v, c) => heightTaskCompletionSource.SetResult(true));
            await Task.WhenAll(
               heightTaskCompletionSource.Task,
               ErrorMessageLabel.FadeToAsync(0, 200)
           );

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
            // Digit deleted (empty) - but focus movement is handled by DigitDeleted event
            // So we don't move focus here to avoid conflicts
            // Just keep focus on current field
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

    private void OnEnterPinFieldDeleted(object? sender, EventArgs e)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _enterPinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        int fieldToFocus = fieldIndex;

        // Determine which field to clear and where to move focus
        if (!string.IsNullOrEmpty(field.Digit))
        {
            // Current field has a digit - clear it and move to previous field
            field.Digit = string.Empty;
            fieldToFocus = fieldIndex > 0 ? fieldIndex - 1 : 0;
        }
        else if (fieldIndex > 0)
        {
            // Current field is empty - clear the previous field
            _enterPinFields[fieldIndex - 1].Digit = string.Empty;
            fieldToFocus = fieldIndex > 1 ? fieldIndex - 2 : 0;
        }
        else
        {
            // Already at first field and it's empty - nothing to delete
            return;
        }

        // Rebuild PIN from all fields (after clearing)
        _currentPin = string.Empty;
        foreach (var pinField in _enterPinFields)
        {
            if (!string.IsNullOrEmpty(pinField.Digit))
            {
                _currentPin += pinField.Digit;
            }
        }

        // Don't call UpdatePinFields() here - it will interfere with focus
        // The Digit property change already updates the field display
        ClearMessages();

        // Move focus to the appropriate field
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _enterPinFields[fieldToFocus].FocusEntry();
            });
        });
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
            // Digit deleted (empty) - but focus movement is handled by DigitDeleted event
            // So we don't move focus here to avoid conflicts
            // Just keep focus on current field
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

    private void OnConfirmPinFieldDeleted(object? sender, EventArgs e)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _confirmPinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        int fieldToFocus = fieldIndex;

        // Determine which field to clear and where to move focus
        if (!string.IsNullOrEmpty(field.Digit))
        {
            // Current field has a digit - clear it and move to previous field
            field.Digit = string.Empty;
            fieldToFocus = fieldIndex > 0 ? fieldIndex - 1 : 0;
        }
        else if (fieldIndex > 0)
        {
            // Current field is empty - clear the previous field
            _confirmPinFields[fieldIndex - 1].Digit = string.Empty;
            fieldToFocus = fieldIndex > 1 ? fieldIndex - 2 : 0;
        }
        else
        {
            // We're on the first confirm field and it's empty
            // Check if all confirm fields are empty
            bool allFieldsEmpty = true;
            foreach (var pinField in _confirmPinFields)
            {
                if (!string.IsNullOrEmpty(pinField.Digit))
                {
                    allFieldsEmpty = false;
                    break;
                }
            }

            if (allFieldsEmpty)
            {
                // All confirm fields are empty, go back to last enter PIN field
                _isConfirmingPin = false;
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (_enterPinFields.Count > 0)
                        {
                            int lastFieldIndex = Math.Max(0, _currentPin.Length - 1);
                            _enterPinFields[lastFieldIndex].FocusEntry();
                        }
                    });
                });
            }
            // If not all empty, do nothing - stay on first field
            return;
        }

        // Rebuild confirm PIN from all fields (after clearing)
        _confirmPin = string.Empty;
        foreach (var pinField in _confirmPinFields)
        {
            if (!string.IsNullOrEmpty(pinField.Digit))
            {
                _confirmPin += pinField.Digit;
            }
        }

        // Don't call UpdateConfirmPinFields() here - it will interfere with focus
        // The Digit property change already updates the field display
        ClearMessages();

        // Check if all confirm fields are now empty after deletion
        bool allConfirmFieldsEmpty = true;
        foreach (var pinField in _confirmPinFields)
        {
            if (!string.IsNullOrEmpty(pinField.Digit))
            {
                allConfirmFieldsEmpty = false;
                break;
            }
        }

        if (allConfirmFieldsEmpty && fieldToFocus == 0)
        {
            // All confirm fields are empty, go back to last enter PIN field
            _isConfirmingPin = false;
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_enterPinFields.Count > 0)
                    {
                        int lastFieldIndex = Math.Max(0, _currentPin.Length - 1);
                        _enterPinFields[lastFieldIndex].FocusEntry();
                    }
                });
            });
        }
        else
        {
            // Move focus to the appropriate field
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _confirmPinFields[fieldToFocus].FocusEntry();
                });
            });
        }
    }
}
