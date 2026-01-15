# KKPinView for .NET MAUI

A secure PIN entry and management library for .NET MAUI applications. Provides PIN setup, authentication, secure storage with AES-256 encryption, and lockout protection.

## Features

- 🔒 **Secure Storage**: AES-256 encryption with device-specific keys
- 🔐 **PIN Authentication**: Easy-to-use PIN entry views
- 🛡️ **Lockout Protection**: Configurable attempt limits and lockout duration
- 🎨 **Customizable UI**: Fully customizable colors, fonts, and dimensions
- 📱 **Cross-Platform**: Supports Android, iOS, and Windows
- ✨ **Modern UI**: Beautiful, native-looking PIN entry interface

## Installation

Install the package from NuGet:

```bash
dotnet add package KKPinView
```

Or via Package Manager:

```
Install-Package KKPinView
```

## Quick Start

### 1. PIN Setup

Use `KKPINSetUPView` when the user needs to create a PIN:

```csharp
using KKPinView.Views;
using KKPinView.Storage;

var setupView = new KKPINSetUPView
{
    OnSetupComplete = (pin) =>
    {
        Console.WriteLine($"PIN setup complete: {pin}");
        // Navigate to authenticated screen
    }
};

await Navigation.PushAsync(setupView);
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
        // Handle forgot PIN flow
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
            // Error is automatically displayed
        }
    },
    ShowForgotPin = true
};

await Navigation.PushAsync(pinView);
```

## Complete Example

### Authentication Flow

```csharp
using KKPinView.Views;
using KKPinView.Storage;

public partial class MainPage : ContentPage
{
    private bool _isAuthenticated = false;
    private bool _hasPIN = false;

    public MainPage()
    {
        InitializeComponent();
        CheckPINStatus();
    }

    private void CheckPINStatus()
    {
        _hasPIN = KKPinStorage.HasStoredPIN();
        
        if (!_hasPIN)
        {
            ShowPINSetup();
        }
        else if (!_isAuthenticated)
        {
            ShowPINEntry();
        }
        else
        {
            ShowMainContent();
        }
    }

    private void ShowPINSetup()
    {
        var setupView = new KKPINSetUPView
        {
            OnSetupComplete = (pin) =>
            {
                _hasPIN = true;
                _isAuthenticated = true;
                CheckPINStatus();
            }
        };
        
        Navigation.PushAsync(setupView);
    }

    private void ShowPINEntry()
    {
        var pinView = new KKPinViews
        {
            OnForgotPin = () =>
            {
                // Delete PIN and show setup screen
                KKPinStorage.DeletePIN();
                _hasPIN = false;
                CheckPINStatus();
            },
            OnSubmit = (isValid) =>
            {
                if (isValid)
                {
                    _isAuthenticated = true;
                    CheckPINStatus();
                }
            }
        };
        
        Navigation.PushAsync(pinView);
    }

    private void ShowMainContent()
    {
        // Your authenticated content here
    }
}
```

## API Documentation

### KKPinViews

Main PIN entry view for authenticating users.

#### Properties

- `OnForgotPin`: Optional callback when "Forgot PIN?" is tapped
- `OnSubmit`: Callback with validation result (`true` if PIN is valid, `false` otherwise)
- `ShowForgotPin`: Whether to show the "Forgot PIN?" button (default: `true`)
- `BackgroundColor`: Background color of the view
- `TextColor`: Text color
- `ErrorTextColor`: Error message color
- `SuccessTextColor`: Success message color
- `TitleFontSize`: Title font size
- `SubtitleFontSize`: Subtitle font size
- `FieldSpacing`: Spacing between PIN fields

#### Behavior

- Automatically validates PIN when all digits are entered
- Displays error messages for invalid PINs
- Handles lockout automatically (disables input when locked out)
- Clears PIN fields after validation

---

### KKPINSetUPView

PIN setup view for creating a new PIN with confirmation.

#### Properties

- `OnSetupComplete`: Optional callback when PIN setup is completed successfully. Receives the PIN string.
- `BackgroundColor`: Background color of the view
- `TextColor`: Text color
- `ErrorTextColor`: Error message color
- `SuccessTextColor`: Success message color
- `TitleFontSize`: Title font size
- `SubtitleFontSize`: Subtitle font size
- `FieldSpacing`: Spacing between PIN fields

#### Behavior

- Two-step flow: Enter PIN → Confirm PIN
- Validates that both PINs match
- Automatically saves PIN when both match
- Clears previous PIN and lockout state before saving
- Displays success/error messages with animations

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

### Constants

Most UI elements can be customized via `KKPinviewConstant`:

```csharp
using KKPinView.Constants;

// PIN Configuration
KKPinviewConstant.TotalDigits = 6;  // Change PIN length (default: 4)

// Lockout Configuration
KKPinviewConstant.MaxPinAttempts = 5;  // Default: 5
KKPinviewConstant.PinLockoutDurationMinutes = 5;  // Default: 5 minutes

// Colors
KKPinviewConstant.BackgroundColor = Colors.White;
KKPinviewConstant.TextColor = Colors.Black;
KKPinviewConstant.ErrorTextColor = Colors.Red;
KKPinviewConstant.SuccessTextColor = Colors.Green;
KKPinviewConstant.DigitFieldBackgroundColor = Colors.LightGray;
KKPinviewConstant.DigitFieldFilledColor = Colors.Blue;

// Fonts
KKPinviewConstant.TitleFontSize = 24;
KKPinviewConstant.SubtitleFontSize = 16;
KKPinviewConstant.DigitFontSize = 20;
KKPinviewConstant.KeypadButtonFontSize = 24;

// Dimensions
KKPinviewConstant.FieldHeight = 60;
KKPinviewConstant.FieldWidth = 60;
KKPinviewConstant.FieldSpacing = 15;
KKPinviewConstant.KeypadButtonSize = 70;
KKPinviewConstant.KeypadSpacing = 10;

// Strings
KKPinviewConstant.TitleTextFormat = "Enter PIN";
KKPinviewConstant.SubtitleText = "Enter your {0}-digit PIN";
KKPinviewConstant.ForgotPinText = "Forgot PIN?";
KKPinviewConstant.SetupTitleText = "Setup PIN";
KKPinviewConstant.ConfirmPinTitleText = "Confirm PIN";
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
│   ├── KKPinViews.xaml/cs          # PIN entry view
│   ├── KKPINSetUPView.xaml/cs      # PIN setup view
│   ├── PinDigitField.xaml/cs       # Individual digit field
│   └── NumericKeypad.xaml/cs       # Custom keypad
├── Storage/
│   └── KKPinStorage.cs             # High-level storage API
├── Security/
│   └── KKPinLockoutManager.cs      # Lockout management
├── Constants/
│   └── KKPinviewConstant.cs        # Configuration constants
└── Platforms/
    ├── Android/
    │   └── KKEncryptionHelper.cs   # Android encryption
    └── iOS/
        └── KKEncryptionHelper.cs   # iOS encryption
```

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

