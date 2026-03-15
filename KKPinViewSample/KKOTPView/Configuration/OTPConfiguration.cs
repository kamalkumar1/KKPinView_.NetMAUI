using KKPinView.Constants;

namespace KKOTPView.Configuration;

/// <summary>
/// Holds all configurable values for an OTP view instance.
/// Used by <see cref="KKOTPView"/> and <see cref="OTPConfig"/> fluent builder.
/// </summary>
public class OTPConfiguration
{
    /// <summary>Number of OTP digits (4, 6, 8, etc.). Default is 6.</summary>
    public int Length { get; set; } = 6;

    /// <summary>When true, digits are masked (dots). When false (default for OTP), digits are visible.</summary>
    public bool IsSecure { get; set; } = false;

    /// <summary>Background color of the OTP view.</summary>
    public Color BackgroundColor { get; set; } = Colors.White;

    /// <summary>Text color for labels and digits.</summary>
    public Color TextColor { get; set; } = Colors.Black;

    /// <summary>Color for error messages.</summary>
    public Color ErrorTextColor { get; set; } = Colors.Red;

    /// <summary>Color for success messages.</summary>
    public Color SuccessTextColor { get; set; } = Colors.Green;

    /// <summary>Border color when digit field is filled.</summary>
    public Color FilledBorderColor { get; set; } = Colors.Green;

    /// <summary>Border color when digit field is empty.</summary>
    public Color EmptyBorderColor { get; set; } = Colors.Gray;

    /// <summary>Border color when OTP validation fails.</summary>
    public Color InvalidBorderColor { get; set; } = Colors.Red;

    /// <summary>Background color of digit fields.</summary>
    public Color DigitFieldBackgroundColor { get; set; } = Colors.Transparent;

    /// <summary>Width of each digit field.</summary>
    public double FieldWidth { get; set; } = 50;

    /// <summary>Height of each digit field.</summary>
    public double FieldHeight { get; set; } = 50;

    /// <summary>Spacing between digit fields.</summary>
    public double FieldSpacing { get; set; } = 15;

    /// <summary>Corner radius for rounded rectangle shape.</summary>
    public double FieldCornerRadius { get; set; } = 10;

    /// <summary>Shape of digit fields (Round or RoundedRectangle).</summary>
    public KKPinFieldShapeType FieldShapeType { get; set; } = KKPinFieldShapeType.Round;

    /// <summary>Font size for digit fields.</summary>
    public double DigitFontSize { get; set; } = 16;

    /// <summary>Font attributes for digit fields.</summary>
    public FontAttributes DigitFontAttributes { get; set; } = FontAttributes.None;

    /// <summary>Font family for digit fields. Empty uses system default.</summary>
    public string DigitFontFamily { get; set; } = string.Empty;

    /// <summary>Text for the "Resend OTP" button.</summary>
    public string ResendText { get; set; } = "Resend OTP";

    /// <summary>When true, the resend button is visible.</summary>
    public bool ShowResendButton { get; set; } = true;

    /// <summary>Cooldown in seconds before resend is enabled (30-60 typical). Default 60.</summary>
    public int ResendCooldownSeconds { get; set; } = 60;

    /// <summary>Format for countdown text. {0} = seconds remaining. Default "Resend in {0}s".</summary>
    public string ResendCountdownFormat { get; set; } = "Resend in {0}s";

    /// <summary>Font size for "Resend OTP" button (when enabled).</summary>
    public double ResendButtonFontSize { get; set; } = 16;

    /// <summary>Font attributes for "Resend OTP" button (when enabled).</summary>
    public FontAttributes ResendButtonFontAttributes { get; set; } = FontAttributes.None;

    /// <summary>Font family for "Resend OTP" button (when enabled). Empty uses system default.</summary>
    public string ResendButtonFontFamily { get; set; } = string.Empty;

    /// <summary>Font size for countdown text ("Resend in Xs").</summary>
    public double ResendCountdownFontSize { get; set; } = 16;

    /// <summary>Font attributes for countdown text.</summary>
    public FontAttributes ResendCountdownFontAttributes { get; set; } = FontAttributes.None;

    /// <summary>Font family for countdown text. Empty uses system default.</summary>
    public string ResendCountdownFontFamily { get; set; } = string.Empty;

