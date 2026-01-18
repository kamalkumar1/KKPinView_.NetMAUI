using KKPinViewSample.ViewModels;

namespace KKPinViewSample;

public partial class MainPage : ContentPage
{
	private readonly MainPageViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();
		
		// Initialize ViewModel
		_viewModel = new MainPageViewModel();
		BindingContext = _viewModel;
		
		// Wire up the SetupView to ViewModel after initialization
		Loaded += OnPageLoaded;
	}
	
	private void OnPageLoaded(object? sender, EventArgs e)
	{
		// Set the SetupView reference in ViewModel to wire up callbacks
		_viewModel.SetupView = SetupView;
		
		// Wire up ViewModel events for UI interactions
		_viewModel.SetupFailed += OnSetupFailed;
	}
	
	private async void OnSetupFailed(string errorMessage)
	{
		// Handle failed PIN setup - show error alert
		await DisplayAlert("Setup Failed", errorMessage, "OK");
	}
}
