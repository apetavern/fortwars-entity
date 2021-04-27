using Sandbox;

namespace Fortwars
{
	[Library("fw_flag")]
	public partial class FlagDisplay : ModelEntity, IFrameUpdate
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

			SetModel("models/rust_props/small_junk/toilet_paper.vmdl");
			WorldScale = 5.0f;
		}

		public virtual void OnFrame()
        {
			// this.SceneObject
			// just rotate it clientside ?
        }
	}
}
