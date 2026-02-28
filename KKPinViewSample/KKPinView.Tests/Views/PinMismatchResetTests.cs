using KKPinView.Helpers;
using Xunit;

namespace KKPinView.Tests.Views;

/// <summary>
/// Documents and tests the expected state after PIN mismatch reset.
/// After mismatch: both PINs cleared via ClearDigitSilently, isConfirmingPin false, focus first Enter PIN field.
/// </summary>
public class PinMismatchResetTests
{
    /// <summary>
    /// State contract after mismatch reset. View must achieve this when PINs don't match.
    /// </summary>
    private sealed class MismatchResetState
    {
        public string CurrentPin { get; set; } = "";
        public string ConfirmPin { get; set; } = "";
        public bool IsConfirmingPin { get; set; }

        public void ApplyReset()
        {
            CurrentPin = string.Empty;
            ConfirmPin = string.Empty;
            IsConfirmingPin = false;
        }
    }

    [Fact]
    public void AfterReset_CurrentPinIsEmpty()
    {
        var state = new MismatchResetState { CurrentPin = "1234", ConfirmPin = "1235", IsConfirmingPin = true };
        state.ApplyReset();
        Assert.Equal(string.Empty, state.CurrentPin);
    }

    [Fact]
    public void AfterReset_ConfirmPinIsEmpty()
    {
        var state = new MismatchResetState { CurrentPin = "1234", ConfirmPin = "1235", IsConfirmingPin = true };
        state.ApplyReset();
        Assert.Equal(string.Empty, state.ConfirmPin);
    }

    [Fact]
    public void AfterReset_IsConfirmingPinIsFalse()
    {
        var state = new MismatchResetState { CurrentPin = "1234", ConfirmPin = "1235", IsConfirmingPin = true };
        state.ApplyReset();
        Assert.False(state.IsConfirmingPin);
    }

    [Fact]
    public void AfterReset_ReEntryStartsFromEnterPin_Spec()
    {
        var state = new MismatchResetState { CurrentPin = "1234", ConfirmPin = "123", IsConfirmingPin = true };
        state.ApplyReset();
        Assert.Empty(state.CurrentPin);
        Assert.False(state.IsConfirmingPin);
    }

    [Fact]
    public void AfterReset_FirstEmptyEnterFieldIndexIsZero()
    {
        // After mismatch reset, enter section is all empty; next tap/input should focus index 0
        var enterDigits = new[] { "", "", "", "" };
        int firstEmpty = PinFieldHelpers.GetFirstEmptyFieldIndex(enterDigits, 4);
        Assert.Equal(0, firstEmpty);
    }
}
