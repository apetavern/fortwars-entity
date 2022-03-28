// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

[Library( "info_health_spawn" )]
[Hammer.EntityTool( "Health Spawn", "FortWars" )]
[Hammer.EditorModel( "models/sbox_props/burger_box/burger_box.vmdl" )]
public class InfoHealthSpawn : Spawner<HealthPickup> { }
