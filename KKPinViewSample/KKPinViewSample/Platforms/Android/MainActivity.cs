using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity;
using KKPinView;

namespace KKPinViewSample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private BackPressCallback? _backPressCallback;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _backPressCallback = new BackPressCallback(this);
        _backPressCallback.Enabled = false;
        OnBackPressedDispatcher.AddCallback(this, _backPressCallback);
        PinOverlay.OverlayVisibilityChanged += OnOverlayVisibilityChanged;
    }

    private void OnOverlayVisibilityChanged(bool visible)
    {
        if (_backPressCallback != null)
            _backPressCallback.Enabled = visible;
    }

    private sealed class BackPressCallback : OnBackPressedCallback
    {
        private readonly MainActivity _activity;

        public BackPressCallback(MainActivity activity) : base(true) => _activity = activity;

        public override void HandleOnBackPressed()
        {
            if (PinOverlay.IsVisible)
                PinOverlay.Hide();
        }
    }
}
