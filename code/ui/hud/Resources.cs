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

			Add.Label( "Resources", "subtitle" );

			AddResource( "/ui/icons/resources/wood.png", BlockMaterial.Wood );
			AddResource( "/ui/icons/resources/steel.png", BlockMaterial.Steel );
			AddResource( "/ui/icons/resources/rubber.png", BlockMaterial.Rubber );
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

			public Resource( string iconPath, BlockMaterial resourceType )
			{
				this.resourceType = resourceType;

				Add.Image( iconPath, "icon" );
				count = Add.Label( "20", "count" );
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
