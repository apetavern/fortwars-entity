using Sandbox;
using System.Linq;

namespace Fortwars
{
	partial class Game
	{
		[Net] public int RedTeamScore { get; set; }
		[Net] public int BlueTeamScore { get; set; }
		[Net] public FortwarsPlayer RedFlagCarrier { get; set; }
		[Net] public FortwarsPlayer BlueFlagCarrier { get; set; }

		[Net] public BogRoll RedFlagRoll { get; set; }
		[Net] public BogRoll BlueFlagRoll { get; set; }

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
				player.ActiveChild.Delete();//Has to be the flag from the checks before this.
				return;
			}

			// The player must be in the enemy flag zone

			// Check if the enemy flag is actually here
			if ( (player.Team is BlueTeam && RedFlagCarrier != null) || (player.Team is RedTeam && BlueFlagCarrier != null) )
				return;

			// Check if the flag exists in the world
			if ( (player.Team is BlueTeam && RedFlagRoll != null) || (player.Team is RedTeam && BlueFlagRoll != null) )
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
				RedFlagRoll = new BogRoll();
				RedFlagRoll.Team = Team.Red;
				player.Inventory.Add( RedFlagRoll, true );
			}

			if ( enemyTeam is BlueTeam )
			{
				BlueFlagCarrier = player;
				BlueFlagRoll = new BogRoll();
				BlueFlagRoll.Team = Team.Blue;
				player.Inventory.Add( BlueFlagRoll, true );
			}

			HideFlag( enemyTeam.ID );

			ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} picked up {enemyTeam.Name} flag", $"avatar:{player.Client.PlayerId}", true );


			foreach ( FortwarsPlayer ply in All.OfType<FortwarsPlayer>() )
			{
				if ( ply.TeamID == player.TeamID )
				{
					PlayLocalSound( To.Single( ply.Client ), "enemyflagtaken" );//positive sound, same team who took flag
				}
				else
				{
					PlayLocalSound( To.Single( ply.Client ), "enemytookflag" );//negative sound, enemy team took flag
				}
			}
		}

		public void PlayerPickupEnemyFlagFloor( FortwarsPlayer player )
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

			ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} picked up {enemyTeam.Name} flag", $"avatar:{player.Client.PlayerId}", true );

			player.PlaySound( "ctf_flag_pickup" );
		}

		[ClientRpc]
		public void PlayLocalSound( string sound )
		{
			PlaySound( sound );
		}

		public void PlayerScoreFlag( FortwarsPlayer player )
		{
			// play a sick sound
			foreach ( FortwarsPlayer ply in All.OfType<FortwarsPlayer>() )
			{
				if ( ply.TeamID == player.TeamID )
				{
					PlayLocalSound( To.Single( ply.Client ), "enemyflagcaptured" );//positive sound, same team who scored
				}
				else
				{
					PlayLocalSound( To.Single( ply.Client ), "enemycapturedourflag" );//negative sound, enemy team scored
				}
			}

			if ( player == BlueFlagCarrier )
			{
				// Make the player drop the flag
				BlueFlagCarrier = null;

				// Up the team score
				RedTeamScore++;

				ShowFlag( Team.Blue );

				// Announce
				ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} scored for {RedTeam.Name}", $"avatar:{player.Client.PlayerId}", true );
			}

			if ( player == RedFlagCarrier )
			{
				// Make the player drop the flag
				RedFlagCarrier = null;

				// Up the team score
				BlueTeamScore++;

				ShowFlag( Team.Red );

				// Announce
				ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} scored for {BlueTeam.Name}", $"avatar:{player.Client.PlayerId}", true );
			}
		}

		public void PlayerDropFlag( FortwarsPlayer player )
		{
			if ( player == BlueFlagCarrier )
			{
				ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} dropped {BlueTeam.Name} flag", $"avatar:{player.Client.PlayerId}", true );
				BlueFlagCarrier = null;
				//ShowFlag( Team.Blue );
				return;
			}
			if ( player == RedFlagCarrier )
			{
				ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} dropped {RedTeam.Name} flag", $"avatar:{player.Client.PlayerId}", true );
				RedFlagCarrier = null;
				//ShowFlag( Team.Red );
				return;
			}
		}

		public void ReturnFlag( Team Team )
		{
			if ( Team == Team.Blue )
			{
				ChatBox.AddInformation( To.Everyone, $"{BlueTeam.Name} flag returned", null, true );
				BlueFlagCarrier = null;
				ShowFlag( Team.Blue );
				return;
			}
			if ( Team == Team.Red )
			{
				ChatBox.AddInformation( To.Everyone, $"{RedTeam.Name} flag returned", null, true );
				RedFlagCarrier = null;
				ShowFlag( Team.Red );
				return;
			}
		}

		public void ResetFlags()
		{
			RedFlagCarrier = null;
			BlueFlagCarrier = null;

			ShowFlag( Team.Red );
			ShowFlag( Team.Blue );
		}

		public void CleanupCTF()
		{
			// Reset Score
			RedTeamScore = 0;
			BlueTeamScore = 0;

			// Return Flags
			ReturnFlag( Team.Red );
			ReturnFlag( Team.Blue );

			// Reset Flags
			ResetFlags();
		}

		private void ShowFlag( Team team )
		{
			(Entity.All.First( e => e is InfoFlagSpawn flagSpawn && flagSpawn.Team == team ) as InfoFlagSpawn)?.ShowFlag();
		}

		private void HideFlag( Team team )
		{
			(Entity.All.First( e => e is InfoFlagSpawn flagSpawn && flagSpawn.Team == team ) as InfoFlagSpawn)?.HideFlag();
		}
	}
}
