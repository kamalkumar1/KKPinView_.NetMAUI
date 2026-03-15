using System.Collections.ObjectModel;
using System.Linq;
using KKPinView.Constants;
using KKPinView.Helpers;
using KKPinView.Security;
using KKPinView.Storage;
using KKPinView.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace KKPinView.Views;

public sealed partial class KKPINSetUPView : ContentView, IDisposable
{
    private bool _disposed;
    private readonly ObservableCollection<PinDigitField> _enterPinFields = new();
    private readonly ObservableCollection<PinDigitField> _confirmPinFields = new();
    private readonly KKPINSetUPViewModel _viewModel;
    private string _currentPin = string.Empty;
    private string _confirmPin = string.Empty;
    private bool _isConfirmingPin = false;
    /// <summary>Index of the enter-PIN field that had focus when keyboard was last closed; -1 means use first field.</summary>
    private int _lastFocusedEnterIndex = -1;
    /// <summary>Index of the confirm-PIN field that had focus when keyboard was last closed; -1 means use first field.</summary>
    private int _lastFocusedConfirmIndex = -1;
    private readonly System.Windows.Input.ICommand _focusFirstEmptyEnterCommand;
    private readonly System.Windows.Input.ICommand _focusFirstEmptyConfirmCommand;
    /// <summary>Tap anywhere (enter or confirm area): focus first empty field in the current step so digits always continue in order.</summary>
    private readonly System.Windows.Input.ICommand _focusFirstEmptyInCurrentStepCommand;

    public KKPINSetUPViewModel ViewModel => _viewModel;

    public KKPINSetUPView()
    {
        _focusFirstEmptyEnterCommand = new Command(FocusFirstEmptyEnterField);
        _focusFirstEmptyConfirmCommand = new Command(FocusFirstEmptyConfirmField);
        _focusFirstEmptyInCurrentStepCommand = new Command(FocusFirstEmptyInCurrentStep);
        InitializeComponent();

        // Create and set ViewModel
        _viewModel = new KKPINSetUPViewModel();
        BindingContext = _viewModel;

        // Initialize UI elements after page is loaded
        Loaded += OnPageLoaded;
    }

