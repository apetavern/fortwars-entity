using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	partial class Game
	{
		[Net] public int RedTeamScore { get; set; }
		[Net] public int BlueTeamScore { get; set; }
		[Net] public FortwarsPlayer RedFlagCarrier { get; set; }
		[Net] public FortwarsPlayer BlueFlagCarrier { get; set; }

		/**
		 * Called when a Player walks into a Team's flagzone.
		 */
		public void OnPlayerTouchFlagzone( FortwarsPlayer player, Team team )
		{
			// don't let spectators interact with the flagzone at all
			if ( player.IsSpectator )
				return;

			// If the player is in their own flag zone
			if ( player.TeamID == team )
			{
				// If the player isn't carrying a flag, nothing to do
				if ( player != RedFlagCarrier && player != BlueFlagCarrier )
					return;

				// Check if the player's team flag is out (you can only capture if your flag is in base)
				if ( (player.Team is BlueTeam && BlueFlagCarrier != null) || (player.Team is RedTeam && RedFlagCarrier != null) )
					return;

				PlayerScoreFlag( player );
				return;
			}

			// The player must be in the enemy flag zone

			// Check if the enemy flag is actually here
			if ( (player.Team is BlueTeam && RedFlagCarrier != null) || (player.Team is RedTeam && BlueFlagCarrier != null) )
				return;

			PlayerPickupEnemyFlag( player );
		}

		public void PlayerPickupEnemyFlag( FortwarsPlayer player )
		{
			BaseTeam enemyTeam = player.TeamID switch
			{
				Team.Blue => RedTeam,
				Team.Red => BlueTeam,
				_ => RedTeam, // shit but shutiup
			};

			if ( enemyTeam is RedTeam )
			{
				RedFlagCarrier = player;
			}

			if ( enemyTeam is BlueTeam )
			{
				BlueFlagCarrier = player;
			}

			ChatBox.AddInformation( To.Everyone, $"{player.Name} picked up {enemyTeam.Name} flag", $"avatar:{player.Client.PlayerId}" );

			player.PlaySound( "ctf_flag_pickup" );
		}

		public void PlayerScoreFlag( FortwarsPlayer player )
		{
			// play a sick sound
			player.PlaySound( "ctf_flag_pickup" );

			if ( player == BlueFlagCarrier )
			{
				// Make the player drop the flag
				BlueFlagCarrier = null;

				// Up the team score
				RedTeamScore++;

				// Announce
				ChatBox.AddInformation( To.Everyone, $"{player.Name} scored for {RedTeam.Name}", $"avatar:{player.Client.PlayerId}" );
			}

			if ( player == RedFlagCarrier )
			{
				// Make the player drop the flag
				RedFlagCarrier = null;

				// Up the team score
				BlueTeamScore++;

				// Announce
				ChatBox.AddInformation( To.Everyone, $"{player.Name} scored for {BlueTeam.Name}", $"avatar:{player.Client.PlayerId}" );
			}
		}

		public void PlayerDropFlag( FortwarsPlayer player )
		{
			if ( player == BlueFlagCarrier )
			{
				ChatBox.AddInformation( To.Everyone, $"{player.Name} dropped {BlueTeam.Name} flag", $"avatar:{player.Client.PlayerId}" );
				BlueFlagCarrier = null;
				return;
			}
			if ( player == RedFlagCarrier )
			{
				ChatBox.AddInformation( To.Everyone, $"{player.Name} dropped {RedTeam.Name} flag", $"avatar:{player.Client.PlayerId}" );
				RedFlagCarrier = null;
				return;
			}
		}

		public void ResetFlags()
		{
			RedFlagCarrier = null;
			BlueFlagCarrier = null;
		}
	}
}
