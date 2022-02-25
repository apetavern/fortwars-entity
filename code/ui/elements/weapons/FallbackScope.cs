using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	[UseTemplate]
	public class FallbackScope : Panel
	{
		public FallbackScope()
		{
			BindClass( "visible", () =>
			{
				if ( (Local.Pawn as FortwarsPlayer).ActiveChild is FortwarsWeapon { IsAiming: true } )
					return true;

				return false;
			} );
		}
	}
}
