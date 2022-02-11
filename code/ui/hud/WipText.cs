using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class WipText : Panel
	{
		public WipText()
		{
			StyleSheet.Load( "/ui/hud/WipText.scss" );
			Add.Label( "Fortwars work-in-progress", "subtitle wip" );
			Add.Image( "ui/misc/logo.png", "logo" );
		}
	}
}
