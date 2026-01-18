using System.Windows.Input;
using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// ViewModel for KKPINSetUPView
/// </summary>
public class KKPINSetUPViewModel : BasePinViewModel
{
    private string _headingText;
    private string _enterPinLabelText;
    private string _confirmPinLabelText;
    private PinInputMethod _inputMethod;
    private int _maxPinLength;
    private bool _showConfirmPin;

    /// <summary>
    /// Initializes a new instance of the KKPINSetUPViewModel class
    /// </summary>
    public KKPINSetUPViewModel()
    {
        // Initialize properties from constants
        _headingText = $"Set {KKPinviewConstant.TotalDigits} digit PIN";
        _enterPinLabelText = KKPinviewConstant.EnterPinMessage;
        _confirmPinLabelText = KKPinviewConstant.ConfirmPinMessage;
        _inputMethod = KKPinviewConstant.InputMethod;
        _maxPinLength = KKPinviewConstant.TotalDigits;
        _showConfirmPin = true; // Show confirm PIN fields by default

        // Initialize commands
        NumberCommand = new Command<string>(OnNumberPressed);
        DeleteCommand = new Command(OnDeletePressed);
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
    /// Gets or sets the input method for PIN entry (NumericKeypad or SystemKeyboard)
    /// </summary>
    public PinInputMethod InputMethod
    {
        get => _inputMethod;
        set => SetProperty(ref _inputMethod, value);
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
    /// Gets the command for number button presses
    /// </summary>
    public ICommand NumberCommand { get; }

    /// <summary>
    /// Gets the command for delete/backspace button presses
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Gets or sets the callback action invoked when PIN setup is successful
    /// </summary>
    public Action? OnSetupSuccess { get; set; }

    /// <summary>
    /// Gets or sets the callback action invoked when PIN setup fails. The string parameter contains the error message.
    /// </summary>
    public Action<string>? OnSetupFailed { get; set; }

    /// <summary>
    /// Event raised when a number is pressed on the keypad
    /// </summary>
    public event EventHandler<string>? NumberPressed;

    /// <summary>
    /// Event raised when the delete/backspace button is pressed
    /// </summary>
    public event EventHandler? DeletePressed;

    private void OnNumberPressed(string number)
    {
        NumberPressed?.Invoke(this, number);
    }

    private void OnDeletePressed()
    {
        DeletePressed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the ViewModel and optionally releases the managed resources
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clear event handlers
            NumberPressed = null;
            DeletePressed = null;

            // Clear callbacks
            OnSetupSuccess = null;
            OnSetupFailed = null;
        }

        base.Dispose(disposing);
    }
}

