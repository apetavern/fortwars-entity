// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_engineer" )]
public class EngineerClass : Class
{
	public override string Name => "Engineer";
	public override string IconPath => "/ui/icons/engineer.png";
	public override string ShortDescription => "Fortify and repair broken structures";

	public override List<string> BuildLoadout => new()
	{
		"repairtool"
	};

	public override List<string> CombatLoadout => new()
	{
		"repairtool"
	};

	public override List<string> Cosmetics => new()
	{
		"models/citizen_clothes/hat/hardhat.yellow.clothing",
		"models/citizen_clothes/shirt/tanktop/tanktop.clothing"
	};

	public override string PreviewWeapon => "models/weapons/amhammer/amhammer_w.vmdl";

	public override HoldTypes PreviewHoldType => HoldTypes.HoldItem;

	public override HoldHandedness PreviewHoldHandedness => HoldHandedness.RightHand;

	public override float PreviewHandpose => 0.07f;
}
