# KKPinView for .NET MAUI

A secure PIN entry and management library for .NET MAUI applications. Provides PIN setup, authentication, secure storage with AES-256 encryption, and lockout protection.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-10.0-blue.svg)](https://dotnet.microsoft.com/apps/maui)

## Features

- 🔒 **Secure Storage**: AES-256 encryption with device-specific keys
- 🔐 **PIN Authentication**: Easy-to-use PIN entry views (setup + confirm, then entry)
- 🛡️ **Lockout Protection**: Configurable max attempts and lockout duration; after too many failed tries, user is locked out with countdown message until retry
- 🎨 **Customizable UI**: All colors, fonts, and dimensions via `KKPinviewConstant` (single source of truth)
- 📱 **Cross-Platform**: Supports Android, iOS.
- ✨ **Modern UI**: Native-looking PIN entry with system keyboard
- ⌨️ **System Keyboard**: Numeric keyboard with auto-focus between fields, tap-to-continue (first empty field)
- 🎯 **Visual Feedback**: Animated red border for invalid PIN; border animates when showing/hiding error state
- 📏 **Dynamic Layout**: Auto-adjusting error/success message heights with fade and scale animations
- 🔄 **PIN Mismatch Flow**: Error message animates in, holds, then fades out; all PIN fields reset and focus returns to first Enter field
- 📍 **Focus Behavior**: After backspace or re-entry, focus goes to the first empty field so the next digit goes in the right box
- 🔐 **Secure PIN Field**: PIN digits can be masked (dots) or visible; controlled via fluent API `PinFieldSecure()`

## Installation

Install the package from NuGet:

```bash
dotnet add package KKPinView
```

Or via Package Manager:

```
Install-Package KKPinView
```

---

## Integration Guide

Follow these steps to integrate KKPinView into your .NET MAUI app.

| Step | Action |
|------|--------|
| 1 | Register `UseKKPinView()` in `MauiProgram.cs` |
| 2 | Configure `KKPinviewConstant.TotalPinTextFields` in `App` constructor |
| 3 | Register Shell routes for `PinSetupView` and `PINView` |
| 4 | Create PIN Setup page (XAML + code-behind) |
| 5 | Create PIN Entry page (XAML + code-behind) |
| 6 | Add navigation from your menu/home page |
| 7 | (Optional) Handle Android back button if needed |
| 8 | (Optional) Add app startup flow (PIN on first launch or resume) |

### Step 1: Register handlers in MauiProgram.cs

Call `UseKKPinView()` in your `MauiProgram.cs`. This configures the backspace-on-empty-field behavior on iOS and Android.

```csharp
using KKPinView;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseKKPinView()   // Required for backspace on empty field
            .ConfigureFonts(fonts => { /* ... */ });

        return builder.Build();
    }
}
```

### Step 2: Configure in App.xaml.cs

Call `Configure()` in your `App` constructor before any PIN view is created. Use the fluent API for easy setup:

```csharp
using KKPinView.Constants;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Minimal: 4 digits, default lockout (5 attempts, 5 min)
       // KKPinviewConstant.Configure(c => c.PinLength(4));

       
       // Or customize more with fluent API
		KKPinviewConstant.Configure(c => c
				.PinLength(4)
				.Lockout(2, 10)
				.LabelColors(errorColor: Colors.Red, successColor: Colors.Green, textColor: Colors.Black)
				.LabelFont(fontSize: 18, attributes: FontAttributes.Bold, fontFamily: "OpenSansSemibold")
				.ErrorMessageFont(fontSize: 17, attributes: FontAttributes.Bold, fontFamily: "OpenSansSemibold")
				.DigitFont(fontSize: 20, attributes: FontAttributes.Bold, fontFamily: "OpenSansSemibold")
				.PinFieldColors(filled: Colors.Green, invalid: Colors.Red)
				.PinStoragePersistsAfterUninstall(true)
				.PinField(fontSize: 20, shape: KKPinFieldShapeType.Round)
				.PinFieldSecure(true));  // true = masked (dots), false = visible digits
    }
}
```

### Step 3: Register Shell routes

Register routes for your PIN pages in `App.xaml.cs` or `AppShell.xaml.cs`:

```csharp
public App()
{
    InitializeComponent();
    KKPinviewConstant.Configure(c => c.PinLength(4));
    Routing.RegisterRoute("PinSetupView", typeof(PinSetupView));
    Routing.RegisterRoute("PINView", typeof(PINView));  // or "PinEntryView"
}
```

In `AppShell.xaml.cs`, also register your Shell items and routes (e.g. `DemoMenuPage`, `PinSetupView`, `PINView`) so `Shell.Current.GoToAsync()` can resolve them.

### Step 4: Create PIN Setup page

**PinSetupView.xaml** – ContentPage with `KKPINSetUPView`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:views="clr-namespace:KKPinView.Views;assembly=KKPinView"
    x:Class="YourApp.PinSetupView"
    Title="PIN Setup"
    BackgroundColor="White">

    <views:KKPINSetUPView
        x:Name="PinSetupContentView"
        VerticalOptions="Fill"
        HorizontalOptions="Fill" />
</ContentPage>
```

**PinSetupView.xaml.cs** – Wire callbacks and show keyboard:

```csharp
using KKPinView.Storage;
using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;

public partial class PinSetupView : ContentPage
{
    public PinSetupView()
    {
        InitializeComponent();
        PinSetupContentView.OnCreationCompleted = () => PinSetupContentView?.ShowKeyboard();
        Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (PinSetupContentView == null) return;

        PinSetupContentView.OnSetupSuccess = () =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync("PINView");  // Navigate to PIN entry
            });
        };

        PinSetupContentView.OnSetupFailed = (errorMessage) =>
        {
            System.Diagnostics.Debug.WriteLine($"PIN setup failed: {errorMessage}");
        };
    }
}
```

### Step 5: Create PIN Entry page

**PINView.xaml** – ContentPage with `KKPinViews`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:views="clr-namespace:KKPinView.Views;assembly=KKPinView"
    x:Class="YourApp.PINView"
    Title="Enter PIN"
    BackgroundColor="White">

    <views:KKPinViews
        x:Name="PinEntryContentView"
        VerticalOptions="Start"
        HorizontalOptions="Fill"
        Margin="0,24,0,0" />
</ContentPage>
```

