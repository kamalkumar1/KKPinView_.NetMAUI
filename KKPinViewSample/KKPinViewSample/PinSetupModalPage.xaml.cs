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
        PinSetupContentView.OnCreationCompleted = () => PinSetupContentView?.ShowKeyboard();
        Loaded += OnPageLoaded;
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        PinSetupContentView?.Dispose();
        if (Shell.Current != null)
            await Shell.Current.Navigation.PopModalAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

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
                    PinSetupContentView?.Dispose();
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
