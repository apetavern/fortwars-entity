using Sandbox;
using System;

namespace Fortwars
{
	[Library( "fw_flag" )]
	public partial class FlagDisplay : ModelEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			SetModel( "models/rust_props/small_junk/toilet_paper.vmdl" );
			Scale = 5.0f;
		}

		[Event.PreRender]
		public virtual void OnPreRender()
		{
			if ( this.SceneObject == null )
				return;

			SceneObject.Rotation = Rotation.From( 45, Time.Now * 90f, 0 );

			// actual origin is off-center, let's just center that
			Vector3 centerOffset = SceneObject.Rotation.Down * 3f;

			// bob up and down
			Vector3 bobbingOffset = Vector3.Up * MathF.Sin( Time.Now * 2f );
			SceneObject.Position = Position + (centerOffset + bobbingOffset) * Scale;
		}
	}
}
