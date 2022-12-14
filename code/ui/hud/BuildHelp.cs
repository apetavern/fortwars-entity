// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class BuildHelp : Panel
{
	public BuildHelp()
	{
		Add.InputHint( Sandbox.InputButton.PrimaryAttack, Sandbox.InputButton.PrimaryAttack, "Move" );
		Add.InputHint( Sandbox.InputButton.Use, Sandbox.InputButton.SecondaryAttack, "Rotate" );
		Add.InputHint( Sandbox.InputButton.Run, Sandbox.InputButton.SlotPrev, "Snap Rotation" );
		Add.InputHint( Sandbox.InputButton.Menu, Sandbox.InputButton.Slot4, "Build Menu" );
		Add.InputHint( Sandbox.InputButton.Reload, Sandbox.InputButton.Reload, "Delete Block" );

		BindClass( "visible", () => FortwarsGame.Instance.Round is BuildRound );
	}
}
