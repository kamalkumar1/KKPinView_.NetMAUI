using KKPinView.Storage;
using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;

namespace KKPinViewSample;

public partial class PinSetupView : ContentPage
{
    public PinSetupView()
    {
        InitializeComponent();
        PinSetupContentView.OnCreationCompleted = () => PinSetupContentView?.ShowKeyboard();
        Loaded += OnPageLoaded;
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        if (Shell.Current != null)
            await Shell.Current.GoToAsync("..");
    }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (PinSetupContentView != null)
        {
            // Handle successful PIN setup
            PinSetupContentView.OnSetupSuccess = () =>
            {
                // Navigate to PIN view on main thread
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var shell = Shell.Current;
                        if (shell != null)
                        {
                            await shell.GoToAsync("PINView");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Shell.Current is null");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                });
            };

            // Handle PIN setup failure
            PinSetupContentView.OnSetupFailed = (errorMessage) =>
            {
                System.Diagnostics.Debug.WriteLine($"PIN setup failed: {errorMessage}");
            };
        }
    }
}

