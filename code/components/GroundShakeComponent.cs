using Sandbox;
using System.Linq;

namespace Fortwars
{
	/// <summary>
	/// Shake the ground when these objects hit something
	/// </summary>
	public class GroundShakeComponent : EntityComponent<FortwarsBlock>
	{
		TimeSince timeSinceLast;
		bool wasIntersectingLastFrame;

		[Event.Frame]
		public void FrameUpdate()
		{
			var velocity = Entity.Velocity;
			var tr = Trace.Sweep( Entity.PhysicsBody, Entity.Transform ).WorldOnly().Run();

			if ( timeSinceLast < 0.2f )
				return;

			if ( tr.Hit && velocity.Length > 1 && !wasIntersectingLastFrame )
			{
				var shakeStrength = 512 - Vector3.DistanceBetween( CurrentView.Position, Entity.PhysicsBody.MassCenter );
				shakeStrength /= 512f;
				shakeStrength = shakeStrength.Clamp( 0, 1 );

				shakeStrength *= velocity.Length;
				shakeStrength *= Entity.PhysicsBody.Mass * 0.0025f;

				shakeStrength *= 0.01f;
				shakeStrength = shakeStrength.Clamp( 0, 10 );

				Log.Trace( shakeStrength );

				_ = new Sandbox.ScreenShake.Perlin( 0.5f, 1f, shakeStrength );
				timeSinceLast = 0;
				wasIntersectingLastFrame = true;
			}
			else
			{
				wasIntersectingLastFrame = false;
			}
		}

		[Event.Frame]
		public static void SystemUpdate()
		{
			foreach ( var entity in Sandbox.Entity.All.OfType<FortwarsBlock>() )
			{
				void Remove()
				{
					var existingGroundShake = entity.Components.Get<GroundShakeComponent>();
					existingGroundShake?.Remove();
				}

				if ( !entity.IsValid() )
				{
					Remove();
					continue;
				}

				entity.Components.GetOrCreate<GroundShakeComponent>();
			}
		}
	}
}
