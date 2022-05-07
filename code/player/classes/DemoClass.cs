// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_demo" )]
public partial class DemoClass : Class
{
	public override string Name => "Demo";
	public override string Description =>
		"Lorem ipsum dolor sit amet";
	public override string IconPath => "/textures/icons/assault.png";

	public override List<string> BuildLoadout => new();

	public override List<string> CombatLoadout => new()
	{
		"fw:data/weapons/ksr1.fwweapon",
		"fw:data/weapons/trj.fwweapon",
		"fw:data/weapons/rpg.fwweapon"
	};
}
