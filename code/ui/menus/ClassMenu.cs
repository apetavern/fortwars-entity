// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Fortwars;

public partial class ClassMenu : Menu
{
	private Panel weaponSelect;
	private ClassPreviewPanel previewpanel;
	private Panel classes;

	private Panel primaries;
	private Panel secondaries;

	private string selectedClass = "data/classes/assault.fwclass";
	private string selectedPrimary = "fw:data/weapons/ksr1.fwweapon";
	private string selectedSecondary = "fw:data/weapons/trj.fwweapon";

	public ClassMenu()
	{
		if ( Local.Pawn is FortwarsPlayer player )
		{
			selectedClass = player.SelectedClass;
			selectedPrimary = player.SelectedPrimary;
			selectedSecondary = player.SelectedSecondary;
		}

		AddClass( "menu" );
		StyleSheet.Load( "ui/menus/ClassMenu.scss" );

		Add.Label( "Loadout", "title" );

		var main = Add.Panel( "main" );
		classes = main.Add.Panel( "classes" );
		previewpanel = new ClassPreviewPanel( selectedClass ) { Parent = main };
		weaponSelect = main.AddChild<Panel>( "weapon-select" );

		classes.Add.Label( "Classes", "subtitle" );

		foreach ( var classAsset in ResourceLibrary.GetAll<ClassAsset>() )
		{
			var classButton = classes.Add.Button( "", "class", () =>
			{
				selectedClass = classAsset.ResourcePath;
				previewpanel.ShowClass( classAsset );
			} );

			classButton.SetClass( "disabled", !classAsset.IsSelectable );

			var classInner = classButton.Add.Panel( "inner" );
			classInner.Add.Label( classAsset.ClassName, "name" );
			classInner.Add.Label( classAsset.ShortDescription, "description" );

			classButton.Add.Image( classAsset.IconPath, "class-icon" );
			classButton.BindClass( "selected", () => selectedClass == classAsset.ResourcePath );
		}

		weaponSelect.Add.Label( "Weapons", "subtitle" );
		primaries = weaponSelect.Add.Panel( "weapons primaries" );
		secondaries = weaponSelect.Add.Panel( "weapons secondaries" );

		foreach ( var weaponAsset in ResourceLibrary.GetAll<WeaponAsset>() )
		{
			Button CreateButton( Panel parent, Action onClick, Func<bool> binding )
			{
				var btn = parent.Add.Button( "", onClick );

				btn.Add.Image( weaponAsset.IconPath, "weapon-icon" );
				btn.Add.Label( weaponAsset.WeaponName );
				btn.BindClass( "selected", binding );

				return btn;
			}

			if ( weaponAsset.InventorySlot == WeaponAsset.InventorySlots.Primary )
			{
				CreateButton(
					primaries,
					() => selectedPrimary = "fw:" + weaponAsset.ResourcePath,
					() => selectedPrimary == "fw:" + weaponAsset.ResourcePath
				);
			}
			else if ( weaponAsset.InventorySlot == WeaponAsset.InventorySlots.Secondary )
			{
				CreateButton(
					secondaries,
					() => selectedSecondary = "fw:" + weaponAsset.ResourcePath,
					() => selectedSecondary == "fw:" + weaponAsset.ResourcePath
				);
			}
		}

		Add.Button( "Close", "close", () => Delete() );
	}

	public override void OnDeleted()
	{
		base.OnDeleted();

		ChangeLoadout( selectedClass, selectedPrimary, selectedSecondary );
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

	[ConCmd.Server( "fw_change_loadout" )]
	public static void ChangeLoadout( string selectedClass, string selectedPrimary, string selectedSecondary )
	{
		var pawn = ConsoleSystem.Caller.Pawn as FortwarsPlayer;
		pawn.AssignLoadout( selectedClass, selectedPrimary, selectedSecondary );
	}
}
