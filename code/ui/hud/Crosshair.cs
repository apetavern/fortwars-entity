using Fortwars;
using Sandbox;
using Sandbox.UI;

public class Crosshair : Panel
{
	public Crosshair()
	{
		StyleSheet.Load( "/ui/hud/Crosshair.scss" );

		for ( int i = 0; i < 5; i++ )
		{
			var p = Add.Panel( "element" );
			p.AddClass( $"el{i}" );
		}

		BindClass( "visible", () =>
		{
			if ( VirtualCursor.InUse )
				return false;

			return Local.Pawn.ActiveChild is not FortwarsWeapon { IsAiming: true };
		} );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn.ActiveChild is not FortwarsWeapon weapon )
			return;

		float size = 32 + weapon.GetCrosshairSize();
		Style.Width = size;
		Style.Height = size;
	}
}
