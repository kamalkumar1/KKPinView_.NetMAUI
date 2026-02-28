# KKPinView for .NET MAUI

A secure PIN entry and management library for .NET MAUI applications. Provides PIN setup, authentication, secure storage with AES-256 encryption, and lockout protection.

## Features

- 🔒 **Secure Storage**: AES-256 encryption with device-specific keys
- 🔐 **PIN Authentication**: Easy-to-use PIN entry views (setup + confirm, then entry)
- 🛡️ **Lockout Protection**: Configurable attempt limits and lockout duration
- 🎨 **Customizable UI**: All colors, fonts, and dimensions via `KKPinviewConstant` (single source of truth)
- 📱 **Cross-Platform**: Supports Android, iOS, and Windows
- ✨ **Modern UI**: Native-looking PIN entry with system keyboard
- ⌨️ **System Keyboard**: Numeric keyboard with auto-focus between fields, tap-to-continue (first empty field)
- 🎯 **Visual Feedback**: Animated red border for invalid PIN; border animates when showing/hiding error state
- 📏 **Dynamic Layout**: Auto-adjusting error/success message heights with fade and scale animations
- 🔄 **PIN Mismatch Flow**: Error message animates in, holds, then fades out; all PIN fields reset and focus returns to first Enter field
- 📍 **Focus Behavior**: After backspace or re-entry, focus goes to the first empty field so the next digit goes in the right box

## Installation

Install the package from NuGet:

```bash
dotnet add package KKPinView
```

Or via Package Manager:

```
Install-Package KKPinView
```

## Screenshots

<!-- Add your screenshots here -->
<!-- 
![PIN Setup View](screenshots/pin-setup.png)
![PIN Entry View](screenshots/pin-entry.png)
![Invalid PIN](screenshots/invalid-pin.png)
![Lockout Screen](screenshots/lockout.png)
![Numeric Keypad](screenshots/numeric-keypad.png)
![System Keyboard](screenshots/system-keyboard.png)
-->

> **Note**: Screenshots will be added here. Please add your screenshots to the `screenshots/` folder and update the paths above.

### Visual Features

- **Invalid PIN Indicator**: All PIN fields show red borders when the PIN is wrong; border color animates in/out
- **PIN Mismatch (Setup)**: On mismatch, error message animates in, displays for a configurable duration, then fades out and all Enter/Confirm fields are cleared and focus moves to the first Enter field
- **Dynamic Error/Success Messages**: Message height and opacity animate; configurable label heights via constants
- **Border Animation**: Border color transitions smoothly when switching between normal, filled, and invalid states
- **Auto-focus**: Focus moves to the next field on digit entry; tap anywhere focuses the first empty field in the current step (Enter or Confirm)
- **Re-entry After Delete**: After backspace, focus is set to the first empty field so typing again fills digits in order

## Quick Start

### Simple Integration Example

The simplest way to integrate KKPinView is to check if a PIN exists and show the appropriate view:

```csharp
using KKPinView.Storage;
using KKPinView.Views;

public partial class MainPage : ContentPage
{
    private KKPINSetUPView? _setupView;
    private KKPinViews? _pinView;

    public MainPage()
    {
        InitializeComponent();
        LoadPinView();
    }

    private void LoadPinView()
    {
        // Check if PIN exists in storage
        bool hasPin = KKPinStorage.HasStoredPIN();

        if (hasPin)
        {
            // PIN exists - show PIN entry view
            ShowPinEntryView();
        }
        else
        {
            // No PIN stored - show PIN setup view
            ShowPinSetupView();
        }
    }

    private void ShowPinSetupView()
    {
        // Clean up existing view
        PinContentView.Content = null;
        _pinView = null;

        // Create and show PIN setup view
        _setupView = new KKPINSetUPView();
        PinContentView.Content = _setupView;

        // Handle successful PIN setup - switch to PIN entry view
        _setupView.OnSetupSuccess = () =>
        {
            ShowPinEntryView();
        };

        // Handle PIN setup failure
        _setupView.OnSetupFailed = (errorMessage) =>
        {
            // Error is already displayed in the view
            System.Diagnostics.Debug.WriteLine($"PIN setup failed: {errorMessage}");
        };
    }

    private void ShowPinEntryView()
    {
        // Clean up existing view
        PinContentView.Content = null;
        _setupView = null;

        // Create and show PIN entry view
        _pinView = new KKPinViews();
        PinContentView.Content = _pinView;

        // Handle "Forgot PIN" - delete PIN and show setup view
        _pinView.OnForgotPin = () =>
        {
            KKPinStorage.DeletePIN();
            ShowPinSetupView();
        };

        // Handle PIN validation result
        _pinView.OnSubmit = (isValid) =>
        {
            if (isValid)
            {
                // PIN is valid - user is authenticated
                // Navigate to your authenticated page or show main content here
                System.Diagnostics.Debug.WriteLine("PIN validated successfully!");
            }
        };
    }
}
```

