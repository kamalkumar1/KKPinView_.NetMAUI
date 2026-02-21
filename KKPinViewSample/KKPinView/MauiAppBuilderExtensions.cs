using KKPinView.Handlers;
using KKPinView.Views;

namespace KKPinView;

/// <summary>
/// Extension methods for MauiAppBuilder to configure KKPinView handlers.
/// Call UseKKPinView() in MauiProgram.cs to enable backspace-on-empty-field to work on iOS.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Configures KKPinView handlers. Required for backspace-on-empty-field to work on iOS.
    /// Call this in MauiProgram.cs: builder.UseKKPinView();
    /// </summary>
    public static MauiAppBuilder UseKKPinView(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<BackspaceAwareEntry, BackspaceAwareEntryHandler>();
        });
        return builder;
    }
}
