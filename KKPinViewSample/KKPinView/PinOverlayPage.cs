using Microsoft.Maui.Controls;

namespace KKPinView;

/// <summary>
/// Full-screen page used to display overlay content via modal navigation.
/// Used internally by PinOverlay when Shell-based modal mode is registered.
/// </summary>
internal sealed class PinOverlayPage : ContentPage
{
    public PinOverlayPage(View content)
    {
        Content = content;
        BackgroundColor = Colors.Transparent;
    }
}
