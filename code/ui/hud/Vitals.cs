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
			var icon = Add.Icon( "favorite" );
			var segmentsContainer = Add.Panel( "segments" );
			Health = Add.Label( "100", "health" );
			Health.Bind( "text", () => Local.Pawn.Health.CeilToInt() );
		}
	}
}
