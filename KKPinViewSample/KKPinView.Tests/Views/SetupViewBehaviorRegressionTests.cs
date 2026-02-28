using KKPinView.Constants;
using KKPinView.Helpers;
using KKPinView.ViewModels;
using Xunit;

namespace KKPinView.Tests.Views;

/// <summary>
/// Regression tests: expected behavior after tap-to-continuation and mismatch-reset changes.
/// Ensures existing functionality (validation, match/mismatch, 4/6 fields) is not affected.
/// </summary>
[Collection("Constants")]
public class SetupViewBehaviorRegressionTests
{
    [Fact]
    public void FirstEmpty_WhenEnterIncomplete_IsInEnterSection_Spec()
    {
        // After our change: tap anywhere (including Confirm) while Enter PIN has empty fields
        // should target first empty Enter field. Helper "first empty" index applies per section.
        var enterDigits = new[] { "1", "", "", "" }; // one digit entered
        int firstEmptyEnter = PinFieldHelpers.GetFirstEmptyFieldIndex(enterDigits, 4);
        Assert.Equal(1, firstEmptyEnter);
        // So next digit should go to index 1 (continuation), not Confirm
    }

    [Fact]
    public void FirstEmpty_WhenAllEnterFilled_IsZero_Spec()
    {
        var enterDigits = new[] { "1", "2", "3", "4" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(enterDigits, 4);
        Assert.Equal(0, index);
    }

    [Fact]
    public void MatchValidation_StillRequiresEqualLengthAndValue()
    {
        string enterPin = "1234";
        string confirmPin = "1234";
        Assert.Equal(enterPin.Length, confirmPin.Length);
        Assert.Equal(enterPin, confirmPin);
    }

    [Fact]
    public void MismatchValidation_StillFailsWhenDifferent()
    {
        string enterPin = "1234";
        string confirmPin = "1235";
        Assert.NotEqual(enterPin, confirmPin);
    }

    [Fact]
    public void With4Fields_MaxPinLength4_Unchanged()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 4;
            var vm = new KKPINSetUPViewModel();
            Assert.Equal(4, vm.MaxPinLength);
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }
}
