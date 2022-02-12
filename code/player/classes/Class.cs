using System.Collections.Generic;

namespace Fortwars
{
	public abstract class Class
	{
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual string IconPath { get; set; }
		public virtual List<string> CombatLoadout { get; set; }
		public virtual List<string> BuildLoadout { get; set; }

		public virtual void AssignBuildLoadout( Inventory inventory )
		{
			AssignLoadout( BuildLoadout, inventory );
		}

		public virtual void AssignCombatLoadout( Inventory inventory )
		{
			AssignLoadout( CombatLoadout, inventory );
		}

		private void AssignLoadout( List<string> items, Inventory inventory )
		{
			foreach ( string itemPath in items )
			{
				inventory.Add( ItemUtils.GetItem( itemPath ) );
			}
		}

		public virtual void Cleanup( Inventory inventory )
		{
			inventory.DeleteContents();
		}
	}
}
