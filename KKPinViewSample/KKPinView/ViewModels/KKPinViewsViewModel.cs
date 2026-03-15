using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// ViewModel for KKPinViews. Title, subtitle and button text are read from
/// <see cref="KKPinviewConstant"/> only; change them via the constant class (e.g. in app startup).
/// </summary>
public sealed class KKPinViewsViewModel : BasePinViewModel
{
    private bool _showForgotPin = true;
    private bool _isPinInvalid;

    /// <summary>Initializes a new instance of the KKPinViewsViewModel class.</summary>
    public KKPinViewsViewModel()
    {
        HasError = false;
        HasSuccessMessage = false;
    }

    /// <summary>Gets the title text. Change via <see cref="KKPinviewConstant.TitleTextFormat"/>.</summary>
    public string TitleText => KKPinviewConstant.TitleTextFormat;

    /// <summary>Gets the subtitle text. Change via <see cref="KKPinviewConstant.SubtitleText"/> and <see cref="KKPinviewConstant.TotalPinTextFields"/>.</summary>
    public string SubtitleText => string.Format(KKPinviewConstant.SubtitleText, KKPinviewConstant.TotalPinTextFields);

    /// <summary>Gets the "Forgot PIN?" button text. Change via <see cref="KKPinviewConstant.ForgotPinText"/>.</summary>
    public string ForgotPinText => KKPinviewConstant.ForgotPinText;

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

