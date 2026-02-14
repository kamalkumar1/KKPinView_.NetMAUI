using KKPinView.Constants;
using Xunit;

namespace KKPinView.Tests.Security;

/// <summary>
/// KKPinLockoutManager uses MAUI Preferences API which requires platform-specific
/// implementation (Android/iOS). It cannot be unit tested in the portable net10.0
/// context. These tests verify lockout-related constants and documentation.
/// Run full KKPinLockoutManager tests on device/emulator.
/// </summary>
public class KKPinLockoutManagerTests
{
    [Fact]
    public void LockoutConstants_AreConfigured()
    {
        Assert.True(KKPinviewConstant.MaxPinAttempts > 0);
        Assert.True(KKPinviewConstant.PinLockoutDurationMinutes > 0);
    }

    [Fact]
    public void LockedOutError_ContainsPlaceholderForMinutes()
    {
        var format = KKPinviewConstant.LockedOutError;
        Assert.Contains("{0}", format);
    }
}
