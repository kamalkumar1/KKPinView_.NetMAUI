using System.Windows.Input;
using KKPinView.Constants;

namespace KKPinView.Views;

public partial class NumericKeypad : ContentView
{
    public static readonly BindableProperty NumberCommandProperty = BindableProperty.Create(
        nameof(NumberCommand), typeof(ICommand), typeof(NumericKeypad));
    
    public static readonly BindableProperty DeleteCommandProperty = BindableProperty.Create(
        nameof(DeleteCommand), typeof(ICommand), typeof(NumericKeypad));
    
    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing), typeof(double), typeof(NumericKeypad), KKPinviewConstant.KeypadSpacing);
    
    public ICommand? NumberCommand
    {
        get => (ICommand?)GetValue(NumberCommandProperty);
        set => SetValue(NumberCommandProperty, value);
    }
    
    public ICommand? DeleteCommand
    {
        get => (ICommand?)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
    
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }
    
    public NumericKeypad()
    {
        InitializeComponent();
        ApplyKeypadStyles();
    }
    
    private void ApplyKeypadStyles()
    {
        // Apply styles to all buttons
        foreach (var child in ((Grid)Content).Children)
        {
            if (child is Button button && button.IsVisible)
            {
                button.FontSize = KKPinviewConstant.KeypadButtonFontSize;
                button.BackgroundColor = KKPinviewConstant.KeypadButtonColor;
                button.TextColor = KKPinviewConstant.KeypadButtonTextColor;
                button.WidthRequest = KKPinviewConstant.KeypadButtonSize;
                button.HeightRequest = KKPinviewConstant.KeypadButtonSize;
                button.CornerRadius = (int)(KKPinviewConstant.KeypadButtonSize / 2);
            }
        }
    }
}

