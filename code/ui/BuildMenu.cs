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
			"fw.block_3x2.vmdl",
			"fw.block_2x2.vmdl",
			"fw.block_1x2.vmdl",
		} );
	}

	public override void Tick()
	{
		base.Tick();

		Parent.SetClass( "buildmenuopen", Player.Local?.Input.Down( InputButton.Menu ) ?? false );
	}

}