**PINView.xaml.cs** – Wire callbacks and show keyboard:

```csharp
using KKPinView.Storage;
using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;

public partial class PINView : ContentPage
{
    public PINView()
    {
        InitializeComponent();
        PinEntryContentView.OnCreationCompleted = () => PinEntryContentView?.ShowKeyboard();
        Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (PinEntryContentView == null) return;

        PinEntryContentView.OnForgotPin = () =>
        {
            KKPinStorage.DeletePIN();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync("PinSetupView");
            });
        };

        PinEntryContentView.OnSubmit = (isValid) =>
        {
            if (!isValid) return;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Shell.Current != null)
                    await Shell.Current.GoToAsync("//MainPage");  // Navigate to home
            });
        };
    }
}
```

### Step 6: Add entry points and navigation

From your home/menu page, navigate based on whether a PIN exists:

```csharp
using KKPinView.Storage;

// "Create PIN" or "Setup PIN" button
private async void OnSetupPinClicked(object? sender, EventArgs e)
{
    if (Shell.Current != null)
        await Shell.Current.GoToAsync("PinSetupView");
}

// "Enter PIN" or "Authenticate" button (enable only if PIN exists)
private async void OnEnterPinClicked(object? sender, EventArgs e)
{
    if (!KKPinStorage.HasStoredPIN())
    {
        await DisplayAlert("No PIN", "Create a PIN first.", "OK");
        return;
    }
    if (Shell.Current != null)
        await Shell.Current.GoToAsync("PINView");
}

// In OnAppearing, enable/disable buttons based on HasStoredPIN()
protected override void OnAppearing()
{
    base.OnAppearing();
    BtnEnterPin.IsEnabled = KKPinStorage.HasStoredPIN();
}
```

### Step 7: (Optional) Android back button

To allow users to go back from the PIN entry/setup pages on Android, no extra code is needed—Shell handles navigation. If you use a custom back callback, ensure it does not conflict with Shell navigation.

### Step 8: App startup flow (optional)

