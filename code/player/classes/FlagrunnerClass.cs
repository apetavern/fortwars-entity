// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_flagrunner" )]
internal class FlagrunnerClass : Class
{
	public override string Name => "Flagrunner";
	public override string ShortDescription => "Scout ahead and forge new paths for your team";
	public override string IconPath => "/ui/icons/flagrunner.png";
	public override string Gadget => "fw:data/weapons/trj.fwweapon";
	public override float SpeedMultiplier => 1.25f;
	public override float JumpMultiplier => 1.25f;
	public override bool TakesFallDamage => false;

	public override List<string> Cosmetics => new()
	{
		"models/cosmetics/robotparts/robolegs.clothing"
	};

	public override string PreviewWeapon => "models/weapons/trj/trj_w.vmdl";

	public override HoldTypes PreviewHoldType => HoldTypes.Pistol;
}
