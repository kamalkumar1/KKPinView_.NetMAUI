# KKPinView.Tests

Unit tests for the KKPinView library.

## Setup

The KKPinView library was configured with a `net10.0` target framework to support unit testing. The test project references KKPinView and runs tests against the shared code.

## Running Tests

```bash
# Run all tests
dotnet test KKPinView.Tests/KKPinView.Tests.csproj

# Run with verbose output
dotnet test KKPinView.Tests/KKPinView.Tests.csproj -v normal

# Run specific test class
dotnet test KKPinView.Tests/KKPinView.Tests.csproj --filter "FullyQualifiedName~KKPinviewConstantTests"
```

## Test Coverage

| Test Class | Coverage |
|------------|----------|
| `PinInputMethodTests` | Enum values and uniqueness |
| `KKPinviewConstantTests` | Default constants and string formats |
| `KKPINSetUPViewModelTests` | Setup ViewModel initialization, commands, properties |
| `KKPinViewsViewModelTests` | PIN entry ViewModel initialization, commands, properties |
| `KKPinLockoutManagerTests` | Lockout manager constructor, configuration, reset behavior |

## Adding More Tests

- **ViewModels**: Test property changes, command execution, callbacks
- **Storage/Security**: Tests that require `KKPinStorage` or `Preferences` may need mocking or run in a device/emulator context
- **Views**: UI components require MAUI application context; consider using device runners or UI test frameworks
