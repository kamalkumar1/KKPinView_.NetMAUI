using System.Collections.ObjectModel;
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

        // Initialize UI elements after page is loaded
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

        InitializePinFields();
        InitializeConfirmPinFields();

        // Add a small delay to ensure fields are fully loaded in the visual tree
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(50), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SetupPinFields();
            });
        });
    }

    private void SetupPinFields()
    {
        foreach (var field in _enterPinFields)
        {
            field.DigitChanged -= OnEnterPinFieldDigitChanged;
            field.DigitCompleted -= OnEnterPinFieldCompleted;
            field.DigitDeleted -= OnEnterPinFieldDeleted;

            field.DigitChanged += OnEnterPinFieldDigitChanged;
            field.DigitCompleted += OnEnterPinFieldCompleted;
            field.DigitDeleted += OnEnterPinFieldDeleted;
        }

        foreach (var field in _confirmPinFields)
        {
            field.DigitChanged -= OnConfirmPinFieldDigitChanged;
            field.DigitCompleted -= OnConfirmPinFieldCompleted;
            field.DigitDeleted -= OnConfirmPinFieldDeleted;

            field.DigitChanged += OnConfirmPinFieldDigitChanged;
            field.DigitCompleted += OnConfirmPinFieldCompleted;
            field.DigitDeleted += OnConfirmPinFieldDeleted;
        }

        if (_enterPinFields.Count > 0)
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

        // Create PIN digit fields based on TotalPinTextFields constant
        for (int i = 0; i < KKPinviewConstant.TotalPinTextFields; i++)
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

        // Create confirm PIN digit fields based on TotalPinTextFields constant
        for (int i = 0; i < KKPinviewConstant.TotalPinTextFields; i++)
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

    private void OnRootTapped(object? sender, TappedEventArgs e)
    {
        foreach (var f in _enterPinFields) f.UnfocusEntry();
        foreach (var f in _confirmPinFields) f.UnfocusEntry();
    }

    // Event handlers for system keyboard input in PIN fields
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
            if (fieldIndex < _enterPinFields.Count - 1)
            {
                _enterPinFields[fieldIndex + 1].FocusEntry();
            }
            else if (_currentPin.Length == _enterPinFields.Count)
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
        else if (_currentPin.Length == _enterPinFields.Count)
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

        // PinDigitField already cleared current field before invoking. Use _currentPin to detect
        // if it had a digit (we haven't rebuilt yet).
        bool currentFieldHadDigit = _currentPin.Length > fieldIndex;
        int fieldToFocus;

        if (currentFieldHadDigit)
        {
            // Current had a digit - already cleared; move focus to previous field
            fieldToFocus = fieldIndex > 0 ? fieldIndex - 1 : 0;
        }
        else if (fieldIndex > 0)
        {
            // Current was empty - clear previous and move focus to it
            _enterPinFields[fieldIndex - 1].ClearDigitSilently();
            fieldToFocus = fieldIndex - 1;
        }
        else
        {
            // First field and empty - nothing to delete
            return;
        }

        // Rebuild PIN from all fields
        _currentPin = string.Empty;
        foreach (var pinField in _enterPinFields)
        {
            if (!string.IsNullOrEmpty(pinField.Digit))
                _currentPin += pinField.Digit;
        }

        ClearMessages();

        // Move focus
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
            else if (_confirmPin.Length == _confirmPinFields.Count)
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
        else if (_confirmPin.Length == _confirmPinFields.Count)
        {
            ValidatePinMatch();
        }
    }

    private void OnConfirmPinFieldDeleted(object? sender, EventArgs e)
    {
        if (sender is not PinDigitField field) return;

        int fieldIndex = _confirmPinFields.IndexOf(field);
        if (fieldIndex < 0) return;

        // PinDigitField already cleared current. Use _confirmPin to detect if it had a digit.
        bool currentFieldHadDigit = _confirmPin.Length > fieldIndex;
        int fieldToFocus;

        if (currentFieldHadDigit)
        {
            // Current had a digit - already cleared; move focus to previous field
            fieldToFocus = fieldIndex > 0 ? fieldIndex - 1 : 0;
        }
        else if (fieldIndex > 0)
        {
            // Current was empty - clear previous and move focus to it
            _confirmPinFields[fieldIndex - 1].ClearDigitSilently();
            fieldToFocus = fieldIndex - 1;
        }
        else
        {
            // First confirm field and empty - check if we should go back to enter PIN
            bool allEmpty = true;
            foreach (var pf in _confirmPinFields)
            {
                if (!string.IsNullOrEmpty(pf.Digit)) { allEmpty = false; break; }
            }
            if (!allEmpty) return;

            _isConfirmingPin = false;
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_enterPinFields.Count > 0)
                    {
                        int last = Math.Max(0, _currentPin.Length - 1);
                        _enterPinFields[last].FocusEntry();
                    }
                });
            });
            return;
        }

        // Rebuild _confirmPin
        _confirmPin = string.Empty;
        foreach (var pf in _confirmPinFields)
        {
            if (!string.IsNullOrEmpty(pf.Digit)) _confirmPin += pf.Digit;
        }
        ClearMessages();

        // All confirm empty and we're on first -> go back to enter PIN
        bool allConfirmEmpty = true;
        foreach (var pf in _confirmPinFields)
        {
            if (!string.IsNullOrEmpty(pf.Digit)) { allConfirmEmpty = false; break; }
        }
        if (allConfirmEmpty && fieldToFocus == 0)
        {
            _isConfirmingPin = false;
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_enterPinFields.Count > 0)
                    {
                        int last = Math.Max(0, _currentPin.Length - 1);
                        _enterPinFields[last].FocusEntry();
                    }
                });
            });
            return;
        }

        // Move focus to previous field
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _confirmPinFields[fieldToFocus].FocusEntry();
            });
        });
    }
}
