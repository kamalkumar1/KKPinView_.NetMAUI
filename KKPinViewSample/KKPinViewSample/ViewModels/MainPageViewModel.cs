using System.ComponentModel;
using System.Runtime.CompilerServices;
using KKPinView.Views;

namespace KKPinViewSample.ViewModels;

/// <summary>
/// ViewModel for MainPage
/// </summary>
public class MainPageViewModel : INotifyPropertyChanged
{
    private KKPINSetUPView? _setupView;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the PIN setup view reference
    /// </summary>
    public KKPINSetUPView? SetupView
    {
        get => _setupView;
        set
        {
            if (_setupView != value)
            {
                _setupView = value;
                OnPropertyChanged();
                SetupCallbacks();
            }
        }
    }

    /// <summary>
    /// Sets up the callbacks for the PIN setup view
    /// </summary>
    public void SetupCallbacks()
    {
        if (_setupView != null)
        {
            _setupView.OnSetupSuccess = OnSetupSuccess;
            _setupView.OnSetupFailed = OnSetupFailed;
        }
    }

    /// <summary>
    /// Handles successful PIN setup
    /// </summary>
    private void OnSetupSuccess()
    {
        // Handle successful PIN setup
        // For example: navigate to next page, show success message, etc.
        // This can be extended with navigation logic or other business logic
    }

    /// <summary>
    /// Handles failed PIN setup
    /// </summary>
    /// <param name="errorMessage">The error message from the setup process</param>
    private void OnSetupFailed(string errorMessage)
    {
        // Handle failed PIN setup
        // The actual alert display should be handled in the view/code-behind
        // This method can be extended with business logic
        SetupFailed?.Invoke(errorMessage);
    }

    /// <summary>
    /// Event raised when PIN setup fails. Can be handled by the view to show alerts.
    /// </summary>
    public event Action<string>? SetupFailed;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

