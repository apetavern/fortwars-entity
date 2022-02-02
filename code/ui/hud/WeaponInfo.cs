using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars.UI
{
	public class WeaponInfo : Panel
	{
		public Label Current;
		public Label Reserve;

		public WeaponInfo()
		{
			var ammoPanel = Add.Panel( "ammopanel" );
			StyleSheet.Load( "/ui/hud/WeaponInfo.scss" );

			Current = ammoPanel.Add.Label( "0", "current" );
			Current.Bind( "text", () =>
			{
				if ( Local.Pawn.ActiveChild is not FortwarsWeapon weapon )
					return "0";
				return weapon.CurrentClip;
			} );

			Reserve = ammoPanel.Add.Label( "0", "reserve" );
			Reserve.Bind( "text", () =>
			{
				if ( Local.Pawn.ActiveChild is not FortwarsWeapon weapon )
					return "0";
				return weapon.ReserveAmmo;
			} );

			ammoPanel.BindClass( "visible", () => Local.Pawn.ActiveChild is FortwarsWeapon );

			AddChild<InventoryBar>();
		}
	}
}
