using KKPinView.Storage;
using Microsoft.Maui.ApplicationModel;

namespace KKPinViewSample;

/// <summary>
/// Modal page for Forgot PIN flow. Resets PIN and dismisses when user taps Reset PIN.
/// </summary>
public partial class ForgotPinModalPage : ContentPage
{
    public ForgotPinModalPage()
    {
        InitializeComponent();
    }

    private void OnResetPinClicked(object? sender, EventArgs e)
    {
        KKPinStorage.DeletePIN();
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(500), () =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Shell.Current != null)
                    await Shell.Current.Navigation.PopModalAsync();
            });
        });
    }
}
