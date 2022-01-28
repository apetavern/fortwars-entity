using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Arena.UI
{
	public class DevMenu : Panel
	{
		public DevMenu()
		{
			StyleSheet.Load( "/UI/DevMenu.scss" );
			Add.Label( "Developer Menu", "title" );

			var buttons = Add.Panel( "buttons" );

			//
			// General
			//
			buttons.Add.Label( "General", "subtitle" );

			buttons.Add.ButtonWithIcon( "Spawn a bot", "smart_toy", "button", () =>
			{
				ConsoleSystem.Run( "bot_add 0 0" );
			} );

			buttons.Add.ButtonWithIcon( "Suicide", "clear", "button", () =>
			{
				ConsoleSystem.Run( "kill" );
			} );

			BindClass( "visible", () => Input.Down( InputButton.Flashlight ) );
		}
	}
}