    /// <summary>Focus the first empty enter-PIN field so the next digit goes there. Used by TapCommand; delete/backspace focus is set explicitly in OnEnterPinFieldDeleted.</summary>
    private void FocusFirstEmptyEnterField()
    {
        if (_enterPinFields.Count == 0) return;
        var digits = _enterPinFields.Select(f => f.Digit).ToList();
        int idx = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, _enterPinFields.Count);
        _enterPinFields[idx].FocusEntry();
    }

    /// <summary>Focus the first empty confirm-PIN field. Used by TapCommand; delete/backspace focus is set explicitly in OnConfirmPinFieldDeleted.</summary>
    private void FocusFirstEmptyConfirmField()
    {
        if (_confirmPinFields.Count == 0) return;
        var digits = _confirmPinFields.Select(f => f.Digit).ToList();
        int idx = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, _confirmPinFields.Count);
        _confirmPinFields[idx].FocusEntry();
    }

    /// <summary>Focus first empty field in the current step: Enter PIN when not yet confirming, Confirm PIN when confirming. So tapping anywhere (including on Confirm fields) keeps input in the correct section.</summary>
    private void FocusFirstEmptyInCurrentStep()
    {
        if (_isConfirmingPin)
            FocusFirstEmptyConfirmField();
        else
            FocusFirstEmptyEnterField();
    }

    /// <summary>Gets the label text for entering the PIN. Change only via <see cref="Constants.KKPinviewConstant.EnterPinMessage"/>.</summary>
    public string EnterPinLabelText => _viewModel.EnterPinLabelText;

    /// <summary>Gets the label text for confirming the PIN. Change only via <see cref="Constants.KKPinviewConstant.ConfirmPinMessage"/>.</summary>
    public string ConfirmPinLabelText => _viewModel.ConfirmPinLabelText;

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

    /// <summary>
    /// Focuses the first PIN field so the keyboard appears. Call from <see cref="OnCreationCompleted"/> or when you want to show the keyboard.
    /// </summary>
    public void ShowKeyboard()
    {
        if (_isConfirmingPin && _confirmPinFields.Count > 0)
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() => _confirmPinFields[0].FocusEntry());
            });
        }
        else if (_enterPinFields.Count > 0)
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
            {
                MainThread.BeginInvokeOnMainThread(() => _enterPinFields[0].FocusEntry());
            });
        }
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
            OnCreationCompleted?.Invoke();
        }
    }

    /// <summary>
    /// Callback invoked when the PIN setup view is fully created and ready. Use this to call <see cref="ShowKeyboard"/> or perform other setup.
    /// </summary>
    public Action? OnCreationCompleted { get; set; }

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
                FieldShapeType = KKPinviewConstant.FieldShapeType,
                FieldWidth = KKPinviewConstant.FieldWidth,
                FieldHeight = KKPinviewConstant.FieldHeight,
                BackgroundColor = KKPinviewConstant.DigitFieldBackgroundColor,
                TextColor = KKPinviewConstant.TextColor,
                IsSecure = KKPinviewConstant.PinFieldIsSecure
            };
            if (KKPinviewConstant.FieldShapeType == KKPinFieldShapeType.RoundedRectangle)
                field.CornerRadius = KKPinviewConstant.FieldCornerRadius;
            field.TapCommand = _focusFirstEmptyInCurrentStepCommand;

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
                FieldShapeType = KKPinviewConstant.FieldShapeType,
                FieldWidth = KKPinviewConstant.FieldWidth,
                FieldHeight = KKPinviewConstant.FieldHeight,
                BackgroundColor = KKPinviewConstant.DigitFieldBackgroundColor,
                TextColor = KKPinviewConstant.TextColor,
                IsSecure = KKPinviewConstant.PinFieldIsSecure
            };
            if (KKPinviewConstant.FieldShapeType == KKPinFieldShapeType.RoundedRectangle)
                field.CornerRadius = KKPinviewConstant.FieldCornerRadius;
            field.TapCommand = _focusFirstEmptyInCurrentStepCommand;

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
                ShowErrorMessage(KKPinviewConstant.SetupSaveFailedMessage);

                // Add delay before triggering failure callback
                Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
                {
                    _viewModel.OnSetupFailed?.Invoke(KKPinviewConstant.SetupSaveFailedMessage);
                });
            }
        }
        else
        {
            _ = RunPinMismatchErrorSequenceAsync();
        }
    }

    /// <summary>Runs the PIN mismatch flow: show error with animation, hold, fade out error, then reset all PIN fields. Keeps OnSetupFailed callback behavior.</summary>
    private async Task RunPinMismatchErrorSequenceAsync()
    {
        try
        {
            // Close keyboard and allow any initial ClearMessages() to settle
            foreach (var f in _enterPinFields) f.UnfocusEntry();
            foreach (var f in _confirmPinFields) f.UnfocusEntry();
            await Task.Delay(50);

            // Animate all fields to red (invalid) border so user sees mismatch feedback
            const uint borderAnimationDurationMs = 220;
            foreach (var f in _enterPinFields)
                f.AnimateBorderToColor(KKPinviewConstant.InvalidPinBorderColor, borderAnimationDurationMs, Easing.CubicOut);
            foreach (var f in _confirmPinFields)
                f.AnimateBorderToColor(KKPinviewConstant.InvalidPinBorderColor, borderAnimationDurationMs, Easing.CubicOut);

            // Notify host immediately so existing functionality (e.g. analytics) is unchanged
            _viewModel.OnSetupFailed?.Invoke(KKPinviewConstant.PinMismatchError);

            // Show error with clean fade + scale-in animation
            await ShowErrorMessageAsync(KKPinviewConstant.PinMismatchError);

            // Hold error visible so user can read it, then fade out
            await Task.Delay(KKPinviewConstant.PinMismatchErrorDisplayDurationMs);

            // Fade out error message
            await ClearMessagesAsync();

            // Reset all PINs after error animation is complete (on UI thread)
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    _currentPin = string.Empty;
                    _confirmPin = string.Empty;
                    foreach (var f in _enterPinFields)
                    {
                        f.BorderColor = KKPinviewConstant.DigitFieldEmptyBorderColor;
                        f.ClearDigitSilently();
                    }
                    foreach (var f in _confirmPinFields)
                    {
                        f.BorderColor = KKPinviewConstant.DigitFieldEmptyBorderColor;
                        f.ClearDigitSilently();
                    }
                    _isConfirmingPin = false;
                    if (_enterPinFields.Count > 0)
                        _enterPinFields[0].FocusEntry();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error clearing PIN fields: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"RunPinMismatchErrorSequence Exception: {ex.Message}");
        }
    }

    private async void ShowErrorMessage(string message)
    {
        await ShowErrorMessageAsync(message);
    }

    /// <summary>Shows the error message with a clean fade + scale-in animation. Returns when the animation completes.</summary>
    private async Task ShowErrorMessageAsync(string message)
    {
        SuccessMessageLabel.HeightRequest = 0;
        _viewModel.ErrorMessage = message;
        _viewModel.HasError = true;
        _viewModel.HasSuccessMessage = false;

        if (ErrorMessageLabel != null)
        {
            ErrorMessageLabel.Opacity = 0;
            ErrorMessageLabel.Scale = 0.3;
            ErrorMessageLabel.HeightRequest = 0;
            var heightAnimation = new Animation(v => ErrorMessageLabel.HeightRequest = v, 0, KKPinviewConstant.ErrorMessageLabelHeight, Easing.CubicOut);
            var heightTaskCompletionSource = new TaskCompletionSource<bool>();
            heightAnimation.Commit(ErrorMessageLabel, "height", 16, 300, Easing.CubicOut, (v, c) => heightTaskCompletionSource.SetResult(true));
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
        await ClearMessagesAsync();
    }

    /// <summary>Fades out error/success messages and clears view model. Returns when the animation completes.</summary>
    private async Task ClearMessagesAsync()
    {
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
        bool keyboardOpen = _enterPinFields.Any(f => f.IsEntryFocused) || _confirmPinFields.Any(f => f.IsEntryFocused);
        if (keyboardOpen)
        {
            for (int i = 0; i < _enterPinFields.Count; i++)
                if (_enterPinFields[i].IsEntryFocused) { _lastFocusedEnterIndex = i; break; }
            for (int i = 0; i < _confirmPinFields.Count; i++)
                if (_confirmPinFields[i].IsEntryFocused) { _lastFocusedConfirmIndex = i; break; }
            // foreach (var f in _enterPinFields) f.UnfocusEntry();
            // foreach (var f in _confirmPinFields) f.UnfocusEntry();
        }
        else
        {
            if (_isConfirmingPin && _confirmPinFields.Count > 0)
            {
                int idx = _lastFocusedConfirmIndex;
                if (idx < 0 || idx >= _confirmPinFields.Count) idx = 0;
                _confirmPinFields[idx].FocusEntry();
            }
            else if (_enterPinFields.Count > 0)
            {
                int idx = _lastFocusedEnterIndex;
                if (idx < 0 || idx >= _enterPinFields.Count) idx = 0;
                _enterPinFields[idx].FocusEntry();
            }
        }
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

        if (!currentFieldHadDigit && fieldIndex > 0)
        {
            // Current was empty - clear previous field so backspace removes the digit to the left
            _enterPinFields[fieldIndex - 1].ClearDigitSilently();
        }
        else if (!currentFieldHadDigit && fieldIndex == 0)
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

        // Focus first empty field so the next digit the user types goes in the correct box (re-entry after partial delete)
        int firstEmptyIndex = Math.Min(_currentPin.Length, _enterPinFields.Count - 1);
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _enterPinFields[firstEmptyIndex].FocusEntry();
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
                        // Focus first empty enter field so next digit goes in the correct box
                        int firstEmpty = Math.Min(_currentPin.Length, _enterPinFields.Count - 1);
                        _enterPinFields[firstEmpty].FocusEntry();
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
                        int firstEmpty = Math.Min(_currentPin.Length, _enterPinFields.Count - 1);
                        _enterPinFields[firstEmpty].FocusEntry();
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

    /// <summary>
    /// Clears PIN values from memory and releases resources. Call before dismissing the page for security.
    /// Safe to call multiple times.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        Loaded -= OnPageLoaded;

        foreach (var field in _enterPinFields)
        {
            field.DigitChanged -= OnEnterPinFieldDigitChanged;
            field.DigitCompleted -= OnEnterPinFieldCompleted;
            field.DigitDeleted -= OnEnterPinFieldDeleted;
            field.ClearDigitSilently();
        }

        foreach (var field in _confirmPinFields)
        {
            field.DigitChanged -= OnConfirmPinFieldDigitChanged;
            field.DigitCompleted -= OnConfirmPinFieldCompleted;
            field.DigitDeleted -= OnConfirmPinFieldDeleted;
            field.ClearDigitSilently();
        }

        _currentPin = string.Empty;
        _confirmPin = string.Empty;
        _viewModel.HasError = false;
        _viewModel.HasSuccessMessage = false;
        _viewModel.ErrorMessage = string.Empty;
        _viewModel.SuccessMessage = string.Empty;
        _viewModel.Dispose();
    }
}
