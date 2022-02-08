using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class Resources : Panel
	{
		public Resources()
		{
			StyleSheet.Load( "/ui/hud/Resources.scss" );

			AddResource( "wood" );
			AddResource( "steel" );
			AddResource( "feathers" );
			AddResource( "rubber" );
		}

		private void AddResource( string name )
		{
			var resource = new Resource( name );
			resource.Parent = this;
		}

		class Resource : Panel
		{
			string name;

			Label count;
			Label limit;

			public Resource( string name )
			{
				this.name = name;

				Add.Label( name, "name" );
				count = Add.Label( "20", "count" );
				Add.Label( "/", "separator" );
				limit = Add.Label( "20", "limit" );
			}

			public override void Tick()
			{
				base.Tick();
				limit.Text = Game.Instance.WoodPerPlayer.ToString();
			}
		}
	}
}
