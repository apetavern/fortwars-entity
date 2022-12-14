// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

[UseTemplate]
public class FallbackScope : Panel
{
	public FallbackScope()
	{
		BindClass( "visible", () =>
		{
			if ( ( Game.LocalPawn as FortwarsPlayer ).ActiveChild is FortwarsWeapon { IsAiming: true } )
				return true;

			return false;
		} );
	}
}
