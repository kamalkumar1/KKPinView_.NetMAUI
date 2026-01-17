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

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius), typeof(int), typeof(NumericKeypad), (int)KKPinviewConstant.KeypadButtonCornerRadius);

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

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public NumericKeypad()
    {
        InitializeComponent();
        BindingContext = this;

        // Initialize CornerRadius from constant
        CornerRadius = (int)KKPinviewConstant.KeypadButtonCornerRadius;

        // Apply styles after a short delay to ensure UI is fully loaded
        // Dispatcher.Dispatch(() =>
        // {
        //     ApplyKeypadStyles();
        // });
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
    }


}

