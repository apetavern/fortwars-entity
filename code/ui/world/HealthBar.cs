using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class HealthBar : WorldPanel
	{
		private Label owner;
		private Label health;
		private Panel inner;

		private FortwarsBlock block;

		internal HealthBar( FortwarsBlock block )
		{
			this.block = block;

			StyleSheet.Load( "/ui/world/HealthBar.scss" );
			health = Add.Label( "0", "health" );
			owner = Add.Label( "owner", "owner" );
			inner = Add.Panel( "inner" );

			// SceneObject.ZBufferMode = ZBufferMode.None;
			PanelBounds = new Rect( -500, -75, 1000, 150 );
		}

		public override void Tick()
		{
			base.Tick();

			health.Text = $"{block.Health.CeilToInt()} / {block.MaxHealth.CeilToInt()}";
			inner.Style.Width = Length.Fraction( block.Health / block.MaxHealth );
			owner.Text = $"Owned by {block.Client?.Name ?? "(disconnected)"}";

			Scale = 2.0f;
			WorldScale = 0.5f;
		}
	}
}
