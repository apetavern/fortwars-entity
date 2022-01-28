using Sandbox;
using System.Linq;

namespace Fortwars
{
	public class BuildRound : BaseRound
	{
		public override string RoundName => "Build";
		public override int RoundDuration => 3;

		protected override void OnStart()
		{
			Log.Info( "Started Build Round" );

			if ( Host.IsServer )
			{
				Player.All.ToList().ForEach( ( player ) => (player as FortwarsPlayer)?.Respawn() );
			}

			foreach ( var wall in Entity.All.OfType<FuncWallToggle>() )
				wall.Show();
		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Build Round" );
		}

		protected override void OnTimeUp()
		{
			Game.Instance.ChangeRound( new CombatRound() );
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
