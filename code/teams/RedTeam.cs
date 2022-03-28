// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

class RedTeam : BaseTeam
{
	public override Team ID => Team.Red;
	public override string Name => "Red Team";
	public override Color Color => Color.Red;
}
