namespace Fortwars.UI;

public class Hud : RootPanel
{
	public Hud()
	{
		AddChild<Vitals>();
		AddChild<GameStatus>();
	}
}
