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

			RootPanel.AddChild<Vitals>();
			RootPanel.AddChild<WeaponInfo>();
			RootPanel.AddChild<RoundStatus>();
			RootPanel.AddChild<BuildHelp>();
			RootPanel.AddChild<BuildMenu>();
			RootPanel.AddChild<Compass>();

			RootPanel.AddChild<DevMenu>();

			RootPanel.AddChild<Sandbox.UI.ChatBox>();
			RootPanel.AddChild<VoiceList>();

			RootPanel.BindClass( "red-team", () => (Local.Pawn as FortwarsPlayer).TeamID == Team.Red );
			RootPanel.BindClass( "blue-team", () => (Local.Pawn as FortwarsPlayer).TeamID == Team.Blue );
		}
	}
}
