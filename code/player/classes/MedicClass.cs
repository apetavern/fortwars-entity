// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[Library( "fwclass_medic" )]
public class MedicClass : Class
{
	public override string Name => "Medic";
	public override string IconPath => "/ui/icons/medic.png";
	public override string ShortDescription => "Heal your buddies and stop them dying";
	public override string Gadget => "medkittool";

	public override List<string> Cosmetics => new()
	{
		"data/labcoat.clothing",
		"models/cosmetics/doctor/headmirror.clothing"
	};

	public override string PreviewWeapon => "models/items/medkit/medkit_w/medkit_preview.vmdl";

	public override HoldTypes PreviewHoldType => HoldTypes.HoldItem;
}
