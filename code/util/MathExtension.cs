using Sandbox;

namespace Fortwars
{
	public static class MathExtension
	{
		/// <summary>
		/// Calculate shortest difference between self and target.
		/// </summary>
		public static float DistanceAngle( this float self, float target )
		{
			float delta = (target - self).UnsignedMod( 360.0f );

			if ( delta > 180.0f )
			{
				delta -= 360.0f;
			}

			return delta;
		}

		/// <summary>
		/// Lerp towards an angle, taking the shortest difference into account.
		/// </summary>
		public static float LerpToAngle( this float self, float target, float delta )
		{
			float distance = self.DistanceAngle( target );

			if ( distance > -delta && delta > distance )
			{
				return target;
			}

			target = self + distance;

			return self.LerpTo( target, delta );
		}
	}
}
