using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	[Library( "fwclass_assault" )]
	public partial class AssaultClass : Class
	{
		public override string Name => "Assault";
		public override string Description =>
			"With high damage and a strong resolve, " +
			"the Assault class will help your team " +
			"clear enemies with relative ease.";
		public override string IconPath => "/textures/icons/assault.png";

		public override List<string> BuildLoadout => new();

		public override List<string> CombatLoadout => new()
		{
			"fw:data/weapons/uts15.fwweapon",
			"fw:data/weapons/trj.fwweapon",
			"fw:data/weapons/rpg.fwweapon"
		};
	}
}
