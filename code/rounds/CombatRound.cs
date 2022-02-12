using Sandbox;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars
{
	public class CombatRound : BaseRound
	{
		public override string RoundName => "Combat";
		public override int RoundDuration => 300;

		protected override void OnStart()
		{
			Log.Info( "Started Combat Round" );

			if ( Host.IsServer )
			{
				Player.All.OfType<FortwarsPlayer>().ToList().ForEach( ( player ) =>
				{
					SetupInventory( player );
					Game.Instance.MoveToSpawnpoint( player );
				} );
			}

			foreach ( var wall in Entity.All.OfType<FuncWallToggle>() )
				wall.Hide();
		}

		public override void SetupInventory( Player player )
		{
			base.SetupInventory( player );
			(player as FortwarsPlayer).Class?.AssignCombatLoadout( player.Inventory as Inventory );
		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Combat Round" );
		}

		protected override void OnTimeUp()
		{
			var game = Game.Instance;
			if ( game == null ) return;

			// Assign a score point to winning team. Do nothing on draw.
			if ( game.RedTeamScore > game.BlueTeamScore )
			{
				game.RedWins++;
			}
			else if ( game.RedTeamScore < game.BlueTeamScore )
			{
				game.BlueWins++;
			}

			// Set winning team.
			if ( game.BlueWins == game.RoundsToWin )
			{
				game.WinningTeam = Team.Blue;
			}
			else if ( game.RedWins == game.RoundsToWin )
			{
				game.WinningTeam = Team.Red;
			}

			// Cleanup game.
			game.CleanupCTF();

			// If a team one, set round to EndRound.
			if ( game.WinningTeam != Team.Invalid )
			{
				game.ChangeRound( new EndRound() );
				return;
			}

			// Otherwise, go back into BuildRound for another round.
			game.ChangeRound( new BuildRound() );
		}

		public override void OnPlayerKilled( Player player )
		{
			_ = StartRespawnTimer( player );

			base.OnPlayerKilled( player );
		}

		private async Task StartRespawnTimer( Player player )
		{
			await Task.Delay( 1000 );

			player.Respawn();
		}

		public override void OnPlayerSpawn( Player player )
		{
			base.OnPlayerSpawn( player );
		}
	}
}
