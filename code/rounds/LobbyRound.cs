using Sandbox;
using System.Linq;

namespace Fortwars
{
	public class LobbyRound : BaseRound
	{
		public override string RoundName => "Lobby";
		public override int RoundDuration => 60;

		protected override void OnStart()
		{
			Log.Info( "Started Lobby Round" );

			if ( Host.IsServer )
			{
				Player.All.ToList().ForEach( ( player ) => (player as FortwarsPlayer)?.Respawn() );
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
