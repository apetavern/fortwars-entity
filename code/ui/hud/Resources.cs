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

			AddResource( "wood", BlockMaterial.Wood );
			AddResource( "steel", BlockMaterial.Steel );
			AddResource( "rubber", BlockMaterial.Rubber );
		}

		private void AddResource( string name, BlockMaterial resourceType )
		{
			var resource = new Resource( name, resourceType );
			resource.Parent = this;
		}

		class Resource : Panel
		{
			private Label count;
			private Label limit;

			private BlockMaterial resourceType;

			public Resource( string name, BlockMaterial resourceType )
			{
				this.resourceType = resourceType;

				Add.Label( name, "name" );
				count = Add.Label( "20", "count" );
				Add.Label( "/", "separator" );
				limit = Add.Label( "20", "limit" );
			}

			public override void Tick()
			{
				base.Tick();

				count.Text = resourceType.GetRemainingCount( Local.Client ).ToString();
				limit.Text = resourceType.PlayerLimit.ToString();
			}
		}
	}
}
