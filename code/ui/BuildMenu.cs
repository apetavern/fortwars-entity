using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

namespace Fortwars
{
	struct BuildMenuItem
	{
		public string Path { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public BuildMenuItem( string path, string name, string description )
		{
			Path = path;
			Name = name;
			Description = description;
		}
	}

	[Library]
	public partial class BuildMenu : Panel
	{
		public static BuildMenu Instance;

		private Panel selection;
		private Panel wrapper;

		private Label currentName;
		private Label currentDescription;

		private BuildMenuItem[] files = new BuildMenuItem[] {
			new ("fw_3x2.vmdl", "3x2", "A wide panel good for defences"),
			new ("fw_1x2.vmdl", "1x2", "A medium panel good for entrances"),
			new ("fw_1x4.vmdl", "1x4", "A tall panel good for ledges"),
			new ("fw_1x1x1.vmdl", "1x1x1", "A thicc block good for climbing"),
			new ("fw_1x2x1.vmdl", "1x2x1", "A thicc block good for cover"),
		};

		public BuildMenu()
		{
			Instance = this;
			StyleSheet.Load( "/ui/BuildMenu.scss" );

			wrapper = Add.Panel( "wrapper" );
			selection = wrapper.Add.Panel( "selected" );

			{
				var center = wrapper.Add.Panel( "center" );
				center.Add.Icon( "question_mark", "image" );
				currentName = center.Add.Label( "Name", "subtitle" );
				currentDescription = center.Add.Label( "Lorem ipsum dolor sit amet", "description" );
			}

			{
				var inner = wrapper.Add.Panel( "inner" );

				float angleIncrement = 360f / files.Length;
				angleIncrement = MathX.DegreeToRadian( angleIncrement );

				int index = 0;
				foreach ( var file in files )
				{
					Vector2 frac = new Vector2( MathF.Sin( angleIncrement * index ), MathF.Cos( angleIncrement * index ) );

					frac = (1.0f + frac) / 2.0f;

					var panel = inner.Add.Panel( "icon" );
					panel.Style.Set( "background-image", $"url( /ui/models/blocks/{file.Path.Replace( ".vmdl", "" )}.png )" );

					panel.Style.Left = Length.Fraction( frac.x );
					panel.Style.Top = Length.Fraction( frac.y );

					index++;
				}
			}

			BindClass( "active", () => Input.Down( InputButton.Menu ) );
		}

		protected override void OnEvent( PanelEvent e )
		{
			base.OnEvent( e );

			if ( e.Name == "onclick" )
			{
				Vector2 relativeMousePos = Mouse.Position - wrapper.Box.Rect.Center;
				float ang = MathF.Atan2( relativeMousePos.y, relativeMousePos.x )
					.RadianToDegree();

				ang = ang.SnapToGrid( 72f ) + 35f + 70f;
				int selectedIndex = (ang.UnsignedMod( 360.0f ) / 72f).FloorToInt();
				var selectedItem = files[selectedIndex];
				ConsoleSystem.Run( "spawn", "models/blocks/wood/" + selectedItem.Path );
			}
		}

		float targetAngle = 0f;

		public override void Tick()
		{
			base.Tick();

			if ( IsVisible )
			{
				Vector2 relativeMousePos = Mouse.Position - wrapper.Box.Rect.Center;
				float ang = MathF.Atan2( relativeMousePos.y, relativeMousePos.x )
					.RadianToDegree();

				ang = ang.SnapToGrid( 72f ) + 35f + 70f;

				float delta = targetAngle.NormalizeDegrees() - ang.NormalizeDegrees();
				targetAngle = targetAngle.LerpToAngle( ang, 50f * Time.Delta );

				int selectedIndex = (ang.UnsignedMod( 360.0f ) / 72f).FloorToInt();
				var selectedItem = files[selectedIndex];

				if ( MathF.Abs( delta ) > 0.5f )
				{
					currentName.Text = selectedItem.Name;
					currentDescription.Text = selectedItem.Description;

					var tx = new PanelTransform();
					tx.AddRotation( 0, 0, targetAngle );
					tx.AddScale( 1.025f );
					selection.Style.Transform = tx;

					_ = ShrinkEffect();
				}
			}
		}

		private async Task ShrinkEffect()
		{
			wrapper.AddClass( "shrink" );
			await Task.DelaySeconds( Time.Delta );
			wrapper.RemoveClass( "shrink" );
		}
	}
}