### 1. PIN Setup

Use `KKPINSetUPView` when the user needs to create a PIN:

```csharp
using KKPinView.Views;
using KKPinView.Storage;

var setupView = new KKPINSetUPView
{
    OnSetupSuccess = () =>
    {
        Console.WriteLine("PIN setup complete");
        // Navigate to authenticated screen
    },
    OnSetupFailed = (errorMessage) =>
    {
        Console.WriteLine($"PIN setup failed: {errorMessage}");
        // Error is already displayed in the view
    }
};

// Add to your ContentPage
PinContentView.Content = setupView;
```

### 2. PIN Entry (Authentication)

Use `KKPinViews` when the user needs to enter their PIN:

```csharp
using KKPinView.Views;
using KKPinView.Storage;

var pinView = new KKPinViews
{
    OnForgotPin = () =>
    {
        Console.WriteLine("Forgot PIN tapped");
        // Handle forgot PIN flow (e.g., delete PIN and show setup)
        KKPinStorage.DeletePIN();
    },
    OnSubmit = (isValid) =>
    {
        if (isValid)
        {
            Console.WriteLine("PIN is valid - access granted");
            // Navigate to authenticated screen
        }
        else
        {
            Console.WriteLine("PIN is invalid");
            // Error is automatically displayed with red borders
        }
    },
    ShowForgotPin = true
};

// Add to your ContentPage
PinContentView.Content = pinView;
```

## Input

KKPinView uses the **system numeric keyboard**. PIN fields are single-digit entries; focus moves automatically to the next field when a digit is entered. Tapping anywhere on the PIN area focuses the first empty field so digits always flow left to right. Backspace is handled per field with focus moving to the previous (or first empty) field as appropriate.

## API Documentation

### KKPinViews

Main PIN entry view for authenticating users.

#### Properties

- `OnForgotPin`: Optional callback when "Forgot PIN?" is tapped
- `OnSubmit`: Callback with validation result (`true` if PIN is valid, `false` otherwise)
- `ShowForgotPin`: Whether to show the "Forgot PIN?" button (default: `true`)
- Display values (colors, fonts, spacing, labels) are read from `KKPinviewConstant`; set them on the constant class (e.g. in app startup) to customize.

#### Behavior

- Validates PIN when all digits are entered (compares to securely stored PIN)
- Error message and invalid-state borders animate in; border color animates when showing or clearing error
- Handles lockout automatically (shows lockout message, disables input when locked out)
- Tap anywhere focuses the first empty field; backspace focuses the previous or first empty field

---

### KKPINSetUPView

PIN setup view for creating a new PIN with confirmation.

#### Properties

- `OnSetupSuccess`: Callback when PIN setup completes successfully (PIN is already saved)
- `OnSetupFailed`: Callback when setup fails (e.g. mismatch or save error); receives the error message string
- `EnterPinLabelText`, `ConfirmPinLabelText`: Read-only; values come from `KKPinviewConstant.EnterPinMessage` and `ConfirmPinMessage`. Change labels by setting those constants.
- Other display values (colors, fonts, spacing) are read from `KKPinviewConstant`; customize via the constant class only.

#### Behavior

