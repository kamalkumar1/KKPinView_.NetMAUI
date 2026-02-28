using KKPinView.Storage;
using Microsoft.Maui.ApplicationModel;

namespace KKPinViewSample;

public partial class DemoMenuPage : ContentPage
{
    public DemoMenuPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BtnPinEntry.IsEnabled = KKPinStorage.HasStoredPIN();
        BtnForgotPin.IsEnabled = KKPinStorage.HasStoredPIN();
    }

    private async void OnResetAndSetupClicked(object? sender, EventArgs e)
    {
        KKPinStorage.DeletePIN();
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("PinSetupView");
    }

    private async void OnPinSetupClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("PinSetupView");
    }

    private async void OnPinEntryClicked(object? sender, EventArgs e)
    {
        if (!KKPinStorage.HasStoredPIN())
        {
            await DisplayAlert("No PIN", "Create a PIN first via \"Reset PIN → PIN Setup\".", "OK");
            return;
        }
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("PINView");
    }

    private async void OnForgotPinClicked(object? sender, EventArgs e)
    {
        if (!KKPinStorage.HasStoredPIN())
        {
            await DisplayAlert("No PIN", "Create a PIN first via \"Reset PIN → PIN Setup\", then open PIN Entry and tap Forgot PIN.", "OK");
            return;
        }
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("PINView");
    }
}
