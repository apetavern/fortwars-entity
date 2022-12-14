// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

partial class FortwarsGame
{
	[Net] public int RedTeamScore { get; set; }
	[Net] public int BlueTeamScore { get; set; }
	[Net] public FortwarsPlayer RedFlagCarrier { get; set; }
	[Net] public FortwarsPlayer BlueFlagCarrier { get; set; }

	[Net] public BogRoll RedFlagRoll { get; set; }
	[Net] public BogRoll BlueFlagRoll { get; set; }

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
			if ( ( player.Team is BlueTeam && BlueFlagCarrier != null ) || ( player.Team is RedTeam && RedFlagCarrier != null ) )
				return;

			PlayerScoreFlag( player );
			player.ActiveChild.Delete();//Has to be the flag from the checks before this.
			return;
		}

		// The player must be in the enemy flag zone

		// Check if the enemy flag is actually here
		if ( ( player.Team is BlueTeam && RedFlagCarrier != null ) || ( player.Team is RedTeam && BlueFlagCarrier != null ) )
			return;

		// Check if the flag exists in the world
		if ( ( player.Team is BlueTeam && RedFlagRoll != null ) || ( player.Team is RedTeam && BlueFlagRoll != null ) )
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
			PlayAnnouncerSound( "announcer.your_flag_taken", Team.Red );
			PlayAnnouncerSound( "announcer.enemy_flag_taken", Team.Blue );
		}

		if ( enemyTeam is BlueTeam )
		{
			BlueFlagCarrier = player;
			BlueFlagRoll = new BogRoll();
			BlueFlagRoll.Team = Team.Blue;
			player.Inventory.Add( BlueFlagRoll, true );
			PlayAnnouncerSound( "announcer.your_flag_taken", Team.Blue );
			PlayAnnouncerSound( "announcer.enemy_flag_taken", Team.Red );
		}

		HideFlag( enemyTeam.ID );

		ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} picked up {enemyTeam.Name} flag", $"avatar:{player.Client.SteamId}", true );


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
			PlayAnnouncerSound( "announcer.your_flag_taken", Team.Red );
			PlayAnnouncerSound( "announcer.enemy_flag_taken", Team.Blue );
			RedFlagCarrier = player;
		}

		if ( enemyTeam is BlueTeam )
		{
			PlayAnnouncerSound( "announcer.your_flag_taken", Team.Blue );
			PlayAnnouncerSound( "announcer.enemy_flag_taken", Team.Red );
			BlueFlagCarrier = player;
		}

		ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} picked up {enemyTeam.Name} flag", $"avatar:{player.Client.SteamId}", true );

		player.PlaySound( "ctf_flag_pickup" );
	}

	[ClientRpc]
	public void PlayLocalSound( string sound )
	{
		PlaySound( sound );
	}

	public void PlayerScoreFlag( FortwarsPlayer player )
	{
		// Up the player's score
		player.Client.AddInt( "captures" );

		foreach ( FortwarsPlayer ply in All.OfType<FortwarsPlayer>() )
		{
			if ( ply.TeamID == player.TeamID )
			{
				PlayLocalSound( To.Single( ply.Client ), "enemyflagcaptured" );// positive sound, same team who scored
			}
			else
			{
				PlayLocalSound( To.Single( ply.Client ), "enemycapturedourflag" );// negative sound, enemy team scored
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
			PlayAnnouncerSound( "announcer.enemy_flag_captured", Team.Red );
			PlayAnnouncerSound( "announcer.your_flag_captured", Team.Blue );
			ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} scored for {RedTeam.Name}", $"avatar:{player.Client.SteamId}", true );
		}

		if ( player == RedFlagCarrier )
		{
			// Make the player drop the flag
			RedFlagCarrier = null;

			// Up the team score
			BlueTeamScore++;

			ShowFlag( Team.Red );

			// Announce
			PlayAnnouncerSound( "announcer.your_flag_captured", Team.Red );
			PlayAnnouncerSound( "announcer.enemy_flag_captured", Team.Blue );
			ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} scored for {BlueTeam.Name}", $"avatar:{player.Client.SteamId}", true );
		}
	}

	public void PlayerDropFlag( FortwarsPlayer player )
	{
		if ( player == BlueFlagCarrier )
		{
			PlayAnnouncerSound( "announcer.enemy_flag_dropped", Team.Red );
			PlayAnnouncerSound( "announcer.your_flag_dropped", Team.Blue );

			ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} dropped {BlueTeam.Name} flag", $"avatar:{player.Client.SteamId}", true );
			BlueFlagCarrier = null;
			ShowFlag( Team.Blue );
			return;
		}
		if ( player == RedFlagCarrier )
		{
			PlayAnnouncerSound( "announcer.enemy_flag_dropped", Team.Blue );
			PlayAnnouncerSound( "announcer.your_flag_dropped", Team.Red );

			ChatBox.AddInformation( To.Everyone, $"{player.Client.Name} dropped {RedTeam.Name} flag", $"avatar:{player.Client.SteamId}", true );
			RedFlagCarrier = null;
			ShowFlag( Team.Red );
			return;
		}
	}

	public void ReturnFlag( Team Team )
	{
		switch ( Team )
		{
			case Team.Blue:
				PlayAnnouncerSound( "announcer.enemy_flag_returned", Team.Red );
				PlayAnnouncerSound( "announcer.your_flag_returned", Team.Blue );

				ChatBox.AddInformation( To.Everyone, $"{BlueTeam.Name} flag returned", null, true );
				BlueFlagCarrier = null;
				ShowFlag( Team.Blue );
				return;
			case Team.Red:
				PlayAnnouncerSound( "announcer.enemy_flag_returned", Team.Blue );
				PlayAnnouncerSound( "announcer.your_flag_returned", Team.Red );

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
		var flagSpawns = All.OfType<InfoFlagSpawn>().ToList();
		flagSpawns.First( e => e.Team == team ).ShowFlag();
	}

	private void HideFlag( Team team )
	{
		var flagSpawns = All.OfType<InfoFlagSpawn>().ToList();
		flagSpawns.First( e => e.Team == team ).HideFlag();
	}
}
