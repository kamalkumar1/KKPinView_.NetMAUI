using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// ViewModel for KKPINSetUPView (system keyboard input only)
/// </summary>
public class KKPINSetUPViewModel : BasePinViewModel
{
    private string _headingText;
    private string _enterPinLabelText;
    private string _confirmPinLabelText;
    private int _maxPinLength;
    private bool _showConfirmPin;

    /// <summary>
    /// Initializes a new instance of the KKPINSetUPViewModel class
    /// </summary>
    public KKPINSetUPViewModel()
    {
        _headingText = $"Set {KKPinviewConstant.TotalPinTextFields} digit PIN";
        _enterPinLabelText = KKPinviewConstant.EnterPinMessage;
        _confirmPinLabelText = KKPinviewConstant.ConfirmPinMessage;
        _maxPinLength = KKPinviewConstant.TotalPinTextFields;
        _showConfirmPin = true;
    }

    /// <summary>
    /// Gets or sets the heading text for the PIN setup view
    /// </summary>
    public string HeadingText
    {
        get => _headingText;
        set => SetProperty(ref _headingText, value);
    }

    /// <summary>
    /// Gets or sets the label text for entering the PIN
    /// </summary>
    public string EnterPinLabelText
    {
        get => _enterPinLabelText;
        set => SetProperty(ref _enterPinLabelText, value);
    }

    /// <summary>
    /// Gets or sets the label text for confirming the PIN
    /// </summary>
    public string ConfirmPinLabelText
    {
        get => _confirmPinLabelText;
        set => SetProperty(ref _confirmPinLabelText, value);
    }

    /// <summary>
    /// Gets or sets the maximum PIN length
    /// </summary>
    public int MaxPinLength
    {
        get => _maxPinLength;
        set => SetProperty(ref _maxPinLength, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the confirm PIN section should be visible
    /// </summary>
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

