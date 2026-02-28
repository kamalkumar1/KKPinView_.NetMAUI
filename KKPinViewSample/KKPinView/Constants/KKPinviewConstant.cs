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
    /// <summary>Shape type for PIN fields. Default is Round.</summary>
    public static PinFieldShapeType FieldShapeType { get; set; } = PinFieldShapeType.Round;

    /// <summary>When true, enables debug logging and behavior.</summary>
    public static bool EnableDebugMode { get; set; } = KKPinViewDebug.IsDebugBuild();

    /// <summary>Number of PIN digit fields (e.g. 4 or 6).</summary>
    public static int TotalPinTextFields { get; set; } = 4;

    /// <summary>Maximum failed PIN attempts before lockout.</summary>
    public static int MaxPinAttempts { get; set; } = 5;
    /// <summary>Lockout duration in minutes after too many failed attempts.</summary>
    public static int PinLockoutDurationMinutes { get; set; } = 5;

    /// <summary>Background color of the PIN view.</summary>
    public static Color BackgroundColor { get; set; } = Colors.White;
    /// <summary>Text color for labels and digits.</summary>
    public static Color TextColor { get; set; } = Colors.Black;
    /// <summary>Text color for error messages.</summary>
    public static Color ErrorTextColor { get; set; } = Colors.Red;
    /// <summary>Text color for success messages.</summary>
    public static Color SuccessTextColor { get; set; } = Colors.Green;
    /// <summary>Background color of each PIN digit field.</summary>
    public static Color DigitFieldBackgroundColor { get; set; } = Colors.Transparent;
    /// <summary>Border color when a digit field is filled.</summary>
    public static Color DigitFieldFilledColor { get; set; } = Colors.Green;
    /// <summary>Border color when the digit field is empty (unfilled).</summary>
    public static Color DigitFieldEmptyBorderColor { get; set; } = Colors.Gray;
    /// <summary>Border color when PIN is invalid (e.g. wrong PIN).</summary>
    public static Color InvalidPinBorderColor { get; set; } = Colors.Red;

    // Fonts
    /// <summary>Font size for title text.</summary>
    public static double TitleFontSize { get; set; } = 24;
    /// <summary>Font size for subtitle and message labels.</summary>
    public static double SubtitleFontSize { get; set; } = 16;
    /// <summary>Font size for digits inside PIN fields.</summary>
    public static double DigitFontSize { get; set; } = 20;

    /// <summary>Height of each PIN digit field.</summary>
    public static double FieldHeight { get; set; } = 50;
    /// <summary>Width of each PIN digit field.</summary>
    public static double FieldWidth { get; set; } = 50;
    /// <summary>Horizontal spacing between PIN digit fields.</summary>
    public static double FieldSpacing { get; set; } = 15;
    /// <summary>Corner radius for rounded-rectangle PIN fields.</summary>
    public static double FieldCornerRadius { get; set; } = 10;

    /// <summary>Title text format for the enter-PIN view.</summary>
    public static string TitleTextFormat { get; set; } = "Enter PIN";
    /// <summary>Subtitle text format; {0} is the PIN length (e.g. "Enter your {0}-digit PIN").</summary>
    public static string SubtitleText { get; set; } = "Enter your {0}-digit PIN";
    /// <summary>Text for the "Forgot PIN?" button.</summary>
    public static string ForgotPinText { get; set; } = "Forgot PIN?";
    /// <summary>Setup page/section title text.</summary>
    public static string SetupTitleText { get; set; } = "Setup PIN";
    /// <summary>Confirm PIN section title text.</summary>
    public static string ConfirmPinTitleText { get; set; } = "Confirm PIN";
    /// <summary>Error message when Enter and Confirm PIN do not match.</summary>
    public static string PinMismatchError { get; set; } = "PINs do not match";
    /// <summary>Error message when the entered PIN is wrong.</summary>
    public static string InvalidPinError { get; set; } = "Invalid PIN";
    /// <summary>Lockout message format; {0} is remaining minutes.</summary>
    public static string LockedOutError { get; set; } = "Too many failed attempts. Please try again in {0} minutes";
    /// <summary>Message shown when PIN setup completes successfully.</summary>
    public static string SetupSuccessMessage { get; set; } = "PIN setup successful";
    /// <summary>Shown when PIN save fails during setup.</summary>
    public static string SetupSaveFailedMessage { get; set; } = "Failed to save PIN. Please try again.";
    /// <summary>Label text for the Enter PIN step (e.g. "Enter your PIN").</summary>
    public static string EnterPinMessage { get; set; } = "Enter your PIN";
    /// <summary>Label text for the Confirm PIN step (e.g. "Confirm your PIN").</summary>
    public static string ConfirmPinMessage { get; set; } = "Confirm your PIN";

    /// <summary>Height reserved for the success message label when visible.</summary>
    public static int SuccessMessageLabelHeight = 24;
    /// <summary>Height reserved for the error message label when visible.</summary>
    public static int ErrorMessageLabelHeight = 24;

    /// <summary>How long the PIN mismatch error is shown (ms) before it fades out and PIN fields are reset.</summary>
    public static int PinMismatchErrorDisplayDurationMs { get; set; } = 1500;
}

