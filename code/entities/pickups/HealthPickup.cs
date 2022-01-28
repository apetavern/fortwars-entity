using Sandbox;

namespace Fortwars
{
	// todo: base pickup
	internal class HealthPickup : ModelEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			float bboxSize = 4;
			SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -bboxSize ), new Vector3( bboxSize ) );

			SetModel( "models/sbox_props/burger_box/burger_box.vmdl" );
			Scale = 3.0f;
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			Components.Add<BobbingComponent>( new() );
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( other is not FortwarsPlayer { ActiveChild: FortwarsWeapon weapon } )
				return;

			weapon.ReserveAmmo += weapon.WeaponAsset.MaxAmmo * 2;

			this.Delete();
		}
	}
}
