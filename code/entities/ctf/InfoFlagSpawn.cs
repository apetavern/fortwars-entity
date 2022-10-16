// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using SandboxEditor;

namespace Fortwars;

/// <summary>
/// This is where the flag spawns.
/// </summary>
[Library( "info_flag_spawn" )]
[Title( "Flag Spawn" ), Category( "FortWars" )]
[EditorModel( "models/rust_props/small_junk/toilet_paper.vmdl" )]
[HammerEntity]
public partial class InfoFlagSpawn : Entity
{
	[Property]
	public Team Team { get; set; }

	private FlagDisplay flag;

	public override void Spawn()
	{
		base.Spawn();

		flag = new FlagDisplay();
		flag.Position = Position;
		flag.Team = Team;

		// make sure our clients know where to render flags
		// todo: probably better as a ClientRpc
		Transmit = TransmitType.Never;
	}

	public void ShowFlag()
	{
		Host.AssertServer();

		Log.Trace( $"Showing {flag}" );
		flag.EnableDrawing = true;
	}

	public void HideFlag()
	{
		Host.AssertServer();

		Log.Trace( $"Hiding {flag}" );
		flag.EnableDrawing = false;
	}
}
