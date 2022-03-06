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
			{
				if (Input.GetAnalog(InputAnalog.Look) != Vector2.Zero) //Use joystick if it's been used at all
				{
					Position = Input.GetAnalog( InputAnalog.Look ) * new Vector2( 1, -1 );
				}
				else if( Mouse.Delta.Length > 50f ) //I use this instead of the position += method because trackpads feel a bit different than mice do, feels better to just set it to the delta position.
				{
					Position = Mouse.Delta * Time.Delta * 500; //Some controllers will still use the mouse. Need to make sure they can actually use the build menu...
				}
			}
			else
			{
				Position += Mouse.Delta * Time.Delta * 500;
			}
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
