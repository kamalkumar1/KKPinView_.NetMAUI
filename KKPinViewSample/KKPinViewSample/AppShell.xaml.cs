namespace KKPinViewSample;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for demo and PIN flows (PINView is shown via overlay, not Shell)
		Routing.RegisterRoute("DemoMenuPage", typeof(DemoMenuPage));
		Routing.RegisterRoute("PinSetupView", typeof(PinSetupView));
	}
}
