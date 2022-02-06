using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class Resources : Panel
	{
		public Resources()
		{
			var wood = new Resource( "wood" );
			wood.Parent = this;
		}

		class Resource : Panel
		{
			string name;

			public Resource( string name )
			{
				this.name = name;

				Add.Label( name, "name" );
				Add.Label( "20", "count" );
				Add.Label( "/", "separator" );
				Add.Label( "20", "max" );
			}
		}
	}
}
