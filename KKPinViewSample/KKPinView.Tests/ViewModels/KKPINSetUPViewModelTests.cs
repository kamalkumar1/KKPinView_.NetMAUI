using KKPinView.Constants;
using KKPinView.ViewModels;
using Xunit;

namespace KKPinView.Tests.ViewModels;

[Collection("Constants")]
public class KKPINSetUPViewModelTests
{
    [Fact]
    public void Constructor_InitializesWithCorrectDefaults()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 4;
            var vm = new KKPINSetUPViewModel();

            Assert.Equal(KKPinviewConstant.EnterPinMessage, vm.EnterPinLabelText);
            Assert.Equal(KKPinviewConstant.ConfirmPinMessage, vm.ConfirmPinLabelText);
            Assert.Equal(4, vm.MaxPinLength);
            Assert.True(vm.ShowConfirmPin);
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
    }

    [Fact]
    public void MaxPinLength_CanBeSet()
    {
        var vm = new KKPINSetUPViewModel();
        vm.MaxPinLength = 6;
        Assert.Equal(6, vm.MaxPinLength);
    }

    [Fact]
    public void ShowConfirmPin_CanBeSet()
    {
        var vm = new KKPINSetUPViewModel();
        vm.ShowConfirmPin = false;
        Assert.False(vm.ShowConfirmPin);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var vm = new KKPINSetUPViewModel();
        var exception = Record.Exception(() => vm.Dispose());
        Assert.Null(exception);
    }
}
