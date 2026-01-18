using System.Windows.Input;
using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// ViewModel for KKPinViews
/// </summary>
public class KKPinViewsViewModel : BasePinViewModel
{
    private string _titleText;
    private string _subtitleText;
    private string _forgotPinText;
    private bool _showForgotPin;
    private bool _isKeypadEnabled;
    private double _keypadOpacity;

    /// <summary>
    /// Initializes a new instance of the KKPinViewsViewModel class
    /// </summary>
    public KKPinViewsViewModel()
    {
        // Initialize properties from constants
        _titleText = KKPinviewConstant.TitleTextFormat;
        _subtitleText = string.Format(KKPinviewConstant.SubtitleText, KKPinviewConstant.TotalDigits);
        _forgotPinText = KKPinviewConstant.ForgotPinText;
        _showForgotPin = true;
        _isKeypadEnabled = true;
        _keypadOpacity = 1.0;

        // Initialize commands
        NumberCommand = new Command<string>(OnNumberPressed);
        DeleteCommand = new Command(OnDeletePressed);
    }

    /// <summary>
    /// Gets or sets the title text displayed at the top of the PIN entry view
    /// </summary>
    public string TitleText
    {
        get => _titleText;
        set => SetProperty(ref _titleText, value);
    }

    /// <summary>
    /// Gets or sets the subtitle text displayed below the title
    /// </summary>
    public string SubtitleText
    {
        get => _subtitleText;
        set => SetProperty(ref _subtitleText, value);
    }

    /// <summary>
    /// Gets or sets the text displayed on the "Forgot PIN?" button
    /// </summary>
    public string ForgotPinText
    {
        get => _forgotPinText;
        set => SetProperty(ref _forgotPinText, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the "Forgot PIN?" button should be visible
    /// </summary>
    public bool ShowForgotPin
    {
        get => _showForgotPin;
        set => SetProperty(ref _showForgotPin, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the numeric keypad is enabled for input
    /// </summary>
    public bool IsKeypadEnabled
    {
        get => _isKeypadEnabled;
        set => SetProperty(ref _isKeypadEnabled, value);
    }

    /// <summary>
    /// Gets or sets the opacity of the numeric keypad (used to visually indicate disabled state)
    /// </summary>
    public double KeypadOpacity
    {
        get => _keypadOpacity;
        set => SetProperty(ref _keypadOpacity, value);
    }

    /// <summary>
    /// Gets the command for number button presses on the keypad
    /// </summary>
    public ICommand NumberCommand { get; }

    /// <summary>
    /// Gets the command for delete/backspace button presses on the keypad
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Gets or sets the callback action invoked when the "Forgot PIN?" button is tapped
    /// </summary>
    public Action? OnForgotPin { get; set; }

    /// <summary>
    /// Gets or sets the callback action invoked when PIN submission is completed. The boolean parameter indicates if the PIN is valid.
    /// </summary>
    public Action<bool>? OnSubmit { get; set; }

    /// <summary>
    /// Event raised when a number is pressed on the keypad
    /// </summary>
    public event EventHandler<string>? NumberPressed;

    /// <summary>
    /// Event raised when the delete/backspace button is pressed on the keypad
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
            OnForgotPin = null;
            OnSubmit = null;
        }

        base.Dispose(disposing);
    }
}

