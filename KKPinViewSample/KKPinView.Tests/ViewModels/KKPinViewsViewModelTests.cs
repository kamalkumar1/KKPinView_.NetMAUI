using KKPinView.Constants;
using KKPinView.ViewModels;

namespace KKPinView.Tests.ViewModels;

public class KKPinViewsViewModelTests
{
    [Fact]
    public void Constructor_InitializesWithCorrectDefaults()
    {
        var vm = new KKPinViewsViewModel();

        Assert.Equal(KKPinviewConstant.TitleTextFormat, vm.TitleText);
        Assert.Contains(KKPinviewConstant.TotalDigits.ToString(), vm.SubtitleText);
        Assert.Equal(KKPinviewConstant.ForgotPinText, vm.ForgotPinText);
        Assert.True(vm.ShowForgotPin);
        Assert.True(vm.IsKeypadEnabled);
        Assert.Equal(1.0, vm.KeypadOpacity);
    }

    [Fact]
    public void NumberCommand_IsNotNull()
    {
        var vm = new KKPinViewsViewModel();
        Assert.NotNull(vm.NumberCommand);
    }

    [Fact]
    public void DeleteCommand_IsNotNull()
    {
        var vm = new KKPinViewsViewModel();
        Assert.NotNull(vm.DeleteCommand);
    }

    [Fact]
    public void NumberCommand_CanExecuteWithValidDigit()
    {
        var vm = new KKPinViewsViewModel();
        Assert.True(vm.NumberCommand.CanExecute("9"));
    }

    [Fact]
    public void TitleText_CanBeSet()
    {
        var vm = new KKPinViewsViewModel();
        vm.TitleText = "Custom Title";
        Assert.Equal("Custom Title", vm.TitleText);
    }

    [Fact]
    public void ShowForgotPin_CanBeSet()
    {
        var vm = new KKPinViewsViewModel();
        vm.ShowForgotPin = false;
        Assert.False(vm.ShowForgotPin);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var vm = new KKPinViewsViewModel();
        var exception = Record.Exception(() => vm.Dispose());
        Assert.Null(exception);
    }
}
