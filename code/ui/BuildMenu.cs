using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

[Library]
public partial class BuildMenu : Panel
{
	public static BuildMenu Instance;

	private Panel selection;
	private Panel wrapper;

	public BuildMenu()
	{
		Instance = this;

		wrapper = Add.Panel( "wrapper" );

		StyleSheet.Load( "/ui/BuildMenu.scss" );

		selection = wrapper.Add.Panel( "selected" );

		var center = wrapper.Add.Panel( "center" );
		center.Add.Icon( "question_mark", "image" );
		center.Add.Label( "Name", "subtitle" );
		center.Add.Label( "Lorem ipsum dolor sit amet", "description" );


		var inner = wrapper.Add.Panel( "inner" );
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

	float targetAngle = 0f;

	public override void Tick()
	{
		base.Tick();

		if ( IsVisible )
		{
			Vector2 relativeMousePos = Mouse.Position - wrapper.Box.Rect.Center;
			float ang = MathF.Atan2( relativeMousePos.y, relativeMousePos.x )
				.RadianToDegree().NormalizeDegrees();

			ang = ang.SnapToGrid( 72f ) + 35f + 70f;
			ang = ang.NormalizeDegrees();

			float delta = targetAngle - ang;

			if ( MathF.Abs( delta ) > 0.5f )
			{
				targetAngle = ang.NormalizeDegrees();

				var tx = new PanelTransform();
				tx.AddRotation( 0, 0, targetAngle );
				selection.Style.Transform = tx;
			}
		}
	}
}
