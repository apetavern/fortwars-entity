// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

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
		buttons.Add.ButtonWithIcon( "Spawn a bot", "smart_toy", "button", () => ConsoleSystem.Run( "bot_add 0 0" ) );
		buttons.Add.ButtonWithIcon( "Suicide", "clear", "button", () => ConsoleSystem.Run( "kill" ) );

		//
		// UI
		//
		buttons.Add.Label( "UI", "subtitle" );
		buttons.Add.ButtonWithIcon( "Show shop UI", "storefront", "button", () => FortwarsHUD.Root.AddChild<ShopMenu>() );
		buttons.Add.ButtonWithIcon( "Show class select", "sports_martial_arts", "button", () => FortwarsHUD.Root.AddChild<ClassMenu>() );
		buttons.Add.ButtonWithIcon( "Reload HUD", "refresh", "button", () => ConsoleSystem.Run( "recreatehud" ) );
		buttons.Add.ButtonWithIcon( "Show crosshair customization", "palette", "button", () => FortwarsHUD.Root.AddChild<CrosshairCustomizer>() );

		//
		// FortwarsGame
		//
		buttons.Add.Label( "FortwarsGame", "subtitle" );
		buttons.Add.ButtonWithIcon( "Skip round", "fast_forward", "button", () => ConsoleSystem.Run( "fw_round_skip" ) );
		buttons.Add.ButtonWithIcon( "Force vote round", "map", "button", () => ConsoleSystem.Run( "fw_force_voteround" ) );
		buttons.Add.ButtonWithIcon( "Extend round", "timer", "button", () => ConsoleSystem.Run( "fw_round_extend" ) );
		buttons.Add.ButtonWithIcon( "Switch teams", "loop", "button", () => ConsoleSystem.Run( "fw_team_swap" ) );
		buttons.Add.ButtonWithIcon( "Clean up blocks", "delete", "button", () => ConsoleSystem.Run( "fw_cleanup" ) );

		//
		// Weapons
		//
		buttons.Add.Label( "Weapons", "subtitle" );

		buttons.Add.ButtonWithIcon( "Give ammo", "FortwarsGamepad", "button", () => ConsoleSystem.Run( "fw_give_ammo 10000" ) );
		buttons.Add.ButtonWithIcon( "Give physgun", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give physgun" ) );
		buttons.Add.ButtonWithIcon( "Give repair tool", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give repairtool" ) );
		buttons.Add.ButtonWithIcon( "Give medkit", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give medkittool" ) );
		buttons.Add.ButtonWithIcon( "Give ammokit", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give ammokittool" ) );

		var dropdown = new DropDown( buttons );
		AddEventListener( "onopen", () =>
		{
			dropdown.Options.Clear();

			foreach ( var file in FileSystem.Mounted.FindFile( "data/", "*.fwweapon", true ) )
			{
				var asset = ResourceLibrary.Get<WeaponAsset>( "data/" + file );
				if ( asset == null )
					continue;

				dropdown.Options.Add( new Option( asset.WeaponName, file ) );
			}
		} );

		buttons.Add.ButtonWithIcon( "Give", "backpack", "button", () => ConsoleSystem.Run( $"give_weapon data/{dropdown.Value}" ) );

		BindClass( "visible", () => Input.Down( InputButton.Flashlight ) && GameX.CheatsEnabled() );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Input.Pressed( InputButton.Flashlight ) )
		{
			CreateEvent( "onopen" );
		}
	}

	[ConCmd.Admin( "spawn_weapon" )]
	public static void SpawnWeapon( string path )
	{
		var caller = ConsoleSystem.Caller;
		var player = caller.Pawn as FortwarsPlayer;

		var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 1024 ).WorldOnly().Run();

		if ( !tr.Hit )
			return;

		var weapon = FortwarsWeapon.FromPath( path );
		weapon.Position = tr.EndPosition + tr.Normal * 16;
	}

	[ConCmd.Admin( "give_weapon" )]
	public static void GiveWeapon( string path )
	{
		var caller = ConsoleSystem.Caller;
		var player = caller.Pawn as FortwarsPlayer;

		player.Inventory.Add( FortwarsWeapon.FromPath( path ), true );
	}
}
