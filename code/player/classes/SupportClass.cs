// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_support" )]
public class SupportClass : Class
{
	public override string Name => "Support";
	public override string IconPath => "/ui/icons/support.png";
	public override string ShortDescription => "Resupply your comrades with ammo";
	public override string Gadget => "ammokittool";

	public override List<string> Cosmetics => new()
	{
		"models/citizen_clothes/hat/tactical_helmet/tactical_helmet.clothing",
		"models/citizen_clothes/vest/tactical_vest/models/tactical_vest.clothing"
	};

	public override string PreviewWeapon => "models/weapons/hksmgii/hksmgii_w.vmdl";

	public override HoldTypes PreviewHoldType => HoldTypes.Rifle;
}
