// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_mystery" )]
public class MysteryClass : Class
{
	public override string Name => "???";
	public override string IconPath => "/ui/icons/mystery.png";
	public override string ShortDescription => "???";
	public override bool Selectable => false;

	public override List<string> BuildLoadout => new() { };
	public override List<string> CombatLoadout => new() { };
	public override List<string> Cosmetics => new() { };

	public override string PreviewWeapon => "";

	public override HoldTypes PreviewHoldType => HoldTypes.Rifle;
}
