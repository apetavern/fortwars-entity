// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

/// <summary>
/// Defines data for each block type
/// </summary>
[Sandbox.ModelEditor.GameData( "fw_block_node" )]
public class BlockNode
{
	public string Name { get; set; }
	public int AdditionalHealth { get; set; }
}
