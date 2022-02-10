using System.Collections.Generic;

namespace Fortwars
{
	public abstract class Class
	{
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual string IconPath { get; set; }
		public virtual List<string> Loadout { get; set; }

		public virtual void AssignLoadout( Inventory inventory )
		{
			foreach ( string weaponPath in Loadout )
			{
				inventory.Add( FortwarsWeapon.FromPath( weaponPath ) );
			}
		}

		public virtual void Cleanup( Inventory inventory )
		{
			inventory.DeleteContents();
		}
	}
}
