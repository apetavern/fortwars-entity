// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;
public static class GameX
{
	public static bool CheatsEnabled()
	{
		// This is shit
		return ConsoleSystem.GetValue( "sv_cheats" ) == "1";
	}
}
