﻿using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	[Library( "fwclass_engineer" )]
	public class EngineerClass : Class
	{
		public override string Name => "Engineer";
		public override string Description =>
			"The engineer is the king of defense. Fortify " +
			"and repair to victory with your enhanced tools " +
			"and gigantic brain!";
		public override string IconPath => "/textures/icons/engineer.png";

		public override List<string> Loadout => new()
		{
			"data/weapons/trj.fwweapon"
		};
	}
}