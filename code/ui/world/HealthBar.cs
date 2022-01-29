using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class HealthBar : WorldPanel
	{
		private Label label;
		private Panel inner;

		private FortwarsBlock block;

		internal HealthBar( FortwarsBlock block )
		{
			this.block = block;

			StyleSheet.Load( "/ui/world/HealthBar.scss" );
			label = Add.Label( "0" );
			inner = Add.Panel( "inner" );

			SceneObject.ZBufferMode = ZBufferMode.None;
			PanelBounds = new Rect( -500, -75, 1000, 150 );
		}

		public override void Tick()
		{
			base.Tick();

			label.Text = $"{block.Health.CeilToInt()} / {block.MaxHealth.CeilToInt()}";
			inner.Style.Width = Length.Fraction( block.Health / block.MaxHealth );

			Scale = 2.0f;
			WorldScale = 0.5f;
		}
	}
}
