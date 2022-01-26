namespace Sandbox.UI
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

			var eyePos = player.EyePos;
			var eyeRot = player.EyeRot;

			var tr = Trace.Ray( eyePos, eyePos + eyeRot.Forward * 1000 )
							.Size( 1.0f )
							.Ignore( player )
							.UseHitboxes()
							.Run();

			panel.PositionAtWorld( tr.EndPos );
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
