using Sandbox;

namespace Fortwars
{
	public static class VirtualCursor
	{
		public static Vector2 Position { get; set; }

		/// <summary>
		/// Mark this every time you use the cursor, otherwise the player's view won't lock
		/// </summary>
		public static bool InUse { get; set; }

		[ClientVar( "fw_virtualcursor_debug" )]
		public static bool Debug { get; set; } = false;

		public delegate void ClickEvent();
		public static ClickEvent OnClick { get; set; }

		public static void Reset( Vector2? newPosition = null )
		{
			Position = newPosition ?? Vector2.Zero;
		}

		public static void Update()
		{
			if ( Input.UsingController )
				Position = Input.GetAnalog( InputAnalog.Look ) * new Vector2( 1, -1 );
			else
				Position += Mouse.Delta * Time.Delta * 500;
		}

		[Event.Frame]
		public static void OnFrame()
		{
			if ( !InUse )
				return;

			if ( Debug )
				DebugOverlay.ScreenText( (Screen.Size * 0.5f) + Position, Position.ToString(), Time.Delta );

			Update();

			InUse = false;
		}
	}
}
