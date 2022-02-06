using Sandbox;
using Sandbox.UI;
using System;

namespace Fortwars
{
	public partial class RadialWheel
	{
		public class PieSelector : Panel
		{
			RadialWheel parentWheel;

			public PieSelector( RadialWheel parentWheel )
			{
				this.parentWheel = parentWheel;
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

			/// <summary>
			/// Generate our cool selection texture based on number of items in parent wheel
			/// </summary>
			private void GenerateTexture()
			{
				using ( Profiler.CreateScope( "Generate radial selector texture" ) )
				{
					int width = 768;
					int height = 768;
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
						float angle = MathF.Atan2( circleCenter.y - y, circleCenter.x - x ).RadianToDegree().NormalizeDegrees();

						// We do this to offset everything to match array indexing at 0
						return angle < parentWheel.AngleIncrement;
					}

					for ( int x = 0; x < width; x++ )
					{
						for ( int y = 0; y < height; y++ )
						{
							float pixelOpacity = 0;

							//
							// Sample multiple points to make it less jagged
							//
							for ( int xOff = -1; xOff <= 1; xOff++ )
							{
								for ( int yOff = -1; yOff <= 1; yOff++ )
								{
									if ( InCircle( x + xOff, y + yOff, circleRadius ) && // Outer ring
										!InCircle( x + xOff, y + yOff, circleRadius * 0.6f ) && // Inner ring
										InSegment( x + xOff, y + yOff ) ) // Pie segment
									{
										pixelOpacity++;
									}
								}
							}

							pixelOpacity = pixelOpacity / 9.0f;
							SetPixel( x, y, Color.White * pixelOpacity );
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
			private float lerpedSelectionAngle = 0f;

			public override void Tick()
			{
				base.Tick();

				// Interpolate angle here because scss transition does a shit job of it
				float angle = parentWheel.SelectedIndex * parentWheel.AngleIncrement;
				lerpedSelectionAngle = lerpedSelectionAngle.LerpToAngle( angle, 25f * Time.Delta );

				var panelTransform = CreateStandardPanelTransform();
				panelTransform.AddRotation( 0, 0, lerpedSelectionAngle + 180f - (parentWheel.AngleIncrement / 2f) );
				Style.Transform = panelTransform;
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
		}
	}
}
