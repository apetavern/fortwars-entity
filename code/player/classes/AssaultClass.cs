using System.Collections.Generic;

namespace Fortwars
{
	public partial class AssaultClass : Class
	{
		public override string Name { get; set; } = Consts.AssaultName;
		public override string Description { get; set; } = Consts.AssaultDescription;
		public override string IconPath { get; set; } = Consts.AssaultIconPath;
		public override List<string> Loadout { get; set; } = Consts.AssaultLoadout;

		public override void AssignLoadout( Inventory inventory )
		{
			foreach ( string weaponPath in Loadout )
			{
				inventory.Add( FortwarsWeapon.FromPath( weaponPath ) );
			}
		}

		public override void Cleanup( Inventory inventory )
		{
			inventory.DeleteContents();
		}
	}
}
