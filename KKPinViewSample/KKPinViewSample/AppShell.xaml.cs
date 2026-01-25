namespace KKPinViewSample;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for pages not in ShellContent
		Routing.RegisterRoute("PinSetupView", typeof(PinSetupView));
		Routing.RegisterRoute("PINView", typeof(PINView));
	}
}
