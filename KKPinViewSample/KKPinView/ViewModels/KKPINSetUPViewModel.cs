using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// ViewModel for KKPINSetUPView. All display strings and lengths are read from
/// <see cref="KKPinviewConstant"/> only; change them via the constant class (e.g. in app startup).
/// </summary>
public sealed class KKPINSetUPViewModel : BasePinViewModel
{
    private bool _showConfirmPin = true;

    /// <summary>Gets the heading text. Change via <see cref="KKPinviewConstant.TotalPinTextFields"/>.</summary>
    public string HeadingText => $"Set {KKPinviewConstant.TotalPinTextFields} digit PIN";

    /// <summary>Gets the setup title text. Change via <see cref="KKPinviewConstant.SetupTitleText"/>.</summary>
    public string SetupTitleText => KKPinviewConstant.SetupTitleText;

    /// <summary>Gets the confirm section title. Change via <see cref="KKPinviewConstant.ConfirmPinTitleText"/>.</summary>
    public string ConfirmPinTitleText => KKPinviewConstant.ConfirmPinTitleText;

    /// <summary>Gets the label text for entering the PIN. Change via <see cref="KKPinviewConstant.EnterPinMessage"/>.</summary>
    public string EnterPinLabelText => KKPinviewConstant.EnterPinMessage;

    /// <summary>Gets the label text for confirming the PIN. Change via <see cref="KKPinviewConstant.ConfirmPinMessage"/>.</summary>
    public string ConfirmPinLabelText => KKPinviewConstant.ConfirmPinMessage;

    /// <summary>Gets the maximum PIN length. Change via <see cref="KKPinviewConstant.TotalPinTextFields"/>.</summary>
    public int MaxPinLength => KKPinviewConstant.TotalPinTextFields;

    /// <summary>Gets or sets whether the confirm PIN section is visible (runtime behavior, not from constants).</summary>
    public bool ShowConfirmPin
    {
        get => _showConfirmPin;
        set => SetProperty(ref _showConfirmPin, value);
    }

    /// <summary>
    /// Gets or sets the callback action invoked when PIN setup is successful
    /// </summary>
    public Action? OnSetupSuccess { get; set; }

    /// <summary>
    /// Gets or sets the callback action invoked when PIN setup fails. The string parameter contains the error message.
    /// </summary>
    public Action<string>? OnSetupFailed { get; set; }

    /// <summary>
    /// Releases the unmanaged resources used by the ViewModel and optionally releases the managed resources
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clear callbacks
            OnSetupSuccess = null;
            OnSetupFailed = null;
        }

        base.Dispose(disposing);
    }
}

