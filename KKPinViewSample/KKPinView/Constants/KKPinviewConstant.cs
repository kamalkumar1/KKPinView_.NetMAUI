namespace KKPinView.Constants;

/// <summary>
/// Constants for KKPinView configuration
/// </summary>
public static class KKPinviewConstant
{
    // PIN Configuration
    public static int TotalDigits { get; set; } = 4;
    
    // Lockout Configuration
    public static int MaxPinAttempts { get; set; } = 5;
    public static int PinLockoutDurationMinutes { get; set; } = 5;
    
    // Colors
    public static Color BackgroundColor { get; set; } = Colors.White;
    public static Color TextColor { get; set; } = Colors.Black;
    public static Color ErrorTextColor { get; set; } = Colors.Red;
    public static Color SuccessTextColor { get; set; } = Colors.Green;
    public static Color DigitFieldBackgroundColor { get; set; } = Colors.LightGray;
    public static Color DigitFieldFilledColor { get; set; } = Colors.Blue;
    public static Color KeypadButtonColor { get; set; } = Colors.White;
    public static Color KeypadButtonTextColor { get; set; } = Colors.Black;
    
    // Fonts
    public static double TitleFontSize { get; set; } = 24;
    public static double SubtitleFontSize { get; set; } = 16;
    public static double DigitFontSize { get; set; } = 20;
    public static double KeypadButtonFontSize { get; set; } = 24;
    
    // Dimensions
    public static double FieldHeight { get; set; } = 60;
    public static double FieldWidth { get; set; } = 60;
    public static double FieldSpacing { get; set; } = 15;
    public static double KeypadButtonSize { get; set; } = 70;
    public static double KeypadSpacing { get; set; } = 10;
    
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
}

