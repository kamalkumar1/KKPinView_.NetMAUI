using KKPinView.Debug;

namespace KKPinView.Constants;

/// <summary>
/// Shape type for PIN digit text fields.
/// </summary>
public enum KKPinFieldShapeType
{
    /// <summary>Rectangle with rounded corners (default).</summary>
    RoundedRectangle,
    /// <summary>Round shape (circle or oval).</summary>
    Round
}

/// <summary>
/// Label appearance properties (title, subtitle, error, success messages).
/// </summary>
public static class LabelConstants
{
    public static Color TextColor { get; set; } = Colors.Black;
    public static Color ErrorTextColor { get; set; } = Colors.Red;
    public static Color SuccessTextColor { get; set; } = Colors.Green;

    public static double FontSize { get; set; } = 18;
    public static FontAttributes FontAttributes { get; set; } = FontAttributes.None;
    public static string FontFamily { get; set; } = string.Empty;

    public static double ErrorMessageFontSize { get; set; } = 24;
    public static FontAttributes ErrorMessageFontAttributes { get; set; } = FontAttributes.None;
    public static string ErrorMessageFontFamily { get; set; } = string.Empty;

    public static int SuccessMessageLabelHeight { get; set; } = 24;
    public static int ErrorMessageLabelHeight { get; set; } = 24;
}

/// <summary>
/// PIN digit field appearance and layout properties.
/// </summary>
public static class PinFieldConstants
{
    public static Color BackgroundColor { get; set; } = Colors.Transparent;
    public static Color FilledBorderColor { get; set; } = Colors.Green;
    public static Color EmptyBorderColor { get; set; } = Colors.Gray;
    public static Color InvalidBorderColor { get; set; } = Colors.Red;

    public static double FontSize { get; set; } = 16;
    public static FontAttributes FontAttributes { get; set; } = FontAttributes.None;
    public static string FontFamily { get; set; } = string.Empty;

    public static double Height { get; set; } = 50;
    public static double Width { get; set; } = 50;
    public static double Spacing { get; set; } = 15;
    public static double CornerRadius { get; set; } = 10;
    public static KKPinFieldShapeType ShapeType { get; set; } = KKPinFieldShapeType.Round;
}

/// <summary>
/// Constants for KKPinView configuration
/// </summary>
public static class KKPinviewConstant
{
    /// <summary>When true, enables debug logging and behavior.</summary>
    public static bool EnableDebugMode { get; set; } = KKPinViewDebug.IsDebugBuild();

    /// <summary>Number of PIN digit fields (e.g. 4 or 6).</summary>
    public static int TotalPinTextFields { get; set; } = 4;

    /// <summary>Maximum failed PIN attempts before lockout.</summary>
    public static int MaxPinAttempts { get; set; } = 5;
    /// <summary>Lockout duration in minutes after too many failed attempts.</summary>
    public static int PinLockoutDurationMinutes { get; set; } = 5;

    /// <summary>
    /// When true, PIN is stored in Keychain (iOS) / EncryptedSharedPreferences (Android) and persists after app uninstall.
    /// When false (default), PIN is stored in app Preferences and is removed when the app is uninstalled.
    /// Note: On Android, app data is always removed on uninstall regardless of this setting.
    /// </summary>
    public static bool PinStoragePersistsAfterUninstall { get; set; } = false;

    /// <summary>Background color of the PIN view.</summary>
    public static Color BackgroundColor { get; set; } = Colors.White;

    // Backward compatibility - forward to nested classes
    public static Color TextColor { get => LabelConstants.TextColor; set => LabelConstants.TextColor = value; }
    public static Color ErrorTextColor { get => LabelConstants.ErrorTextColor; set => LabelConstants.ErrorTextColor = value; }
    public static Color SuccessTextColor { get => LabelConstants.SuccessTextColor; set => LabelConstants.SuccessTextColor = value; }
    public static Color DigitFieldBackgroundColor { get => PinFieldConstants.BackgroundColor; set => PinFieldConstants.BackgroundColor = value; }
    public static Color DigitFieldFilledColor { get => PinFieldConstants.FilledBorderColor; set => PinFieldConstants.FilledBorderColor = value; }
    public static Color DigitFieldEmptyBorderColor { get => PinFieldConstants.EmptyBorderColor; set => PinFieldConstants.EmptyBorderColor = value; }
    public static Color InvalidPinBorderColor { get => PinFieldConstants.InvalidBorderColor; set => PinFieldConstants.InvalidBorderColor = value; }

