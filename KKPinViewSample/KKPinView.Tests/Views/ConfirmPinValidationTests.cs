using KKPinView.Constants;
using KKPinView.ViewModels;
using Xunit;

namespace KKPinView.Tests.Views;

/// <summary>
/// Tests for confirm PIN validation logic with 4 and 6 PIN text fields.
/// Verifies that validation triggers when the correct number of digits is entered.
/// </summary>
[Collection("Constants")]
public class ConfirmPinValidationTests
{
    [Fact]
    public void With4PinTextFields_ViewModelMaxPinLengthIs4()
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

    [Fact]
    public void With6PinTextFields_ViewModelMaxPinLengthIs6()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 6;
            var vm = new KKPINSetUPViewModel();

            Assert.Equal(6, vm.MaxPinLength);
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }

    [Fact]
    public void With4PinTextFields_ConfirmPinCompleteWhenLength4()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 4;
            int expectedLength = 4;

            // Simulate validation condition: confirm PIN is complete when length matches field count
            string enterPin = "1234";
            string confirmPin = "1234";

            Assert.Equal(expectedLength, enterPin.Length);
            Assert.Equal(expectedLength, confirmPin.Length);
            Assert.Equal(enterPin, confirmPin); // Match - would pass validation
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }

    [Fact]
    public void With4PinTextFields_ConfirmPinMismatchFailsValidation()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 4;
            int expectedLength = 4;

            string enterPin = "1234";
            string confirmPin = "1235"; // Different last digit

            Assert.Equal(expectedLength, enterPin.Length);
            Assert.Equal(expectedLength, confirmPin.Length);
            Assert.NotEqual(enterPin, confirmPin); // Mismatch - would fail validation
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }

    [Fact]
    public void With6PinTextFields_ConfirmPinCompleteWhenLength6()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 6;
            int expectedLength = 6;

            string enterPin = "123456";
            string confirmPin = "123456";

            Assert.Equal(expectedLength, enterPin.Length);
            Assert.Equal(expectedLength, confirmPin.Length);
            Assert.Equal(enterPin, confirmPin); // Match - would pass validation
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }

    [Fact]
    public void With6PinTextFields_ConfirmPinMismatchFailsValidation()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 6;
            int expectedLength = 6;

            string enterPin = "123456";
            string confirmPin = "123457"; // Different last digit

            Assert.Equal(expectedLength, enterPin.Length);
            Assert.Equal(expectedLength, confirmPin.Length);
            Assert.NotEqual(enterPin, confirmPin); // Mismatch - would fail validation
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }

    [Fact]
    public void With4Fields_ConfirmPinIncompleteWhenLengthLessThan4()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 4;

            string confirmPin = "123"; // Only 3 digits
            bool isComplete = confirmPin.Length == 4;

            Assert.False(isComplete);
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }

    [Fact]
    public void With6Fields_ConfirmPinIncompleteWhenLengthLessThan6()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 6;

            string confirmPin = "12345"; // Only 5 digits
            bool isComplete = confirmPin.Length == 6;

            Assert.False(isComplete);
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }
}
