using Fortwars;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.ComponentModel;

namespace Fortwars
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

			buttons.Add.ButtonWithIcon( "Show shop UI", "storefront", "button", () =>
			{
				Local.Hud.AddChild<ShopMenu>();
			} );

			buttons.Add.ButtonWithIcon( "Skip round", "fast_forward", "button", () =>
			{
				ConsoleSystem.Run( "fw_round_skip" );
			} );

			buttons.Add.Label( "Spawn Weapon", "minitext" );

			var row = buttons.Add.Panel();
			var dropdown = new DropDown( row );

			AddEventListener( "onopen", () =>
			{
				dropdown.Options.Clear();

				foreach ( var file in FileSystem.Mounted.FindFile( "data/", "*.fwweapon", true ) )
				{
					var asset = WeaponAsset.FromPath<WeaponAsset>( "data/" + file );
					if ( asset == null )
						continue;

					dropdown.Options.Add( new Option( asset.WeaponName, file ) );
				}
			} );

			row.Add.ButtonWithIcon( "Spawn", "gamepad", "button", () =>
			{
				ConsoleSystem.Run( $"spawn_weapon data/{dropdown.Value}" );
			} );

			BindClass( "visible", () => Input.Down( InputButton.Flashlight ) );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Input.Pressed( InputButton.Flashlight ) )
			{
				CreateEvent( "onopen" );
			}
		}

		[ServerCmd( "spawn_weapon" )]
		public static void SpawnWeapon( string path )
		{
			var caller = ConsoleSystem.Caller;
			var player = caller.Pawn;

			var tr = Trace.Ray( player.EyePos, player.EyePos + player.EyeRot.Forward * 1024 ).WorldOnly().Run();

			if ( !tr.Hit )
				return;

			var weapon = FortwarsWeapon.FromPath( path );
			weapon.Position = tr.EndPos + tr.Normal * 16;
		}
	}
}
