using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;

namespace KKPinViewSample;

/// <summary>
/// Modal page for PIN validation. Dismisses automatically when PIN is valid.
/// </summary>
public partial class PinValidateModalPage : ContentPage
{
    public PinValidateModalPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(350), () =>
            MainThread.BeginInvokeOnMainThread(() => PinEntryContentView?.ShowKeyboard()));
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (PinEntryContentView == null) return;

        PinEntryContentView.ViewModel.ShowForgotPin = false;

        PinEntryContentView.OnSubmit = (isValid) =>
        {
            if (!isValid) return;
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(500), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Shell.Current != null)
                        await Shell.Current.Navigation.PopModalAsync();
                });
            });
        };
    }
}
