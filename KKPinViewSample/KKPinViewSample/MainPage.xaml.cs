using KKPinView.Storage;
using KKPinView.Views;

namespace KKPinViewSample;

/// <summary>
/// Example integration of KKPinView library
/// This demonstrates how to dynamically show PIN setup or PIN entry view based on PIN storage status
/// </summary>
public partial class MainPage : ContentPage
{
	private KKPINSetUPView? _setupView;
	private KKPinViews? _pinView;

	public MainPage()
	{
		InitializeComponent();
		LoadPinView();
	}

	/// <summary>
	/// Checks if PIN is stored and shows the appropriate view
	/// </summary>
	private void LoadPinView()
	{
		// Check if PIN exists in storage
		bool hasPin = KKPinStorage.HasStoredPIN();

		if (hasPin)
		{
			// PIN exists - show PIN entry view
			ShowPinEntryView();
		}
		else
		{
			// No PIN stored - show PIN setup view
			ShowPinSetupView();
		}
	}

	/// <summary>
	/// Shows the PIN setup view
	/// </summary>
	private void ShowPinSetupView()
	{
		// Clean up existing view
		PinContentView.Content = null;
		_pinView = null;

		// Create and show PIN setup view
		_setupView = new KKPINSetUPView();
		PinContentView.Content = _setupView;

		// Handle successful PIN setup - switch to PIN entry view
		_setupView.OnSetupSuccess = () =>
		{
			ShowPinEntryView();
		};

		// Handle PIN setup failure
		_setupView.OnSetupFailed = (errorMessage) =>
		{
			// Error is already displayed in the view
			System.Diagnostics.Debug.WriteLine($"PIN setup failed: {errorMessage}");
		};
	}

	/// <summary>
	/// Shows the PIN entry view
	/// </summary>
	private void ShowPinEntryView()
	{
		// Clean up existing view
		PinContentView.Content = null;
		_setupView = null;

		// Create and show PIN entry view
		_pinView = new KKPinViews();
		PinContentView.Content = _pinView;

		// Handle "Forgot PIN" - delete PIN and show setup view
		_pinView.OnForgotPin = () =>
		{
			KKPinStorage.DeletePIN();
			ShowPinSetupView();
		};

		// Handle PIN validation result
		_pinView.OnSubmit = (isValid) =>
		{
			if (isValid)
			{
				// PIN is valid - user is authenticated
				// Navigate to your authenticated page or show main content here
				System.Diagnostics.Debug.WriteLine("PIN validated successfully!");
			}
		};
	}
}
