using Sandbox;

namespace Fortwars
{
	public class Pickup : ModelEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			float bboxSize = 4;
			SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -bboxSize ), new Vector3( bboxSize ) );

			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			Components.Add<BobbingComponent>( new() );
		}
	}
}