To require PIN on first launch or app resume, check `KKPinStorage.HasStoredPIN()` in `CreateWindow` or `OnResume` and navigate accordingly. Example in `CreateWindow`:

```csharp
protected override Window CreateWindow(IActivationState? activationState)
{
    var shell = new AppShell();
    return new Window(shell);
}
```

Then in your Shell or first page, redirect to `PinSetupView` or `PINView` based on `HasStoredPIN()`.

---

## Integration Patterns

### Pattern A: Shell navigation (recommended)

Use separate `ContentPage`s for Setup and Entry, and navigate with `Shell.Current.GoToAsync()`. This matches the step-by-step guide above and the KKPinViewSample app.

### Pattern B: Single page with content swap

Use one page and swap `Content` between `KKPINSetUPView` and `KKPinViews` based on `KKPinStorage.HasStoredPIN()`:

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
        if (KKPinStorage.HasStoredPIN())
            ShowPinEntryView();
        else
            ShowPinSetupView();
    }

    private void ShowPinSetupView()
    {
        PinContentView.Content = null;
        _pinView = null;
        _setupView = new KKPINSetUPView();
        PinContentView.Content = _setupView;
        _setupView.OnSetupSuccess = () => ShowPinEntryView();
        _setupView.OnSetupFailed = (msg) => { };
    }

    private void ShowPinEntryView()
    {
        PinContentView.Content = null;
        _setupView = null;
        _pinView = new KKPinViews();
        PinContentView.Content = _pinView;
        _pinView.OnForgotPin = () => { KKPinStorage.DeletePIN(); ShowPinSetupView(); };
        _pinView.OnSubmit = (isValid) => { if (isValid) { /* Navigate or show main content */ } };
    }
}
```

---

## Quick Reference

### PIN Setup (`KKPINSetUPView`)

```csharp
var setupView = new KKPINSetUPView
{
    OnSetupSuccess = () => { /* Navigate or swap view */ },
    OnSetupFailed = (msg) => { /* Error already shown in view */ }
};
// Add to ContentPage: Content = setupView;
setupView.OnCreationCompleted = () => setupView.ShowKeyboard();
```

### PIN Entry (`KKPinViews`)

```csharp
var pinView = new KKPinViews
{
    OnForgotPin = () => { KKPinStorage.DeletePIN(); /* Navigate to setup */ },
    OnSubmit = (isValid) => { if (isValid) { /* Navigate to authenticated screen */ } },
    ShowForgotPin = true
};
// Add to ContentPage: Content = pinView;
pinView.OnCreationCompleted = () => pinView.ShowKeyboard();
```

---

## Input and keyboard

KKPinView uses the **system numeric keyboard**. PIN fields are single-digit entries; focus moves automatically to the next field when a digit is entered. Tapping anywhere on the PIN area focuses the first empty field. Backspace moves focus to the previous (or first empty) field.

**Tip:** Set `OnCreationCompleted = () => view.ShowKeyboard()` so the keyboard opens when the view is ready. The library does not auto-focus; the host app controls when to show the keyboard via this callback.

---

## API Documentation

### KKPinViews

Main PIN entry view for authenticating users.

#### Properties

- `OnForgotPin`: Optional callback when "Forgot PIN?" is tapped
- `OnSubmit`: Callback with validation result (`true` if PIN is valid, `false` otherwise)
- `ShowForgotPin`: Whether to show the "Forgot PIN?" button (default: `true`)
- Display values (colors, fonts, spacing, labels) are read from `KKPinviewConstant`; set them on the constant class (e.g. in app startup) to customize.

#### Methods

- `OnCreationCompleted`: Callback invoked when the PIN view is fully created and ready. Use this to call `ShowKeyboard()` or perform other setup.
- `ShowKeyboard()`: Focuses the first PIN field to bring up the system keyboard. Call from `OnCreationCompleted` or when you want to show the keyboard.

#### Behavior

- Validates PIN when all digits are entered (compares to securely stored PIN)
- Error message and invalid-state borders animate in; border color animates when showing or clearing error
- Handles lockout automatically (shows lockout message, disables input when locked out)
- Tap anywhere focuses the first empty field; backspace focuses the previous or first empty field

---

### KKPINSetUPView

PIN setup view for creating a new PIN with confirmation.

#### Properties

- `OnCreationCompleted`: Callback invoked when the PIN setup view is fully created and ready. Use this to call `ShowKeyboard()` or perform other setup.
- `OnSetupSuccess`: Callback when PIN setup completes successfully (PIN is already saved)
- `OnSetupFailed`: Callback when setup fails (e.g. mismatch or save error); receives the error message string
- `EnterPinLabelText`, `ConfirmPinLabelText`: Read-only; values come from `KKPinviewConstant.EnterPinMessage` and `ConfirmPinMessage`. Change labels by setting those constants.
- Other display values (colors, fonts, spacing) are read from `KKPinviewConstant`; customize via the constant class only.

#### Methods

- `ShowKeyboard()`: Focuses the first PIN field to bring up the system keyboard. Call from `OnCreationCompleted` or when you want to show the keyboard.

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

### Easy configuration: Configure()

Use `KKPinviewConstant.Configure()` for one-call setup. Call from your `App` constructor:

```csharp
using KKPinView.Constants;

