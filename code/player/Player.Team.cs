// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

partial class FortwarsPlayer
{
	[Net] public Team TeamID { get; set; }
	private BaseTeam _team;

	public BaseTeam Team
	{
		get => _team;

		set
		{
			// A player must be on a valid team.
			if ( value != null && value != _team )
			{
				_team = value;

				// make sure our player loadouts are set
				_team.OnPlayerSpawn( this );

				if ( IsServer )
				{
					TeamID = _team.ID;
					// Remove opposite team tag (for team swaps)
					Tags.Remove( _team.ID == Fortwars.Team.Red ? "blueteam" : "redteam" );
					// Add team tag
					Tags.Add( _team.ID == Fortwars.Team.Red ? "redteam" : "blueteam" );
				}
			}
		}
	}

	[ConCmd.Server( "fw_team_swap" )]
	public static void TeamSwapCommand()
	{
		var player = ConsoleSystem.Caller.Pawn as FortwarsPlayer;
		switch ( player.TeamID )
		{
			case Fortwars.Team.Invalid:
			case Fortwars.Team.Red:
				player.TeamID = Fortwars.Team.Blue;
				break;
			case Fortwars.Team.Blue:
				player.TeamID = Fortwars.Team.Red;
				break;
		}
		player.Respawn();
	}
}
