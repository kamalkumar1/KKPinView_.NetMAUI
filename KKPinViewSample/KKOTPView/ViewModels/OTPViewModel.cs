using System.ComponentModel;
using System.Runtime.CompilerServices;
using KKOTPView.Configuration;
using Microsoft.Maui.ApplicationModel;

namespace KKOTPView.ViewModels;

/// <summary>
/// ViewModel for OTP view. Configuration is sourced from <see cref="OTPConfiguration"/>.
/// </summary>
public sealed class OTPViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly OTPConfiguration _config;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;
    private bool _hasError;
    private bool _hasSuccessMessage;
    private bool _isPinInvalid;
    private bool _disposed;
    private string _resendButtonText;
    private bool _isResendEnabled = true;
    private int _countdownRemaining;
    private bool _countdownActive;

    /// <summary>Initializes a new instance with the given configuration.</summary>
    public OTPViewModel(OTPConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _resendButtonText = _config.ResendText;
    }

    /// <summary>Gets the background color of the view.</summary>
    public Color BackgroundColor => _config.BackgroundColor;

    /// <summary>Gets the text color for labels.</summary>
    public Color TextColor => _config.TextColor;

    /// <summary>Gets the color for error messages.</summary>
    public Color ErrorTextColor => _config.ErrorTextColor;

    /// <summary>Gets the color for success messages.</summary>
    public Color SuccessTextColor => _config.SuccessTextColor;

    /// <summary>Gets the font size for title and messages.</summary>
    public double TitleFontSize => _config.TitleFontSize;

    /// <summary>Gets the font attributes for title and messages.</summary>
    public FontAttributes TitleFontAttributes => _config.TitleFontAttributes;

    /// <summary>Gets the font family for title and messages.</summary>
    public string TitleFontFamily => _config.TitleFontFamily;

    /// <summary>Gets the font size for error message text.</summary>
    public double ErrorMessageFontSize => _config.ErrorMessageFontSize;

    /// <summary>Gets the font attributes for error message text.</summary>
    public FontAttributes ErrorMessageFontAttributes => _config.ErrorMessageFontAttributes;

    /// <summary>Gets the font family for error message text.</summary>
    public string ErrorMessageFontFamily => _config.ErrorMessageFontFamily;

    /// <summary>Gets the spacing between digit fields.</summary>
    public double FieldSpacing => _config.FieldSpacing;

    /// <summary>Gets the resend button text (dynamic: "Resend OTP" or "Resend in Xs").</summary>
    public string ForgotPinText
    {
        get => _resendButtonText;
        private set => SetProperty(ref _resendButtonText, value);
    }

    /// <summary>Gets whether the resend button is visible.</summary>
    public bool ShowForgotPin => _config.ShowResendButton;

    /// <summary>Gets whether the resend button is enabled (false during countdown).</summary>
    public bool IsResendEnabled
    {
        get => _isResendEnabled;
        private set => SetProperty(ref _isResendEnabled, value);
    }

    /// <summary>Gets the resend button font size (switches between Resend OTP and countdown font).</summary>
    public double ResendFontSize => _countdownActive ? _config.ResendCountdownFontSize : _config.ResendButtonFontSize;

    /// <summary>Gets the resend button font attributes.</summary>
    public FontAttributes ResendFontAttributes => _countdownActive ? _config.ResendCountdownFontAttributes : _config.ResendButtonFontAttributes;

    /// <summary>Gets the resend button font family.</summary>
    public string ResendFontFamily => _countdownActive ? _config.ResendCountdownFontFamily : _config.ResendButtonFontFamily;

    /// <summary>Starts the resend countdown. Call when OTP is sent or when view loads (if AutoStartCountdown).</summary>
    internal void StartCountdown()
    {
        if (_countdownActive || _disposed) return;
        _countdownActive = true;
        _countdownRemaining = _config.ResendCooldownSeconds;
        IsResendEnabled = false;
        ForgotPinText = string.Format(_config.ResendCountdownFormat, _countdownRemaining);
        OnPropertyChanged(nameof(ResendFontSize));
        OnPropertyChanged(nameof(ResendFontAttributes));
        OnPropertyChanged(nameof(ResendFontFamily));
        ScheduleCountdownTick();
    }

    private void ScheduleCountdownTick()
    {
        if (_disposed || !_countdownActive) return;
        Application.Current?.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), () =>
        {
            if (_disposed) return;
            _countdownRemaining--;
            if (_countdownRemaining <= 0)
            {
                _countdownActive = false;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsResendEnabled = true;
                    ForgotPinText = _config.ResendText;
                    OnPropertyChanged(nameof(ResendFontSize));
                    OnPropertyChanged(nameof(ResendFontAttributes));
                    OnPropertyChanged(nameof(ResendFontFamily));
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ForgotPinText = string.Format(_config.ResendCountdownFormat, _countdownRemaining);
                });
                ScheduleCountdownTick();
            }
        });
    }

    /// <summary>Stops the countdown and enables resend.</summary>
    internal void StopCountdown()
    {
        _countdownActive = false;
        IsResendEnabled = true;
        ForgotPinText = _config.ResendText;
        OnPropertyChanged(nameof(ResendFontSize));
        OnPropertyChanged(nameof(ResendFontAttributes));
        OnPropertyChanged(nameof(ResendFontFamily));
    }

    /// <summary>Gets or sets whether the entered OTP is invalid.</summary>
    public bool IsPinInvalid
    {
        get => _isPinInvalid;
        set => SetProperty(ref _isPinInvalid, value);
    }

    /// <summary>Gets or sets the error message text to display.</summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>Gets or sets the success message text to display.</summary>
    public string SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    /// <summary>Gets or sets whether an error message should be displayed.</summary>
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    /// <summary>Gets or sets whether a success message should be displayed.</summary>
    public bool HasSuccessMessage
    {
        get => _hasSuccessMessage;
        set => SetProperty(ref _hasSuccessMessage, value);
    }

    /// <summary>Alias for HasSuccessMessage.</summary>
    public bool HasSuccess
    {
        get => _hasSuccessMessage;
        set => SetProperty(ref _hasSuccessMessage, value);
    }

    /// <summary>Callback invoked when resend button is tapped.</summary>
    public Action? OnForgotPin { get; set; }

    /// <summary>Callback invoked when OTP is submitted. Parameter indicates if valid.</summary>
    public Action<bool>? OnSubmit { get; set; }

    /// <summary>Gets the configuration (for success/error message constants).</summary>
    internal OTPConfiguration Config => _config;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;
        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _countdownActive = false;
        PropertyChanged = null;
        OnForgotPin = null;
        OnSubmit = null;
        _errorMessage = string.Empty;
        _successMessage = string.Empty;
        GC.SuppressFinalize(this);
    }
}