- Two-step flow: Enter PIN → Confirm PIN
- On match: saves PIN, shows success animation, invokes `OnSetupSuccess`
- On mismatch: shows error with animation, holds for `PinMismatchErrorDisplayDurationMs`, then fades out and resets all Enter/Confirm fields and focus to first Enter field; invokes `OnSetupFailed`
- Tap anywhere focuses the first empty field in the current step (Enter or Confirm)

---

### KKPinStorage

High-level API for securely storing and retrieving PINs.

#### Methods

```csharp
// Save a PIN
bool SavePIN(string pin)

// Load stored PIN
string? LoadPIN()

// Verify a PIN
bool VerifyPIN(string pin)

// Check if PIN exists
bool HasStoredPIN()

// Delete stored PIN
void DeletePIN()
```

#### Example

```csharp
using KKPinView.Storage;

// Save PIN
if (KKPinStorage.SavePIN("1234"))
{
    Console.WriteLine("PIN saved successfully");
}

// Verify PIN
if (KKPinStorage.VerifyPIN("1234"))
{
    Console.WriteLine("PIN is correct");
}

// Check if PIN exists
if (KKPinStorage.HasStoredPIN())
{
    // Show PIN entry screen
}
else
{
    // Show PIN setup screen
}
```

---

### KKPinLockoutManager

Manages PIN validation attempts and lockout logic.

#### Properties

- `FailedAttempts`: Current number of failed attempts
- `MaxAttempts`: Maximum allowed attempts
- `IsLockedOut`: Whether currently locked out
- `RemainingLockoutMinutes`: Remaining lockout time
- `HasReachedMaxAttempts`: Whether max attempts reached

#### Methods

```csharp
// Validate PIN (handles attempt tracking)
bool ValidatePIN(string pin)

// Reset failed attempts
void ResetFailedAttempts()

// Check lockout status
void CheckLockoutStatus()

// Get error message
string? GetErrorMessage()
```

#### Example

```csharp
using KKPinView.Security;

var manager = new KKPinLockoutManager();

if (manager.IsLockedOut)
{
    Console.WriteLine($"Locked out for {manager.RemainingLockoutMinutes} minutes");
}

if (manager.ValidatePIN("1234"))
{
    Console.WriteLine("PIN is valid");
}
else
{
    if (manager.GetErrorMessage() is string error)
    {
        Console.WriteLine(error);
    }
}
```

## Customization

### Constants (single source of truth)

All UI text, colors, dimensions, and behavior are configured **only** via `KKPinviewConstant`. ViewModels expose these as read-only; set constants (e.g. in app startup) to customize.

```csharp
using KKPinView.Constants;

// PIN Configuration
KKPinviewConstant.TotalPinTextFields = 4;  // 4 or 6 (default: 4)

// Lockout
KKPinviewConstant.MaxPinAttempts = 5;
KKPinviewConstant.PinLockoutDurationMinutes = 5;

// Colors
KKPinviewConstant.BackgroundColor = Colors.White;
KKPinviewConstant.TextColor = Colors.Black;
KKPinviewConstant.ErrorTextColor = Colors.Red;
KKPinviewConstant.SuccessTextColor = Colors.Green;
KKPinviewConstant.DigitFieldBackgroundColor = Colors.Transparent;
KKPinviewConstant.DigitFieldFilledColor = Colors.Green;
KKPinviewConstant.DigitFieldEmptyBorderColor = Colors.Gray;   // Unfilled field border
KKPinviewConstant.InvalidPinBorderColor = Colors.Red;        // Wrong PIN border

// Fonts
KKPinviewConstant.TitleFontSize = 24;
KKPinviewConstant.SubtitleFontSize = 16;
KKPinviewConstant.DigitFontSize = 20;

// Dimensions
KKPinviewConstant.FieldHeight = 50;
KKPinviewConstant.FieldWidth = 50;
KKPinviewConstant.FieldSpacing = 15;
KKPinviewConstant.FieldCornerRadius = 10;
KKPinviewConstant.SuccessMessageLabelHeight = 24;
KKPinviewConstant.ErrorMessageLabelHeight = 24;

// Strings (labels and messages)
KKPinviewConstant.TitleTextFormat = "Enter PIN";
KKPinviewConstant.SubtitleText = "Enter your {0}-digit PIN";
KKPinviewConstant.ForgotPinText = "Forgot PIN?";
KKPinviewConstant.SetupTitleText = "Setup PIN";
KKPinviewConstant.ConfirmPinTitleText = "Confirm PIN";
KKPinviewConstant.EnterPinMessage = "Enter your PIN";
KKPinviewConstant.ConfirmPinMessage = "Confirm your PIN";
KKPinviewConstant.PinMismatchError = "PINs do not match";
KKPinviewConstant.InvalidPinError = "Invalid PIN";
KKPinviewConstant.LockedOutError = "Too many failed attempts. Please try again in {0} minutes";
KKPinviewConstant.SetupSuccessMessage = "PIN setup successful";
KKPinviewConstant.SetupSaveFailedMessage = "Failed to save PIN. Please try again.";

// PIN mismatch: how long (ms) the error is shown before fade-out and field reset
KKPinviewConstant.PinMismatchErrorDisplayDurationMs = 1500;

// Shape
KKPinviewConstant.FieldShapeType = PinFieldShapeType.Round;  // or RoundedRectangle
```

