namespace Sandbox.ScreenShake
{
	public class ViewPunch : CameraModifier
	{
		float RotationAmount;
		float Length;

		Vector2 direction;

		TimeSince lifeTime = 0;

		public ViewPunch( float length = 1.0f, float rotation = 0.6f )
		{
			RotationAmount = rotation;
			Length = length;

			direction = Vector2.Random.Normal;
		}

		public override bool Update( ref CameraSetup cam )
		{
			var delta = ((float)lifeTime).LerpInverse( 0, Length, true );
			delta = Easing.EaseOut( delta );
			var invdelta = 1 - delta;

			cam.Rotation *= Rotation.FromAxis( Vector3.Up, direction.x * invdelta * RotationAmount );
			cam.Rotation *= Rotation.FromAxis( Vector3.Right, direction.y * invdelta * RotationAmount );

			return lifeTime < Length;
		}
	}
}
