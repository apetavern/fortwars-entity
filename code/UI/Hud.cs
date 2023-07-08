namespace Fortwars.UI;

public class Hud : RootPanel
{
	public Hud()
	{
		StyleSheet.Load( "/UI/Styles/Hud.scss" );
		AddChild<Vitals>();
		AddChild<State>();
	}
}
