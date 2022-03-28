// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

/// <summary>
/// This is where players spawn.
/// </summary>
[Library( "info_player_teamspawn" )]
[Hammer.EntityTool( "Team Spawn", "FortWars" )]
[Hammer.EditorModel( "models/citizen/citizen.vmdl" )]
public partial class InfoPlayerTeamspawn : Entity
{
	[Property]
	public Team Team { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Never;
	}
}
