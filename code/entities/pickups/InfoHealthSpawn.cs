// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;

namespace Fortwars
{
    [Library( "info_health_spawn" )]
    [Hammer.EntityTool( "Health Spawn", "FortWars" )]
    [Hammer.EditorModel( "models/sbox_props/burger_box/burger_box.vmdl" )]
    public class InfoHealthSpawn : Spawner<HealthPickup> { }
}
