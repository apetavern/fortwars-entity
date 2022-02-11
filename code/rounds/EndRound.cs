namespace Fortwars
{
	public partial class EndRound : BaseRound
	{
		public override string RoundName => "Game Over";
		public override int RoundDuration => 15;

		protected override void OnStart()
		{
			Log.Info( "Started End Round" );
		}

		protected override void OnTimeUp()
		{
			// Eventually, we will map vote so resetting this will be redundant.
			Game.Instance.BlueWins = 0;
			Game.Instance.RedWins = 0;
			Game.Instance.ChangeRound( new BuildRound() );
		}
	}
}
