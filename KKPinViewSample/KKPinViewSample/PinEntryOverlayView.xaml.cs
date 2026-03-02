using KKPinView.Storage;
using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace KKPinViewSample;

/// <summary>
/// View used as overlay content for PIN entry (single instance, no Page). Use with PinOverlay.Show(this).
/// </summary>
public partial class PinEntryOverlayView : ContentView
{
    public PinEntryOverlayView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent != null)
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(350), () =>
                MainThread.BeginInvokeOnMainThread(() => PinEntryContentView?.ShowKeyboard()));
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (PinEntryContentView == null) return;

        PinEntryContentView.OnForgotPin = () =>
        {
            KKPinStorage.DeletePIN();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await App.HidePinOverlayAsync();
                try
                {
                    if (Shell.Current != null)
                        await Shell.Current.GoToAsync("PinSetupView");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                }
            });
        };

        PinEntryContentView.OnSubmit = (isValid) =>
        {
            if (!isValid) return;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await App.HidePinOverlayAsync();
                try
                {
                    if (Shell.Current != null)
                        await Shell.Current.GoToAsync("//MainPage");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                }
            });
        };
    }
}
