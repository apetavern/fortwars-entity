using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

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
			buttons.Add.ButtonWithIcon( "Spawn a bot", "smart_toy", "button", () => ConsoleSystem.Run( "bot_add 0 0" ) );
			buttons.Add.ButtonWithIcon( "Suicide", "clear", "button", () => ConsoleSystem.Run( "kill" ) );

			//
			// UI
			//
			buttons.Add.Label( "UI", "subtitle" );
			buttons.Add.ButtonWithIcon( "Show shop UI", "storefront", "button", () => Local.Hud.AddChild<ShopMenu>() );
			buttons.Add.ButtonWithIcon( "Show class select", "sports_martial_arts", "button", () => Local.Hud.AddChild<ClassMenu>() );
			buttons.Add.ButtonWithIcon( "Reload HUD", "refresh", "button", () => ConsoleSystem.Run( "recreatehud" ) );
			buttons.Add.ButtonWithIcon( "Show crosshair customization", "palette", "button", () => Local.Hud.AddChild<CrosshairCustomizer>() );

			//
			// Game
			//
			buttons.Add.Label( "Game", "subtitle" );
			buttons.Add.ButtonWithIcon( "Skip round", "fast_forward", "button", () => ConsoleSystem.Run( "fw_round_skip" ) );
			buttons.Add.ButtonWithIcon( "Extend round", "timer", "button", () => ConsoleSystem.Run( "fw_round_extend" ) );
			buttons.Add.ButtonWithIcon( "Switch teams", "loop", "button", () => ConsoleSystem.Run( "fw_team_swap" ) );

			//
			// Weapons
			//
			buttons.Add.Label( "Weapons", "subtitle" );
			buttons.Add.ButtonWithIcon( "Give ammo", "gamepad", "button", () => ConsoleSystem.Run( "fw_give_ammo 10000" ) );
			buttons.Add.ButtonWithIcon( "Give physgun", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give physgun" ) );
			buttons.Add.ButtonWithIcon( "Give repair tool", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give repairtool" ) );
			buttons.Add.ButtonWithIcon( "Give medkit", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give medkittool" ) );
			buttons.Add.ButtonWithIcon( "Give ammokit", "add", "button", () => ConsoleSystem.Run( "fw_inventory_give ammokittool" ) );

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

			row.Add.ButtonWithIcon( "Spawn", "gamepad", "button", () => ConsoleSystem.Run( $"spawn_weapon data/{dropdown.Value}" ) );

			row.Add.ButtonWithIcon( "Give", "backpack", "button", () => ConsoleSystem.Run( $"give_weapon data/{dropdown.Value}" ) );

			BindClass( "visible", () => Input.Down( InputButton.Flashlight ) && Global.CheatsEnabled() );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Input.Pressed( InputButton.Flashlight ) )
			{
				CreateEvent( "onopen" );
			}
		}

		[AdminCmd( "spawn_weapon" )]
		public static void SpawnWeapon( string path )
		{
			var caller = ConsoleSystem.Caller;
			var player = caller.Pawn;

			var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 1024 ).WorldOnly().Run();

			if ( !tr.Hit )
				return;

			var weapon = FortwarsWeapon.FromPath( path );
			weapon.Position = tr.EndPos + tr.Normal * 16;
		}

		[AdminCmd( "give_weapon" )]
		public static void GiveWeapon( string path )
		{
			var caller = ConsoleSystem.Caller;
			var player = caller.Pawn;

			player.Inventory.Add( FortwarsWeapon.FromPath( path ), true );
		}
	}
}
