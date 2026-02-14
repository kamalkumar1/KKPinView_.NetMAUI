using KKPinView.Constants;

namespace KKPinView.Tests.Constants;

public class KKPinviewConstantTests
{
    [Fact]
    public void TotalDigits_DefaultsTo4()
    {
        Assert.Equal(4, KKPinviewConstant.TotalDigits);
    }

    [Fact]
    public void MaxPinAttempts_DefaultsTo5()
    {
        Assert.Equal(5, KKPinviewConstant.MaxPinAttempts);
    }

    [Fact]
    public void PinLockoutDurationMinutes_IsPositive()
    {
        Assert.True(KKPinviewConstant.PinLockoutDurationMinutes > 0);
    }

    [Fact]
    public void FieldHeight_IsPositive()
    {
        Assert.True(KKPinviewConstant.FieldHeight > 0);
    }

    [Fact]
    public void FieldWidth_IsPositive()
    {
        Assert.True(KKPinviewConstant.FieldWidth > 0);
    }

    [Fact]
    public void StringConstants_AreNotNullOrEmpty()
    {
        Assert.False(string.IsNullOrEmpty(KKPinviewConstant.InvalidPinError));
        Assert.False(string.IsNullOrEmpty(KKPinviewConstant.LockedOutError));
        Assert.False(string.IsNullOrEmpty(KKPinviewConstant.PinMismatchError));
        Assert.False(string.IsNullOrEmpty(KKPinviewConstant.ForgotPinText));
        Assert.False(string.IsNullOrEmpty(KKPinviewConstant.EnterPinMessage));
        Assert.False(string.IsNullOrEmpty(KKPinviewConstant.ConfirmPinMessage));
    }

    [Fact]
    public void LockedOutError_ContainsPlaceholder()
    {
        Assert.Contains("{0}", KKPinviewConstant.LockedOutError);
    }
}