    /// <summary>When true, countdown starts automatically when view loads (OTP assumed just sent).</summary>
    public bool AutoStartCountdown { get; set; } = true;

    /// <summary>When true, paste of full OTP into any field auto-fills all fields. Default true.</summary>
    public bool EnablePasteSupport { get; set; } = true;

    /// <summary>When true, enables platform auto-read of SMS OTP (iOS one-time code, Android SmsRetriever).</summary>
    public bool EnableAutoReadSMS { get; set; } = false;

    /// <summary>Font size for title and messages.</summary>
    public double TitleFontSize { get; set; } = 18;

    /// <summary>Font attributes for title and messages.</summary>
    public FontAttributes TitleFontAttributes { get; set; } = FontAttributes.None;

    /// <summary>Font family for title and messages. Empty uses system default.</summary>
    public string TitleFontFamily { get; set; } = string.Empty;

    /// <summary>Font size for error messages.</summary>
    public double ErrorMessageFontSize { get; set; } = 24;

    /// <summary>Font attributes for error messages.</summary>
    public FontAttributes ErrorMessageFontAttributes { get; set; } = FontAttributes.None;

    /// <summary>Font family for error messages. Empty uses system default.</summary>
    public string ErrorMessageFontFamily { get; set; } = string.Empty;

    /// <summary>Height for success message label.</summary>
    public int SuccessMessageLabelHeight { get; set; } = 24;

    /// <summary>Height for error message label.</summary>
    public int ErrorMessageLabelHeight { get; set; } = 24;

    /// <summary>Message shown when OTP validation succeeds.</summary>
    public string SuccessMessage { get; set; } = "Verification successful";

    /// <summary>Message shown when OTP is invalid.</summary>
    public string InvalidErrorMessage { get; set; } = "Invalid code. Please try again.";

    /// <summary>Optional custom validator. If set, used instead of PIN storage validation. Return true if valid.</summary>
    public Func<string, bool>? CustomValidator { get; set; }

    /// <summary>Creates a copy of this configuration.</summary>
    public OTPConfiguration Clone()
    {
        return new OTPConfiguration
        {
            Length = Length,
            IsSecure = IsSecure,
            BackgroundColor = BackgroundColor,
            TextColor = TextColor,
            ErrorTextColor = ErrorTextColor,
            SuccessTextColor = SuccessTextColor,
            FilledBorderColor = FilledBorderColor,
            EmptyBorderColor = EmptyBorderColor,
            InvalidBorderColor = InvalidBorderColor,
            DigitFieldBackgroundColor = DigitFieldBackgroundColor,
            FieldWidth = FieldWidth,
            FieldHeight = FieldHeight,
            FieldSpacing = FieldSpacing,
            FieldCornerRadius = FieldCornerRadius,
            FieldShapeType = FieldShapeType,
            DigitFontSize = DigitFontSize,
            DigitFontAttributes = DigitFontAttributes,
            DigitFontFamily = DigitFontFamily,
            ResendText = ResendText,
            ShowResendButton = ShowResendButton,
            ResendCooldownSeconds = ResendCooldownSeconds,
            ResendCountdownFormat = ResendCountdownFormat,
            ResendButtonFontSize = ResendButtonFontSize,
            ResendButtonFontAttributes = ResendButtonFontAttributes,
            ResendButtonFontFamily = ResendButtonFontFamily,
            ResendCountdownFontSize = ResendCountdownFontSize,
            ResendCountdownFontAttributes = ResendCountdownFontAttributes,
            ResendCountdownFontFamily = ResendCountdownFontFamily,
            AutoStartCountdown = AutoStartCountdown,
            EnablePasteSupport = EnablePasteSupport,
            EnableAutoReadSMS = EnableAutoReadSMS,
            TitleFontSize = TitleFontSize,
            TitleFontAttributes = TitleFontAttributes,
            TitleFontFamily = TitleFontFamily,
            ErrorMessageFontSize = ErrorMessageFontSize,
            ErrorMessageFontAttributes = ErrorMessageFontAttributes,
            ErrorMessageFontFamily = ErrorMessageFontFamily,
            SuccessMessageLabelHeight = SuccessMessageLabelHeight,
            ErrorMessageLabelHeight = ErrorMessageLabelHeight,
            SuccessMessage = SuccessMessage,
            InvalidErrorMessage = InvalidErrorMessage,
            CustomValidator = CustomValidator
        };
    }
}
