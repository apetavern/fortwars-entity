using System.Collections.Generic;

namespace Fortwars
{
	public abstract class Class
	{
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }
		public virtual string IconPath { get; set; }
		public virtual List<string> Loadout { get; set; }

		public abstract void AssignLoadout( Inventory inventory );

		public abstract void Cleanup( Inventory inventory );
	}
}
