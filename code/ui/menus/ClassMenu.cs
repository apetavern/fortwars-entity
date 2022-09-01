// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars;

public partial class ClassMenu : Menu
{
	private Panel weaponSelect;
	private ClassPreviewPanel previewpanel;
	private Panel classes;

	private Panel primaries;
	private Panel secondaries;

	// TODO: fetch these from player
	private string selectedClass = "fwclass_assault";
	private string selectedPrimary = "fw:data/weapons/ksr1.fwweapon";
	private string selectedSecondary = "fw:data/weapons/trj.fwweapon";

	public ClassMenu()
	{
		AddClass( "menu" );
		StyleSheet.Load( "ui/menus/ClassMenu.scss" );

		Add.Label( "Loadout", "title" );

		var main = Add.Panel( "main" );
		classes = main.Add.Panel( "classes" );
		previewpanel = main.AddChild<ClassPreviewPanel>();
		weaponSelect = main.AddChild<Panel>( "weapon-select" );

		classes.Add.Label( "Classes", "subtitle" );

		var classArray = new string[]
		{
			"fwclass_assault",
			"fwclass_medic",
			"fwclass_support",
			"fwclass_engineer",
			"fwclass_mystery"
		};

		foreach ( var classId in classArray )
		{
			var classType = TypeLibrary.Create<Class>( classId );
			var classButton = classes.Add.Button( "", "class", () =>
			{
				selectedClass = classId;
				previewpanel.ShowClass( classType );
			} );

			classButton.SetClass( "disabled", !classType.Selectable );
			
			var classInner = classButton.Add.Panel( "inner" );
			classInner.Add.Label( classType.Name, "name" );
			classInner.Add.Label( classType.ShortDescription, "description" );

			classButton.Add.Image( classType.IconPath, "class-icon" );

			classButton.BindClass( "selected", () => selectedClass == classId );
		}

		weaponSelect.Add.Label( "Weapons", "subtitle" );
		primaries = weaponSelect.Add.Panel( "weapons primaries" );
		secondaries = weaponSelect.Add.Panel( "weapons secondaries" );

		foreach ( var file in FileSystem.Mounted.FindFile( "data/", "*.fwweapon", true ) )
		{
			var asset = ResourceLibrary.Get<WeaponAsset>( "data/" + file );
			if ( asset == null )
				continue;

			Log.Trace( $"{asset.WeaponName}, {asset.InventorySlot}" );

			Button CreateButton( Panel parent, Action onClick, Func<bool> binding )
			{
				var btn = parent.Add.Button( "", onClick );

				btn.Add.Image( asset.IconPath, "weapon-icon" );
				btn.Add.Label( asset.WeaponName );
				btn.BindClass( "selected", binding );

				return btn;
			}

			if ( asset.InventorySlot == WeaponAsset.InventorySlots.Primary )
			{
				CreateButton(
					primaries,
					() => selectedPrimary = "fw:" + file,
					() => selectedPrimary == "fw:" + file
				);
			}
			else if ( asset.InventorySlot == WeaponAsset.InventorySlots.Secondary )
			{
				CreateButton(
					secondaries,
					() => selectedSecondary = "fw:" + file,
					() => selectedSecondary == "fw:" + file
				);
			}

		}

		Add.Button( "Close", "close", () => Delete() );
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
}
