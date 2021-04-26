using Sandbox;
using Sandbox.UI;

namespace Fortwars.UI
{
	[Library]
	public partial class FortwarsHUD : Hud
	{
		public FortwarsHUD()
		{
			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "/ui/Hud.scss" );

			RootPanel.AddChild<NameTags>();

			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<Vitals>();
			RootPanel.AddChild<RoundStatus>();
			RootPanel.AddChild<BuildMenu>();
		}
	}
}