    public static double TitleFontSize { get => LabelConstants.FontSize; set => LabelConstants.FontSize = value; }
    public static FontAttributes TitleFontAttributes { get => LabelConstants.FontAttributes; set => LabelConstants.FontAttributes = value; }
    public static string TitleFontFamily { get => LabelConstants.FontFamily; set => LabelConstants.FontFamily = value; }
    public static double ErrorMessageFontSize { get => LabelConstants.ErrorMessageFontSize; set => LabelConstants.ErrorMessageFontSize = value; }
    public static FontAttributes ErrorMessageFontAttributes { get => LabelConstants.ErrorMessageFontAttributes; set => LabelConstants.ErrorMessageFontAttributes = value; }
    public static string ErrorMessageFontFamily { get => LabelConstants.ErrorMessageFontFamily; set => LabelConstants.ErrorMessageFontFamily = value; }

    public static double DigitFontSize { get => PinFieldConstants.FontSize; set => PinFieldConstants.FontSize = value; }
    public static FontAttributes DigitFontAttributes { get => PinFieldConstants.FontAttributes; set => PinFieldConstants.FontAttributes = value; }
    public static string DigitFontFamily { get => PinFieldConstants.FontFamily; set => PinFieldConstants.FontFamily = value; }

    public static double FieldHeight { get => PinFieldConstants.Height; set => PinFieldConstants.Height = value; }
    public static double FieldWidth { get => PinFieldConstants.Width; set => PinFieldConstants.Width = value; }
    public static double FieldSpacing { get => PinFieldConstants.Spacing; set => PinFieldConstants.Spacing = value; }
    public static double FieldCornerRadius { get => PinFieldConstants.CornerRadius; set => PinFieldConstants.CornerRadius = value; }
    public static KKPinFieldShapeType FieldShapeType { get => PinFieldConstants.ShapeType; set => PinFieldConstants.ShapeType = value; }

    public static int SuccessMessageLabelHeight { get => LabelConstants.SuccessMessageLabelHeight; set => LabelConstants.SuccessMessageLabelHeight = value; }
    public static int ErrorMessageLabelHeight { get => LabelConstants.ErrorMessageLabelHeight; set => LabelConstants.ErrorMessageLabelHeight = value; }

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

    /// <summary>How long the PIN mismatch error is shown (ms) before it fades out and PIN fields are reset.</summary>
    public static int PinMismatchErrorDisplayDurationMs { get; set; } = 1500;

    /// <summary>
    /// One-call configuration for easy integration. Call from App constructor before any PIN view is created.
    /// </summary>
    /// <param name="configure">Optional. Use fluent methods to customize (e.g. c => c.PinLength(4).Lockout(5, 5))</param>
    /// <example>
    /// <code>
    /// // Minimal - use defaults (4 digits, 5 attempts, 5 min lockout)
    /// KKPinviewConstant.Configure();
    ///
    /// // Customize
    /// KKPinviewConstant.Configure(c => c
    ///     .PinLength(6)
    ///     .Lockout(3, 10)
    ///     .PinStoragePersistsAfterUninstall(true)  // optional: PIN survives app uninstall (iOS)
    ///     .LabelColors(errorColor: Colors.Red)
    ///     .PinFieldColors(filled: Colors.Green, invalid: Colors.Red)
    ///     .PinField(fontSize: 20, shape: KKPinFieldShapeType.RoundedRectangle));
    /// </code>
    /// </example>
    public static void Configure(Action<KKPinViewConfig>? configure = null)
    {
        configure?.Invoke(new KKPinViewConfig());
    }
}
