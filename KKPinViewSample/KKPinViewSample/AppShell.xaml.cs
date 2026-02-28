namespace KKPinViewSample;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for demo and PIN flows
		Routing.RegisterRoute("DemoMenuPage", typeof(DemoMenuPage));
		Routing.RegisterRoute("PinSetupView", typeof(PinSetupView));
		Routing.RegisterRoute("PINView", typeof(PINView));
	}
}
