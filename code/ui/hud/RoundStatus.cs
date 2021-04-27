
using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars.UI
{
	public class RoundStatus : Panel
	{
		public Label Time;
		public Label Phase;

		public Label BlueScore;
		public Label RedScore;

		private float lastTimeLeft;

		public RoundStatus()
		{
			BlueScore = Add.Label( "0", "score" );
			var RoundInfo = Add.Panel( "round-info" );
			Time = RoundInfo.Add.Label( "00:00", "time" );
			Phase = RoundInfo.Add.Label( "Unknown", "phase" );
			RedScore = Add.Label( "0", "score" );

			// feature request: add multiple classes via constructor
			BlueScore.AddClass( "score__blue" );
			RedScore.AddClass( "score__red" );
		}

		public override void Tick()
		{
			var game = Game.Instance;
			if ( game == null ) return;

			var round = game.Round;
			if ( round == null ) return;

			Phase.Text = round.RoundName.ToUpper();
			Time.Text = TimeSpan.FromSeconds( Math.Floor(round.TimeLeft) ).ToString( @"mm\:ss" );

			BlueScore.Text = $"{game.BlueTeamScore}";
			RedScore.Text = $"{game.RedTeamScore}";

			// This is kinda UI, not sure where to put this
			if (round.TimeLeft != lastTimeLeft)
			{
				if (round.TimeLeft < 6.0f)
					Sound.FromScreen( "ui_countdown" );
				lastTimeLeft = round.TimeLeft;
			}
		}
	}
}
