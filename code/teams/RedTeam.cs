using System;
using Sandbox;

namespace Fortwars
{
	class RedTeam : BaseTeam
	{
		public override Team ID => Team.Red;
		public override string Name => "Red Team";

		public override void OnPlayerSpawn( FortwarsPlayer player )
		{
			if ( Host.IsServer )
			{
				player.RemoveClothing();
				player.AttachClothing( "models/citizen_clothes/trousers/trousers_tracksuit.vmdl" );
				player.AttachClothing( "models/citizen_clothes/jacket/jacket.red.vmdl" );
				player.AttachClothing( "models/citizen_clothes/shoes/trainers.vmdl" );
				player.AttachClothing( "models/citizen_clothes/hat/hat_beret.red.vmdl" );

				player.SetBodygroup( "Legs", 1 );
				player.SetBodygroup( "Feet", 1 );
				player.SetBodygroup( "Chest", 0 );
			}
		}
	}
}
