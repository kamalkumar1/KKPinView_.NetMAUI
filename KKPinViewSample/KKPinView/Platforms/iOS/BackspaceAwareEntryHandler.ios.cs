#if IOS

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace KKPinView.Handlers;

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

public partial class BackspaceAwareEntryHandler
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
