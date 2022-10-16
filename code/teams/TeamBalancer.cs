// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars
{
	/// <summary>
	/// Balances teams automatically when they're not balanced
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

			if ( !CheckBalanced( out _, out _ ) )
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
			if ( CheckBalanced( out int countDifference, out var largestTeam ) )
				return;

			// Team hasn't balanced so do it ourselves
			// TODO: Keep parties together

			var players = Entity.All.OfType<FortwarsPlayer>().ToList();
			int index = 0;

			Log.Trace( $"Teams are unbalanced by {countDifference} players." );

			// Take random players from largest team
			var playersToMove = Entity.All.OfType<FortwarsPlayer>()
				.Where( p => p.TeamID == largestTeam )
				.OrderBy( x => Guid.NewGuid() )
				.Take( countDifference / 2 )
				.ToList();

			foreach ( var player in playersToMove )
			{
				var lastTeam = player.Team;
				int team = index % 2;

				switch ( largestTeam )
				{
					case Team.Red:
						player.Team = Game.Instance.BlueTeam;
						break;
					case Team.Blue:
						player.Team = Game.Instance.RedTeam;
						break;
				}

				ChatBox.AddInformation( To.Single( player ), $"You were moved to team {player.Team.Name}" );
				player.Respawn();

				Log.Trace( $"Player {player.Client.Name} was moved to team {player.Team.Name}" );
				index++;
			}

			timeSinceLastBalanceCheck = 0;
		}

		private static bool CheckBalanced( out int countDifference, out Team largestTeam )
		{
			largestTeam = Team.Invalid;
			countDifference = -1; // Default value, means we don't need to balance

			var playersOnRed = Entity.All.OfType<FortwarsPlayer>().Where( p => p.TeamID == Team.Red ).ToList();
			var playersOnBlue = Entity.All.OfType<FortwarsPlayer>().Where( p => p.TeamID == Team.Blue ).ToList();

			int totalPlayerCount = playersOnRed.Count + playersOnBlue.Count;

			if ( playersOnRed.Count != playersOnBlue.Count && totalPlayerCount > 1 )
			{
				// Do we have an odd number of players and is it only unbalanced by one
				if ( totalPlayerCount % 2 == 1 && Math.Abs( playersOnRed.Count - playersOnBlue.Count ) <= 1 )
					return true;

				Log.Trace( $"Teams are unbalanced! ({playersOnRed.Count} red vs {playersOnBlue.Count} blue)" );
				countDifference = Math.Abs( playersOnRed.Count - playersOnBlue.Count );
				largestTeam = ( playersOnRed.Count > playersOnBlue.Count ) ? Team.Red : Team.Blue;

				return false;
			}

			return true;
		}
	}
}
