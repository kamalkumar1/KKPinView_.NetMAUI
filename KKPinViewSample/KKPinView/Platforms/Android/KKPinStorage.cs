using System;
using System.Diagnostics;
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
        try
        {
            var encrypted = SecureStorage.GetAsync(PinKey).Result;
            if (string.IsNullOrEmpty(encrypted))
            {
                return null;
            }
            
            if (string.IsNullOrEmpty(secureKey))
            {
                Debug.WriteLine("❌ LoadPIN: Secure key is null or empty");
                return null;
            }
            
            return KKEncryptionHelperAndroid.DecryptString(encrypted, secureKey);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ LoadPIN error: {ex.Message}");
            return null;
        }
    }
    
    public string GetDeviceId()
    {
        try
        {
            var androidId = Android.Provider.Settings.Secure.GetString(
                Android.App.Application.Context.ContentResolver,
                Android.Provider.Settings.Secure.AndroidId);
            
            return androidId ?? "Android_Device";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ GetDeviceId error: {ex.Message}");
            return "Android_Device";
        }
    }
}

