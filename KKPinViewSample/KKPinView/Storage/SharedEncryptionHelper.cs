using System.Security.Cryptography;
using System.Text;

namespace KKPinView.Storage;

/// <summary>
/// Shared AES-256 encryption for PIN storage. Used when storing in Preferences (non-permanent mode).
/// </summary>
internal static class SharedEncryptionHelper
{
    public static string? EncryptString(string plainText, string secureKey)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(secureKey))
            return null;

        try
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var keyBytes = DeriveKeyFromString(secureKey);
            try
            {
                aes.Key = keyBytes;
                aes.GenerateIV();

                using var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                try
                {
                    var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    try
                    {
                        var combined = new byte[aes.IV.Length + encryptedBytes.Length];
                        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                        Buffer.BlockCopy(encryptedBytes, 0, combined, aes.IV.Length, encryptedBytes.Length);
                        return Convert.ToBase64String(combined);
                    }
                    finally
                    {
                        Array.Clear(encryptedBytes, 0, encryptedBytes.Length);
                    }
                }
                finally
                {
                    Array.Clear(plainBytes, 0, plainBytes.Length);
                }
            }
            finally
            {
                Array.Clear(keyBytes, 0, keyBytes.Length);
            }
        }
        catch
        {
            return null;
        }
    }

    public static string? DecryptString(string encryptedText, string secureKey)
    {
        if (string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(secureKey))
            return null;

        try
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var keyBytes = DeriveKeyFromString(secureKey);
            try
            {
                aes.Key = keyBytes;

                var combinedBytes = Convert.FromBase64String(encryptedText);
                try
                {
                    var iv = new byte[16];
                    var encryptedBytes = new byte[combinedBytes.Length - 16];
                    Buffer.BlockCopy(combinedBytes, 0, iv, 0, 16);
                    Buffer.BlockCopy(combinedBytes, 16, encryptedBytes, 0, encryptedBytes.Length);
                    aes.IV = iv;

                    using var decryptor = aes.CreateDecryptor();
                    var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    try
                    {
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                    finally
                    {
                        Array.Clear(decryptedBytes, 0, decryptedBytes.Length);
                    }
                }
                finally
                {
                    Array.Clear(combinedBytes, 0, combinedBytes.Length);
                }
            }
            finally
            {
                Array.Clear(keyBytes, 0, keyBytes.Length);
            }
        }
        catch
        {
            return null;
        }
    }

    private static byte[] DeriveKeyFromString(string secureKey)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(secureKey));
    }
}
