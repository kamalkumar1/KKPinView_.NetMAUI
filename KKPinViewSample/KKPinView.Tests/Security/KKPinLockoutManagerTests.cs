using KKPinView.Constants;
using KKPinView.Security;

namespace KKPinView.Tests.Security;

public class KKPinLockoutManagerTests
{
    [Fact]
    public void Constructor_WithDefaultValues_UsesConstants()
    {
        var manager = new KKPinLockoutManager();
        Assert.Equal(KKPinviewConstant.MaxPinAttempts, manager.MaxAttempts);
        Assert.Equal(KKPinviewConstant.PinLockoutDurationMinutes, manager.LockoutDurationMinutes);
    }

    [Fact]
    public void Constructor_WithCustomValues_UsesProvidedValues()
    {
        var manager = new KKPinLockoutManager(maxAttempts: 3, lockoutDurationMinutes: 10);
        Assert.Equal(3, manager.MaxAttempts);
        Assert.Equal(10, manager.LockoutDurationMinutes);
    }

    [Fact]
    public void Constructor_WithZeroValues_UsesConstants()
    {
        var manager = new KKPinLockoutManager(0, 0);
        Assert.Equal(KKPinviewConstant.MaxPinAttempts, manager.MaxAttempts);
        Assert.Equal(KKPinviewConstant.PinLockoutDurationMinutes, manager.LockoutDurationMinutes);
    }

    [Fact]
    public void FailedAttempts_InitiallyZero()
    {
        var manager = new KKPinLockoutManager(5, 5);
        Assert.Equal(0, manager.FailedAttempts);
    }

    [Fact]
    public void HasReachedMaxAttempts_InitiallyFalse()
    {
        var manager = new KKPinLockoutManager(5, 5);
        Assert.False(manager.HasReachedMaxAttempts);
    }

    [Fact]
    public void ResetFailedAttempts_DoesNotThrow()
    {
        var manager = new KKPinLockoutManager(5, 5);
        var exception = Record.Exception(() => manager.ResetFailedAttempts());
        Assert.Null(exception);
    }

    [Fact]
    public void GetErrorMessage_WhenNoFailedAttempts_ReturnsNull()
    {
        var manager = new KKPinLockoutManager(5, 5);
        var message = manager.GetErrorMessage();
        Assert.Null(message);
    }
}
