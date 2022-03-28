// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{
	public Scoreboard()
	{
		StyleSheet.Load( "/ui/Scoreboard.scss" );
	}

	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "player", "name" );
		Header.Add.Label( "kills", "kills" );
		Header.Add.Label( "deaths", "deaths" );
		Header.Add.Label( "ping", "ping" );
		Header.Add.Label( "fps", "fps" );
	}
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{
	public Label Fps;

	public ScoreboardEntry()
	{
		Fps = Add.Label( "", "fps" );
	}

	public override void UpdateFrom( Client client )
	{
		base.UpdateFrom( client );

		Fps.Text = client.GetInt( "fps", 0 ).ToString();
	}
}
