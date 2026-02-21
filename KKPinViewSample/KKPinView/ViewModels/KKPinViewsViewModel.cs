using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// ViewModel for KKPinViews (system keyboard input only)
/// </summary>
public class KKPinViewsViewModel : BasePinViewModel
{
    private string _titleText;
    private string _subtitleText;
    private string _forgotPinText;
    private bool _showForgotPin;
    private bool _isPinInvalid;

    /// <summary>
    /// Initializes a new instance of the KKPinViewsViewModel class
    /// </summary>
    public KKPinViewsViewModel()
    {
        HasError = false;
        HasSuccessMessage = false;
        _titleText = KKPinviewConstant.TitleTextFormat;
        _subtitleText = string.Format(KKPinviewConstant.SubtitleText, KKPinviewConstant.TotalPinTextFields);
        _forgotPinText = KKPinviewConstant.ForgotPinText;
        _showForgotPin = true;
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
    /// Gets or sets a value indicating whether the entered PIN is invalid
    /// </summary>
    public bool IsPinInvalid
    {
        get => _isPinInvalid;
        set => SetProperty(ref _isPinInvalid, value);
    }

    /// <summary>
    /// Gets or sets the callback action invoked when the "Forgot PIN?" button is tapped
    /// </summary>
    public Action? OnForgotPin { get; set; }

    /// <summary>
    /// Gets or sets the callback action invoked when PIN submission is completed. The boolean parameter indicates if the PIN is valid.
    /// </summary>
    public Action<bool>? OnSubmit { get; set; }

    /// <summary>
    /// Releases the unmanaged resources used by the ViewModel and optionally releases the managed resources
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clear callbacks
            OnForgotPin = null;
            OnSubmit = null;
        }

        base.Dispose(disposing);
    }
}

