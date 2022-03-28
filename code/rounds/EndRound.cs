// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public partial class EndRound : BaseRound
{
	public override string RoundName => "Game Over";
	public override int RoundDuration => 8;

	protected override void OnStart()
	{
		Log.Info( "Started End Round" );
	}

	protected override void OnTimeUp()
	{
		Game.Instance.WinningTeam = Team.Invalid;
		Game.Instance.ChangeRound( new VoteRound() );
	}
}
