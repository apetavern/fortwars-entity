using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars.UI
{
	public class Vitals : Panel
	{
		public Label Health;

		public Vitals()
		{
			StyleSheet.Load( "/ui/hud/Vitals.scss" );

			var icon = Add.Icon( "favorite" );
			Health = Add.Label( "100", "health" );
			Health.Bind( "text", () => Local.Pawn.Health.CeilToInt() );

			Add.Label( $"Team {(Local.Pawn as FortwarsPlayer).TeamID}", "team" );
		}
	}
}
