
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

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
			StyleSheet.Load( "/ui/hud/RoundStatus.scss" );

			var scores = Add.Panel( "scores" );
			BlueScore = scores.Add.Label( "0", "score blue" );
			RedScore = scores.Add.Label( "0", "score red" );

			// scores.Add.Label( "Red", "red team" );
			// scores.Add.Label( "Blue", "blue team" );

			var RoundInfo = Add.Panel( "round-info" );
			Phase = RoundInfo.Add.Label( "Unknown", "phase" );
			Time = RoundInfo.Add.Label( "00:00", "time" );
		}

		public override void Tick()
		{
			var game = Game.Instance;
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
		}
	}
}
