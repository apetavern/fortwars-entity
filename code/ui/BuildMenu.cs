using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;

namespace Fortwars
{
	[UseTemplate]
	public partial class BuildMenu : Panel
	{
		public static BuildMenu Instance;

		public Panel Selection { get; set; }
		public Panel Wrapper { get; set; }

		//
		// @text
		//
		public string CurrentIcon { get; set; }
		public string CurrentName { get; set; }
		public string CurrentDescription { get; set; }

		//
		// @ref
		//
		public Panel Inner { get; set; }

		private float lerpedSelectionAngle = 0f;

		private struct BuildMenuItem
		{
			public string Path { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string Icon { get; set; }
			public BuildMenuItem( string path, string name, string description, string icon = "question_mark" )
			{
				Path = path;
				Name = name;
				Description = description;
				Icon = icon;
			}

			public static BuildMenuItem[] Items => new BuildMenuItem[]
			{
				new ("fw_3x2.vmdl", "3x2", "A wide panel good for defences", "crop_5_4"),
				new ("fw_1x2.vmdl", "1x2", "A medium panel good for entrances", "crop_portrait"),
				new ("fw_1x4.vmdl", "1x4", "A tall panel good for ledges", "crop_7_5"),
				new ("fw_1x1x1.vmdl", "1x1x1", "A thicc block good for climbing", "view_in_ar"),
				new ("fw_1x2x1.vmdl", "1x2x1", "A thicc block good for cover", "view_in_ar"),
			};
		}

		public BuildMenu()
		{
			Instance = this;
			BindClass( "active", () => Input.Down( InputButton.Menu ) );

			VirtualCursor.OnClick += OnClick;
		}

		private void OnClick()
		{
			ConsoleSystem.Run( "spawn", "models/blocks/wood/" + GetCurrentItem()?.Path );
			_ = ApplyShrinkEffect();
		}

		public override void OnDeleted()
		{
			base.OnDeleted();
			VirtualCursor.OnClick -= OnClick;
		}

		protected override void PostTemplateApplied()
		{
			base.PostTemplateApplied();
			BuildIcons();
		}

		private float AngleIncrement => 360f / BuildMenuItem.Items.Length;

		/// <summary>
		/// Puts icons on the wheel so the player knows what they're selecting
		/// </summary>
		private void BuildIcons()
		{
			int index = -1;
			foreach ( var file in BuildMenuItem.Items )
			{
				Vector2 frac = MathExtension.InverseAtan2( AngleIncrement * index );

				frac = (1.0f + frac) / 2.0f; // Normalize from -1,1 to 0,1

				var panel = Inner.Add.Panel( "icon" );
				panel.Style.Set( "background-image", $"url( /ui/models/blocks/{file.Path.Replace( ".vmdl", "" )}.png )" );

				panel.Style.Left = Length.Fraction( frac.x );
				panel.Style.Top = Length.Fraction( frac.y );

				index++;
			}
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
		/// Get the current angle based on the mouse position, relative to the center of the menu.
		/// Returns <see langword="null"/> if we're not really looking at anything
		/// </summary>
		private float GetCurrentAngle()
		{
			Vector2 relativeMousePos = VirtualCursor.Position;

			float ang = MathF.Atan2( relativeMousePos.y, relativeMousePos.x )
				.RadianToDegree();

			float centerOffset = AngleIncrement / 4f; // makes it so icon appears in center
			ang = ang.SnapToGrid( AngleIncrement ) + AngleIncrement + centerOffset;

			return ang;
		}

		/// <summary>
		/// Get the current <see cref="BuildMenuItem"/> based on the value returned from <see cref="GetCurrentAngle"/>
		/// </summary>
		private BuildMenuItem? GetCurrentItem()
		{
			var ang = GetCurrentAngle();

			int selectedIndex = (ang.UnsignedMod( 360.0f ) / AngleIncrement).FloorToInt();
			var selectedItem = BuildMenuItem.Items[selectedIndex];

			return selectedItem;
		}

		public override void Tick()
		{
			base.Tick();

			if ( !HasClass( "active" ) )
			{
				VirtualCursor.Reset();
				return;
			}

			VirtualCursor.InUse = true;

			var angle = GetCurrentAngle();
			var selectedItem = GetCurrentItem();

			// Interpolate angle here because scss transition does a shit job of it
			float deltaAngle = lerpedSelectionAngle.NormalizeDegrees() - angle.NormalizeDegrees();
			lerpedSelectionAngle = lerpedSelectionAngle.LerpToAngle( angle, 50f * Time.Delta );

			if ( MathF.Abs( deltaAngle ) > 0.5f )
			{
				CurrentIcon = selectedItem?.Icon ?? "question_mark";
				CurrentName = selectedItem?.Name ?? "None";
				CurrentDescription = selectedItem?.Description ?? "Select something";

				var panelTransform = CreateStandardPanelTransform();

				panelTransform.AddRotation( 0, 0, lerpedSelectionAngle );
				Selection.Style.Transform = panelTransform;

				_ = ApplyShrinkEffect();
			}
		}

		private async Task ApplyShrinkEffect()
		{
			Wrapper.AddClass( "shrink" );
			await Task.DelaySeconds( Time.Delta );
			Wrapper.RemoveClass( "shrink" );
		}
	}
}
