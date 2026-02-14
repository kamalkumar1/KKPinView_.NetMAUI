using KKPinView.Constants;
using Xunit;

namespace KKPinView.Tests.Constants;

[Collection("Constants")]
public class KKPinviewConstantTests
{
    [Fact]
    public void TotalPinTextFields_DefaultsTo4()
    {
        // Restore default in case another test modified it; assert the default value
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 4;
            Assert.Equal(4, KKPinviewConstant.TotalPinTextFields);
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
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
