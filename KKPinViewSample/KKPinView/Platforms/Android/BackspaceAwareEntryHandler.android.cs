#if ANDROID

using KKPinView.Platforms.Android;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace KKPinView.Handlers;

/// <summary>
/// Uses BackspaceAwareEditText - intercepts backspace on empty field via InputConnection
/// without blocking soft keyboard text input.
/// </summary>
public partial class BackspaceAwareEntryHandler : EntryHandler
{
    protected override MauiAppCompatEditText CreatePlatformView()
    {
        var context = MauiContext?.Context ?? global::Android.App.Application.Context;
        return new BackspaceAwareEditText(context!);
    }
}

#endif
