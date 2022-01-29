﻿using Fortwars;
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

			buttons.Add.ButtonWithIcon( "Reload HUD", "refresh", "button", () =>
			{
				ConsoleSystem.Run( "recreatehud" );
			} );

			//
			// Game
			//
			buttons.Add.Label( "Game", "subtitle" );

			buttons.Add.ButtonWithIcon( "Give tons of ammo", "gamepad", "button", () =>
			{
				ConsoleSystem.Run( "give_ammo 10000" );
			} );

			buttons.Add.Label( "Spawn Weapon", "minitext" );

			var row = buttons.Add.Panel();
			var dropdown = new DropDown( row );
			foreach ( var file in FileSystem.Mounted.FindFile( "data/", "*.fwweapon", true ) )
			{
				Log.Trace( file );
				var asset = WeaponAsset.FromPath<WeaponAsset>( "data/" + file );
				dropdown.Options.Add( new Option( asset.WeaponName, file ) );
			}

			row.Add.ButtonWithIcon( "Spawn", "gamepad", "button", () =>
			{
				ConsoleSystem.Run( $"spawn_weapon data/{dropdown.Value}" );
			} );

			BindClass( "visible", () => Input.Down( InputButton.Flashlight ) );
		}

		[ServerCmd( "spawn_weapon" )]
		public static void SpawnWeapon( string path )
		{
			var caller = ConsoleSystem.Caller;
			var player = caller.Pawn;

			var tr = Trace.Ray( player.EyePos, player.EyePos + player.EyeRot.Forward * 1024 ).WorldOnly().Run();

			if ( !tr.Hit )
				return;

			Log.Trace( $"Spawning {path}" );
			var weapon = FortwarsWeapon.FromPath( path );
			weapon.Position = tr.EndPos + tr.Normal * 16;
		}
	}
}