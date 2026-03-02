using KKPinView;

namespace KKPinViewSample;

public partial class App : Application
{
	private static PinEntryOverlayView? _pinOverlayView;

	/// <summary>Set to true to show PIN overlay when app resumes from background.</summary>
	public static bool RequirePinOnResume { get; set; }

	public App()
	{
		InitializeComponent();
		KKPinView.Constants.KKPinviewConstant.TotalPinTextFields = 4;
		Routing.RegisterRoute("PinSetupView", typeof(PinSetupView));
		// PINView is shown via overlay (modal page) to avoid nesting Pages
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var shell = new AppShell();
		_pinOverlayView = new PinEntryOverlayView();
		PinOverlay.Register(shell);
		return new Window(shell);
	}

	/// <summary>Show the PIN overlay (full-screen modal).</summary>
	public static void ShowPinOverlay()
	{
		if (_pinOverlayView != null)
			PinOverlay.Show(_pinOverlayView);
	}

	/// <summary>Hide the PIN overlay. Call before navigating after PIN success or Forgot PIN.</summary>
	public static void HidePinOverlay()
	{
		PinOverlay.Hide();
	}

	/// <summary>Hide the PIN overlay and await completion. Use before navigating to ensure modal is dismissed.</summary>
	public static System.Threading.Tasks.Task HidePinOverlayAsync()
	{
		return PinOverlay.HideAsync();
	}

	protected override void OnResume()
	{
		base.OnResume();
		if (RequirePinOnResume && KKPinView.Storage.KKPinStorage.HasStoredPIN())
			ShowPinOverlay();
	}
}