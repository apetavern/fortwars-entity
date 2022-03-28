// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars;

    public partial class Victory : Panel
    {
        public Label VictoryLabel { get; set; }

        public Victory()
        {
            StyleSheet.Load( "/ui/hud/Victory.scss" );
            VictoryLabel = Add.Label( "Some Team Won!", "win-label" );
        }

        public override void Tick()
        {
            var game = Game.Instance;
            if ( game == null ) return;

            var winningTeam = game.WinningTeam;

            VictoryLabel.Text = $"{winningTeam} Wins!";

            SetClass( "show", winningTeam != Team.Invalid );
            VictoryLabel.SetClass( "red", winningTeam == Team.Red );
            VictoryLabel.SetClass( "blue", winningTeam == Team.Blue );
        }
    }
