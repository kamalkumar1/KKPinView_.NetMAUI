using KKPinView.Storage;

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
        BtnValidatePin.IsEnabled = KKPinStorage.HasStoredPIN();
        BtnForgotPin.IsEnabled = KKPinStorage.HasStoredPIN();
    }

    private async void OnSetupPinClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
            await Shell.Current.Navigation.PushModalAsync(new PinSetupModalPage());
    }

    private async void OnValidatePinClicked(object? sender, EventArgs e)
    {
        if (!KKPinStorage.HasStoredPIN())
        {
            await DisplayAlert("No PIN", "Create a PIN first with Setup PIN.", "OK");
            return;
        }
        if (Shell.Current != null)
            await Shell.Current.Navigation.PushModalAsync(new PinValidateModalPage());
    }

    private async void OnForgotPinClicked(object? sender, EventArgs e)
    {
        if (!KKPinStorage.HasStoredPIN())
        {
            await DisplayAlert("No PIN", "No PIN to reset. Create one with Setup PIN first.", "OK");
            return;
        }
        if (Shell.Current != null)
            await Shell.Current.Navigation.PushModalAsync(new ForgotPinModalPage());
    }
}
