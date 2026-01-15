using KKPinView.Storage;
using KKPinView.Views;

namespace KKPinViewSample;

public partial class MainPage : ContentPage
{
	private bool _isAuthenticated = false;

	public MainPage()
	{
		InitializeComponent();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateUI();
	}

	private void UpdateUI()
	{
		var hasPin = KKPinStorage.HasStoredPIN();
		
		StatusLabel.Text = hasPin ? "PIN is configured" : "No PIN configured";
		PinStatusLabel.Text = hasPin ? "You can authenticate using your PIN" : "Please setup a PIN to get started";
		
		SetupPinButton.IsEnabled = !hasPin || !_isAuthenticated;
		EnterPinButton.IsEnabled = hasPin;
		DeletePinButton.IsEnabled = hasPin;
		
		AuthenticatedContent.IsVisible = _isAuthenticated;
	}

	private async void OnSetupPinClicked(object? sender, EventArgs e)
	{
		var setupView = new KKPINSetUPView
		{
			OnSetupComplete = (pin) =>
			{
				Console.WriteLine($"✅ PIN setup complete!");
				// PIN is automatically saved by KKPINSetUPView
				_isAuthenticated = true;
				MainThread.BeginInvokeOnMainThread(() =>
				{
					UpdateUI();
				});
			}
		};

		await Navigation.PushAsync(setupView);
	}

	private async void OnEnterPinClicked(object? sender, EventArgs e)
	{
		var pinView = new KKPinViews
		{
			OnForgotPin = async () =>
			{
				// Show confirmation dialog
				bool delete = await DisplayAlert(
					"Forgot PIN?",
					"This will delete your current PIN. You will need to setup a new PIN.",
					"Delete PIN",
					"Cancel");

				if (delete)
				{
					KKPinStorage.DeletePIN();
					_isAuthenticated = false;
					MainThread.BeginInvokeOnMainThread(() =>
					{
						UpdateUI();
					});
				}
			},
			OnSubmit = (isValid) =>
			{
				if (isValid)
				{
					Console.WriteLine("✅ PIN is valid - access granted");
					_isAuthenticated = true;
					MainThread.BeginInvokeOnMainThread(() =>
					{
						UpdateUI();
					});
				}
				else
				{
					Console.WriteLine("❌ PIN is invalid");
					// Error is automatically displayed by KKPinViews
				}
			},
			ShowForgotPin = true
		};

		await Navigation.PushAsync(pinView);
	}

	private async void OnDeletePinClicked(object? sender, EventArgs e)
	{
		bool delete = await DisplayAlert(
			"Delete PIN",
			"Are you sure you want to delete your PIN? You will need to setup a new PIN to authenticate.",
			"Delete",
			"Cancel");

		if (delete)
		{
			KKPinStorage.DeletePIN();
			_isAuthenticated = false;
			UpdateUI();
			await DisplayAlert("Success", "PIN has been deleted.", "OK");
		}
	}
}
