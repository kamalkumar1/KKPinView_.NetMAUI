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

        // Set up button press animations after UI is loaded
        Loaded += OnKeypadLoaded;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
    }

    private void OnKeypadLoaded(object? sender, EventArgs e)
    {
        // Attach press/release handlers to all buttons for scale animation
        AttachButtonAnimations();
    }

    private void AttachButtonAnimations()
    {
        if (KeypadGrid == null) return;

        foreach (var child in KeypadGrid.Children)
        {
            if (child is Button button && button.IsVisible)
            {
                // Remove any existing handlers first
                button.Pressed -= OnButtonPressed;
                button.Released -= OnButtonReleased;

                // Attach new handlers
                button.Pressed += OnButtonPressed;
                button.Released += OnButtonReleased;
            }
        }
    }

    private async void OnButtonPressed(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // Scale down to 0.9 when pressed
            await button.ScaleToAsync(0.9, 100, Easing.SinOut);
        }
    }

    private async void OnButtonReleased(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // Scale back to 1.0 when released
            await button.ScaleToAsync(1.0, 100, Easing.SinOut);
        }
    }
}

