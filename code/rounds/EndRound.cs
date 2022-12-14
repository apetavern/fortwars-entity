// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public partial class EndRound : BaseRound
{
	public override string RoundName => "FortwarsGame Over";
	public override int RoundDuration => 8;

	protected override void OnStart()
	{
		Log.Info( "Started End Round" );
	}

	protected override void OnTimeUp()
	{
		FortwarsGame.Instance.WinningTeam = Team.Invalid;
		FortwarsGame.Instance.ChangeRound( new VoteRound() );
	}
}
