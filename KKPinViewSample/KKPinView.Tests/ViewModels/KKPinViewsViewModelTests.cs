using KKPinView.Constants;
using KKPinView.ViewModels;
using Xunit;

namespace KKPinView.Tests.ViewModels;

[Collection("Constants")]
public class KKPinViewsViewModelTests
{
    [Fact]
    public void Constructor_InitializesWithCorrectDefaults()
    {
        var original = KKPinviewConstant.TotalPinTextFields;
        try
        {
            KKPinviewConstant.TotalPinTextFields = 4;
            var vm = new KKPinViewsViewModel();

            Assert.Equal(KKPinviewConstant.TitleTextFormat, vm.TitleText);
            Assert.Contains("4", vm.SubtitleText);
            Assert.Equal(KKPinviewConstant.ForgotPinText, vm.ForgotPinText);
            Assert.True(vm.ShowForgotPin);
        }
        finally
        {
            KKPinviewConstant.TotalPinTextFields = original;
        }
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
