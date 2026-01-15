using System;
using System.Diagnostics;
using Foundation;
using KKPinView.Debug;
using KKPinView.Storage;
using Microsoft.Maui.Storage;

namespace KKPinView.Platforms.iOS;

/// <summary>
/// iOS-specific implementation of PIN storage
/// </summary>
internal class KKPinStorageiOS : IKKPinStorage
{
    private const string PinKey = "KKPinView_StoredPIN";
    
    public bool SavePIN(string pin, string secureKey)
    {
        KKPinViewDebug.LogVerbose("iOS: Saving PIN");
        
        if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(secureKey))
        {
            KKPinViewDebug.LogError("iOS: Invalid input parameters");
            return false;
        }
        
        try
        {
            var pinData = NSData.FromString(pin, NSStringEncoding.UTF8);
            if (pinData == null)
            {
                KKPinViewDebug.LogError("iOS: Failed to create NSData from PIN");
                return false;
            }
            
            var encrypted = KKEncryptionHelperiOS.EncryptData(pinData, secureKey);
            if (encrypted == null)
            {
                KKPinViewDebug.LogError("iOS: Encryption failed");
                return false;
            }
            
            var encryptedString = Convert.ToBase64String(encrypted.ToArray());
            SecureStorage.SetAsync(PinKey, encryptedString).Wait();
            KKPinViewDebug.LogVerbose("iOS: PIN saved successfully");
            return true;
        }
        catch (Exception ex)
        {
            KKPinViewDebug.LogError("iOS: SavePIN error", ex);
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
            
            var encryptedData = NSData.FromArray(Convert.FromBase64String(encrypted));
            if (encryptedData == null)
            {
                Debug.WriteLine("❌ LoadPIN: Failed to create NSData from encrypted string");
                return null;
            }
            
            var decrypted = KKEncryptionHelperiOS.DecryptData(encryptedData, secureKey);
            if (decrypted == null)
            {
                Debug.WriteLine("❌ LoadPIN: Decryption failed");
                return null;
            }
            
            return NSString.FromData(decrypted, NSStringEncoding.UTF8) ?? string.Empty;
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
            return UIKit.UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? "iOS_Device";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ GetDeviceId error: {ex.Message}");
            return "iOS_Device";
        }
    }
}

