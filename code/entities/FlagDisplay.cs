using Sandbox;

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
			SceneObject.Position = Position + SceneObject.Rotation.Down * Scale * 3;
		}
	}
}
