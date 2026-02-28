using KKPinView.Helpers;
using Xunit;

namespace KKPinView.Tests.Helpers;

/// <summary>
/// Tests for "first empty field" logic used when user taps anywhere so digits continue in order.
/// Covers: tap on any box -> focus first empty (continuation); existing behavior unchanged.
/// </summary>
public class FirstEmptyFieldLogicTests
{
    [Fact]
    public void GetFirstEmptyFieldIndex_AllEmpty_ReturnsZero()
    {
        var digits = new[] { "", "", "", "" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, digits.Length);
        Assert.Equal(0, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_FirstEmpty_ReturnsZero()
    {
        var digits = new[] { "", "2", "3", "4" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, digits.Length);
        Assert.Equal(0, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_SecondEmpty_ReturnsOne()
    {
        var digits = new[] { "1", "", "3", "4" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, digits.Length);
        Assert.Equal(1, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_ThirdEmpty_ReturnsTwo()
    {
        var digits = new[] { "1", "2", "", "4" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, digits.Length);
        Assert.Equal(2, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_AllFilled_ReturnsZero()
    {
        var digits = new[] { "1", "2", "3", "4" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, digits.Length);
        Assert.Equal(0, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_SingleFieldEmpty_ReturnsZero()
    {
        var digits = new[] { "" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, 1);
        Assert.Equal(0, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_SingleFieldFilled_ReturnsZero()
    {
        var digits = new[] { "1" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, 1);
        Assert.Equal(0, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_CountZero_ReturnsZero()
    {
        var digits = new[] { "1", "2" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, 0);
        Assert.Equal(0, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_NullDigits_ReturnsZero()
    {
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(null, 4);
        Assert.Equal(0, index);
    }

    [Fact]
    public void GetFirstEmptyFieldIndex_With6Fields_FirstEmptyAtFour_ReturnsFour()
    {
        var digits = new[] { "1", "2", "3", "4", "", "6" };
        int index = PinFieldHelpers.GetFirstEmptyFieldIndex(digits, 6);
        Assert.Equal(4, index);
    }
}
