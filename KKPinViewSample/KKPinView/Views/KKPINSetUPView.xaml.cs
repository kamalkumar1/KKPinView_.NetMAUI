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
    
    // BindableProperty for FieldSpacing
    public static readonly BindableProperty FieldSpacingProperty = BindableProperty.Create(
        nameof(FieldSpacing), typeof(double), typeof(KKPINSetUPView), KKPinviewConstant.FieldSpacing);
    
    private readonly ObservableCollection<PinDigitField> _enterPinFields = new();
    private string _currentPin = string.Empty;
    
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
    
    public double FieldSpacing
    {
        get => (double)GetValue(FieldSpacingProperty);
        set => SetValue(FieldSpacingProperty, value);
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
        
        BindingContext = this;
        
        // Initialize commands for keypad
        NumberCommand = new Command<string>(OnNumberPressed);
        DeleteCommand = new Command(OnDeletePressed);
        
        // Initialize PIN fields based on TotalDigits constant
        InitializePinFields();
        
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
        if (_currentPin.Length >= KKPinviewConstant.TotalDigits)
            return;
        
        _currentPin += number;
        UpdatePinFields();
    }
    
    private void OnDeletePressed()
    {
        if (_currentPin.Length > 0)
        {
            _currentPin = _currentPin.Substring(0, _currentPin.Length - 1);
            UpdatePinFields();
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
}
