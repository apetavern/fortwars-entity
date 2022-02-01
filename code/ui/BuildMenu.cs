using Sandbox;
using Sandbox.UI;
using System;

[Library]
public partial class BuildMenu : Panel
{
	public static BuildMenu Instance;

	public BuildMenu()
	{
		Instance = this;

		StyleSheet.Load( "/ui/BuildMenu.scss" );

		var inner = Add.Panel( "inner" );
		var files = new string[] {
			"fw_3x2.vmdl",
			"fw_1x2.vmdl",
			"fw_1x4.vmdl",
			"fw_1x1x1.vmdl",
			"fw_1x2x1.vmdl"
		};

		float angleIncrement = 360f / files.Length;
		angleIncrement = MathX.DegreeToRadian( angleIncrement );

		int index = 0;
		foreach ( var file in files )
		{
			Vector2 frac = new Vector2( MathF.Sin( angleIncrement * index ), MathF.Cos( angleIncrement * index ) );

			frac = (1.0f + frac) / 2.0f;

			var panel = inner.Add.Panel( "icon" );
			panel.Style.Set( "background-image", $"url( /ui/models/blocks/{file.Replace( ".vmdl", "" )}.png )" );
			panel.AddEventListener( "onclick", () => ConsoleSystem.Run( "spawn", "models/blocks/wood/" + file ) );

			panel.Style.Left = Length.Fraction( frac.x );
			panel.Style.Top = Length.Fraction( frac.y );

			index++;
		}

		BindClass( "active", () => Input.Down( InputButton.Menu ) );
	}
}
