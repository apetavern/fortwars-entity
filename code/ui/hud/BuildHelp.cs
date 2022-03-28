// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox.UI;

namespace Fortwars;

public class BuildHelp : Panel
{
	public BuildHelp()
	{
		Add.InputHint( Sandbox.InputButton.Attack1, Sandbox.InputButton.Attack1, "Move" );
		Add.InputHint( Sandbox.InputButton.Use, Sandbox.InputButton.Attack2, "Rotate" );
		Add.InputHint( Sandbox.InputButton.Run, Sandbox.InputButton.SlotPrev, "Snap Rotation" );
		Add.InputHint( Sandbox.InputButton.Menu, Sandbox.InputButton.Slot4, "Build Menu" );
		Add.InputHint( Sandbox.InputButton.Reload, Sandbox.InputButton.Reload, "Delete Block" );

		BindClass( "visible", () => Game.Instance.Round is BuildRound );
	}
}
