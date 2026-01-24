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
        nameof(BorderColor), typeof(Color), typeof(PinDigitField), Colors.Gray, propertyChanged: OnBorderColorChanged);
    
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius), typeof(double), typeof(PinDigitField), KKPinviewConstant.FieldCornerRadius, propertyChanged: OnCornerRadiusChanged);
    
    public static readonly BindableProperty UseRoundShapeProperty = BindableProperty.Create(
        nameof(UseRoundShape), typeof(bool), typeof(PinDigitField), KKPinviewConstant.UseRoundFields, propertyChanged: OnShapeChanged);
    
    public static readonly BindableProperty IsEditableProperty = BindableProperty.Create(
        nameof(IsEditable), typeof(bool), typeof(PinDigitField), false, propertyChanged: OnIsEditableChanged);
    
    public event EventHandler<string>? DigitChanged;
    public event EventHandler? DigitCompleted;
    
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
    
    public bool IsEditable
    {
        get => (bool)GetValue(IsEditableProperty);
        set => SetValue(IsEditableProperty, value);
    }
    
    /// <summary>
    /// Gets the stroke shape (round rectangle) used for the border based on corner radius settings
    /// </summary>
    public RoundRectangle? StrokeShape { get; private set; }
    
    public PinDigitField()
    {
        InitializeComponent();
        UpdateStrokeShape();
        
        // Initialize editable state after component is loaded
        Loaded += OnFieldLoaded;
    }
    
    private void OnFieldLoaded(object? sender, EventArgs e)
    {
        // Ensure Entry is always visible
        if (DigitEntry != null)
        {
            DigitEntry.IsVisible = true;
        }
        UpdateEditableState();
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
    
    private static void OnIsEditableChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PinDigitField field)
        {
            field.UpdateEditableState();
        }
    }
    
    private static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PinDigitField field && newValue is Color color)
        {
            field.UpdateBorderColor(color);
        }
    }
    
    private void UpdateBorderColor(Color color)
    {
        if (DigitBorder != null)
        {
            DigitBorder.Stroke = color;
        }
    }
    
    private void UpdateAppearance()
    {
        // Use BorderColor property if it's been explicitly set (e.g., red for invalid)
        // Otherwise, use default behavior based on IsFilled
        if (BorderColor != Colors.Gray && BorderColor != KKPinviewConstant.DigitFieldFilledColor)
        {
            // Custom border color is set (e.g., red for invalid), use it
            DigitBorder.Stroke = BorderColor;
        }
        else
        {
            // Use default behavior based on filled state
            if (IsFilled)
            {
                DigitBorder.Stroke = KKPinviewConstant.DigitFieldFilledColor;
            }
            else
            {
                DigitBorder.Stroke = Colors.Gray;
            }
        }
        
        // Update visibility based on editable state
        UpdateEditableState();
    }
    
    private void UpdateEditableState()
    {
        if (DigitEntry != null)
        {
            // Entry is always visible, but read-only when not editable (numeric keypad mode)
            DigitEntry.IsReadOnly = !IsEditable;
            DigitEntry.IsEnabled = true; // Always enabled to show text, but read-only controls editing
            
            // When read-only, prevent focus (numeric keypad mode)
            if (!IsEditable)
            {
                DigitEntry.Unfocus();
            }
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
    
    private void OnDigitEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Only process text changes when editable (system keyboard mode)
        if (!IsEditable) return;
        
        if (sender is Entry entry)
        {
            // Filter to only allow single digit
            string newText = e.NewTextValue ?? string.Empty;
            if (newText.Length > 1)
            {
                newText = newText.Length > 0 ? newText.Substring(newText.Length - 1) : string.Empty;
            }
            
            // Only allow digits
            if (!string.IsNullOrEmpty(newText) && !char.IsDigit(newText[0]))
            {
                newText = string.Empty;
            }
            
            if (newText != e.NewTextValue)
            {
                entry.Text = newText;
                return;
            }
            
            Digit = newText;
            IsFilled = !string.IsNullOrEmpty(newText);
            DigitChanged?.Invoke(this, newText);
        }
    }
    
    private void OnDigitEntryCompleted(object? sender, EventArgs e)
    {
        // Only process completion when editable (system keyboard mode)
        if (!IsEditable) return;
        
        DigitCompleted?.Invoke(this, EventArgs.Empty);
    }
    
    public void FocusEntry()
    {
        DigitEntry?.Focus();
    }
}

