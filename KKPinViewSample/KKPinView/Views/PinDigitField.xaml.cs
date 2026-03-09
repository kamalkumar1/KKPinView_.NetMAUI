using KKPinView.Constants;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Shapes;
using MauiEntry = Microsoft.Maui.Controls.Entry;
using MauiColor = Microsoft.Maui.Graphics.Color;

#if ANDROID
using AndroidX.AppCompat.Widget;
using Android.Views.InputMethods;
using Android.Content;
using Android.Graphics;
#endif

#if IOS
using UIKit;
using Foundation;
#endif

namespace KKPinView.Views;

public sealed partial class PinDigitField : ContentView
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

    public static readonly BindableProperty DigitFontAttributesProperty = BindableProperty.Create(
        nameof(DigitFontAttributes), typeof(FontAttributes), typeof(PinDigitField), KKPinviewConstant.DigitFontAttributes);

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(MauiColor), typeof(PinDigitField), KKPinviewConstant.TextColor);

    public static new readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
        nameof(BackgroundColor), typeof(MauiColor), typeof(PinDigitField), KKPinviewConstant.DigitFieldBackgroundColor);

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor), typeof(MauiColor), typeof(PinDigitField), KKPinviewConstant.DigitFieldEmptyBorderColor, propertyChanged: OnBorderColorChanged);

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius), typeof(double), typeof(PinDigitField), KKPinviewConstant.FieldCornerRadius, propertyChanged: OnCornerRadiusChanged);

    public static readonly BindableProperty FieldShapeTypeProperty = BindableProperty.Create(
        nameof(FieldShapeType), typeof(KKPinFieldShapeType), typeof(PinDigitField), KKPinviewConstant.FieldShapeType, propertyChanged: OnShapeChanged);

    // Numeric keypad logic fully removed

    public event EventHandler<string>? DigitChanged;
    public event EventHandler? DigitCompleted;
    public event EventHandler? DigitDeleted;

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

    public FontAttributes DigitFontAttributes
    {
        get => (FontAttributes)GetValue(DigitFontAttributesProperty);
        set => SetValue(DigitFontAttributesProperty, value);
    }

    public MauiColor TextColor
    {
        get => (MauiColor)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public new MauiColor BackgroundColor
    {
        get => (MauiColor)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    public MauiColor BorderColor
    {
        get => (MauiColor)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>Shape type for the PIN field. Comes from KKPinviewConstant.FieldShapeType by default.</summary>
    public KKPinFieldShapeType FieldShapeType
    {
        get => (KKPinFieldShapeType)GetValue(FieldShapeTypeProperty);
        set => SetValue(FieldShapeTypeProperty, value);
    }

    // Numeric keypad property fully removed

    /// <summary>
    /// Gets the stroke shape (round rectangle) used for the border based on corner radius settings
    /// </summary>
    public RoundRectangle? StrokeShape { get; private set; }

    /// <summary>
    /// Command that focuses the entry and brings the keyboard. Bind to TapGestureRecognizer.Command to bring keyboard on overlay tap without code-behind handler.
    /// When <see cref="TapCommand"/> is set (e.g. by parent), overlay tap runs that instead so input always goes to the logical first-empty field.
    /// </summary>
    public System.Windows.Input.ICommand FocusEntryCommand { get; }

    /// <summary>
    /// If set by the parent, overlay tap executes this instead of focusing this field. Use to focus the first empty field so digits always flow left-to-right.
    /// Delete/backspace focus is set by the parent's DigitDeleted handler; TapCommand only affects tap-to-open and does not change delete logic.
    /// </summary>
    public System.Windows.Input.ICommand? TapCommand { get; set; }

    private bool _isProgrammaticClear;
    private bool _suppressDigitEvents;

    /// <summary>
    /// Clears the digit without triggering DigitDeleted. Use when clearing from parent (e.g. delete handler).
    /// </summary>
    public void ClearDigitSilently()
    {
        _isProgrammaticClear = true;
        Digit = string.Empty;
        IsFilled = false;
        _isProgrammaticClear = false;
    }

    public PinDigitField()
    {
        FocusEntryCommand = new Command(() =>
        {
            if (TapCommand != null)
                TapCommand.Execute(null);
            else
                FocusEntry();
        });
        InitializeComponent();
        UpdateStrokeShape();

        // Initialize editable state after component is loaded
        Loaded += OnFieldLoaded;

        // Remove Android underline and make cursor invisible when handler is attached
        HandlerChanged += OnHandlerChanged;

        // Keep cursor at end and invisible; user cannot move cursor (InputTransparent on entry)
        if (DigitEntry != null)
            DigitEntry.Focused += OnDigitEntryFocused;
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        RemoveAndroidUnderline();
        MakeCursorInvisible();
        SetupBackspaceOnEmptyFieldHandler();
    }

    private void OnFieldLoaded(object? sender, EventArgs e)
    {
        if (DigitEntry != null)
        {
            DigitEntry.IsVisible = true;
            DigitEntry.IsReadOnly = false;
        }
        RemoveAndroidUnderline();
        MakeCursorInvisible();
        SetupBackspaceOnEmptyFieldHandler();
    }

    private void OnDigitEntryFocused(object? sender, FocusEventArgs e)
    {
        if (e.IsFocused && DigitEntry != null)
            SetCursorToEnd(DigitEntry);
    }

    private static void SetCursorToEnd(MauiEntry entry)
    {
        var len = entry.Text?.Length ?? 0;
        entry.CursorPosition = len;
        entry.SelectionLength = 0;
    }

    private void MakeCursorInvisible()
    {
#if IOS
        // Native UITextField only — no MAUI platform-specific Entry API (custom handler uses BackspaceAwareTextField)
        if (DigitEntry?.Handler?.PlatformView is KKPinView.Handlers.BackspaceAwareTextField nativeField)
            nativeField.TintColor = UIKit.UIColor.Clear;
#endif
#if ANDROID
        if (DigitEntry?.Handler?.PlatformView is AppCompatEditText editText)
        {
            editText.SetCursorVisible(false);
            if (editText.TextCursorDrawable != null)
                editText.TextCursorDrawable.SetColorFilter(new PorterDuffColorFilter(global::Android.Graphics.Color.Transparent, PorterDuff.Mode.SrcIn!));
        }
#endif
    }

    private void SetupBackspaceOnEmptyFieldHandler()
    {
#if IOS
        if (DigitEntry?.Handler?.PlatformView is KKPinView.Handlers.BackspaceAwareTextField iosTextField)
        {
            iosTextField.EmptyBackspacePressed -= OnIOSEmptyBackspace;
            iosTextField.EmptyBackspacePressed += OnIOSEmptyBackspace;
        }
#endif
#if ANDROID
        if (DigitEntry?.Handler?.PlatformView is KKPinView.Platforms.Android.BackspaceAwareEditText androidEditText)
        {
            androidEditText.EmptyBackspacePressed -= OnAndroidEmptyBackspace;
            androidEditText.EmptyBackspacePressed += OnAndroidEmptyBackspace;
        }
#endif
    }

#if IOS
    private void OnIOSEmptyBackspace(object? sender, EventArgs e)
    {
        DigitDeleted?.Invoke(this, EventArgs.Empty);
    }
#endif

#if ANDROID
    private void OnAndroidEmptyBackspace(object? sender, EventArgs e)
    {
        DigitDeleted?.Invoke(this, EventArgs.Empty);
    }
#endif

    private void RemoveAndroidUnderline()
    {
#if ANDROID
        if (DigitEntry?.Handler?.PlatformView is AppCompatEditText editText)
        {
            editText.Background = null;
            editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
#endif
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

    // Numeric keypad handler fully removed

    private static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PinDigitField field && newValue is MauiColor color)
        {
            field.UpdateBorderColor(color);
        }
    }

    private void UpdateBorderColor(MauiColor color)
    {
        if (DigitBorder != null)
        {
            DigitBorder.Stroke = color;
        }
    }

    private const string BorderAnimationName = "PinDigitFieldBorder";

    /// <summary>Animates the border stroke from the current color to <paramref name="targetColor"/> over the given duration.</summary>
    public void AnimateBorderToColor(MauiColor targetColor, uint durationMs = 250, Easing? easing = null)
    {
        if (DigitBorder == null) return;
        this.AbortAnimation(BorderAnimationName);
        MauiColor fromColor = BorderColor;
        easing ??= Easing.CubicInOut;
        var animation = new Animation(v =>
        {
            MauiColor interpolated = Lerp(fromColor, targetColor, (float)v);
            BorderColor = interpolated;
        }, 0, 1);
        animation.Commit(this, BorderAnimationName, 16, durationMs, easing);
    }

    private static MauiColor Lerp(MauiColor from, MauiColor to, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        float r = from.Red + (to.Red - from.Red) * t;
        float g = from.Green + (to.Green - from.Green) * t;
        float b = from.Blue + (to.Blue - from.Blue) * t;
        float a = from.Alpha + (to.Alpha - from.Alpha) * t;
        return new MauiColor(r, g, b, a);
    }

    private void UpdateAppearance()
    {
        // Use BorderColor property if it's been explicitly set (e.g., red for invalid)
        // Otherwise, use default behavior based on IsFilled
        if (BorderColor != KKPinviewConstant.DigitFieldEmptyBorderColor && BorderColor != KKPinviewConstant.DigitFieldFilledColor)
        {
            // Custom border color is set (e.g., red for invalid), use it
            DigitBorder.Stroke = BorderColor;
        }
        else
        {
            if (IsFilled)
                DigitBorder.Stroke = KKPinviewConstant.DigitFieldFilledColor;
            else
                DigitBorder.Stroke = KKPinviewConstant.DigitFieldEmptyBorderColor;
        }
        // Entry is always editable for system keyboard
        if (DigitEntry != null)
        {
            DigitEntry.IsReadOnly = false;
            DigitEntry.IsEnabled = true;
        }
    }

    // Numeric keypad state update fully removed

    private void UpdateStrokeShape()
    {
        StrokeShape = FieldShapeType switch
        {
            KKPinFieldShapeType.Round => new RoundRectangle
            {
                CornerRadius = new CornerRadius(Math.Min(FieldWidth, FieldHeight) / 2)
            },
            _ => new RoundRectangle { CornerRadius = new CornerRadius(CornerRadius) } // RoundedRectangle (default)
        };

        if (DigitBorder != null)
        {
            DigitBorder.StrokeShape = StrokeShape;
        }
    }

    private void OnDigitEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_suppressDigitEvents) return;

        if (sender is not MauiEntry entry) return;

        string oldText = e.OldTextValue ?? string.Empty;
        string newText = e.NewTextValue ?? string.Empty;

        // OTP: enforce single character only (take last character if paste/append in same field)
        if (newText.Length > 1)
            newText = newText.Length > 0 ? newText.Substring(newText.Length - 1) : string.Empty;
        if (!string.IsNullOrEmpty(newText) && !char.IsDigit(newText[0]))
            newText = string.Empty;

        // Delete/backspace: old had content, new is empty
        if (!string.IsNullOrEmpty(oldText) && string.IsNullOrEmpty(newText))
        {
            if (_isProgrammaticClear) return;
            Digit = string.Empty;
            IsFilled = false;
            DigitDeleted?.Invoke(this, EventArgs.Empty);
            return;
        }

        // Update Digit only; binding updates Entry.Text. Do not set entry.Text so TextChanged keeps firing on every keystroke.
        _suppressDigitEvents = true;
        Digit = newText;
        IsFilled = !string.IsNullOrEmpty(newText);
        _suppressDigitEvents = false;

        var digitToSend = newText;
        SetCursorToEnd(entry);
        Dispatcher.DispatchDelayed(TimeSpan.Zero, () => DigitChanged?.Invoke(this, digitToSend));
    }

    private void OnDigitEntryCompleted(object? sender, EventArgs e)
    {
        // Always process completion for system keyboard
#if IOS
        DigitEntry?.Unfocus();
#else
        DigitCompleted?.Invoke(this, EventArgs.Empty);
#endif
    }

    public void FocusEntry()
    {
        DigitEntry?.Focus();

#if ANDROID
        // Explicitly show keyboard on Android (especially for simulators)
        // Use a small delay to ensure focus is complete before showing keyboard
        Task.Delay(100).ContinueWith(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (DigitEntry?.Handler?.PlatformView is AppCompatEditText editText)
                {
                    // Request focus on the native view
                    editText.RequestFocus();
                    
                    // Show keyboard explicitly
                    var inputMethodManager = editText.Context?.GetSystemService(Context.InputMethodService) as InputMethodManager;
                    if (inputMethodManager != null)
                    {
                        inputMethodManager.ShowSoftInput(editText, ShowFlags.Implicit);
                    }
                }
            });
        });
#endif
    }

    public void UnfocusEntry()
    {
        DigitEntry?.Unfocus();
    }

    /// <summary>
    /// True when the inner entry has focus (keyboard is open for this field).
    /// </summary>
    public bool IsEntryFocused => DigitEntry?.IsFocused ?? false;
}

