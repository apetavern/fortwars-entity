using Sandbox;

namespace Fortwars
{
	partial class Game
	{
		[Net] public BaseRound Round { get; private set; }
		private BaseRound _lastRound;

		[ServerVar( "fw_min_players", Help = "The minimum players required to start." )]
		public static int MinPlayers { get; set; } = 1;

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
