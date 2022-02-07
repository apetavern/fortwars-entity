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

		// TODO: Link to weapon
		BindClass( "visible", () => !VirtualCursor.InUse && !Input.Down( InputButton.Attack2 ) );
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
