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
			StyleSheet.Load( "/ui/hud/WeaponInfo.scss" );

			var ammoPanel = Add.Panel( "ammopanel" );
			ammoPanel.BindClass( "visible", () => Local.Pawn.ActiveChild is FortwarsWeapon );

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

			AddChild<InventoryBar>();
		}
	}
}
