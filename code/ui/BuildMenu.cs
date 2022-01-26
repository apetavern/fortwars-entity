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
			panel.Style.Set( "background-image", $"url( /ui/models/blocks/{file.Replace( ".vmdl", "" )}.png )" );
			panel.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn", "models/blocks/" + file ) );
		};

		Canvas.AddItems( new string[] {
			"fw_3x2.vmdl",
			"fw_1x2.vmdl",
			"fw_1x4.vmdl",
			"fw_1x1x1.vmdl",
			"fw_1x2x1.vmdl",

		} );
	}

	public override void Tick()
	{
		base.Tick();

		Parent.SetClass( "buildmenuopen", Input.Down( InputButton.Menu ) );
	}

}
