using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	partial class Game
	{
		[Net, Change( nameof( OnRoundChange ) )] public BaseRound Round { get; private set; }

		[ServerVar( "fw_min_players", Help = "The minimum players required to start." )]
		public static int MinPlayers { get; set; } = 1;

		[Net] public int BestOf { get; set; } = 3;
		public int RoundsToWin => MathX.CeilToInt( (float)BestOf / 2 );

		[Net] public int BlueWins { get; set; } = 0;
		[Net] public int RedWins { get; set; } = 0;

		[Net] public Team WinningTeam { get; set; } = Team.Invalid;

		public struct MapVote
		{
			public int MapIndex { get; set; }
			public long PlayerId { get; set; }
			public MapVote( int mapIndex, long playerId )
			{
				MapIndex = mapIndex;
				PlayerId = playerId;
			}
		}

		[Net] public IList<MapVote> MapVotes { get; set; }

		public void ChangeRound( BaseRound round )
		{
			Assert.NotNull( round );

			Round?.Finish();
			Round = round;
			Round?.Start();
		}

		private void CheckMinimumPlayers()
		{
			if ( Player.All.Count >= MinPlayers )
			{
				if ( Round is LobbyRound || Round == null )
				{
					ChangeRound( new BuildRound() );
				}
			}
			else if ( Round is not LobbyRound )
			{
				ChangeRound( new LobbyRound() );
			}
		}

		[ServerCmd( "vote_map" )]
		public static void VoteMap( int index )
		{
			var playerId = ConsoleSystem.Caller.PlayerId;
			var mapVotes = Game.Instance?.MapVotes;

			foreach ( var vote in mapVotes )
			{
				if ( vote.PlayerId == playerId ) return;
			}

			Game.Instance?.MapVotes.Add( new MapVote( index, playerId ) );
		}

		public static string[] GetMaps()
		{
			var packageTask = Package.Fetch( Global.GameIdent, true ).ContinueWith( t =>
			{
				var package = t.Result;
				return package.GameConfiguration.MapList.ToArray();
			} );

			return packageTask.Result;
		}
	}
}
