using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

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

			Game.Explode( Position, Owner );

			Delete();
		}
	}
}
