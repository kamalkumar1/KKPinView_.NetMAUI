using System.ComponentModel;
using System.Runtime.CompilerServices;
using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// Base ViewModel for PIN views. Configuration values (colors, fonts, spacing) are read from
/// <see cref="KKPinviewConstant"/> only; change them via the constant class (e.g. in app startup).
/// </summary>
public abstract class BasePinViewModel : INotifyPropertyChanged, IDisposable
{
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;
    private bool _hasError;
    private bool _hasSuccessMessage;
    private bool _disposed;

    /// <summary>Gets the background color of the view. Change via <see cref="KKPinviewConstant.BackgroundColor"/>.</summary>
    public Color BackgroundColor => KKPinviewConstant.BackgroundColor;

    /// <summary>Gets the text color for labels. Change via <see cref="KKPinviewConstant.TextColor"/>.</summary>
    public Color TextColor => KKPinviewConstant.TextColor;

    /// <summary>Gets the color for error messages. Change via <see cref="KKPinviewConstant.ErrorTextColor"/>.</summary>
    public Color ErrorTextColor => KKPinviewConstant.ErrorTextColor;

    /// <summary>Gets the color for success messages. Change via <see cref="KKPinviewConstant.SuccessTextColor"/>.</summary>
    public Color SuccessTextColor => KKPinviewConstant.SuccessTextColor;

    /// <summary>Gets the font size for title text. Change via <see cref="KKPinviewConstant.TitleFontSize"/>.</summary>
    public double TitleFontSize => KKPinviewConstant.TitleFontSize;

    /// <summary>Gets the font size for subtitle text. Change via <see cref="KKPinviewConstant.SubtitleFontSize"/>.</summary>
    public double SubtitleFontSize => KKPinviewConstant.SubtitleFontSize;

    /// <summary>Gets the spacing between PIN digit fields. Change via <see cref="KKPinviewConstant.FieldSpacing"/>.</summary>
    public double FieldSpacing => KKPinviewConstant.FieldSpacing;
    
    /// <summary>
    /// Gets or sets the error message text to display
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// Gets or sets the success message text to display
    /// </summary>
    public string SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether an error message should be displayed
    /// </summary>
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a success message should be displayed
    /// </summary>
    public bool HasSuccessMessage
    {
        get => _hasSuccessMessage;
        set => SetProperty(ref _hasSuccessMessage, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a success message should be displayed (alias for HasSuccessMessage)
    /// </summary>
    public bool HasSuccess
    {
        get => _hasSuccessMessage;
        set => SetProperty(ref _hasSuccessMessage, value);
    }

    /// <summary>
    /// Occurs when a property value changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event for the specified property
    /// </summary>
    /// <param name="propertyName">The name of the property that changed</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the property value and raises PropertyChanged if the value has changed
    /// </summary>
    /// <typeparam name="T">The type of the property</typeparam>
    /// <param name="backingStore">The backing field for the property</param>
    /// <param name="value">The new value to set</param>
    /// <param name="propertyName">The name of the property</param>
    /// <returns>True if the value changed, false otherwise</returns>
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Releases all resources used by the ViewModel
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the ViewModel and optionally releases the managed resources
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Clear event handlers
                PropertyChanged = null;
            }
            _disposed = true;
        }
    }
}

