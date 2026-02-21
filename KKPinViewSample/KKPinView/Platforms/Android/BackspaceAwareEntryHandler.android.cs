#if ANDROID

using Microsoft.Maui.Handlers;

namespace KKPinView.Handlers;

/// <summary>
/// Uses default Entry handler - backspace on empty is handled via KeyPress in PinDigitField.
/// </summary>
public partial class BackspaceAwareEntryHandler : EntryHandler
{
}

#endif
