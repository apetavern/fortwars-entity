using Sandbox;

namespace Fortwars
{
	[Library( "info_ammo_spawn" )]
	[Hammer.EntityTool( "Ammo Spawn", "FortWars" )]
	[Hammer.EditorModel( "models/rust_props/small_junk/carton_box.vmdl" )]
	public class InfoAmmoSpawn : ModelEntity
	{
		AmmoPickup pickup;

		public override void Spawn()
		{
			base.Spawn();

			pickup = new();
			pickup.Position = Position;
		}
	}
}
