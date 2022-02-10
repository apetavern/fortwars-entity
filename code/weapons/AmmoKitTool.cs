using Sandbox;
using System.Collections.Generic;

namespace Fortwars
{
	[Library( "ammokittool", Title = "Ammokit" )]
	public partial class AmmoKitTool : DropTool
	{
		public override void Spawn()
		{
			base.Spawn();

			IsAmmo = true;
			ViewModelEntity?.SetMaterialGroup( "ammo" );
			SetMaterialGroup( "ammo" );
		}
	}
}
