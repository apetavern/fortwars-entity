namespace Fortwars;

public class PlayerCamera
{
	public virtual void Update( Player player )
	{
		Camera.Position = player.EyePosition;
		Camera.Rotation = player.EyeRotation;
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		Camera.FirstPersonViewer = player;
		Camera.ZNear = 0.5f;

		Camera.Main.SetViewModelCamera( 50 );
	}
}
