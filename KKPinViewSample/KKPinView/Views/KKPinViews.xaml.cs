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
        UpdateUI();
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
        _viewModel.HasSuccessMessage = true;
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
