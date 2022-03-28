// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using Sandbox.UI;

namespace Fortwars;

    [UseTemplate]
    public class FallbackScope : Panel
    {
        public FallbackScope()
        {
            BindClass( "visible", () =>
            {
                if ( ( Local.Pawn as FortwarsPlayer ).ActiveChild is FortwarsWeapon { IsAiming: true } )
                    return true;

                return false;
            } );
        }
    }
