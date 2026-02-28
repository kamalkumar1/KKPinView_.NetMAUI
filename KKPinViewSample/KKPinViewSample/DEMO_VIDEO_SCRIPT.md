# KKPinView Demo – Video Script for LinkedIn

Use this flow to record a short demo of the KKPinView library and share it on LinkedIn.

---

## Before Recording

1. Build and run the app (Android/iOS/Windows).
2. Open the **Demo** home screen (KKPinView Demo).
3. Use a clean state: tap **"1. Reset PIN → PIN Setup"** once so there is no stored PIN, then go back to the demo menu (or keep the app on the demo menu after a reset).

---

## Recording Order (about 60–90 seconds)

### 1. PIN Setup (15–20 s)
- Tap **"1. Reset PIN → PIN Setup"**.
- Enter a 4-digit PIN (e.g. `1234`).
- Enter the same PIN again in Confirm.
- **Show:** Success message and any success animation.
- *Optional:* Tap **"2. PIN Setup"** again and enter a *different* confirm PIN to show **mismatch error** and the **reset-after-error** behavior.

### 2. PIN Entry – Correct PIN (10–15 s)
- From the app, go back to the **Demo** menu (back button or Shell).
- Tap **"3. PIN Entry (authenticate)"**.
- Enter the same PIN you set (`1234`).
- **Show:** Success and navigation back (or to the next screen).

### 3. PIN Entry – Wrong PIN (10–15 s)
- Tap **"3. PIN Entry"** again.
- Enter a **wrong** PIN (e.g. `0000`).
- **Show:** Red border on fields and error message (invalid PIN / lockout after several attempts if configured).

### 4. Forgot PIN (10–15 s)
- From PIN Entry, tap **"Forgot PIN?"** (if your app shows it).
- **Show:** PIN is cleared and user is taken back to **PIN Setup** (or your configured flow).

### 5. Full flow in one take (optional, 30–40 s)
- **Reset PIN → PIN Setup** → Enter PIN → Confirm PIN → Success.
- Navigate to **PIN Entry** → Enter correct PIN → Success / home.
- Or: PIN Entry → Wrong PIN (red border) → Then correct PIN → Success.

---

## Tips for Recording

- **Device/simulator:** Use a device or simulator with a clear screen; avoid small windows so the PIN fields are easy to see.
- **Speed:** Pause 1–2 seconds on each main screen so viewers can read labels (e.g. “Enter PIN”, “Confirm PIN”, error message).
- **Length:** Aim for **60–90 seconds** for LinkedIn; shorter (45–60 s) often works better.
- **Captions:** Add short on-screen text if needed (e.g. “Setup”, “Wrong PIN”, “Success”) for accessibility and silent viewing.

---

## Suggested LinkedIn Post

**Short version:**
> Quick demo of **KKPinView** for .NET MAUI: PIN setup, confirmation, secure storage, and invalid-PIN feedback. All configurable via a single constants class. 🔐 #dotnet #MAUI #Xamarin #MobileDev

**With link:**
> I built **KKPinView** – a .NET MAUI library for PIN entry, setup, and secure storage. This demo shows setup → confirm → success, wrong PIN (red border + error), and the “Forgot PIN” flow. Perfect for apps that need a simple, customizable PIN screen. [Link to repo or NuGet]

**Hashtags (pick a few):**  
`#dotnet` `#MAUI` `#Xamarin` `#MobileDevelopment` `#OpenSource` `#CSharp`

---

## Project Structure (for your reference)

```
KKPinViewSample/
├── App.xaml / App.xaml.cs          # App entry, optional constant config
├── AppShell.xaml / .cs             # Shell; home = Demo menu
├── DemoMenuPage.xaml / .cs         # Demo menu (this script’s entry point)
├── PinSetupView.xaml / .cs         # PIN Setup (KKPINSetUPView)
├── PINView.xaml / .cs              # PIN Entry (KKPinViews)
├── MainPage.xaml / .cs             # Legacy home (optional)
└── DEMO_VIDEO_SCRIPT.md            # This file
```

To open the demo menu directly after deployment, the app shell’s home content is set to `DemoMenuPage`.
