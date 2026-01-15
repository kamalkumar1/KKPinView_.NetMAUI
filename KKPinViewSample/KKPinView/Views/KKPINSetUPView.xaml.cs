using System.Collections.ObjectModel;
using System.Windows.Input;
using KKPinView.Constants;
using KKPinView.Security;
using KKPinView.Storage;

namespace KKPinView.Views;

public partial class KKPINSetUPView : ContentPage
{
    private readonly ObservableCollection<PinDigitField> _pinFields = new();
    private string _currentPin = string.Empty;
    private string _firstPin = string.Empty;
    private bool _isConfirming = false;
    
    public static readonly BindableProperty OnSetupCompleteProperty = BindableProperty.Create(
        nameof(OnSetupComplete), typeof(Action<string>), typeof(KKPINSetUPView));
    
    public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
        nameof(BackgroundColor), typeof(Color), typeof(KKPINSetUPView), KKPinviewConstant.BackgroundColor);
    
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(KKPINSetUPView), KKPinviewConstant.TextColor);
    
    public static readonly BindableProperty ErrorTextColorProperty = BindableProperty.Create(
        nameof(ErrorTextColor), typeof(Color), typeof(KKPINSetUPView), KKPinviewConstant.ErrorTextColor);
    
    public static readonly BindableProperty SuccessTextColorProperty = BindableProperty.Create(
        nameof(SuccessTextColor), typeof(Color), typeof(KKPINSetUPView), KKPinviewConstant.SuccessTextColor);
    
    public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(
        nameof(TitleFontSize), typeof(double), typeof(KKPINSetUPView), KKPinviewConstant.TitleFontSize);
    
    public static readonly BindableProperty SubtitleFontSizeProperty = BindableProperty.Create(
        nameof(SubtitleFontSize), typeof(double), typeof(KKPINSetUPView), KKPinviewConstant.SubtitleFontSize);
    
    public static readonly BindableProperty FieldSpacingProperty = BindableProperty.Create(
        nameof(FieldSpacing), typeof(double), typeof(KKPINSetUPView), KKPinviewConstant.FieldSpacing);
    
    public Action<string>? OnSetupComplete
    {
        get => (Action<string>?)GetValue(OnSetupCompleteProperty);
        set => SetValue(OnSetupCompleteProperty, value);
    }
    
    public Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }
    
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }
    
    public Color ErrorTextColor
    {
        get => (Color)GetValue(ErrorTextColorProperty);
        set => SetValue(ErrorTextColorProperty, value);
    }
    
    public Color SuccessTextColor
    {
        get => (Color)GetValue(SuccessTextColorProperty);
        set => SetValue(SuccessTextColorProperty, value);
    }
    
    public double TitleFontSize
    {
        get => (double)GetValue(TitleFontSizeProperty);
        set => SetValue(TitleFontSizeProperty, value);
    }
    
    public double SubtitleFontSize
    {
        get => (double)GetValue(SubtitleFontSizeProperty);
        set => SetValue(SubtitleFontSizeProperty, value);
    }
    
    public double FieldSpacing
    {
        get => (double)GetValue(FieldSpacingProperty);
        set => SetValue(FieldSpacingProperty, value);
    }
    
    public string TitleText { get; private set; } = KKPinviewConstant.SetupTitleText;
    public string SubtitleText { get; private set; } = KKPinviewConstant.EnterPinMessage;
    public string ErrorMessage { get; private set; } = string.Empty;
    public string SuccessMessage { get; private set; } = string.Empty;
    public bool HasError { get; private set; }
    public bool HasSuccessMessage { get; private set; }
    
    public ICommand NumberCommand { get; }
    public ICommand DeleteCommand { get; }
    
    public KKPINSetUPView()
    {
        InitializeComponent();
        
        NumberCommand = new Command<string>(OnNumberPressed);
        DeleteCommand = new Command(OnDeletePressed);
        
        BindingContext = this;
        InitializePinFields();
    }
    
    private void InitializePinFields()
    {
        PinFieldsContainer.Children.Clear();
        _pinFields.Clear();
        _currentPin = string.Empty;
        _firstPin = string.Empty;
        _isConfirming = false;
        TitleText = KKPinviewConstant.SetupTitleText;
        SubtitleText = KKPinviewConstant.EnterPinMessage;
        
        for (int i = 0; i < KKPinviewConstant.TotalDigits; i++)
        {
            var field = new PinDigitField();
            _pinFields.Add(field);
            PinFieldsContainer.Children.Add(field);
        }
        
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(SubtitleText));
    }
    
    private void OnNumberPressed(string number)
    {
        if (_currentPin.Length >= KKPinviewConstant.TotalDigits)
            return;
        
        _currentPin += number;
        UpdatePinFields();
        
        if (_currentPin.Length == KKPinviewConstant.TotalDigits)
        {
            ProcessPinEntry();
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
    
    private void ProcessPinEntry()
    {
        if (!_isConfirming)
        {
            // First PIN entry
            _firstPin = _currentPin;
            _isConfirming = true;
            TitleText = KKPinviewConstant.ConfirmPinTitleText;
            SubtitleText = KKPinviewConstant.ConfirmPinMessage;
            ClearPin();
            
            OnPropertyChanged(nameof(TitleText));
            OnPropertyChanged(nameof(SubtitleText));
        }
        else
        {
            // Confirmation PIN entry
            if (_currentPin == _firstPin)
            {
                // PINs match - save and complete
                var lockoutManager = new KKPinLockoutManager();
                lockoutManager.ResetFailedAttempts();
                KKPinStorage.DeletePIN(); // Clear any existing PIN first
                
                if (KKPinStorage.SavePIN(_currentPin))
                {
                    ShowSuccessMessage(KKPinviewConstant.SetupSuccessMessage);
                    OnSetupComplete?.Invoke(_currentPin);
                    
                    // Clear after a delay
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ClearPin();
                            InitializePinFields();
                        });
                    });
                }
                else
                {
                    ShowErrorMessage("Failed to save PIN. Please try again.");
                    ClearPin();
                    InitializePinFields();
                }
            }
            else
            {
                // PINs don't match
                ShowErrorMessage(KKPinviewConstant.PinMismatchError);
                ClearPin();
                InitializePinFields();
            }
        }
    }
    
    private void ShowErrorMessage(string message)
    {
        ErrorMessage = message;
        HasError = true;
        HasSuccessMessage = false;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(HasSuccessMessage));
    }
    
    private void ShowSuccessMessage(string message)
    {
        SuccessMessage = message;
        HasSuccessMessage = true;
        HasError = false;
        OnPropertyChanged(nameof(SuccessMessage));
        OnPropertyChanged(nameof(HasSuccessMessage));
        OnPropertyChanged(nameof(HasError));
    }
    
    private void ClearMessages()
    {
        HasError = false;
        HasSuccessMessage = false;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(HasSuccessMessage));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(SuccessMessage));
    }
    
    private void ClearPin()
    {
        _currentPin = string.Empty;
        UpdatePinFields();
        ClearMessages();
    }
}

