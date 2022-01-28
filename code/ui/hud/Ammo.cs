using FortWars;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars.UI
{
	public class Ammo : Panel
	{
		public Label Current;
		public Label Reserve;

		public Ammo()
		{
			Current = Add.Label( "0", "current" );
			Current.Bind( "text", () =>
			{
				if ( Local.Pawn.ActiveChild is not FortwarsWeapon weapon )
					return "0";
				return weapon.CurrentClip;
			} );

			Add.Label( "/", "separator" );

			Reserve = Add.Label( "0", "reserve" );
			Reserve.Bind( "text", () =>
			{
				if ( Local.Pawn.ActiveChild is not FortwarsWeapon weapon )
					return "0";
				return weapon.ReserveAmmo;
			} );
		}
	}
}
