using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	[Library( "fwclass_support" )]
	public class SupportClass : Class
	{
		public override string Name => "Support";
		public override string Description =>
			"Specialization is for suckers! With support, " +
			"you can help your team in as many creative ways " +
			"as you can think of.";
		public override string IconPath => "/textures/icons/support.png";

		public override List<string> Loadout => new()
		{
			"data/weapons/aiax50.fwweapon"
		};
	}
}
