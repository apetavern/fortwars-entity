using Sandbox;

namespace Fortwars
{
	public class AmmoPickup : Pickup
	{
		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/rust_props/small_junk/carton_box.vmdl" );
			Scale = 1.5f;
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( !IsServer )
				return;

			if ( other is not FortwarsPlayer { ActiveChild: FortwarsWeapon weapon } )
				return;

			weapon.ReserveAmmo += weapon.WeaponAsset.MaxAmmo * 2;

			this.Delete();
		}
	}
}
