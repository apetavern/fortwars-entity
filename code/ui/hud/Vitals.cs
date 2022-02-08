using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class Vitals : Panel
	{
		public Label Health;

		public Vitals()
		{
			StyleSheet.Load( "/ui/hud/Vitals.scss" );

			var icon = Add.Icon( "add_box" );
			Health = Add.Label( "100", "health" );
			Health.Bind( "text", () => Local.Pawn.Health.CeilToInt() );
		}
	}
}
