using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	public partial class ClassMenu : Menu
	{
		ClassInfo classInfo;
		public ClassMenu()
		{
			StyleSheet.Load( "ui/menus/ClassMenu.scss" );
			AddClass( "menu" );

			Add.Label( "Select a class", "title" );

			var main = Add.Panel( "main" );
			main.AddChild<ClassPreviewPanel>();

			var classes = Add.Panel( "classes" );
			var classArray = new Class[]
			{
				new AssaultClass(),
				new MedicClass(),
				new SupportClass(),
				new EngineerClass()
			};

			classInfo = AddChild<ClassInfo>();

			foreach ( var classType in classArray )
			{
				var classButton = classes.Add.Button( classType.Name, "class", () => Delete() );
				classButton.Add.Image( "ui/icons/placeholder.png", "class-icon" );
				classButton.Add.Label( "0 / 0", "class-count" );

				classButton.AddEventListener( "onmouseover", () =>
				{
					classInfo.Update( classType );
				} );
			}
		}

		class ClassInfo : Panel
		{
			Label name;
			Label description;
			Label loadout;

			public ClassInfo()
			{
				name = Add.Label( "Class Name", "subtitle" );
				description = Add.Label( "Class Description" );
				loadout = Add.Label( "Class Loadout" );
			}

			public void Update( Class @class )
			{
				name.Text = @class.Name;
				description.Text = @class.Description;
			}
		}
	}
}
