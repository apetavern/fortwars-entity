// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars;

public partial class ClassMenu : Menu
{
	private ClassInfo classInfo;
	private ClassPreviewPanel previewpanel;
	private Panel classes;
	
	public ClassMenu()
	{
		AddClass( "menu" );
		StyleSheet.Load( "ui/menus/ClassMenu.scss" );

		Add.Label( "Select a class", "title" );

		var main = Add.Panel( "main" );
		classes = main.Add.Panel( "classes" );
		previewpanel = main.AddChild<ClassPreviewPanel>();
		classInfo = main.AddChild<ClassInfo>();

		var classArray = new string[]
		{
			"fwclass_assault",
			"fwclass_medic",
			"fwclass_support",
			"fwclass_engineer"
		};

		foreach ( var classId in classArray )
		{
			var classType = TypeLibrary.Create<Class>( classId );
			var classButton = classes.Add.Button( "", "class", () => SetClass( classId ) );
			var classInner = classButton.Add.Panel( "inner" );
			classInner.Add.Label( classType.Name, "name" );
			classInner.Add.Label( classType.ShortDescription, "description" );
			
			classButton.Add.Image( "ui/icons/"+ classType.Name.ToLower() + ".png", "class-icon" );

			classButton.AddEventListener( "onmouseover", () =>
			{
				classInfo.Update( classType );
				previewpanel.ShowClass( classType );
			} );
		}

		Add.Button( "Cancel", "cancel", () => Delete() );
	}

	[Event.Frame]
	public static void OnFrame()
	{
		if ( Input.Pressed( InputButton.View ) )
		{
			Toggle();
		}
	}

	public static void Toggle()
	{
		// Do we already have the class menu shown?
		var existingClassMenu = Local.Hud.Children.OfType<ClassMenu>().FirstOrDefault();
		if ( existingClassMenu != null )
		{
			existingClassMenu.Delete();
		}
		else
		{
			// No class menu, let's add one
			Local.Hud.AddChild<ClassMenu>();
		}
	}

	[ConCmd.Server( "fw_change_class" )]
	public static void ChangeClass( string classId )
	{
		var pawn = ConsoleSystem.Caller.Pawn as FortwarsPlayer;
		var classType = TypeLibrary.Create<Class>( classId );
		pawn.AssignClass( classType );
	}

	private void SetClass( string classId )
	{
		Delete();
		ChangeClass( classId );
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
					if ( item.StartsWith( "fw:" ) )
					{
						string assetName = item.Remove( 0, 3 );
						var asset = ResourceLibrary.Get<WeaponAsset>( item.Remove( 0, 3 ) );
						loadoutStr += $"• {asset.WeaponName}\n";
					}
					else
					{
						loadoutStr += $"• {TypeLibrary.GetDescription( item )?.Title}\n";
					}
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
