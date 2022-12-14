// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

partial class Grenade
{
	[Library( "fw_grenade_throwable", Title = "Grenade Throwable" )]
	partial class Throwable : BasePhysics
	{
		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/weapons/fraggrenade/fraggrenade_w.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			SetBodyGroup( 0, 1 );
		}

		public async Task ExplodeAfterSeconds( float seconds )
		{
			await Task.DelaySeconds( seconds );

			FortwarsGame.Explode( Position, Owner );

			Delete();
		}
	}
}