## Security Features

### Encryption

- **Algorithm**: AES-256-CBC
- **Key Derivation**: SHA256 with device-specific salt
- **Key Storage**: Secure keychain/keyring storage with device binding

### File Protection

- **Protection Type**: Platform-specific secure storage
- **Access**: PINs are only accessible when device is unlocked
- **Storage Location**: Secure storage (Keychain on iOS, EncryptedSharedPreferences on Android)

### Lockout Protection

- **Default Max Attempts**: 5
- **Default Lockout Duration**: 5 minutes
- **Configurable**: Can be customized via `KKPinviewConstant` or `KKPinLockoutManager`

### Security Notes

- PINs are encrypted before storage
- Encryption keys are device-specific (cannot be transferred)
- Files are protected at the OS level
- Failed attempts are tracked and enforced
- Lockout state persists across app launches

## Architecture

```
KKPinView/
├── Views/
│   ├── KKPinViews.xaml/cs          # PIN entry (authenticate)
│   ├── KKPINSetUPView.xaml/cs      # PIN setup (enter + confirm)
│   ├── PinDigitField.xaml/cs       # Single digit field (system keyboard)
│   └── BackspaceAwareEntry.cs      # Entry with empty-backspace event
├── Helpers/
│   └── PinFieldHelpers.cs           # First-empty field index, etc.
├── Handlers/
│   └── BackspaceAwareEntryHandler  # Platform backspace handling
├── Storage/
│   └── KKPinStorage.cs             # Save/Load/Verify/Delete PIN
├── Security/
│   └── KKPinLockoutManager.cs      # Lockout and attempt tracking
├── ViewModels/
│   ├── BasePinViewModel.cs         # Shared bindings (read from constants)
│   ├── KKPINSetUPViewModel.cs      # Setup view model
│   └── KKPinViewsViewModel.cs     # Entry view model
├── Constants/
│   └── KKPinviewConstant.cs        # All configuration (single source)
├── MauiAppBuilderExtensions.cs    # Optional UseKKPinView() registration
└── Platforms/
    ├── Android/
    │   ├── BackspaceAwareEditText  # Android backspace on empty
    │   └── (encryption/storage)
    └── iOS/
        └── (BackspaceAwareTextField, encryption/storage)
```

## Sample App

The **KKPinViewSample** project includes:

- **Demo menu** – Home screen with buttons to try PIN Setup, PIN Entry, Reset, and Forgot PIN flow
- **DEMO_VIDEO_SCRIPT.md** – Step-by-step script for recording a short demo (e.g. for LinkedIn)

Run the sample, open the Demo menu, and use "Reset PIN → PIN Setup" to start a clean flow. All configuration is via `KKPinviewConstant` in the sample app.

## Requirements

- .NET 10.0 or later
- .NET MAUI
- Android API 21+ (Android 5.0+)
- iOS 15.0+
- Windows 10.0.19041.0+ (optional)

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues, questions, or feature requests, please open an issue on GitHub.

## Author

Created by kamalkumar

---

**Made with ❤️ using .NET MAUI**

