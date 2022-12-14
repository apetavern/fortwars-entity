// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class Pickup : AnimatedEntity
{
	public Spawner Spawner { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		float bboxSize = 4;
		SetupPhysicsFromAABB( PhysicsMotionType.Static, new Vector3( -bboxSize ), new Vector3( bboxSize ) );

		Tags.Add( "trigger" );

		EnableSolidCollisions = false;
		EnableTouch = true;

		Components.Add( new BobbingComponent() );
	}

	protected override void OnDestroy()
	{
		if ( Game.IsServer && Spawner.IsValid() )
			Spawner.ResetSpawnTimer();

		base.OnDestroy();
	}
}
