﻿using Sandbox;

namespace Fortwars.entities.pickups
{
	internal class HealthPickup : ModelEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			float bboxSize = 4;
			SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -bboxSize ), new Vector3( bboxSize ) );

			SetModel( "models/sbox_props/burger_box/burger_box.vmdl" );
			Scale = 5.0f;
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			Components.Add<BobbingComponent>( new() );
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( other is not FortwarsPlayer player )
				return;

			player.Health += 50f;
			player.Health = player.Health.Clamp( 0, 100 );
			this.Delete();
		}
	}
}