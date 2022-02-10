using Sandbox;
using Sandbox.UI;

namespace Fortwars;

public partial class Crosshair : Panel
{
	private static Crosshair Instance { get; set; }

	private static CrosshairConfig _config = CrosshairConfig.Default;
	public static CrosshairConfig Config
	{
		get => _config; set
		{
			_config = value;
			Instance.Build();
		}
	}

	public Crosshair()
	{
		StyleSheet.Load( "/ui/elements/crosshair/Crosshair.scss" );

		for ( int i = 0; i < 5; i++ )
		{
			var p = Add.Panel( "element" );
			p.AddClass( $"el{i}" );
		}

		Instance = this;
		Build();

		BindClass( "visible", () =>
		{
			if ( VirtualCursor.InUse )
				return false;

			return Local.Pawn.ActiveChild is not FortwarsWeapon { IsAiming: true };
		} );
	}

	/// <summary>
	/// Builds the styling for the selected crosshair configuration
	/// </summary>
	private void Build()
	{
		StyleSheet.Parse( "crosshair.fortwarsweapon {" +

			".el0, .el1, .el2, .el3 {" +
			$"background-color: {Config.Color.Hex};" +
			$"border: {(Config.Outline ? $"1px solid {Config.OutlineColor}" : "none")};" +
			"}" +

			//
			// Left/right
			//
			".el0, .el1 " +
			"{" +
			$"width: {Config.Size}px;" +
			$"height: {Config.Thickness}px;" +
			"}" +

			//
			// Top/bottom
			//
			".el2, .el3 {" +
			$"width: {Config.Thickness}px;" +
			$"height: {Config.Size}px;" +
			"}" +

			".el0 {" +
			$"right: {Config.Size}px;" +
			"}" +

			".el1 {" +
			$"left: {Config.Size}px;" +
			"}" +

			".el2 {" +
			$"bottom: {Config.Size}px;" +
			"}" +

			".el3 {" +
			$"top: {Config.Size}px;" +
			"}" +

		"}" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn.ActiveChild is not FortwarsWeapon weapon )
			return;

		float size = Config.Gap + (Config.Size * 2) + weapon.GetCrosshairSize();
		Style.Width = size;
		Style.Height = size;

		// Vector2 screenScale = new Vector2( 1920, 1080 ) / Screen.Size;

		var transform = new PanelTransform();
		transform.AddScale( ScaleFromScreen );
		transform.AddTranslate( Length.Percent( -50 ), Length.Percent( -50 ) );
		Style.Transform = transform;
	}
}
