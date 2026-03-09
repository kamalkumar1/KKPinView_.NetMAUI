namespace KKPinView.Constants;

/// <summary>
/// Fluent configuration for KKPinView. Use with <see cref="KKPinviewConstant.Configure"/> for easy setup.
/// </summary>
public sealed class KKPinViewConfig
{
    /// <summary>Sets the number of PIN digits (4 or 6).</summary>
    public KKPinViewConfig PinLength(int length)
    {
        KKPinviewConstant.TotalPinTextFields = length;
        return this;
    }

    /// <summary>Sets lockout: max failed attempts and lockout duration in minutes.</summary>
    public KKPinViewConfig Lockout(int maxAttempts = 5, int lockoutMinutes = 5)
    {
        KKPinviewConstant.MaxPinAttempts = maxAttempts;
        KKPinviewConstant.PinLockoutDurationMinutes = lockoutMinutes;
        return this;
    }

    /// <summary>When true, PIN persists after uninstall (iOS Keychain). When false (default), PIN is removed on uninstall.</summary>
    public KKPinViewConfig PinStoragePersistsAfterUninstall(bool persists = false)
    {
        KKPinviewConstant.PinStoragePersistsAfterUninstall = persists;
        return this;
    }

    /// <summary>Sets view background color.</summary>
    public KKPinViewConfig BackgroundColor(Color color)
    {
        KKPinviewConstant.BackgroundColor = color;
        return this;
    }

    /// <summary>Sets label colors (text, error, success).</summary>
    public KKPinViewConfig LabelColors(Color? textColor = null, Color? errorColor = null, Color? successColor = null)
    {
        if (textColor is { } c) LabelConstants.TextColor = c;
        if (errorColor is { } e) LabelConstants.ErrorTextColor = e;
        if (successColor is { } s) LabelConstants.SuccessTextColor = s;
        return this;
    }

    /// <summary>Sets label font (size, attributes, family). Empty family = system font.</summary>
    public KKPinViewConfig LabelFont(double fontSize = 18, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        LabelConstants.FontSize = fontSize;
        LabelConstants.FontAttributes = attributes;
        if (fontFamily != null) LabelConstants.FontFamily = fontFamily;
        return this;
    }

    /// <summary>Sets error message font.</summary>
    public KKPinViewConfig ErrorMessageFont(double fontSize = 24, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        LabelConstants.ErrorMessageFontSize = fontSize;
        LabelConstants.ErrorMessageFontAttributes = attributes;
        if (fontFamily != null) LabelConstants.ErrorMessageFontFamily = fontFamily;
        return this;
    }

    /// <summary>Sets PIN digit field font (size, attributes, family).</summary>
    public KKPinViewConfig DigitFont(double fontSize = 16, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        PinFieldConstants.FontSize = fontSize;
        PinFieldConstants.FontAttributes = attributes;
        if (fontFamily != null) PinFieldConstants.FontFamily = fontFamily;
        return this;
    }

    /// <summary>Sets PIN field border colors (filled, empty, invalid).</summary>
    public KKPinViewConfig PinFieldColors(Color? filled = null, Color? empty = null, Color? invalid = null)
    {
        if (filled is { } f) PinFieldConstants.FilledBorderColor = f;
        if (empty is { } emp) PinFieldConstants.EmptyBorderColor = emp;
        if (invalid is { } inv) PinFieldConstants.InvalidBorderColor = inv;
        return this;
    }

    /// <summary>Sets PIN field font and dimensions.</summary>
    public KKPinViewConfig PinField(double fontSize = 16, double height = 50, double width = 50, double spacing = 15, KKPinFieldShapeType shape = KKPinFieldShapeType.Round)
    {
        PinFieldConstants.FontSize = fontSize;
        PinFieldConstants.Height = height;
        PinFieldConstants.Width = width;
        PinFieldConstants.Spacing = spacing;
        PinFieldConstants.ShapeType = shape;
        return this;
    }

    /// <summary>Sets PIN field corner radius (for RoundedRectangle shape).</summary>
    public KKPinViewConfig PinFieldCornerRadius(double radius)
    {
        PinFieldConstants.CornerRadius = radius;
        return this;
    }

    /// <summary>Sets custom label strings.</summary>
    public KKPinViewConfig Labels(string? enterPin = null, string? confirmPin = null, string? forgotPin = null)
    {
        if (enterPin != null) KKPinviewConstant.EnterPinMessage = enterPin;
        if (confirmPin != null) KKPinviewConstant.ConfirmPinMessage = confirmPin;
        if (forgotPin != null) KKPinviewConstant.ForgotPinText = forgotPin;
        return this;
    }
}
