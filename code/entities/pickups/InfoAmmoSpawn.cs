// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;

namespace Fortwars
{
    [Library( "info_ammo_spawn" )]
    [Hammer.EntityTool( "Ammo Spawn", "FortWars" )]
    [Hammer.EditorModel( "models/rust_props/small_junk/carton_box.vmdl" )]
    public class InfoAmmoSpawn : Spawner<AmmoPickup> { }
}
