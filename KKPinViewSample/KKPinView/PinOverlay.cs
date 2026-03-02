using Microsoft.Maui.Controls;

namespace KKPinView;

/// <summary>
/// Static API to show/hide the PIN overlay from anywhere (e.g. from a menu or on app resume).
/// Register the Shell once in CreateWindow, then call Show/Hide as needed.
/// Uses modal navigation to present the overlay on top of the main content.
/// </summary>
public static class PinOverlay
{
    private static Shell? _shell;
    private static bool _isVisible;

    /// <summary>
    /// Raised when overlay is shown (true) or hidden (false). Use on Android to enable a back-button callback only when visible.
    /// </summary>
    public static event Action<bool>? OverlayVisibilityChanged;

    internal static void RaiseOverlayVisibilityChanged(bool visible)
    {
        _isVisible = visible;
        OverlayVisibilityChanged?.Invoke(visible);
    }

    /// <summary>
    /// Register the Shell for overlay navigation. Call once from Application.CreateWindow after creating the Shell.
    /// The overlay will be shown as a full-screen modal page on top of the Shell.
    /// </summary>
    /// <example>
    /// var shell = new AppShell(); PinOverlay.Register(shell); return new Window(shell);
    /// </example>
    public static void Register(Shell shell)
    {
        _shell = shell ?? throw new ArgumentNullException(nameof(shell));
    }

    /// <summary>
    /// Show the overlay with the given content. Use a View (e.g. ContentView with PIN UI), not a Page; reuse the same instance to avoid duplicate content.
    /// </summary>
    public static void Show(View content)
    {
        if (_shell == null)
            return;

        var page = new PinOverlayPage(content);
        _ = _shell.Navigation.PushModalAsync(page);
        RaiseOverlayVisibilityChanged(true);
    }

    /// <summary>
    /// Hide the overlay. After this, run any navigation (e.g. GoToAsync) in your app as needed.
    /// </summary>
    public static void Hide()
    {
        if (_shell == null)
            return;

        if (_shell.Navigation.ModalStack.Count > 0)
        {
            _ = _shell.Navigation.PopModalAsync();
            RaiseOverlayVisibilityChanged(false);
        }
    }

    /// <summary>
    /// Hide the overlay asynchronously. Await this before navigating (e.g. GoToAsync) so the modal is fully dismissed first.
    /// </summary>
    public static async System.Threading.Tasks.Task HideAsync()
    {
        if (_shell == null)
            return;

        if (_shell.Navigation.ModalStack.Count > 0)
        {
            await _shell.Navigation.PopModalAsync();
            RaiseOverlayVisibilityChanged(false);
        }
    }

    /// <summary>
    /// True when the overlay is visible. Use on Android to dismiss overlay on back button when this is true.
    /// </summary>
    public static bool IsVisible => _isVisible;
}
