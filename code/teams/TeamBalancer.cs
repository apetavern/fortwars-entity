using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars
{
	/// <summary>
	/// Balance teams automatically when they're not balanced
	/// </summary>
	public static class TeamBalancer
	{
		private static TimeSince timeSinceLastBalanceCheck = 0;

		[Event.Tick.Server]
		public static void OnServerTick()
		{
			if ( timeSinceLastBalanceCheck < 15 )
				return;

			timeSinceLastBalanceCheck = 0;

			if ( !CheckBalanced() )
			{
				_ = BalanceTeams();
			}
		}

		private static async Task BalanceTeams()
		{
			ChatBox.AddInformation( To.Everyone, "Teams will be auto-balanced in 5 seconds." );
			await Task.Delay( 5000 );

			//
			// Team might've balanced itself
			//
			if ( CheckBalanced() )
				return;

			// Team hasn't balanced so do it ourselves
			// TODO: Keep parties together

			var players = Entity.All.OfType<FortwarsPlayer>().ToList();
			int index = 0;
			foreach ( var player in players )
			{
				var lastTeam = player.Team;
				int team = index % 2;

				if ( team == 0 )
					player.Team = Game.Instance.BlueTeam;
				else
					player.Team = Game.Instance.RedTeam;

				// Did this player just get balanced?
				if ( player.Team != lastTeam )
				{
					ChatBox.AddInformation( To.Single( player ), $"You were moved to team {player.Team.Name}" );
					player.Respawn();

					Log.Trace( $"Player {player.Client.Name} was moved to team {player.Team.Name}" );
				}
				index++;
			}

			timeSinceLastBalanceCheck = 0;
		}

		private static bool CheckBalanced()
		{
			var playersOnRed = Entity.All.OfType<FortwarsPlayer>().Where( p => p.TeamID == Team.Red ).ToList();
			var playersOnBlue = Entity.All.OfType<FortwarsPlayer>().Where( p => p.TeamID == Team.Blue ).ToList();

			int totalPlayerCount = playersOnRed.Count + playersOnBlue.Count;

			if ( playersOnRed.Count != playersOnBlue.Count && totalPlayerCount > 1 )
			{
				// Do we have an odd number of players and is it only unbalanced by one
				if ( totalPlayerCount % 2 == 1 && Math.Abs( playersOnRed.Count - playersOnBlue.Count ) <= 1 )
					return true;

				Log.Trace( $"Teams are unbalanced! ({playersOnRed.Count} red vs {playersOnBlue.Count} blue)" );
				return false;
			}

			return true;
		}
	}
}
