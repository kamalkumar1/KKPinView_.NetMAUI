#if ANDROID

using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Maui.Platform;

namespace KKPinView.Platforms.Android;

/// <summary>
/// EditText that invokes EmptyBackspacePressed when backspace is pressed on an empty field.
/// Uses InputConnection and dispatchKeyEvent (deleteSurroundingText not called when empty on many devices).
/// </summary>
internal class BackspaceAwareEditText : MauiAppCompatEditText
{
    public event EventHandler? EmptyBackspacePressed;

    public BackspaceAwareEditText(global::Android.Content.Context context) : base(context)
    {
    }

    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        if (e?.Action == KeyEventActions.Down && e.KeyCode == Keycode.Del && string.IsNullOrEmpty(Text))
        {
            EmptyBackspacePressed?.Invoke(this, EventArgs.Empty);
            return true;
        }
        return base.DispatchKeyEvent(e);
    }

    public override global::Android.Views.InputMethods.IInputConnection? OnCreateInputConnection(global::Android.Views.InputMethods.EditorInfo outAttrs)
    {
        var baseConnection = base.OnCreateInputConnection(outAttrs);
        if (baseConnection == null) return null;
        return new BackspaceInterceptConnection(baseConnection, this);
    }

    private class BackspaceInterceptConnection : InputConnectionWrapper
    {
        private readonly BackspaceAwareEditText _editText;

        public BackspaceInterceptConnection(global::Android.Views.InputMethods.IInputConnection? target, BackspaceAwareEditText editText)
            : base(target ?? new BaseInputConnection(editText, true), true)
        {
            _editText = editText;
        }

        public override bool DeleteSurroundingText(int beforeLength, int afterLength)
        {
            if (beforeLength > 0 && string.IsNullOrEmpty(_editText.Text))
            {
                _editText.EmptyBackspacePressed?.Invoke(_editText, EventArgs.Empty);
                return true; // Consumed - move to previous field
            }
            return base.DeleteSurroundingText(beforeLength, afterLength);
        }
    }
}

#endif
