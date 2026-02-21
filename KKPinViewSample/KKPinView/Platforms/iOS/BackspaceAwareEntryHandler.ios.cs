#if IOS

using KKPinView.Platforms.iOS;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace KKPinView.Platforms.iOS;

internal class BackspaceAwareTextField : MauiTextField
{
    public event EventHandler? EmptyBackspacePressed;

    public override void DeleteBackward()
    {
        if (string.IsNullOrEmpty(Text))
            EmptyBackspacePressed?.Invoke(this, EventArgs.Empty);
        else
            base.DeleteBackward();
    }
}

public partial class BackspaceAwareEntryHandler : EntryHandler
{
    protected override MauiTextField CreatePlatformView()
    {
        return new BackspaceAwareTextField
        {
            BorderStyle = UITextBorderStyle.None,
            ClipsToBounds = true
        };
    }
}

#endif
