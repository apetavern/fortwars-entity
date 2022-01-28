using Sandbox;
using Sandbox.UI;

namespace Fortwars.UI
{
	[Library]
	public partial class FortwarsHUD : HudEntity<RootPanel>
	{
		public FortwarsHUD()
		{
			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
			RootPanel.StyleSheet.Load( "/ui/hud/BuildHelp.scss" );

			RootPanel.AddChild<Sandbox.UI.NameTags>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<Scoreboard>();

			RootPanel.AddChild<Sandbox.UI.ChatBox>();
			RootPanel.AddChild<Vitals>();
			RootPanel.AddChild<Ammo>();
			RootPanel.AddChild<RoundStatus>();
			RootPanel.AddChild<BuildHelp>();
			RootPanel.AddChild<BuildMenu>();
		}
	}
}
