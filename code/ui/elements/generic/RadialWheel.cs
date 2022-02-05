using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Fortwars
{
	[UseTemplate]
	public partial class RadialWheel : Panel
	{
		//
		// @text
		//
		public string CurrentName { get; set; }
		public string CurrentDescription { get; set; }

		//
		// @ref
		//
		public Panel Wrapper { get; set; }
		public Image CurrentImage { get; set; }
		public Panel Inner { get; set; }
		public InputHint BuildHint { get; set; }
		public RichLabel BuildError { get; set; }

		//
		// Runtime
		//
		private float lerpedSelectionAngle = 0f;
		private PieSelector selector;

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

		public RadialWheel()
		{
			SetTemplate( "ui/elements/generic/RadialWheel.html" );
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

			//
			// Create pie selector here so that it regenerates
			// when there's changes etc.
			//
			selector?.Delete();
			selector = new PieSelector( Items.Length );
			selector.Parent = Wrapper;
		}

		private float AngleIncrement => 360f / Items.Length;
		private List<Panel> icons = new();

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

				var panel = Inner.Add.Image( item.Icon, "item-icon" );

				panel.Style.Left = Length.Fraction( frac.x );
				panel.Style.Top = Length.Fraction( frac.y );

				icons.Add( panel );

				index++;
			}
		}

		/// <summary>
		/// Create a panel transform with all the shit we'd usually put in SCSS
		/// </summary>
		private PanelTransform CreateStandardPanelTransform()
		{
			var panelTransform = new PanelTransform();
			panelTransform.AddScale( 1.05f );
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

			ang = ang.SnapToGrid( AngleIncrement );

			return ang;
		}

		protected int GetCurrentIndex()
		{
			var ang = GetCurrentAngle();
			return (ang.UnsignedMod( 360.0f ) / AngleIncrement).FloorToInt();
		}

		/// <summary>
		/// Get the current <see cref="Item"/> based on the value returned from <see cref="GetCurrentAngle"/>
		/// </summary>
		protected Item? GetCurrentItem()
		{
			int selectedIndex = GetCurrentIndex();
			var selectedItem = Items[selectedIndex];

			return selectedItem;
		}

		int lastIndex;
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
			int selectedIndex = GetCurrentIndex();

			for ( int i = 0; i < icons.Count; i++ )
			{
				icons[i].SetClass( "active", i == selectedIndex );
			}

			// Interpolate angle here because scss transition does a shit job of it
			float deltaAngle = lerpedSelectionAngle.NormalizeDegrees() - angle.NormalizeDegrees();
			lerpedSelectionAngle = lerpedSelectionAngle.LerpToAngle( angle, 50f * Time.Delta );

			if ( MathF.Abs( deltaAngle ) >= 0.5f )
			{
				CurrentImage.SetTexture( selectedItem?.Icon ?? "" );
				CurrentName = selectedItem?.Name ?? "None";
				CurrentDescription = selectedItem?.Description ?? "Select something";

				var panelTransform = CreateStandardPanelTransform();

				panelTransform.AddRotation( 0, 0, lerpedSelectionAngle + AngleIncrement );
				selector.Style.Transform = panelTransform;

				BuildError.Style.Display = DisplayMode.None;
				BuildHint.Style.Display = DisplayMode.Flex;

				if ( lastIndex != selectedIndex )
					OnChange();
			}

			lastIndex = GetCurrentIndex();
		}

		protected virtual void OnChange() { }
	}

	public class PieSelector : Panel
	{
		int itemCount;
		public PieSelector( int itemCount )
		{
			this.itemCount = itemCount;
		}

		public override void OnParentChanged()
		{
			base.OnParentChanged();
			GenerateTexture();
		}

		public override void OnHotloaded()
		{
			base.OnHotloaded();
			GenerateTexture();
		}

		private void GenerateTexture()
		{
			int width = 2048;
			int height = 2048;
			int stride = 4;

			Vector2 circleSize = new Vector2( width, height );
			Vector2 circleCenter = circleSize / 2.0f;
			float circleRadius = width / 2f;

			// RGBA texture
			byte[] textureData = new byte[width * height * 4];
			void SetPixel( int x, int y, Color col )
			{
				textureData[((x + (y * width)) * stride) + 0] = col.r.ColorComponentToByte();
				textureData[((x + (y * width)) * stride) + 1] = col.g.ColorComponentToByte();
				textureData[((x + (y * width)) * stride) + 2] = col.b.ColorComponentToByte();
				textureData[((x + (y * width)) * stride) + 3] = col.a.ColorComponentToByte();
			}

			//
			// Is this pixel in a circle
			//
			bool InCircle( int x, int y, float radius )
			{
				return MathF.Pow( x - circleCenter.x, 2 ) + MathF.Pow( y - circleCenter.y, 2 ) < MathF.Pow( radius, 2 );
			}

			//
			// Is this pixel in a segment of the pie
			//
			bool InSegment( int x, int y )
			{
				float angleIncrement = 360 / itemCount;
				float angle = MathF.Atan2( circleCenter.y - y, circleCenter.x - x ).RadianToDegree().NormalizeDegrees();

				// We do this to offset everything to match array indexing at 0
				return angle < angleIncrement;
			}

			for ( int x = 0; x < width; x++ )
			{
				for ( int y = 0; y < height; y++ )
				{
					if ( InCircle( x, y, circleRadius ) && // Outer ring
						!InCircle( x, y, circleRadius * 0.6f ) && // Inner ring
						InSegment( x, y ) ) // Pie segment
					{
						SetPixel( x, y, Color.White );
					}
					else
					{
						SetPixel( x, y, Color.White * 0.0f );
					}
				}
			}

			var newTexture = Texture.Create( width, height )
				.WithStaticUsage()
				.WithData( textureData )
				.WithName( "PieSelector" )
				.Finish();

			Style.BackgroundImage = newTexture;
		}
	}
}
