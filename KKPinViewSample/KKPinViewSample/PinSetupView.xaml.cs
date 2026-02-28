using KKPinView.Storage;
using KKPinView.Views;
using Microsoft.Maui.ApplicationModel;

namespace KKPinViewSample;

public partial class PinSetupView : ContentPage
{
    public PinSetupView()
    {
        InitializeComponent();
        
        // // Ensure the view is set if XAML didn't load it
        // if (PinSetupContentView == null)
        // {
        //     PinSetupContentView = new KKPINSetUPView
        //     {
        //         VerticalOptions = LayoutOptions.Fill,
        //         HorizontalOptions = LayoutOptions.Fill
        //     };
        //     Content = PinSetupContentView;
        // }
        
        // Set up event handlers after page is loaded
        Loaded += OnPageLoaded;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Bring keyboard when page is visible (after setup or when opening setup)
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(350), () =>
        {
            MainThread.BeginInvokeOnMainThread(() => PinSetupContentView?.ShowKeyboard());
        });
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

