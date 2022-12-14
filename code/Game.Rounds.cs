// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.Collections.Generic;

namespace Fortwars;

partial class FortwarsGame
{
	[Net, Change( nameof( OnRoundChange ) )] public BaseRound Round { get; private set; }

	[Net]
	public int MinPlayers { get; set; } = 2;

	[Net] public int BestOf { get; set; } = 3;
	public int RoundsToWin => MathX.CeilToInt( (float)BestOf / 2 );

	[Net] public int BlueWins { get; set; } = 0;
	[Net] public int RedWins { get; set; } = 0;

	[Net] public Team WinningTeam { get; set; } = Team.Invalid;

	public struct MapVote
	{
		public int MapIndex { get; set; }
		public long SteamId { get; set; }
		public MapVote( int mapIndex, long SteamId )
		{
			MapIndex = mapIndex;
			SteamId = SteamId;
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

	[ConCmd.Server( "vote_map" )]
	public static void VoteMap( int index )
	{
		var SteamId = ConsoleSystem.Caller.SteamId;
		var mapVotes = Instance?.MapVotes;

		foreach ( var vote in mapVotes )
		{
			if ( vote.SteamId == SteamId ) return;
		}

		Instance?.MapVotes.Add( new MapVote( index, SteamId ) );
	}

	public static List<string> GetMaps()
	{
		var packageTask = Package.Fetch( Game.Server.GameIdent, true ).ContinueWith( t =>
		{
			Package package = t.Result;
			return package.GetMeta<List<string>>( "MapList" );
		} );

		return packageTask.Result;
	}
}
