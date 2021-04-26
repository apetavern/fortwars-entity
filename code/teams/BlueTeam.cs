using System;
using Sandbox;

namespace Fortwars
{
	class BlueTeam : BaseTeam
	{
		public override Team ID => Team.Blue;
		public override string Name => "Blue Team";
		public override Color Color => Color.Blue;

        public override void OnPlayerSpawn( FortwarsPlayer player )
		{
			if ( Host.IsServer )
			{
				player.RemoveClothing();
				player.AttachClothing( "models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl" );
				player.AttachClothing( "models/citizen_clothes/jacket/labcoat.vmdl" );
				player.AttachClothing( "models/citizen_clothes/shoes/trainers.vmdl" );
				player.AttachClothing( "models/citizen_clothes/hat/hat_woolly.vmdl" );

				player.SetBodygroup( "Legs", 1 );
				player.SetBodygroup( "Feet", 1 );
				player.SetBodygroup( "Chest", 0 );
			}
		}
	}
}
