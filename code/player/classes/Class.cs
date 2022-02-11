using Sandbox;
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
			foreach ( string weaponPath in items )
			{
				if ( weaponPath.StartsWith( "fw:" ) )
				{
					inventory.Add( FortwarsWeapon.FromPath( weaponPath.Remove( 0, 3 ) ) );
				}
				else
				{
					inventory.Add( Library.Create<Carriable>( weaponPath ) );
				}
			}
		}

		public virtual void Cleanup( Inventory inventory )
		{
			inventory.DeleteContents();
		}
	}
}
