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

		[Event.Frame]
		public virtual void OnFrame()
		{
			// this.SceneObject
			// just rotate it clientside ?
		}
	}
}
