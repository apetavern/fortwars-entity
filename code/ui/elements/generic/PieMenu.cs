using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Fortwars
{
	[UseTemplate]
	public partial class PieMenu : Panel
	{
		public Panel Selector { get; set; }
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

		public struct Item
		{
			public string Icon { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }

			public Item( string name, string description, string icon = "question_mark" )
			{
				Name = name;
				Description = description;
				Icon = icon;
			}
		}

		public virtual Item[] Items { get; }

		public PieMenu()
		{
			SetTemplate( "ui/elements/generic/PieMenu.html" );
			AddClass( "pie-menu" );

			BindClass( "active", () => Input.Down( InputButton.Menu ) );
		}

		public override void OnDeleted()
		{
			base.OnDeleted();
		}

		protected override void PostTemplateApplied()
		{
			base.PostTemplateApplied();
			BuildIcons();
		}

		private float AngleIncrement => 360f / Items.Length;

		/// <summary>
		/// Puts icons on the wheel so the player knows what they're selecting
		/// </summary>
		private void BuildIcons()
		{
			int index = -1;
			foreach ( var item in Items )
			{
				Vector2 frac = MathExtension.InverseAtan2( AngleIncrement * index );

				frac = (1.0f + frac) / 2.0f; // Normalize from -1,1 to 0,1

				var panel = Inner.Add.Icon( item.Icon, "item-icon" );

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
		protected float GetCurrentAngle()
		{
			Vector2 relativeMousePos = VirtualCursor.Position;

			float ang = MathF.Atan2( relativeMousePos.y, relativeMousePos.x )
				.RadianToDegree();

			float centerOffset = AngleIncrement / 4f; // makes it so icon appears in center
			ang = ang.SnapToGrid( AngleIncrement ) + AngleIncrement + centerOffset;

			return ang;
		}

		/// <summary>
		/// Get the current <see cref="Item"/> based on the value returned from <see cref="GetCurrentAngle"/>
		/// </summary>
		protected Item? GetCurrentItem()
		{
			var ang = GetCurrentAngle();

			int selectedIndex = (ang.UnsignedMod( 360.0f ) / AngleIncrement).FloorToInt();
			var selectedItem = Items[selectedIndex];

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
				Selector.Style.Transform = panelTransform;

				OnChange();
			}
		}

		protected virtual void OnChange()
		{

		}
	}
}
