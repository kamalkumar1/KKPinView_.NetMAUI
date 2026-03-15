using KKPinView.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace KKPinViewSample;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		// Minimal - use defaults (4 digits, 5 attempts, 5 min lockout)
		//KKPinviewConstant.Configure();

		// Customize with fluent API
		KKPinviewConstant.Configure(c => c
				.PinLength(4)
				.Lockout(2, 10)
				.LabelColors(errorColor: Colors.Red, successColor: Colors.Green, textColor: Colors.Black)
				.LabelFont(fontSize: 18, attributes: FontAttributes.Bold, fontFamily: "OpenSansSemibold")
				.ErrorMessageFont(fontSize: 17, attributes: FontAttributes.Bold, fontFamily: "OpenSansSemibold")
				.DigitFont(fontSize: 20, attributes: FontAttributes.Bold, fontFamily: "OpenSansSemibold")
				.PinFieldColors(filled: Colors.Green, invalid: Colors.Red)
				.PinStoragePersistsAfterUninstall(true)
				.PinField(fontSize: 20, shape: KKPinFieldShapeType.Round)
				.PinFieldSecure(true));  // true = masked (dots), false = visible digits

		// Register routes early to ensure they're available
		Routing.RegisterRoute("PinSetupView", typeof(PinSetupView));
		Routing.RegisterRoute("PINView", typeof(PINView));
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}