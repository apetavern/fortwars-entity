using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

namespace Fortwars
{
	public partial class BuildMenu : Panel
	{
		public static BuildMenu Instance;

		private Panel selection;
		private Panel wrapper;

		private Label currentName;
		private Label currentDescription;

		private float lerpedSelectionAngle = 0f;

		private struct BuildMenuItem
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

		private BuildMenuItem[] items => new BuildMenuItem[]
		{
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

			//
			// Center - item info
			//
			{
				var center = wrapper.Add.Panel( "center" );
				center.Add.Icon( "question_mark", "image" );
				currentName = center.Add.Label( "Name", "subtitle" );
				currentDescription = center.Add.Label( "Lorem ipsum dolor sit amet", "description" );
			}

			//
			// Inner - item icons
			//
			{
				var inner = wrapper.Add.Panel( "inner" );

				float angleIncrement = 360f / items.Length;
				angleIncrement = MathX.DegreeToRadian( angleIncrement );

				int index = 0;
				foreach ( var file in items )
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

		/// <summary>
		/// Create a panel transform with all the shit we'd usually put in SCSS
		/// </summary>
		private PanelTransform CreateStandardPanelTransform()
		{
			var panelTransform = new PanelTransform();
			panelTransform.AddScale( 1.025f );
			return panelTransform;
		}

		/// <summary>
		/// Get the current angle based on the mouse position, relative to the center of the menu
		/// </summary>
		private float GetCurrentAngle()
		{
			Vector2 relativeMousePos = Mouse.Position - wrapper.Box.Rect.Center;
			float ang = MathF.Atan2( relativeMousePos.y, relativeMousePos.x )
				.RadianToDegree();

			ang = ang.SnapToGrid( 72f ) + 35f + 70f;

			return ang;
		}

		/// <summary>
		/// Get the current <see cref="BuildMenuItem"/> based on the value returned from <see cref="GetCurrentAngle"/>
		/// </summary>
		private BuildMenuItem? GetCurrentItem()
		{
			var ang = GetCurrentAngle();
			int selectedIndex = (ang.UnsignedMod( 360.0f ) / 72f).FloorToInt();
			var selectedItem = items[selectedIndex];

			return selectedItem;
		}

		protected override void OnEvent( PanelEvent e )
		{
			base.OnEvent( e );

			if ( e.Name == "onclick" )
			{
				ConsoleSystem.Run( "spawn", "models/blocks/wood/" + GetCurrentItem()?.Path );
			}
		}

		public override void Tick()
		{
			base.Tick();

			if ( IsVisible )
			{
				float angle = GetCurrentAngle();
				var selectedItem = GetCurrentItem();

				// Interpolate angle here because scss transition does a shit job of it
				float deltaAngle = lerpedSelectionAngle.NormalizeDegrees() - angle.NormalizeDegrees();
				lerpedSelectionAngle = lerpedSelectionAngle.LerpToAngle( angle, 50f * Time.Delta );

				if ( MathF.Abs( deltaAngle ) > 0.5f )
				{
					currentName.Text = selectedItem?.Name ?? "None";
					currentDescription.Text = selectedItem?.Description ?? "Select something";

					var panelTransform = CreateStandardPanelTransform();
					panelTransform.AddRotation( 0, 0, lerpedSelectionAngle );
					selection.Style.Transform = panelTransform;

					_ = ApplyShrinkEffect();
				}
			}
		}

		private async Task ApplyShrinkEffect()
		{
			wrapper.AddClass( "shrink" );
			await Task.DelaySeconds( Time.Delta );
			wrapper.RemoveClass( "shrink" );
		}
	}
}
