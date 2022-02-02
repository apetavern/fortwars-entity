using Sandbox;
using Sandbox.UI.Construct;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	public static class PlayerExtensions
	{
		public static float CalcRelativeYaw( this Player player, float angle )
		{
			float mod( float a, float n ) => (a % n + n) % n;
			float length = player.EyeRot.Yaw() - angle;

			float d = mod( Math.Abs( length ), 360 );
			float r = d > 180 ? 360 - d : d;
			r *= (length >= 0 && length <= 180) || (length <= -180 && length >= -360) ? 1 : -1;
			r -= 90;

			return r;
		}
	}

	public class Compass : Panel
	{
		private List<IconCompassPoint> worldIcons = new();
		private List<CompassPoint> compassPoints = new();

		private Label currentFacing;

		public Compass()
		{
			StyleSheet.Load( "ui/hud/Compass.scss" );

			// NSEW etc
			for ( int i = 0; i < 8; i++ )
			{
				var compassPoint = new LabelledCompassPoint( this, i );
				compassPoint.Angle = i * 45f;
				compassPoints.Add( compassPoint );
			}

			// Angle numbers
			float angleIncrement = 15f;
			for ( int i = 0; i < 360.0f / angleIncrement; i++ )
			{
				if ( i % (360 / (8 * angleIncrement)) == 0 )
					continue;

				var compassPoint = new LabelledCompassPoint( this, i, "small" );
				compassPoint.Label.Text = (360 - (i * angleIncrement)).CeilToInt().ToString();
				compassPoint.Angle = i * angleIncrement;
				compassPoints.Add( compassPoint );
			}

			// Ticks
			for ( int i = 0; i < 360; i++ )
			{
				if ( i % angleIncrement == 0 )
					continue;

				var compassPoint = new CompassPoint( this );
				compassPoint.Angle = i;
				compassPoint.Parent = this;

				compassPoints.Add( compassPoint );
			}

			Add.Icon( "keyboard_arrow_up", "current-facing-icon" );
			currentFacing = Add.Label( "0", "current-facing" );
		}

		public override void Tick()
		{
			base.Tick();

			var existingIcons = worldIcons.Select( i => i.showIcon ).ToList();

			// TODO: This doesn't support entity deletion and probably should
			foreach ( var showIconInterface in Entity.All.OfType<IShowIcon>().ToList() )
			{
				if ( existingIcons.Contains( showIconInterface ) )
					continue;

				if ( string.IsNullOrEmpty( showIconInterface.NonDiegeticIcon() ) )
					continue;

				var iconCompassPoint = new IconCompassPoint( this, showIconInterface );
				worldIcons.Add( iconCompassPoint );
			}

			if ( Local.Pawn is FortwarsPlayer player )
			{
				float relativeAngle = player.CalcRelativeYaw( 0 );
				if ( relativeAngle < 0 )
					relativeAngle += 360f;
				relativeAngle = 360 - relativeAngle;

				currentFacing.Text = relativeAngle.CeilToInt().ToString();
			}
		}
	}

	class CompassPoint : Panel
	{
		public float Angle { get; set; }

		public CompassPoint( Panel parent )
		{
			AddClass( "compass-point" );
			Parent = parent;
		}

		public override void Tick()
		{
			if ( Local.Pawn is FortwarsPlayer player )
			{
				float relativeAngle = player.CalcRelativeYaw( Angle );

				float position = relativeAngle.LerpInverse( -45, 45 );
				Style.Left = Length.Fraction( position );

				float t = (position <= 0.5f) ? position : (1.0f - position);
				Style.Opacity = MathF.Pow( 0.0f.LerpTo( 1.0f, t ), 0.5f );
			}
		}
	}

	class IconCompassPoint : CompassPoint
	{
		public IconPanel Icon { get; set; }
		public IShowIcon showIcon { get; set; }
		public IconCompassPoint( Panel parent, IShowIcon entity ) : base( parent )
		{
			AddClass( entity.CustomClassName() );

			Icon = Add.Icon( entity.NonDiegeticIcon() );
			showIcon = entity;
		}

		public override void Tick()
		{
			Angle = (showIcon.IconWorldPosition() - CurrentView.Position).EulerAngles.yaw;
			base.Tick();
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
					return "N";
				case 1:
					return "NW";
				case 2:
					return "W";
				case 3:
					return "SW";
				case 4:
					return "S";
				case 5:
					return "SE";
				case 6:
					return "E";
				case 7:
					return "NE";
			}

			return "?";
		}
	}
}
