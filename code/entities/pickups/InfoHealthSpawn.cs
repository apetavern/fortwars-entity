// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using SandboxEditor;

namespace Fortwars;

[Library( "info_health_spawn" )]
[Title( "Health Spawn" ), Category( "FortWars" )]
[EditorModel( "models/sbox_props/burger_box/burger_box.vmdl" )]
[HammerEntity]
public class InfoHealthSpawn : Spawner<HealthPickup> { }
