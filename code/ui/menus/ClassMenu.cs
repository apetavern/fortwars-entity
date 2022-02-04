using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public class ClassMenu : Menu
	{
		public ClassMenu()
		{
			StyleSheet.Load( "ui/menus/ClassMenu.scss" );
			AddClass( "menu" );

			Add.Label( "Select a class", "title" );
			Add.Label( "Lorem ipsum dolor sit amet", "subtitle" );

			var classes = Add.Panel( "classes" );
			foreach ( var classType in Fortwars.Class.Classes )
			{
				var classButton = classes.Add.ButtonWithIcon( classType.Name, classType.Icon, "class", () => Delete() );
				classButton.Add.Label( "0 / 0", "class-count" );
			}
		}
	}
}
