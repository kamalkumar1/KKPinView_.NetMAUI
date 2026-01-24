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

    public static readonly BindableProperty IsKeypadEnabledProperty = BindableProperty.Create(
        nameof(IsKeypadEnabled), typeof(bool), typeof(NumericKeypad), true, propertyChanged: OnIsKeypadEnabledChanged);

    public static readonly BindableProperty KeypadOpacityProperty = BindableProperty.Create(
        nameof(KeypadOpacity), typeof(double), typeof(NumericKeypad), 1.0, propertyChanged: OnKeypadOpacityChanged);

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

    public bool IsKeypadEnabled
    {
        get => (bool)GetValue(IsKeypadEnabledProperty);
        set => SetValue(IsKeypadEnabledProperty, value);
    }

    public double KeypadOpacity
    {
        get => (double)GetValue(KeypadOpacityProperty);
        set => SetValue(KeypadOpacityProperty, value);
    }

    private static void OnIsKeypadEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NumericKeypad keypad && newValue is bool isEnabled)
        {
            keypad.UpdateKeypadEnabledState(isEnabled);
        }
    }

    private static void OnKeypadOpacityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NumericKeypad keypad && newValue is double opacity)
        {
            keypad.Opacity = opacity;
        }
    }

    private void UpdateKeypadEnabledState(bool isEnabled)
    {
        if (KeypadGrid == null) return;

        foreach (var child in KeypadGrid.Children)
        {
            if (child is Button button)
            {
                button.IsEnabled = isEnabled;
            }
        }
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
        
        // Initialize enabled state and opacity
        UpdateKeypadEnabledState(IsKeypadEnabled);
        Opacity = KeypadOpacity;
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
                button.Clicked -= OnButtonClicked;

                // Attach new handlers
                // Use both Pressed/Released for long presses and Clicked for quick taps
                button.Pressed += OnButtonPressed;
                button.Released += OnButtonReleased;
                button.Clicked += OnButtonClicked;
            }
        }
    }

    private async void OnButtonPressed(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // Cancel any ongoing animation
            button.AbortAnimation("ScaleAnimation");
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

    private async void OnButtonClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // For quick taps, trigger the animation sequence
            // This ensures animation works even if Pressed/Released don't fire
            button.AbortAnimation("ScaleAnimation");
            
            // Quick scale down and up animation
            await button.ScaleToAsync(0.9, 50, Easing.SinOut);
            await button.ScaleToAsync(1.0, 50, Easing.SinOut);
        }
    }
}

