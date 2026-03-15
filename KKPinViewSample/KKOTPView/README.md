# KKOTPView

OTP (One-Time Password) entry view for .NET MAUI with fluent API and globally followed OTP UX patterns.

## Features (Globally Followed OTP Standards)

- **Resend with countdown** – 30–60 second cooldown, "Resend in Xs" text, disabled during countdown
- **Paste support** – Paste full OTP into any field; digits are distributed and auto-submitted when complete
- **Clear on resend** – Input is cleared when the resend button is tapped
- **Auto-read SMS (iOS)** – One-time-code autocomplete when SMS OTP is received
- **Auto-focus** – First field focused when view loads
- **Auto-submit** – Validation runs when all digits are entered

## Quick Start

### Instance-level fluent API

```csharp
var otpView = new KKOTPView()
    .Length(6)
    .Secure(false)
    .FieldColors(filled: Colors.Green, invalid: Colors.Red)
    .ResendText("Resend code")
    .ResendCooldown(60)
    .OnSubmit(isValid => HandleValidation(isValid))
    .OnResend(() => RequestNewOTP());

Content = otpView;
```

### App-wide defaults (MauiProgram.cs)

```csharp
OTPConfig.Configure(c => c
    .Length(6)
    .Secure(false)
    .ResendCooldown(60)
    .FieldColors(filled: Colors.Green, invalid: Colors.Red)
    .ResendText("Resend code")
    .EnableAutoReadSMS(true)
    .Messages(successMessage: "Verified!", invalidErrorMessage: "Invalid code"));
```

### Custom validation (e.g. server API)

```csharp
var otpView = new KKOTPView()
    .Length(6)
    .OnValidate(otp => ValidateOTPWithServer(otp))
    .OnSubmit(isValid => { /* handle result */ });
```

## Fluent Methods

| Method | Description |
|--------|-------------|
| `Length(int)` | Number of OTP digits (4, 6, 8) |
| `Secure(bool)` | Mask digits (default: false for OTP) |
| `BackgroundColor(Color)` | View background |
| `LabelColors(text?, error?, success?)` | Label colors |
| `FieldColors(filled?, empty?, invalid?)` | Digit field border colors |
| `FieldSize(width?, height?, spacing?)` | Field dimensions |
| `FieldShape(shape, cornerRadius)` | Round or RoundedRectangle |
| `DigitFont(size?, attributes?, family?)` | Digit field font |
| `LabelFont(titleSize?, errorSize?, attributes?, family?)` | Label, message, and resend button font |
| `ResendText(string)` | Resend button text |
| `ResendButtonFont(size?, attributes?, family?)` | Font for "Resend OTP" (when enabled) |
| `ResendCountdownFont(size?, attributes?, family?)` | Font for "Resend in Xs" (countdown) |
| `ResendCooldown(int)` | Cooldown seconds (30–60 typical) |
| `ShowResendButton(bool)` | Show/hide resend button |
| `EnablePaste(bool)` | Paste support (default: true) |
| `EnableAutoReadSMS(bool)` | iOS one-time-code autocomplete |
| `AutoStartCountdown(bool)` | Start countdown on load (default: true) |
| `Messages(success?, invalid?)` | Custom message strings |
| `OnValidate(Func<string,bool>)` | Custom validator |
| `OnSubmit(Action<bool>)` | Validation complete callback |
| `OnResend(Action)` | Resend button callback |

## Public Methods

| Method | Description |
|--------|-------------|
| `ShowKeyboard()` | Focus first field to show keyboard |
| `StartCountdown()` | Start resend countdown (call after sending OTP) |
| `ClearOTP()` | Clear all digit fields |

## Defaults

- **Length**: 6 digits
- **Secure**: false (digits visible, typical for OTP)
- **ResendText**: "Resend OTP"
- **ResendCooldown**: 60 seconds
- **ShowResendButton**: true
- **EnablePaste**: true
- **AutoStartCountdown**: true
