namespace KKPinView.Helpers;

/// <summary>
/// Helpers for PIN field behavior. Used by setup and enter-PIN views for "first empty" focus logic.
/// </summary>
internal static class PinFieldHelpers
{
    /// <summary>
    /// Returns the index of the first field whose digit is empty, or 0 if all are filled.
    /// Used so that tapping anywhere focuses the continuation position (first empty).
    /// </summary>
    /// <param name="digits">Digit values (one per field), may be null or empty string for empty.</param>
    /// <param name="count">Total number of fields (must be &gt; 0).</param>
    /// <returns>Index in [0, count-1].</returns>
    internal static int GetFirstEmptyFieldIndex(IReadOnlyList<string>? digits, int count)
    {
        if (count <= 0) return 0;
        int len = digits?.Count ?? 0;
        for (int i = 0; i < count && i < len; i++)
        {
            if (string.IsNullOrEmpty(digits![i]))
                return i;
        }
        return 0;
    }
}
