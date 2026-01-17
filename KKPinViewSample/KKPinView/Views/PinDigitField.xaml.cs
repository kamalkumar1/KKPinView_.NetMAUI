using KKPinView.Constants;
using Microsoft.Maui.Controls.Shapes;

namespace KKPinView.Views;

public partial class PinDigitField : ContentView
{
    public static readonly BindableProperty DigitProperty = BindableProperty.Create(
        nameof(Digit), typeof(string), typeof(PinDigitField), string.Empty);
    
    public static readonly BindableProperty IsFilledProperty = BindableProperty.Create(
        nameof(IsFilled), typeof(bool), typeof(PinDigitField), false, propertyChanged: OnIsFilledChanged);
    
    public static readonly BindableProperty FieldWidthProperty = BindableProperty.Create(
        nameof(FieldWidth), typeof(double), typeof(PinDigitField), KKPinviewConstant.FieldWidth);
    
    public static readonly BindableProperty FieldHeightProperty = BindableProperty.Create(
        nameof(FieldHeight), typeof(double), typeof(PinDigitField), KKPinviewConstant.FieldHeight);
    
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(PinDigitField), KKPinviewConstant.DigitFontSize);
    
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(PinDigitField), KKPinviewConstant.TextColor);
    
    public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
        nameof(BackgroundColor), typeof(Color), typeof(PinDigitField), KKPinviewConstant.DigitFieldBackgroundColor);
    
    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor), typeof(Color), typeof(PinDigitField), Colors.Gray);
    
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius), typeof(double), typeof(PinDigitField), KKPinviewConstant.FieldCornerRadius, propertyChanged: OnCornerRadiusChanged);
    
    public static readonly BindableProperty UseRoundShapeProperty = BindableProperty.Create(
        nameof(UseRoundShape), typeof(bool), typeof(PinDigitField), KKPinviewConstant.UseRoundFields, propertyChanged: OnShapeChanged);
    
    public string Digit
    {
        get => (string)GetValue(DigitProperty);
        set => SetValue(DigitProperty, value);
    }
    
    public bool IsFilled
    {
        get => (bool)GetValue(IsFilledProperty);
        set => SetValue(IsFilledProperty, value);
    }
    
    public double FieldWidth
    {
        get => (double)GetValue(FieldWidthProperty);
        set => SetValue(FieldWidthProperty, value);
    }
    
    public double FieldHeight
    {
        get => (double)GetValue(FieldHeightProperty);
        set => SetValue(FieldHeightProperty, value);
    }
    
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }
    
    public Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }
    
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }
    
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }
    
    public bool UseRoundShape
    {
        get => (bool)GetValue(UseRoundShapeProperty);
        set => SetValue(UseRoundShapeProperty, value);
    }
    
    /// <summary>
    /// Gets the stroke shape (round rectangle) used for the border based on corner radius settings
    /// </summary>
    public RoundRectangle? StrokeShape { get; private set; }
    
    public PinDigitField()
    {
        InitializeComponent();
        UpdateStrokeShape();
    }
    
    private static void OnCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PinDigitField field)
        {
            field.UpdateStrokeShape();
        }
    }
    
    private static void OnShapeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PinDigitField field)
        {
            field.UpdateStrokeShape();
        }
    }
    
    private static void OnIsFilledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PinDigitField field)
        {
            field.UpdateAppearance();
        }
    }
    
    private void UpdateAppearance()
    {
        if (IsFilled)
        {
            DigitBorder.Stroke = KKPinviewConstant.DigitFieldFilledColor;
            if (DotLabel != null) DotLabel.IsVisible = true;
        }
        else
        {
            DigitBorder.Stroke = Colors.Gray;
            if (DotLabel != null) DotLabel.IsVisible = false;
        }
    }
    
    private void UpdateStrokeShape()
    {
        if (UseRoundShape)
        {
            // Round shape (circle/oval) - use radius equal to half the smaller dimension
            var radius = Math.Min(FieldWidth, FieldHeight) / 2;
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(radius)
            };
        }
        else
        {
            // Rectangle with corner radius from constant
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(CornerRadius)
            };
        }
        
        if (DigitBorder != null)
        {
            DigitBorder.StrokeShape = StrokeShape;
        }
    }
}

