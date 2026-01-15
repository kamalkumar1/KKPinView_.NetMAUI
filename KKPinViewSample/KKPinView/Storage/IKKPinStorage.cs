namespace KKPinView.Storage;

/// <summary>
/// Platform-specific implementation interface for PIN storage
/// </summary>
internal interface IKKPinStorage
{
    /// <summary>
    /// Saves a PIN securely using platform-specific encryption
    /// </summary>
    bool SavePIN(string pin, string secureKey);
    
    /// <summary>
    /// Loads the stored PIN using platform-specific decryption
    /// </summary>
    string? LoadPIN(string secureKey);
    
    /// <summary>
    /// Gets a device-specific identifier
    /// </summary>
    string GetDeviceId();
}

