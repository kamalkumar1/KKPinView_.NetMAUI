using System.Collections.ObjectModel;
using System.Windows.Input;
using KKPinView.Constants;

namespace KKPinView.Views;

public partial class KKPINSetUPView : ContentPage
{
    // BindableProperty for HeadingText
    public static readonly BindableProperty HeadingTextProperty = BindableProperty.Create(
        nameof(HeadingText), typeof(string), typeof(KKPINSetUPView), $"Set {KKPinviewConstant.TotalDigits} digit PIN");
    
    // BindableProperty for TitleFontSize
    public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(
        nameof(TitleFontSize), typeof(double), typeof(KKPINSetUPView), KKPinviewConstant.TitleFontSize);
    
    // BindableProperty for TextColor
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(KKPINSetUPView), KKPinviewConstant.TextColor);
    
    // BindableProperty for SubtitleFontSize
    public static readonly BindableProperty SubtitleFontSizeProperty = BindableProperty.Create(
        nameof(SubtitleFontSize), typeof(double), typeof(KKPINSetUPView), KKPinviewConstant.SubtitleFontSize);
    
    // BindableProperty for EnterPinLabelText
    public static readonly BindableProperty EnterPinLabelTextProperty = BindableProperty.Create(
        nameof(EnterPinLabelText), typeof(string), typeof(KKPINSetUPView), KKPinviewConstant.EnterPinMessage);
    
    // BindableProperty for ConfirmPinLabelText
    public static readonly BindableProperty ConfirmPinLabelTextProperty = BindableProperty.Create(
        nameof(ConfirmPinLabelText), typeof(string), typeof(KKPINSetUPView), KKPinviewConstant.ConfirmPinMessage);
    
    // BindableProperty for FieldSpacing
    public static readonly BindableProperty FieldSpacingProperty = BindableProperty.Create(
        nameof(FieldSpacing), typeof(double), typeof(KKPINSetUPView), KKPinviewConstant.FieldSpacing);
    
    // BindableProperty for ErrorMessage
    public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(
        nameof(ErrorMessage), typeof(string), typeof(KKPINSetUPView), string.Empty);
    
    // BindableProperty for HasError
    public static readonly BindableProperty HasErrorProperty = BindableProperty.Create(
        nameof(HasError), typeof(bool), typeof(KKPINSetUPView), false);
    
    // BindableProperty for SuccessMessage
    public static readonly BindableProperty SuccessMessageProperty = BindableProperty.Create(
        nameof(SuccessMessage), typeof(string), typeof(KKPINSetUPView), string.Empty);
    
    // BindableProperty for HasSuccessMessage
    public static readonly BindableProperty HasSuccessMessageProperty = BindableProperty.Create(
        nameof(HasSuccessMessage), typeof(bool), typeof(KKPINSetUPView), false);
    
    // BindableProperty for ErrorTextColor
    public static readonly BindableProperty ErrorTextColorProperty = BindableProperty.Create(
        nameof(ErrorTextColor), typeof(Color), typeof(KKPINSetUPView), KKPinviewConstant.ErrorTextColor);
    
    // BindableProperty for SuccessTextColor
    public static readonly BindableProperty SuccessTextColorProperty = BindableProperty.Create(
        nameof(SuccessTextColor), typeof(Color), typeof(KKPINSetUPView), KKPinviewConstant.SuccessTextColor);
    
    private readonly ObservableCollection<PinDigitField> _enterPinFields = new();
    private readonly ObservableCollection<PinDigitField> _confirmPinFields = new();
    private string _currentPin = string.Empty;
    private string _confirmPin = string.Empty;
    private bool _isConfirmingPin = false;
    
    public ICommand? NumberCommand { get; private set; }
    public ICommand? DeleteCommand { get; private set; }
    
    public string HeadingText
    {
        get => (string)GetValue(HeadingTextProperty);
        set => SetValue(HeadingTextProperty, value);
    }
    
    public double TitleFontSize
    {
        get => (double)GetValue(TitleFontSizeProperty);
        set => SetValue(TitleFontSizeProperty, value);
    }
    
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }
    
    public double SubtitleFontSize
    {
        get => (double)GetValue(SubtitleFontSizeProperty);
        set => SetValue(SubtitleFontSizeProperty, value);
    }
    
    public string EnterPinLabelText
    {
        get => (string)GetValue(EnterPinLabelTextProperty);
        set => SetValue(EnterPinLabelTextProperty, value);
    }
    
    public string ConfirmPinLabelText
    {
        get => (string)GetValue(ConfirmPinLabelTextProperty);
        set => SetValue(ConfirmPinLabelTextProperty, value);
    }
    
    public double FieldSpacing
    {
        get => (double)GetValue(FieldSpacingProperty);
        set => SetValue(FieldSpacingProperty, value);
    }
    
    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }
    
    public bool HasError
    {
        get => (bool)GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }
    
    public string SuccessMessage
    {
        get => (string)GetValue(SuccessMessageProperty);
        set => SetValue(SuccessMessageProperty, value);
    }
    
    public bool HasSuccessMessage
    {
        get => (bool)GetValue(HasSuccessMessageProperty);
        set => SetValue(HasSuccessMessageProperty, value);
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
    
    public KKPINSetUPView()
    {
        InitializeComponent();
        
        // Set background color from constant
        BackgroundColor = KKPinviewConstant.BackgroundColor;
        
        // Initialize HeadingText with value from constant
        HeadingText = $"Set {KKPinviewConstant.TotalDigits} digit PIN";
        
        // Initialize EnterPinLabelText from constant
        EnterPinLabelText = KKPinviewConstant.EnterPinMessage;
        
        // Initialize ConfirmPinLabelText from constant
        ConfirmPinLabelText = KKPinviewConstant.ConfirmPinMessage;
        
        // Initialize error and success message colors from constants
        ErrorTextColor = KKPinviewConstant.ErrorTextColor;
        SuccessTextColor = KKPinviewConstant.SuccessTextColor;
        
        BindingContext = this;
        
        // Initialize commands for keypad
        NumberCommand = new Command<string>(OnNumberPressed);
        DeleteCommand = new Command(OnDeletePressed);
        
        // Initialize PIN fields based on TotalDigits constant
        InitializePinFields();
        InitializeConfirmPinFields();
        
        // Set commands on keypad after initialization
        Loaded += OnPageLoaded;
    }
    
    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (Keypad != null)
        {
            Keypad.NumberCommand = NumberCommand;
            Keypad.DeleteCommand = DeleteCommand;
        }
    }
    
    private void OnNumberPressed(string number)
    {
        if (_isConfirmingPin)
        {
            // Entering confirm PIN
            if (_confirmPin.Length >= KKPinviewConstant.TotalDigits)
                return;
            
            _confirmPin += number;
            UpdateConfirmPinFields();
            
            // Clear any previous messages when user starts typing again
            ClearMessages();
            
            // If confirm PIN is complete, validate it
            if (_confirmPin.Length == KKPinviewConstant.TotalDigits)
            {
                ValidatePinMatch();
            }
        }
        else
        {
            // Entering first PIN
            if (_currentPin.Length >= KKPinviewConstant.TotalDigits)
                return;
            
            _currentPin += number;
            UpdatePinFields();
            
            // If first PIN is complete, switch to confirm mode
            if (_currentPin.Length == KKPinviewConstant.TotalDigits)
            {
                _isConfirmingPin = true;
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
                _isConfirmingPin = false;
                ClearMessages();
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
            }
        }
    }
    
    private void UpdatePinFields()
    {
        for (int i = 0; i < _enterPinFields.Count; i++)
        {
            _enterPinFields[i].IsFilled = i < _currentPin.Length;
            if (i < _currentPin.Length)
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
        for (int i = 0; i < _confirmPinFields.Count; i++)
        {
            _confirmPinFields[i].IsFilled = i < _confirmPin.Length;
            if (i < _confirmPin.Length)
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
            // PINs match - show success message
            ShowSuccessMessage(KKPinviewConstant.SetupSuccessMessage);
        }
        else
        {
            // PINs don't match - show error message
            ShowErrorMessage(KKPinviewConstant.PinMismatchError);
            
            // Clear confirm PIN fields to allow re-entry
            _confirmPin = string.Empty;
            UpdateConfirmPinFields();
        }
    }
    
    private void ShowErrorMessage(string message)
    {
        ErrorMessage = message;
        HasError = true;
        HasSuccessMessage = false;
    }
    
    private void ShowSuccessMessage(string message)
    {
        SuccessMessage = message;
        HasSuccessMessage = true;
        HasError = false;
    }
    
    private void ClearMessages()
    {
        HasError = false;
        HasSuccessMessage = false;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
    }
}
