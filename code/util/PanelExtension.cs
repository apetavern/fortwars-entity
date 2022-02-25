using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	public static class PanelExtension
	{
		public static void PositionAtCrosshair( this Panel panel )
		{
			panel.PositionAtCrosshair( Local.Pawn as Sandbox.Player );
		}

		public static void PositionAtCrosshair( this Panel panel, Player player )
		{
			if ( !player.IsValid() ) return;

			var EyePosition = player.EyePosition;
			var EyeRotation = player.EyeRotation;

			var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 1000 )
							.Size( 1.0f )
							.Ignore( player )
							.UseHitboxes()
							.Run();

			panel.PositionAtWorld( tr.EndPosition );
		}

		public static void PositionAtWorld( this Panel panel, Vector3 pos )
		{
			var screenpos = pos.ToScreen();

			if ( screenpos.z < 0 )
				return;

			panel.Style.Left = Length.Fraction( screenpos.x );
			panel.Style.Top = Length.Fraction( screenpos.y );
			panel.Style.Dirty();
		}
	}
}
