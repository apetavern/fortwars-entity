// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using SandboxEditor;

namespace Fortwars;

/// <summary>
/// Overrides game settings for this map
/// </summary>
[Library( "fw_config" )]
[Title( "Map Config" ), Category( "FortWars" )]
[EditorSprite( "materials/editor/fw_config.vmat" )]
[HammerEntity]
public partial class ConfigEntity : Entity
{
	/// <summary>
	/// Build round time in seconds
	/// </summary>
	[Property] public int BuildTime { get; set; } = 150;

	/// <summary>
	/// Combat round time in seconds
	/// </summary>
	[Property] public int CombatTime { get; set; } = 300;

	/// <summary>
	/// Time between respawns in seconds
	/// </summary>
	[Property] public int RespawnTimer { get; set; } = 10;

	/// <summary>
	/// Wood blocks per player
	/// </summary>
	[Property] public int WoodBlocksPerPlayer { get; set; } = 30; // TODO

	/// <summary>
	/// Steel blocks per player
	/// </summary>
	[Property] public int SteelBlocksPerPlayer { get; set; } = 15; // TODO

	public override void Spawn()
	{
		base.Spawn();

		Log.Trace( $"Spawned config {this}" );

		BuildRound.RoundLength = BuildTime;
		CombatRound.RoundLength = CombatTime;

		FortwarsPlayer.TimeBetweenSpawns = RespawnTimer;

		Transmit = TransmitType.Never;
	}
}
