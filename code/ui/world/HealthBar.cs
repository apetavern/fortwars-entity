using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class HealthBar : Panel
	{
		private Label owner;
		private Label health;
		private Panel inner;

		private FortwarsBlock block;

		public HealthBar( FortwarsBlock block )
		{
			this.Parent = Local.Hud;
			this.block = block;

			StyleSheet.Load( "/ui/world/HealthBar.scss" );
			health = Add.Label( "0", "health" );
			owner = Add.Label( "owner", "owner" );
			inner = Add.Panel( "inner" );
		}

		public override void Tick()
		{
			base.Tick();

			health.Text = $"{block.Health.CeilToInt()} / {block.MaxHealth.CeilToInt()}";
			inner.Style.Width = Length.Fraction( block.Health / block.MaxHealth );
			owner.Text = $"Owned by {block.Client?.Name ?? "(disconnected)"}";
		}
	}
}
