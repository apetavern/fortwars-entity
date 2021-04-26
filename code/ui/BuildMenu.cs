using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Tests;

[Library]
public partial class BuildMenu : Panel
{
	public static BuildMenu Instance;

	VirtualScrollPanel Canvas;

	public BuildMenu()
	{
		Instance = this;

		StyleSheet.Load( "/ui/BuildMenu.scss" );

		AddChild( out Canvas, "canvas" );

		Add.Panel();

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemSize = new Vector2( 100, 100 );
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var file = (string)data;
			var panel = cell.Add.Panel( "icon" );
			panel.Style.Set( "background-image", $"url( /ui/models/{file}.png )" );
			panel.AddEvent( "onclick", () => ConsoleSystem.Run( "spawn", "models/" + file ) );
		};

		Canvas.AddItems( new string[] {
			"citizen_props/concreteroaddivider01.vmdl",
			"citizen_props/crate01.vmdl",
			"rust_props/wooden_crates/wooden_crate_a.vmdl",
			"rust_props/wooden_crates/wooden_crate_b.vmdl",
			"rust_props/wooden_crates/wooden_crate_c.vmdl"
		} );
	}

	public override void Tick()
	{
		base.Tick();

		Parent.SetClass( "buildmenuopen", Player.Local?.Input.Down( InputButton.Menu ) ?? false );
	}

}
