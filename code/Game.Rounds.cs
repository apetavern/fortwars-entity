using Sandbox;

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
	}
}