// Minimal - defaults (4 digits, 5 attempts, 5 min lockout)
KKPinviewConstant.Configure();

// Or customize with fluent API
KKPinviewConstant.Configure(c => c
    .PinLength(6)                                    // 6-digit PIN
    .Lockout(3, 10)                                  // 3 attempts, 10 min lockout
    .BackgroundColor(Colors.White)
    .LabelColors(textColor: Colors.Black, errorColor: Colors.Red)
    .LabelFont(fontSize: 18, attributes: FontAttributes.Bold)
    .ErrorMessageFont(fontSize: 24)
    .PinFieldColors(filled: Colors.Green, empty: Colors.Gray, invalid: Colors.Red)
    .PinField(fontSize: 20, height: 50, width: 50, spacing: 15, shape: KKPinFieldShapeType.Round)
    .PinFieldCornerRadius(10)                        // For RoundedRectangle
    .PinFieldSecure(true)                            // true = masked (dots), false = visible digits
    .Labels(enterPin: "Enter your PIN", confirmPin: "Confirm your PIN", forgotPin: "Forgot PIN?"));
```

### Constants (advanced)

For fine-grained control, set properties directly on `KKPinviewConstant`, `LabelConstants`, and `PinFieldConstants`:

```csharp
using KKPinView.Constants;

// PIN Configuration
KKPinviewConstant.TotalPinTextFields = 4;  // 4 or 6 (default: 4)

// Lockout
KKPinviewConstant.MaxPinAttempts = 5;
KKPinviewConstant.PinLockoutDurationMinutes = 5;

// Colors
KKPinviewConstant.BackgroundColor = Colors.White;

// Label properties (titles, messages)
LabelConstants.TextColor = Colors.Black;
LabelConstants.ErrorTextColor = Colors.Red;
LabelConstants.SuccessTextColor = Colors.Green;
LabelConstants.FontSize = 16;
LabelConstants.FontAttributes = FontAttributes.None;
LabelConstants.FontFamily = string.Empty;
LabelConstants.ErrorMessageFontSize = 24;
LabelConstants.ErrorMessageFontAttributes = FontAttributes.None;
LabelConstants.ErrorMessageFontFamily = string.Empty;
LabelConstants.SuccessMessageLabelHeight = 24;
LabelConstants.ErrorMessageLabelHeight = 24;

// Pin field properties (digit boxes)
PinFieldConstants.BackgroundColor = Colors.Transparent;
PinFieldConstants.FilledBorderColor = Colors.Green;
PinFieldConstants.EmptyBorderColor = Colors.Gray;
PinFieldConstants.InvalidBorderColor = Colors.Red;
PinFieldConstants.FontSize = 20;
PinFieldConstants.FontAttributes = FontAttributes.None;
PinFieldConstants.FontFamily = string.Empty;
PinFieldConstants.Height = 50;
PinFieldConstants.Width = 50;
PinFieldConstants.Spacing = 15;
PinFieldConstants.CornerRadius = 10;

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
PinFieldConstants.ShapeType = KKPinFieldShapeType.Round;  // or RoundedRectangle
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
├── MauiAppBuilderExtensions.cs    # UseKKPinView() - required for backspace on empty
└── Platforms/
    ├── Android/
    │   ├── BackspaceAwareEditText  # Android backspace on empty
    │   └── (encryption/storage)
    └── iOS/
        └── (BackspaceAwareTextField, encryption/storage)
