namespace KKPinView.Views;

/// <summary>
/// Entry control that raises EmptyBackspacePressed when backspace is pressed on an empty field.
/// Used by PinDigitField to enable cursor movement on empty-field backspace (iOS and Android).
/// </summary>
public class BackspaceAwareEntry : Entry
{
  protected override void OnTextChanged(string oldValue, string newValue)
  {
    base.OnTextChanged(oldValue, newValue);


  }

  // public event EventHandler? EmptyBackspacePressed;
}
