// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

[Library]
public partial class FortwarsHUD : HudEntity<RootPanel>
{
	public static FortwarsHUD Instance { get; private set; }
	public static RootPanel Root => Instance?.RootPanel;

	public FortwarsHUD()
	{
		if ( !Game.IsClient ) return;

		Instance = this;

		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );
		RootPanel.StyleSheet.Load( "/ui/hud/BuildHelp.scss" );

		var hud = RootPanel.Add.Panel( "hud" );

		hud.AddChild<KillFeed>();
		hud.AddChild<Scoreboard>();

		hud.AddChild<Vitals>();
		hud.AddChild<Resources>();
		hud.AddChild<InventoryBar>();
		hud.AddChild<RoundStatus>();
		hud.AddChild<BuildHelp>();
		hud.AddChild<BuildWheel>();
		hud.AddChild<BuildWheelMetal>();
		hud.AddChild<Compass>();
		hud.AddChild<Hitmarker>();
		hud.AddChild<DamageIndicator>();
		hud.AddChild<MessageFeed>();
		hud.AddChild<MapVote>();

		hud.AddChild<ChatBox>();
		hud.AddChild<VoiceFeed>();

		hud.AddChild<Victory>();

		RootPanel.AddChild<Dead>();
		RootPanel.AddChild<DevMenu>();
		RootPanel.AddChild<ClassMenu>();

		hud.BindClass( "red-team", () => ( Game.LocalPawn as FortwarsPlayer ).TeamID == Team.Red );
		hud.BindClass( "blue-team", () => ( Game.LocalPawn as FortwarsPlayer ).TeamID == Team.Blue );
	}
}
