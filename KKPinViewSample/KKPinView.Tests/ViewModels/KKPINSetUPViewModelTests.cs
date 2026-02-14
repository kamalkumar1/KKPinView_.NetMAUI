using KKPinView.Constants;
using KKPinView.ViewModels;

namespace KKPinView.Tests.ViewModels;

public class KKPINSetUPViewModelTests
{
    [Fact]
    public void Constructor_InitializesWithCorrectDefaults()
    {
        var vm = new KKPINSetUPViewModel();

        Assert.Equal(KKPinviewConstant.EnterPinMessage, vm.EnterPinLabelText);
        Assert.Equal(KKPinviewConstant.ConfirmPinMessage, vm.ConfirmPinLabelText);
        Assert.Equal(KKPinviewConstant.TotalDigits, vm.MaxPinLength);
        Assert.True(vm.ShowConfirmPin);
        Assert.Equal(KKPinviewConstant.InputMethod, vm.InputMethod);
    }

    [Fact]
    public void NumberCommand_IsNotNull()
    {
        var vm = new KKPINSetUPViewModel();
        Assert.NotNull(vm.NumberCommand);
    }

    [Fact]
    public void DeleteCommand_IsNotNull()
    {
        var vm = new KKPINSetUPViewModel();
        Assert.NotNull(vm.DeleteCommand);
    }

    [Fact]
    public void NumberCommand_CanExecuteWithValidDigit()
    {
        var vm = new KKPINSetUPViewModel();
        Assert.True(vm.NumberCommand.CanExecute("5"));
    }

    [Fact]
    public void DeleteCommand_CanExecute()
    {
        var vm = new KKPINSetUPViewModel();
        Assert.True(vm.DeleteCommand.CanExecute(null));
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
