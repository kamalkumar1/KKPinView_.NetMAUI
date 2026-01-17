using System;
using KKPinView.Constants;
using KKPinView.Debug;
using KKPinView.Storage;

namespace KKPinView.Security;

/// <summary>
/// Manages PIN validation attempts and lockout logic
/// </summary>
public class KKPinLockoutManager
{
    private const string FailedAttemptsKey = "KKPinView_FailedAttempts";
    private const string LockoutUntilKey = "KKPinView_LockoutUntil";

    public int FailedAttempts { get; private set; }
    public int MaxAttempts { get; }
    public int LockoutDurationMinutes { get; }

    public bool IsLockedOut
    {
        get
        {
            CheckLockoutStatus();
            if (Preferences.ContainsKey(LockoutUntilKey))
            {
                var lockoutUntil = Preferences.Get(LockoutUntilKey, DateTime.MinValue);
                return DateTime.UtcNow < lockoutUntil;
            }
            return false;
        }
    }

    public int RemainingLockoutMinutes
    {
        get
        {
            if (!IsLockedOut) return 0;
            var lockoutUntil = Preferences.Get(LockoutUntilKey, DateTime.MinValue);
            var remaining = lockoutUntil - DateTime.UtcNow;
            return Math.Max(0, (int)remaining.TotalMinutes);
        }
    }

    public bool HasReachedMaxAttempts => FailedAttempts >= MaxAttempts;

    public KKPinLockoutManager(int maxAttempts = 0, int lockoutDurationMinutes = 0)
    {
        MaxAttempts = maxAttempts > 0 ? maxAttempts : KKPinviewConstant.MaxPinAttempts;
        LockoutDurationMinutes = lockoutDurationMinutes > 0 ? lockoutDurationMinutes : KKPinviewConstant.PinLockoutDurationMinutes;
        LoadFailedAttempts();
    }

    /// <summary>
    /// Validates a PIN and handles attempt tracking
    /// </summary>
    public bool ValidatePIN(string pin)
    {
        KKPinViewDebug.LogMethodEntry(new object[] { pin });

        CheckLockoutStatus();

        if (IsLockedOut)
        {
            if (KKPinViewDebug.BypassLockout)
            {
                KKPinViewDebug.LogWarning("Lockout bypassed due to debug mode");
            }
            else
            {
                KKPinViewDebug.Log($"PIN validation blocked - locked out for {RemainingLockoutMinutes} minutes");
                return false;
            }
        }

        var isValid = KKPinStorage.VerifyPIN(pin);

        if (isValid)
        {
            KKPinViewDebug.Log("PIN validation successful");
            ResetFailedAttempts();
            KKPinViewDebug.LogMethodExit(true);
            return true;
        }
        else
        {
            KKPinViewDebug.LogWarning($"PIN validation failed. Attempts: {FailedAttempts + 1}/{MaxAttempts}");
            IncrementFailedAttempts();
            KKPinViewDebug.LogMethodExit(false);
            return false;
        }
    }

    /// <summary>
    /// Resets failed attempts counter
    /// </summary>
    public void ResetFailedAttempts()
    {
        FailedAttempts = 0;
        Preferences.Remove(FailedAttemptsKey);
        Preferences.Remove(LockoutUntilKey);
    }

    /// <summary>
    /// Checks and updates lockout status
    /// </summary>
    public void CheckLockoutStatus()
    {
        if (Preferences.ContainsKey(LockoutUntilKey))
        {
            var lockoutUntil = Preferences.Get(LockoutUntilKey, DateTime.MinValue);
            if (DateTime.UtcNow >= lockoutUntil)
            {
                // Lockout expired, reset
                ResetFailedAttempts();
            }
        }
    }

    /// <summary>
    /// Gets error message for current state
    /// </summary>
    public string? GetErrorMessage()
    {
        if (IsLockedOut)
        {
            return string.Format(KKPinviewConstant.LockedOutError, RemainingLockoutMinutes);
        }

        if (HasReachedMaxAttempts)
        {
            return string.Format(KKPinviewConstant.LockedOutError, RemainingLockoutMinutes);
        }

        if (FailedAttempts > 0)
        {
            return KKPinviewConstant.InvalidPinError;
        }

        return null;
    }

    private void IncrementFailedAttempts()
    {
        FailedAttempts++;
        Preferences.Set(FailedAttemptsKey, FailedAttempts);

        if (FailedAttempts >= MaxAttempts)
        {
            var lockoutUntil = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
            Preferences.Set(LockoutUntilKey, lockoutUntil);
        }
    }

    private void LoadFailedAttempts()
    {
        FailedAttempts = Preferences.Get(FailedAttemptsKey, 0);
        CheckLockoutStatus();
    }
}

