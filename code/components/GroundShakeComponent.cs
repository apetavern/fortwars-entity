using Sandbox;
using System.Linq;

namespace Fortwars
{
	/// <summary>
	/// Shake the ground when these objects hit something
	/// </summary>
	public class GroundShakeComponent : EntityComponent<FortwarsBlock>
	{
		TimeSince timeSinceLastShake;
		bool wasShakingLastFrame;
		float massScale => 0.0025f;
		float strengthScale => 0.01f;
		float strengthMax => 10;

		[Event.Frame]
		public void FrameUpdate()
		{
			var velocity = Entity.Velocity;
			var tr = Trace.Sweep( Entity.PhysicsBody, Entity.Transform ).WorldOnly().Run();

			if ( timeSinceLastShake < 0.2f )
				return;

			if ( tr.Hit && velocity.Length > 1 && !wasShakingLastFrame )
			{
				var shakeStrength = Vector3.DistanceBetween( CurrentView.Position, Entity.PhysicsBody.MassCenter ).LerpInverse( 512, 0 );
				shakeStrength *= velocity.Length;
				shakeStrength *= Entity.PhysicsBody.Mass * massScale;
				shakeStrength *= strengthScale;
				shakeStrength = shakeStrength.Clamp( 0, strengthMax );

				_ = new Sandbox.ScreenShake.Perlin( 0.5f, 1f, shakeStrength );
				timeSinceLastShake = 0;
				wasShakingLastFrame = true;
			}
			else
			{
				wasShakingLastFrame = false;
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
