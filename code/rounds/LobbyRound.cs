using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortwars
{
	public class LobbyRound : BaseRound
	{
		public override string RoundName => "Lobby";
		public override int RoundDuration => 600;

		protected override void OnStart()
		{
			Log.Info( "Started Lobby Round" );

			if ( Host.IsServer )
			{
				Sandbox.Player.All.ForEach( ( player ) => (player as Player).Respawn() );
			}
		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Lobby Round" );
		}

		public override void OnPlayerKilled( Player player )
		{
			player.Respawn();

			base.OnPlayerKilled( player );
		}
		public override void OnPlayerSpawn( Player player )
		{
			base.OnPlayerSpawn( player );
		}
	}
}
