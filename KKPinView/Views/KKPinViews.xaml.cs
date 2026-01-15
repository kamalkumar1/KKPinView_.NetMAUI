using System.Collections.ObjectModel;
using System.Windows.Input;
using KKPinView.Constants;
using KKPinView.Security;
using KKPinView.Storage;

namespace KKPinView.Views;

public partial class KKPinViews : ContentPage
{
    private readonly ObservableCollection<PinDigitField> _pinFields = new();
    private readonly KKPinLockoutManager _lockoutManager;
    private string _currentPin = string.Empty;
    
    public static readonly BindableProperty OnForgotPinProperty = BindableProperty.Create(
        nameof(OnForgotPin), typeof(Action), typeof(KKPinViews));
    
    public static readonly BindableProperty OnSubmitProperty = BindableProperty.Create(
        nameof(OnSubmit), typeof(Action<bool>), typeof(KKPinViews));
    
    public static readonly BindableProperty ShowForgotPinProperty = BindableProperty.Create(
        nameof(ShowForgotPin), typeof(bool), typeof(KKPinViews), true);
    
    public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
        nameof(BackgroundColor), typeof(Color), typeof(KKPinViews), KKPinviewConstant.BackgroundColor);
    
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(KKPinViews), KKPinviewConstant.TextColor);
    
    public static readonly BindableProperty ErrorTextColorProperty = BindableProperty.Create(
        nameof(ErrorTextColor), typeof(Color), typeof(KKPinViews), KKPinviewConstant.ErrorTextColor);
    
    public static readonly BindableProperty SuccessTextColorProperty = BindableProperty.Create(
        nameof(SuccessTextColor), typeof(Color), typeof(KKPinViews), KKPinviewConstant.SuccessTextColor);
    
    public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(
        nameof(TitleFontSize), typeof(double), typeof(KKPinViews), KKPinviewConstant.TitleFontSize);
    
    public static readonly BindableProperty SubtitleFontSizeProperty = BindableProperty.Create(
        nameof(SubtitleFontSize), typeof(double), typeof(KKPinViews), KKPinviewConstant.SubtitleFontSize);
    
    public static readonly BindableProperty FieldSpacingProperty = BindableProperty.Create(
        nameof(FieldSpacing), typeof(double), typeof(KKPinViews), KKPinviewConstant.FieldSpacing);
    
    public Action? OnForgotPin
    {
        get => (Action?)GetValue(OnForgotPinProperty);
        set => SetValue(OnForgotPinProperty, value);
    }
    
    public Action<bool>? OnSubmit
    {
        get => (Action<bool>?)GetValue(OnSubmitProperty);
        set => SetValue(OnSubmitProperty, value);
    }
    
    public bool ShowForgotPin
    {
        get => (bool)GetValue(ShowForgotPinProperty);
        set => SetValue(ShowForgotPinProperty, value);
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
    
    public string TitleText { get; private set; } = KKPinviewConstant.TitleTextFormat;
    public string SubtitleText { get; private set; } = string.Format(KKPinviewConstant.SubtitleText, KKPinviewConstant.TotalDigits);
    public string ForgotPinText { get; private set; } = KKPinviewConstant.ForgotPinText;
    public string ErrorMessage { get; private set; } = string.Empty;
    public string SuccessMessage { get; private set; } = string.Empty;
    public bool HasError { get; private set; }
    public bool HasSuccessMessage { get; private set; }
    public bool IsKeypadEnabled { get; private set; } = true;
    public double KeypadOpacity { get; private set; } = 1.0;
    
    public ICommand NumberCommand { get; }
    public ICommand DeleteCommand { get; }
    
    public KKPinViews()
    {
        InitializeComponent();
        _lockoutManager = new KKPinLockoutManager();
        
        NumberCommand = new Command<string>(OnNumberPressed);
        DeleteCommand = new Command(OnDeletePressed);
        
        BindingContext = this;
        InitializePinFields();
        UpdateUI();
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
    
    private void OnNumberPressed(string number)
    {
        if (!IsKeypadEnabled || _currentPin.Length >= KKPinviewConstant.TotalDigits)
            return;
        
        _currentPin += number;
        UpdatePinFields();
        
        if (_currentPin.Length == KKPinviewConstant.TotalDigits)
        {
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
        var isValid = _lockoutManager.ValidatePIN(_currentPin);
        
        if (isValid)
        {
            ShowSuccessMessage(KKPinviewConstant.SetupSuccessMessage);
            OnSubmit?.Invoke(true);
            
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
            ShowErrorMessage(error ?? KKPinviewConstant.InvalidPinError);
            OnSubmit?.Invoke(false);
            
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
    
    private void UpdateUI()
    {
        IsKeypadEnabled = !_lockoutManager.IsLockedOut;
        KeypadOpacity = IsKeypadEnabled ? 1.0 : 0.5;
        
        if (_lockoutManager.IsLockedOut)
        {
            var error = _lockoutManager.GetErrorMessage();
            ShowErrorMessage(error ?? KKPinviewConstant.LockedOutError);
        }
        
        OnPropertyChanged(nameof(IsKeypadEnabled));
        OnPropertyChanged(nameof(KeypadOpacity));
    }
    
    private void OnForgotPinClicked(object? sender, EventArgs e)
    {
        OnForgotPin?.Invoke();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _lockoutManager.CheckLockoutStatus();
        UpdateUI();
    }
}

