using Microsoft.Extensions.DependencyInjection;

namespace KKPinViewSample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		// Register routes early to ensure they're available
		Routing.RegisterRoute("PinSetupView", typeof(PinSetupView));
		Routing.RegisterRoute("PINView", typeof(PINView));
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}