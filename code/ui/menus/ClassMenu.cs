using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

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
			var classArray = new string[]
			{
				"fwclass_assault",
				"fwclass_medic",
				"fwclass_support",
				"fwclass_engineer"
			};

			classInfo = AddChild<ClassInfo>();

			foreach ( var classId in classArray )
			{
				var classType = Library.Create<Class>( classId );
				var classButton = classes.Add.Button( classType.Name, "class", () => SetClass( classId ) );
				classButton.Add.Image( "ui/icons/placeholder.png", "class-icon" );
				classButton.Add.Label( "0 / 0", "class-count" );

				classButton.AddEventListener( "onmouseover", () =>
				{
					classInfo.Update( classType );
				} );
			}
		}

		[ServerCmd( "fw_change_class" )]
		public static void ChangeClass( string classId )
		{
			var pawn = ConsoleSystem.Caller.Pawn as FortwarsPlayer;
			var classType = Library.Create<Class>( classId );
			pawn.AssignClass( classType );
		}

		private void SetClass( string classId )
		{
			Delete();
			ConsoleSystem.Run( $"fw_change_class {classId}" );
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

			public void Update( Class classType )
			{
				name.Text = classType.Name;
				description.Text = classType.Description;

				string loadoutStr = "";

				void AddItems( List<string> items )
				{
					foreach ( var item in items )
					{
						loadoutStr += $"+ {item}\n";
					}
				}

				loadoutStr += "\nCombat phase:\n";
				AddItems( classType.CombatLoadout );

				loadoutStr += "\nBuild phase:\n";
				AddItems( classType.BuildLoadout );

				loadout.Text = loadoutStr;
			}
		}
	}
}
