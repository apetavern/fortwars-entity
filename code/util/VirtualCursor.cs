using Sandbox;

namespace Fortwars
{
	public static class VirtualCursor
	{
		public static Vector2 Position { get; set; }
		public static bool InUse { get; set; }
		public static bool UsingController => Input.UsingController;

		public static void Reset( Vector2? newPosition = null )
		{
			Position = newPosition ?? Vector2.Zero;
		}

		public static void Update()
		{
			if ( Input.UsingController )
			{
				Position = Input.GetAnalog( InputAnalog.Look ) * new Vector2( 1, -1 );
			}
			else
			{
				Position += Mouse.Delta;
			}
		}

		[Event.Frame]
		public static void OnFrame()
		{
			if ( !InUse )
				return;

			DebugOverlay.ScreenText( (Screen.Size * 0.5f) + Position, Position.ToString(), Time.Delta );

			Update();

			InUse = false;
		}
	}
}
