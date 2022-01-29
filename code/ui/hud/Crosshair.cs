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
	}

	public override void Tick()
	{
		base.Tick();
		this.PositionAtCrosshair();

		if ( Local.Pawn.ActiveChild is not FortwarsWeapon weapon )
			return;

		float size = 64 + weapon.GetCrosshairSize();
		Style.Width = size;
		Style.Height = size;
	}
}
