using System;
using KKPinView.Debug;
using KKPinView.Storage;
using Microsoft.Maui.Storage;

namespace KKPinView.Platforms.Android;

/// <summary>
/// Android-specific implementation of PIN storage
/// </summary>
internal class KKPinStorageAndroid : IKKPinStorage
{
    private const string PinKey = "KKPinView_StoredPIN";
    
    public bool SavePIN(string pin, string secureKey)
    {
        KKPinViewDebug.LogVerbose("Android: Saving PIN");
        
        if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(secureKey))
        {
            KKPinViewDebug.LogError("Android: Invalid input parameters");
            return false;
        }
        
        try
        {
            var encrypted = KKEncryptionHelperAndroid.EncryptString(pin, secureKey);
            if (encrypted == null)
            {
                KKPinViewDebug.LogError("Android: Encryption failed");
                return false;
            }
            
            SecureStorage.SetAsync(PinKey, encrypted).Wait();
            KKPinViewDebug.LogVerbose("Android: PIN saved successfully");
            return true;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("Android: SavePIN error", ex);
            return false;
        }
    }
    
    public string? LoadPIN(string secureKey)
    {
        KKPinViewDebug.LogVerbose("Android: Loading PIN");
        
        try
        {
            var encrypted = SecureStorage.GetAsync(PinKey).Result;
            if (string.IsNullOrEmpty(encrypted))
            {
                KKPinViewDebug.LogVerbose("Android: No encrypted PIN found in storage");
                return null;
            }
            
            if (string.IsNullOrEmpty(secureKey))
            {
                KKPinViewDebug.LogError("Android: LoadPIN: Secure key is null or empty");
                return null;
            }
            
            var decrypted = KKEncryptionHelperAndroid.DecryptString(encrypted, secureKey);
            if (decrypted != null)
            {
                KKPinViewDebug.LogVerbose("Android: PIN loaded successfully");
            }
            else
            {
                KKPinViewDebug.LogError("Android: LoadPIN: Decryption returned null");
            }
            
            return decrypted;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("Android: LoadPIN error", ex);
            return null;
        }
    }
    
    public string GetDeviceId()
    {
        try
        {
            var context = global::Android.App.Application.Context;
            if (context == null)
            {
                KKPinViewDebug.LogWarning("Android: Application context is null, using fallback device ID");
                return "Android_Device";
            }
            
            var androidId = global::Android.Provider.Settings.Secure.GetString(
                context.ContentResolver,
                global::Android.Provider.Settings.Secure.AndroidId);
            
            return androidId ?? "Android_Device";
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("Android: GetDeviceId error", ex);
            return "Android_Device";
        }
    }
}

