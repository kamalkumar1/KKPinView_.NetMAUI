using KKPinView.Storage;
using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace KKPinViewSample;

public partial class PINView : ContentPage
{
    public PINView()
    {
        InitializeComponent();
        
        // Set up event handlers after page is loaded
        Loaded += OnPageLoaded;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Bring keyboard when page is visible (e.g. after setup completed and navigated here)
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(350), () =>
        {
            MainThread.BeginInvokeOnMainThread(() => PinEntryContentView?.ShowKeyboard());
        });
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (PinEntryContentView != null)
        {
            // Handle "Forgot PIN" - delete PIN and navigate to setup view
            PinEntryContentView.OnForgotPin = () =>
            {
                KKPinStorage.DeletePIN();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    App.HidePinOverlay();
                    try
                    {
                        var shell = Shell.Current;
                        if (shell != null)
                            await shell.GoToAsync("PinSetupView");
                        else
                            System.Diagnostics.Debug.WriteLine("Shell.Current is null");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                });
            };

            // Handle PIN validation result
            PinEntryContentView.OnSubmit = (isValid) =>
            {
                if (isValid)
                {
                    System.Diagnostics.Debug.WriteLine("PIN validated successfully!");
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        App.HidePinOverlay();
                        try
                        {
                            var shell = Shell.Current;
                            if (shell != null)
                                await shell.GoToAsync("//MainPage");
                            else
                                System.Diagnostics.Debug.WriteLine("Shell.Current is null");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                        }
                    });
                }
            };
        }
    }
}

