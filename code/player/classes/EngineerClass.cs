// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_engineer" )]
public class EngineerClass : Class
{
	public override string Name => "Engineer";
	public override string Description =>
		"The engineer is the king of defense. Fortify " +
		"and repair to victory with your enhanced tools " +
		"and gigantic brain!";
	public override string IconPath => "/textures/icons/engineer.png";

	public override List<string> BuildLoadout => new()
	{
		"repairtool"
	};

	public override List<string> CombatLoadout => new()
	{
		"fw:data/weapons/aiax50.fwweapon",
		"fw:data/weapons/trj.fwweapon",
		"repairtool"
	};
}
