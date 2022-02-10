using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	[Library( "fwclass_medic" )]
	public class MedicClass : Class
	{
		public override string Name => "Medic";
		public override string Description =>
			"Nobody knows how to keep the team alive " +
			"like a medic! This class will help ensure your " +
			"buddies stay in tip-top shape.";
		public override string IconPath => "/textures/icons/medic.png";

		public override List<string> Loadout => new()
		{
			"data/weapons/hksmgii.fwweapon"
		};
	}
}
