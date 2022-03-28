// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;

namespace Fortwars;

[Library]
public partial class FortwarsHUD : HudEntity<RootPanel>
{
	public FortwarsHUD()
	{
		if ( !IsClient ) return;

		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
		RootPanel.StyleSheet.Load( "/ui/hud/BuildHelp.scss" );

		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Scoreboard>();

		RootPanel.AddChild<Vitals>();
		RootPanel.AddChild<Resources>();
		RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<RoundStatus>();
		RootPanel.AddChild<BuildHelp>();
		RootPanel.AddChild<BuildWheel>();
		RootPanel.AddChild<BuildWheelMetal>();
		RootPanel.AddChild<Compass>();
		RootPanel.AddChild<WipText>();
		RootPanel.AddChild<Hitmarker>();
		RootPanel.AddChild<DamageIndicator>();
		RootPanel.AddChild<MessageFeed>();
		RootPanel.AddChild<MapVote>();

		RootPanel.AddChild<ClassMenu>();
		RootPanel.AddChild<Victory>();

		RootPanel.AddChild<Dead>();

		RootPanel.AddChild<DevMenu>();

		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();

		RootPanel.BindClass( "red-team", () => ( Local.Pawn as FortwarsPlayer ).TeamID == Team.Red );
		RootPanel.BindClass( "blue-team", () => ( Local.Pawn as FortwarsPlayer ).TeamID == Team.Blue );
	}
}
