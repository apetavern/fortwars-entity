// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_assault" )]
public partial class AssaultClass : Class
{
	public override string Name => "Assault";
	public override string ShortDescription => "Go around clearing groups of enemies";
	public override string IconPath => "/ui/icons/assault.png";

	public override List<string> BuildLoadout => new();

	public override List<string> CombatLoadout => new()
	{
		"fw:data/weapons/rpg.fwweapon"
	};
	
	public override List<string> Cosmetics => new()
	{
		"models/citizen_clothes/hat/tactical_helmet/tactical_helmet_army.clothing",
		"models/citizen_clothes/vest/tactical_vest/models/tactical_vest_army.clothing"
	};

	public override string PreviewWeapon => "models/weapons/rpg/rpg_w.vmdl";

	public override HoldTypes PreviewHoldType => HoldTypes.Rifle;
}
