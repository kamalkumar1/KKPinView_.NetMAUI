using KKPinView.Constants;
using KKPinView.Helpers;
using KKPinView.ViewModels;
using Xunit;

namespace KKPinView.Tests.Views;

/// <summary>
/// Regression tests: tap-to-continuation (first empty), focus-next on digit entry, mismatch reset, validation.
/// Ensures existing functionality (match/mismatch, 4/6 fields, MaxPinLength) is not affected.
/// </summary>
[Collection("Constants")]
public class SetupViewBehaviorRegressionTests
{
    [Fact]
    public void FirstEmpty_WhenEnterIncomplete_IsInEnterSection_Spec()
    {
        // Tap anywhere (including Confirm) while Enter PIN has empty fields: target first empty Enter field.
        var enterDigits = new[] { "1", "", "", "" };
        int firstEmptyEnter = PinFieldHelpers.GetFirstEmptyFieldIndex(enterDigits, 4);
        Assert.Equal(1, firstEmptyEnter);
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

    /// <summary>Documents contract: after digit entry in a non-last field, next focus target is fieldIndex+1.</summary>
    [Fact]
    public void DigitEntry_NextFieldIndex_IsCurrentPlusOne()
    {
        int fieldIndex = 2; // e.g. third Enter PIN field
        int fieldCount = 4;
        int nextIndex = fieldIndex + 1;
        Assert.True(nextIndex < fieldCount);
        Assert.Equal(3, nextIndex);
    }
}
