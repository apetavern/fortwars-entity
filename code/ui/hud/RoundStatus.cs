// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.Collections.Generic;

namespace Fortwars;

public class RoundStatus : Panel
{
	public Panel Wins;
	public List<Panel> WinsPointPanels = new();

	public Label Time;
	public Label Phase;

	public Label BlueScore;
	public Label RedScore;

	private float lastTimeLeft;

	public RoundStatus()
	{
		StyleSheet.Load( "/ui/hud/RoundStatus.scss" );

		var RoundInfo = Add.Panel( "round-info" );
		Phase = RoundInfo.Add.Label( "Unknown", "phase" );
		Time = RoundInfo.Add.Label( "00:00", "time" );

		//
		// Scores
		//
		{
			var scores = Add.Panel( "scores" );

			//
			// Blue
			//
			BlueScore = scores.Add.Label( "0", "score blue" );
			BlueScore.Add.Icon( "warning", "warning-icon" )
				.BindClass( "visible", () => FortwarsGame.Instance.BlueFlagRoll.IsValid() );

			//
			// Red
			//
			RedScore = scores.Add.Label( "0", "score red" );
			RedScore.Add.Icon( "warning", "warning-icon" )
				.BindClass( "visible", () => FortwarsGame.Instance.RedFlagRoll.IsValid() );
		}

		var game = FortwarsGame.Instance;
		if ( game == null ) return;

		Wins = Add.Panel( "wins" );

		for ( int i = 0; i < game.BestOf; i++ )
		{
			WinsPointPanels.Add( Wins.Add.Panel( "point" ) );
		}
	}

	public override void Tick()
	{
		var game = FortwarsGame.Instance;
		if ( game == null ) return;

		var round = game.Round;
		if ( round == null ) return;

		Phase.Text = round.RoundName.ToUpper();
		Time.Text = TimeSpan.FromSeconds( Math.Floor( round.TimeLeft ) ).ToString( @"mm\:ss" );

		BlueScore.Text = $"{game.BlueTeamScore}";
		RedScore.Text = $"{game.RedTeamScore}";

		// This is kinda UI, not sure where to put this
		if ( round.TimeLeft != lastTimeLeft )
		{
			if ( round.TimeLeft < 6.0f )
				Sound.FromScreen( "ui_countdown" );
			lastTimeLeft = round.TimeLeft;
		}

		var blueFillIndex = game.BlueWins - 1;
		var redFillIndex = game.BestOf - game.RedWins;

		for ( int i = 0; i < WinsPointPanels.Count; i++ )
		{
			var panel = WinsPointPanels[i];
			panel.SetClass( "blue", i <= blueFillIndex );
			panel.SetClass( "red", redFillIndex <= i );
		}
	}
}
