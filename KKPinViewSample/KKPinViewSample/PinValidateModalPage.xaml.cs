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
        PinEntryContentView.OnCreationCompleted = () => PinEntryContentView?.ShowKeyboard();
        Loaded += OnPageLoaded;
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (PinEntryContentView == null) return;

        PinEntryContentView.ViewModel.ShowForgotPin = false;

        PinEntryContentView.OnSubmit = (isValid) =>
        {
            if (!isValid) return;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);
                await Navigation.PopModalAsync();
            });
        };

    }
}
