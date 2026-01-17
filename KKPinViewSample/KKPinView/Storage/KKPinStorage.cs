using System;
using System.Diagnostics;
using KKPinView.Debug;
using Microsoft.Maui.Storage;

#if ANDROID
using KKPinView.Platforms.Android;
#elif IOS
using KKPinView.Platforms.iOS;
#endif

namespace KKPinView.Storage;

/// <summary>
/// High-level API for securely storing and retrieving PINs
/// </summary>
public static class KKPinStorage
{
    private const string PinKey = "KKPinView_StoredPIN";
    private const string KeyName = "KKPinView_SecureKey";
    
    private static IKKPinStorage PlatformStorage
    {
        get
        {
#if ANDROID
            return new KKPinStorageAndroid();
#elif IOS
            return new KKPinStorageiOS();
#else
            // Fallback implementation for other platforms
            return new KKPinStorageFallback();
#endif
        }
    }
    
    /// <summary>
    /// Saves a PIN securely
    /// </summary>
    public static bool SavePIN(string pin)
    {
        KKPinViewDebug.LogMethodEntry(new object[] { pin });
        
        if (string.IsNullOrEmpty(pin))
        {
            KKPinViewDebug.LogError("PIN cannot be null or empty");
            return false;
        }
        
        try
        {
            var secureKey = GetOrCreateSecureKey();
            if (string.IsNullOrEmpty(secureKey))
            {
                KKPinViewDebug.LogError("Failed to get secure key");
                return false;
            }
            
            KKPinViewDebug.LogVerbose($"Saving PIN with secure key (length: {secureKey.Length})");
            KKPinViewDebug.LogPin("Saving PIN", pin);
            
            var result = PlatformStorage.SavePIN(pin, secureKey);
            KKPinViewDebug.LogMethodExit(result);
            return result;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("SavePIN error", ex);
            return false;
        }
    }
    
    /// <summary>
    /// Loads the stored PIN
    /// </summary>
    public static string? LoadPIN()
    {
        KKPinViewDebug.LogMethodEntry();
        
        try
        {
            var secureKey = GetOrCreateSecureKey();
            if (string.IsNullOrEmpty(secureKey))
            {
                KKPinViewDebug.LogError("Failed to get secure key");
                return null;
            }
            
            KKPinViewDebug.LogVerbose("Loading PIN from secure storage");
            var pin = PlatformStorage.LoadPIN(secureKey);
            KKPinViewDebug.LogPin("Loaded PIN", pin);
            KKPinViewDebug.LogMethodExit(pin != null ? "[PIN_LOADED]" : "[NO_PIN]");
            return pin;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("LoadPIN error", ex);
            return null;
        }
    }
    
    /// <summary>
    /// Verifies a PIN against the stored PIN
    /// </summary>
    public static bool VerifyPIN(string pin)
    {
        KKPinViewDebug.LogMethodEntry(new object[] { pin });
        
        var storedPin = LoadPIN();
        var isValid = storedPin != null && storedPin == pin;
        
        KKPinViewDebug.LogVerbose($"PIN verification result: {isValid}");
        KKPinViewDebug.LogMethodExit(isValid);
        return isValid;
    }
    
    /// <summary>
    /// Checks if a PIN is stored
    /// </summary>
    public static bool HasStoredPIN()
    {
        try
        {
            return SecureStorage.GetAsync(PinKey).Result != null;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("HasStoredPIN error", ex);
            return false;
        }
    }
    
    /// <summary>
    /// Deletes the stored PIN
    /// </summary>
    public static void DeletePIN()
    {
        try
        {
            SecureStorage.Remove(PinKey);
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("DeletePIN error", ex);
        }
    }
    
    /// <summary>
    /// Gets or creates a device-specific secure key
    /// </summary>
    private static string GetOrCreateSecureKey()
    {
        KKPinViewDebug.LogVerbose("Getting or creating secure key");
        
        try
        {
            var existingKey = SecureStorage.GetAsync(KeyName).Result;
            if (!string.IsNullOrEmpty(existingKey))
            {
                KKPinViewDebug.LogVerbose("Using existing secure key");
                return existingKey;
            }
            
            // Generate a new device-specific key
            var deviceId = PlatformStorage.GetDeviceId();
            KKPinViewDebug.LogVerbose($"Device ID: {deviceId}");
            
            var keyBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            
            var keyString = Convert.ToBase64String(keyBytes) + deviceId;
            SecureStorage.SetAsync(KeyName, keyString).Wait();
            KKPinViewDebug.LogVerbose("Generated and saved new secure key");
            return keyString;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("GetOrCreateSecureKey error", ex);
            return string.Empty;
        }
    }
}

#if !ANDROID && !IOS
/// <summary>
/// Fallback implementation for platforms other than Android and iOS
/// </summary>
internal class KKPinStorageFallback : IKKPinStorage
{
    private const string PinKey = "KKPinView_StoredPIN";
    
    public bool SavePIN(string pin, string secureKey)
    {
        try
        {
            SecureStorage.SetAsync(PinKey, pin).Wait();
            return true;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("Fallback: SavePIN error", ex);
            return false;
        }
    }
    
    public string? LoadPIN(string secureKey)
    {
        try
        {
            return SecureStorage.GetAsync(PinKey).Result;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("Fallback: LoadPIN error", ex);
            return null;
        }
    }
    
    public string GetDeviceId()
    {
        return Environment.MachineName;
    }
}
#endif

