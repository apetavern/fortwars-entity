namespace Fortwars;

public class Hud : RootPanel
{
	public Hud()
	{
		StyleSheet.Load( "/UI/Styles/Hud.scss" );
		AddChild<Health>();
		AddChild<State>();
	}
}
