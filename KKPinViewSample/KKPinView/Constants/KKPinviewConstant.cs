using KKPinView.Debug;

namespace KKPinView.Constants;

/// <summary>
/// Shape type for PIN digit text fields.
/// </summary>
public enum PinFieldShapeType
{
    /// <summary>Rectangle with rounded corners (default).</summary>
    RoundedRectangle,
    /// <summary>Round shape (circle or oval).</summary>
    Round
}

/// <summary>
/// Constants for KKPinView configuration
/// </summary>
public static class KKPinviewConstant
{
    /// <summary>Shape type for PIN fields. Default is Rounded.</summary>
    public static PinFieldShapeType FieldShapeType { get; set; } = PinFieldShapeType.Round;
    // Debug Configuration
    public static bool EnableDebugMode { get; set; } = KKPinViewDebug.IsDebugBuild();

    // PIN Configuration
    public static int TotalPinTextFields { get; set; } = 4;

    // Lockout Configuration
    public static int MaxPinAttempts { get; set; } = 5;
    public static int PinLockoutDurationMinutes { get; set; } = 5;

    // Colors
    public static Color BackgroundColor { get; set; } = Colors.White;
    public static Color TextColor { get; set; } = Colors.Black;
    public static Color ErrorTextColor { get; set; } = Colors.Red;
    public static Color SuccessTextColor { get; set; } = Colors.Green;
    public static Color DigitFieldBackgroundColor { get; set; } = Colors.Transparent;
    public static Color DigitFieldFilledColor { get; set; } = Colors.Green;
    public static Color InvalidPinBorderColor { get; set; } = Colors.Red;

    // Fonts
    public static double TitleFontSize { get; set; } = 24;
    public static double SubtitleFontSize { get; set; } = 16;
    public static double DigitFontSize { get; set; } = 20;

    // Dimensions
    public static double FieldHeight { get; set; } = 50;
    public static double FieldWidth { get; set; } = 50;
    public static double FieldSpacing { get; set; } = 15;
    public static double FieldCornerRadius { get; set; } = 10;


    // Strings
    public static string TitleTextFormat { get; set; } = "Enter PIN";
    public static string SubtitleText { get; set; } = "Enter your {0}-digit PIN";
    public static string ForgotPinText { get; set; } = "Forgot PIN?";
    public static string SetupTitleText { get; set; } = "Setup PIN";
    public static string ConfirmPinTitleText { get; set; } = "Confirm PIN";
    public static string PinMismatchError { get; set; } = "PINs do not match";
    public static string InvalidPinError { get; set; } = "Invalid PIN";
    public static string LockedOutError { get; set; } = "Too many failed attempts. Please try again in {0} minutes";
    public static string SetupSuccessMessage { get; set; } = "PIN setup successful";
    public static string EnterPinMessage { get; set; } = "Enter your PIN";
    public static string ConfirmPinMessage { get; set; } = "Confirm your PIN";

    public static int SuccessMessageLabelHeight = 24;
    public static int ErrorMessageLabelHeight = 24;
}