```

## API Design

The library uses access specifiers and sealing to protect the implementation and prevent unintended overrides.

### Public API (consumer-facing)

| Type | Purpose |
|------|---------|
| `KKPinViews` | Main PIN entry view |
| `KKPINSetUPView` | PIN setup view |
| `PinDigitField` | Single digit field (used in XAML) |
| `BackspaceAwareEntry` | Entry with backspace-on-empty support |
| `KKPinStorage` | Save/Load/Verify/Delete PIN |
| `KKPinLockoutManager` | Lockout and attempt tracking |
| `KKPinviewConstant`, `KKPinViewConfig`, `LabelConstants`, `PinFieldConstants`, `KKPinFieldShapeType` | Configuration |
| `BasePinViewModel`, `KKPinViewsViewModel`, `KKPINSetUPViewModel` | ViewModels (exposed via `ViewModel` property) |
| `MauiAppBuilderExtensions` | `UseKKPinView()` registration |

### Internal (implementation details)

| Type | Reason |
|------|--------|
| `PinFieldHelpers` | First-empty field logic; used only by views |
| `IKKPinStorage`, `KKPinStorageFallback`, `KKPinStorageiOS`, `KKPinStorageAndroid` | Platform storage abstraction |
| `SharedEncryptionHelper` | AES encryption for Preferences mode |
| `KKEncryptionHelperiOS`, `KKEncryptionHelperAndroid` | Platform-specific encryption |
| `BackspaceAwareTextField`, `BackspaceAwareEditText` | Platform-specific views |
| `PlatformClass1` | Placeholder; not referenced |

### Sealed classes (no inheritance)

These types are `sealed` so consumers cannot subclass and override behavior:

- `KKPinViews`, `KKPINSetUPView`, `PinDigitField`, `BackspaceAwareEntry`
- `KKPinLockoutManager`
- `KKPinViewsViewModel`, `KKPINSetUPViewModel`
- `KKPinViewConfig`

Customization is done via `KKPinviewConstant.Configure()`, bindable properties, and callbacks—not inheritance.

### Test access

The test project (`KKPinView.Tests`) uses `InternalsVisibleTo` to access `PinFieldHelpers` for unit tests.

## Sample App

The **KKPinViewSample** project demonstrates the full integration:

- **Demo menu** – Home screen with buttons for Reset PIN, PIN Setup, PIN Entry, and Forgot PIN flow
- **PinSetupView** – `ContentPage` hosting `KKPINSetUPView`
- **PINView** – `ContentPage` hosting `KKPinViews`
- **AppShell** – Shell with routes for `MainPage`, `DemoMenuPage`, `PinSetupView`, `PINView`
- **DEMO_VIDEO_SCRIPT.md** – Step-by-step script for recording a demo

Run the sample and use "Reset PIN → PIN Setup" to start a clean flow. All configuration is via `KKPinviewConstant` in the sample app.

## Screenshots

| Screenshot | Description |
|------------|--------------|
| [demo-menu.png](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/demo-menu.png) | Demo menu with Setup PIN, Validate PIN, and Forgot PIN buttons |
| [pin-setup.png](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/pin-setup.png) | PIN Setup – Enter and Confirm PIN flow |
| [pin-entry.png](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/pin-entry.png) | PIN Entry – Authentication screen |
| [invalid-Entrypin.png](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/invalid-Entrypin.png) | Invalid PIN – Red border feedback (entry) |
| [invalid-setuppin.png](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/invalid-setuppin.png) | Invalid PIN – Red border feedback (setup) |

![Demo Menu](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/demo-menu.png)
![PIN Setup](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/pin-setup.png)
![PIN Entry](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/pin-entry.png)
![Invalid PIN - Setup](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/invalid-setuppin.png)
![Invalid PIN - Entry](https://raw.githubusercontent.com/kamalkumar1/KKPinView_.NetMAUI/main/KKPinViewSample/KKPinViewSample/screenshots/invalid-Entrypin.png)

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

