using Sandbox;
using Sandbox.UI.Construct;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace Fortwars
{
	public class Compass : Panel
	{
		private List<CompassPoint> compassPoints = new();

		public Compass()
		{
			StyleSheet.Load( "ui/hud/Compass.scss" );

			// NSEW etc
			for ( int i = 0; i < 8; i++ )
			{
				var compassPoint = new LabelledCompassPoint( this, i );
				compassPoint.Angle = i * 45f;

				compassPoint.Parent = this;
				compassPoints.Add( compassPoint );
			}

			// Angle numbers
			float angleIncrement = 15f;
			for ( int i = 0; i < (360 / angleIncrement); i++ )
			{
				if ( i % 3 == 0 )
					continue;

				var compassPoint = new LabelledCompassPoint( this, i, "small" );
				compassPoint.Label.Text = (i * angleIncrement).CeilToInt().ToString();
				compassPoint.Angle = i * angleIncrement;

				compassPoint.Parent = this;
				compassPoints.Add( compassPoint );
			}

			// Ticks
			for ( int i = 0; i < 360; i++ )
			{
				if ( i % 15 == 0 )
					continue;

				var compassPoint = new CompassPoint( this );
				compassPoint.Angle = i;
				compassPoint.Parent = this;

				compassPoints.Add( compassPoint );
			}
		}
	}

	class CompassPoint : Panel
	{
		public float Angle { get; set; }

		public CompassPoint( Panel parent )
		{
			Parent = parent;
		}

		public float CalcRelativeAngle( float a, float b )
		{
			float mod( float a, float n ) => (a % n + n) % n;
			float length = a - b;

			float d = mod( Math.Abs( length ), 360 );
			float r = d > 180 ? 360 - d : d;
			r *= (length >= 0 && length <= 180) || (length <= -180 && length >= -360) ? 1 : -1;

			return r;
		}

		public override void Tick()
		{
			if ( Local.Pawn is FortwarsPlayer player )
			{
				float playerAng = player.EyeRot.Yaw();
				float relativeAngle = CalcRelativeAngle( playerAng, Angle );

				float position = relativeAngle.LerpInverse( -45f, 45f );
				Style.Left = Length.Fraction( position );

				float t = (position <= 0.5f) ? position : (1.0f - position);
				Style.Opacity = 0.0f.LerpTo( 1.0f, t );
			}
		}
	}

	class LabelledCompassPoint : CompassPoint
	{
		public Label Label { get; set; }

		public LabelledCompassPoint( Panel parent, int index, string className = null ) : base( parent )
		{
			AddClass( className );

			Label = Add.Label( GetDirectionString( index ) );
			Angle = 0;
		}

		public string GetDirectionString( int index )
		{
			switch ( index )
			{
				case 0:
					return "E";
				case 1:
					return "NE";
				case 2:
					return "N";
				case 3:
					return "NW";
				case 4:
					return "W";
				case 5:
					return "SW";
				case 6:
					return "S";
				case 7:
					return "SE";
			}

			return "?";
		}
	}
}
