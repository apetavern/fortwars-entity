using Sandbox;

namespace Fortwars
{
	[Library( "info_flag_spawn" )]
	public partial class InfoFlagSpawn : ModelEntity
	{
		[HammerProp("team")]
		public int Team { get; set; }

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
			WorldScale = 5.0f;

			if (Team == 0)
            {
				RenderColor = Color32.Red;
            } else if (Team == 1)
            {
				RenderColor = Color32.Blue;
            }

			// Transmit = TransmitType.Never;

			// 

			Log.Info( "FlagSpawn created at: " + WorldPos );

		}
	}
}
