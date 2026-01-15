using System;
using System.Diagnostics;
using System.Text;
using KKPinView.Platforms.Android;
#if IOS
using KKPinView.Platforms.iOS;
#endif

#if ANDROID
using AndroidX.Security.Crypto;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;
#elif IOS
using Foundation;
using Security;
#endif

namespace KKPinView.Storage;

/// <summary>
/// High-level API for securely storing and retrieving PINs
/// </summary>
public static class KKPinStorage
{
    private const string PinKey = "KKPinView_StoredPIN";
    private const string PinService = "KKPinView";
    
    /// <summary>
    /// Saves a PIN securely
    /// </summary>
    public static bool SavePIN(string pin)
    {
        if (string.IsNullOrEmpty(pin))
        {
            Debug.WriteLine("❌ SavePIN: PIN cannot be null or empty");
            return false;
        }
        
        try
        {
            var secureKey = GetOrCreateSecureKey();
            if (string.IsNullOrEmpty(secureKey))
            {
                Debug.WriteLine("❌ SavePIN: Failed to get secure key");
                return false;
            }
            
#if ANDROID
            var encrypted = KKEncryptionHelperAndroid.EncryptString(pin, secureKey);
            if (encrypted == null)
            {
                return false;
            }
            SecureStorage.SetAsync(PinKey, encrypted).Wait();
            return true;
#elif IOS
            var pinData = Foundation.NSData.FromString(pin, Foundation.NSStringEncoding.UTF8);
            if (pinData == null)
            {
                return false;
            }
            
            var encrypted = KKEncryptionHelperiOS.EncryptData(pinData, secureKey);
            if (encrypted == null)
            {
                return false;
            }
            
            var encryptedString = Convert.ToBase64String(encrypted.ToArray());
            SecureStorage.SetAsync(PinKey, encryptedString).Wait();
            return true;
#else
            // Fallback for other platforms
            SecureStorage.SetAsync(PinKey, pin).Wait();
            return true;
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ SavePIN error: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Loads the stored PIN
    /// </summary>
    public static string? LoadPIN()
    {
        try
        {
            var encrypted = SecureStorage.GetAsync(PinKey).Result;
            if (string.IsNullOrEmpty(encrypted))
            {
                return null;
            }
            
            var secureKey = GetOrCreateSecureKey();
            if (string.IsNullOrEmpty(secureKey))
            {
                Debug.WriteLine("❌ LoadPIN: Failed to get secure key");
                return null;
            }
            
#if ANDROID
            return KKEncryptionHelperAndroid.DecryptString(encrypted, secureKey);
#elif IOS
            var encryptedData = Foundation.NSData.FromArray(Convert.FromBase64String(encrypted));
            if (encryptedData == null)
            {
                return null;
            }
            
            var decrypted = KKEncryptionHelperiOS.DecryptData(encryptedData, secureKey);
            if (decrypted == null)
            {
                return null;
            }
            
            return Foundation.NSString.FromData(decrypted, Foundation.NSStringEncoding.UTF8) ?? string.Empty;
#else
            // Fallback for other platforms
            return encrypted;
#endif
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ LoadPIN error: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Verifies a PIN against the stored PIN
    /// </summary>
    public static bool VerifyPIN(string pin)
    {
        var storedPin = LoadPIN();
        return storedPin != null && storedPin == pin;
    }
    
    /// <summary>
    /// Checks if a PIN is stored
    /// </summary>
    public static bool HasStoredPIN()
    {
        return SecureStorage.GetAsync(PinKey).Result != null;
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
            Debug.WriteLine($"❌ DeletePIN error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Gets or creates a device-specific secure key
    /// </summary>
    private static string GetOrCreateSecureKey()
    {
        const string KeyName = "KKPinView_SecureKey";
        
        try
        {
            var existingKey = SecureStorage.GetAsync(KeyName).Result;
            if (!string.IsNullOrEmpty(existingKey))
            {
                return existingKey;
            }
            
            // Generate a new device-specific key
            var deviceId = GetDeviceId();
            var keyBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(keyBytes);
            }
            
            var keyString = Convert.ToBase64String(keyBytes) + deviceId;
            SecureStorage.SetAsync(KeyName, keyString).Wait();
            return keyString;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ GetOrCreateSecureKey error: {ex.Message}");
            return string.Empty;
        }
    }
    
    /// <summary>
    /// Gets a device-specific identifier
    /// </summary>
    private static string GetDeviceId()
    {
#if ANDROID
        return Android.Provider.Settings.Secure.GetString(
            Android.App.Application.Context.ContentResolver,
            Android.Provider.Settings.Secure.AndroidId) ?? "Android_Device";
#elif IOS
        return UIKit.UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? "iOS_Device";
#else
        return Environment.MachineName;
#endif
    }
}

