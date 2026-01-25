namespace KKPinViewSample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Handles navigation to PinSetupView when button is clicked
	/// </summary>
	private async void OnMoveToPinViewClicked(object? sender, EventArgs e)
	{
		try
		{
			var shell = Shell.Current;
			if (shell != null)
			{
				await shell.GoToAsync("PinSetupView");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine("Shell.Current is null");
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
			System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
		}
	}
}
