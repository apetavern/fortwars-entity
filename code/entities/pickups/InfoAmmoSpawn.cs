// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using SandboxEditor;

namespace Fortwars;

[Library( "info_ammo_spawn" )]
[Title( "Ammo Spawn" ), Category( "FortWars" )]
[EditorModel( "models/rust_props/small_junk/carton_box.vmdl" )]
[HammerEntity]
public class InfoAmmoSpawn : Spawner<AmmoPickup> { }
