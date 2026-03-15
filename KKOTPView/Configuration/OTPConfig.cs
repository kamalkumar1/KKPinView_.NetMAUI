namespace KKOTPView.Configuration;

/// <summary>
/// Fluent builder for app-wide OTP configuration.
/// Call from MauiProgram.cs or App constructor to set defaults for all OTP views.
/// </summary>
/// <example>
/// <code>
/// OTPConfig.Configure(c => c
///     .Length(6)
///     .Secure(false)
///     .FieldColors(filled: Colors.Green, invalid: Colors.Red)
///     .ResendText("Resend code"));
/// </code>
/// </example>
public static class OTPConfig
{
    private static readonly OTPConfiguration Defaults = new();

    /// <summary>
    /// Configures app-wide OTP defaults. Pass a lambda to customize.
    /// </summary>
    /// <param name="configure">Optional. Use fluent methods to customize.</param>
    /// <returns>The builder for further chaining.</returns>
    public static OTPConfigBuilder Configure(Action<OTPConfigBuilder>? configure = null)
    {
        var builder = new OTPConfigBuilder(Defaults);
        configure?.Invoke(builder);
        return builder;
    }

    /// <summary>Gets the current default configuration. New OTP views will use a clone of this.</summary>
    public static OTPConfiguration GetDefaults() => Defaults.Clone();
}

/// <summary>
/// Fluent builder for OTP configuration. Use with <see cref="OTPConfig.Configure"/>.
/// </summary>
public sealed class OTPConfigBuilder
{
    private readonly OTPConfiguration _config;

    internal OTPConfigBuilder(OTPConfiguration config)
    {
        _config = config;
    }

    /// <summary>Sets the number of OTP digits (4, 6, 8).</summary>
    public OTPConfigBuilder Length(int length)
    {
        _config.Length = length;
        return this;
    }

    /// <summary>When true, digits are masked. When false (default), digits are visible.</summary>
    public OTPConfigBuilder Secure(bool isSecure = true)
    {
        _config.IsSecure = isSecure;
        return this;
    }

    /// <summary>Sets the view background color.</summary>
    public OTPConfigBuilder BackgroundColor(Color color)
    {
        _config.BackgroundColor = color;
        return this;
    }

    /// <summary>Sets label colors (text, error, success).</summary>
    public OTPConfigBuilder LabelColors(Color? textColor = null, Color? errorColor = null, Color? successColor = null)
    {
        if (textColor.HasValue) _config.TextColor = textColor.Value;
        if (errorColor.HasValue) _config.ErrorTextColor = errorColor.Value;
        if (successColor.HasValue) _config.SuccessTextColor = successColor.Value;
        return this;
    }

    /// <summary>Sets digit field border colors (filled, empty, invalid).</summary>
    public OTPConfigBuilder FieldColors(Color? filled = null, Color? empty = null, Color? invalid = null)
    {
        if (filled.HasValue) _config.FilledBorderColor = filled.Value;
        if (empty.HasValue) _config.EmptyBorderColor = empty.Value;
        if (invalid.HasValue) _config.InvalidBorderColor = invalid.Value;
        return this;
    }

    /// <summary>Sets digit field dimensions and spacing.</summary>
    public OTPConfigBuilder FieldSize(double width = 50, double height = 50, double spacing = 15)
    {
        _config.FieldWidth = width;
        _config.FieldHeight = height;
        _config.FieldSpacing = spacing;
        return this;
    }

    /// <summary>Sets digit field shape and corner radius.</summary>
    public OTPConfigBuilder FieldShape(KKPinView.Constants.KKPinFieldShapeType shape, double cornerRadius = 10)
    {
        _config.FieldShapeType = shape;
        _config.FieldCornerRadius = cornerRadius;
        return this;
    }

    /// <summary>Sets digit field font.</summary>
    public OTPConfigBuilder DigitFont(double fontSize = 16, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        _config.DigitFontSize = fontSize;
        _config.DigitFontAttributes = attributes;
        if (fontFamily != null) _config.DigitFontFamily = fontFamily;
        return this;
    }

    /// <summary>Sets the "Resend OTP" button text.</summary>
    public OTPConfigBuilder ResendText(string text)
    {
        _config.ResendText = text;
        return this;
    }

    /// <summary>Shows or hides the resend button.</summary>
    public OTPConfigBuilder ShowResendButton(bool show = true)
    {
        _config.ShowResendButton = show;
        return this;
    }

    /// <summary>Sets label font (title, error message).</summary>
    public OTPConfigBuilder LabelFont(double titleSize = 18, double errorSize = 24, FontAttributes attributes = FontAttributes.None, string? fontFamily = null)
    {
        _config.TitleFontSize = titleSize;
        _config.ErrorMessageFontSize = errorSize;
        _config.TitleFontAttributes = attributes;
        _config.ErrorMessageFontAttributes = attributes;
        if (fontFamily != null)
        {
            _config.TitleFontFamily = fontFamily;
            _config.ErrorMessageFontFamily = fontFamily;
        }
        return this;
    }

    /// <summary>Sets custom message strings.</summary>
    public OTPConfigBuilder Messages(string? successMessage = null, string? invalidErrorMessage = null)
    {
        if (successMessage != null) _config.SuccessMessage = successMessage;
        if (invalidErrorMessage != null) _config.InvalidErrorMessage = invalidErrorMessage;
        return this;
    }
}
