using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;

namespace KKPinViewSample;

/// <summary>
/// Modal page for PIN setup. Dismisses automatically when setup completes successfully.
/// </summary>
public partial class PinSetupModalPage : ContentPage
{
    public PinSetupModalPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(350), () =>
            MainThread.BeginInvokeOnMainThread(() => PinSetupContentView?.ShowKeyboard()));
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (PinSetupContentView == null) return;

        PinSetupContentView.OnSetupSuccess = () =>
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(500), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Shell.Current != null)
                        await Shell.Current.Navigation.PopModalAsync();
                });
            });
        };

        PinSetupContentView.OnSetupFailed = (errorMessage) =>
        {
            System.Diagnostics.Debug.WriteLine($"PIN setup failed: {errorMessage}");
        };
    }
}
