// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

// Based on the scoreboard from Hover, located at https://github.com/Facepunch/sbox-hover
// Copyright (c) 2022 Facepunch

using System.Collections.Generic;

namespace Fortwars;

[UseTemplate]
public class Scoreboard : Panel
{
	public Dictionary<IClient, ScoreboardEntry> Rows = new();

	public Panel BlueSection { get; set; }
	public Panel RedSection { get; set; }

	public string RedTeamHeader => $"Red ({FortwarsGame.Instance.RedTeam.GetPlayerCount()})";
	public string BlueTeamHeader => $"Blue ({FortwarsGame.Instance.BlueTeam.GetPlayerCount()})";

	public override void Tick()
	{
		base.Tick();

		SetClass( "open", Input.Down( InputButton.Score ) );

		if ( !IsVisible )
			return;

		foreach ( var client in Game.Clients.Except( Rows.Keys ) )
		{
			var entry = AddClient( client );
			Rows[client] = entry;
		}

		foreach ( var client in Rows.Keys.Except( Game.Clients ) )
		{
			if ( Rows.TryGetValue( client, out var row ) )
			{
				row?.Delete();
				Rows.Remove( client );
			}
		}

		foreach ( var kv in Rows )
		{
			CheckTeamIndex( kv.Value );
		}
	}

	protected virtual ScoreboardEntry AddClient( IClient entry )
	{
		return new ScoreboardEntry()
		{
			Client = entry
		};
	}

	private string GetTimeLeftFormatted()
	{
		return TimeSpan.FromSeconds( FortwarsGame.Instance.Round.TimeLeft ).ToString( @"mm\:ss" );
	}

	private int GetFlagCaptures( Team team )
	{
		return team == Team.Blue ? FortwarsGame.Instance.BlueTeamScore : FortwarsGame.Instance.RedTeamScore;
	}

	private Panel GetTeamSection( Team team )
	{
		return team == Team.Blue ? BlueSection : RedSection;
	}

	private void CheckTeamIndex( ScoreboardEntry entry )
	{
		var team = ( entry.Client.Pawn as FortwarsPlayer )?.TeamID;
		var section = GetTeamSection( team ?? Team.Invalid );

		if ( entry.Parent != section )
		{
			entry.Parent = section;
		}
	}
}

public class ScoreboardEntry : Panel
{
	public IClient Client { get; set; }
	public Label PlayerName { get; set; }
	public Label Captures { get; set; }
	public Label Kills { get; set; }
	public Label Deaths { get; set; }
	public Label Ping { get; set; }

	private RealTimeSince TimeSinceUpdate { get; set; }

	public ScoreboardEntry()
	{
		AddClass( "entry" );

		PlayerName = Add.Label( "PlayerName", "name" );
		Captures = Add.Label( "", "captures" );
		Kills = Add.Label( "", "kills" );
		Deaths = Add.Label( "", "deaths" );
		Ping = Add.Label( "", "ping" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible )
			return;

		if ( !Client.IsValid() )
			return;

		if ( TimeSinceUpdate < 0.1f )
			return;

		TimeSinceUpdate = 0;
		UpdateData();
	}

	public virtual void UpdateData()
	{
		PlayerName.Text = Client.Name;
		Captures.Text = Client.GetInt( "captures" ).ToString();
		Kills.Text = Client.GetInt( "kills" ).ToString();
		Deaths.Text = Client.GetInt( "deaths" ).ToString();
		Ping.Text = Client.Ping.ToString();
		SetClass( "me", Client == Game.LocalClient );
	}

	public virtual void UpdateFrom( IClient client )
	{
		Client = client;
		UpdateData();
	}
}
