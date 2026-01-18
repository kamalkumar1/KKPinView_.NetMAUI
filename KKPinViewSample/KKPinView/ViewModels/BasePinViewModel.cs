using System.ComponentModel;
using System.Runtime.CompilerServices;
using KKPinView.Constants;

namespace KKPinView.ViewModels;

/// <summary>
/// Base ViewModel class with common properties for PIN views
/// </summary>
public abstract class BasePinViewModel : INotifyPropertyChanged, IDisposable
{
    private Color _backgroundColor = KKPinviewConstant.BackgroundColor;
    private Color _textColor = KKPinviewConstant.TextColor;
    private Color _errorTextColor = KKPinviewConstant.ErrorTextColor;
    private Color _successTextColor = KKPinviewConstant.SuccessTextColor;
    
    private double _titleFontSize = KKPinviewConstant.TitleFontSize;
    private double _subtitleFontSize = KKPinviewConstant.SubtitleFontSize;
    private double _fieldSpacing = KKPinviewConstant.FieldSpacing;
    
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;
    private bool _hasError;
    private bool _hasSuccessMessage;
    private bool _disposed;

    /// <summary>
    /// Gets or sets the background color of the view
    /// </summary>
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    /// <summary>
    /// Gets or sets the text color used for labels and text elements
    /// </summary>
    public Color TextColor
    {
        get => _textColor;
        set => SetProperty(ref _textColor, value);
    }

    /// <summary>
    /// Gets or sets the color used for error messages
    /// </summary>
    public Color ErrorTextColor
    {
        get => _errorTextColor;
        set => SetProperty(ref _errorTextColor, value);
    }

    /// <summary>
    /// Gets or sets the color used for success messages
    /// </summary>
    public Color SuccessTextColor
    {
        get => _successTextColor;
        set => SetProperty(ref _successTextColor, value);
    }
    
    /// <summary>
    /// Gets or sets the font size for title text
    /// </summary>
    public double TitleFontSize
    {
        get => _titleFontSize;
        set => SetProperty(ref _titleFontSize, value);
    }

    /// <summary>
    /// Gets or sets the font size for subtitle text
    /// </summary>
    public double SubtitleFontSize
    {
        get => _subtitleFontSize;
        set => SetProperty(ref _subtitleFontSize, value);
    }

    /// <summary>
    /// Gets or sets the spacing between PIN digit fields
    /// </summary>
    public double FieldSpacing
    {
        get => _fieldSpacing;
        set => SetProperty(ref _fieldSpacing, value);
    }
    
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

