// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;

namespace Fortwars;

    partial class FortwarsPlayer
    {
        [Net] public Team TeamID { get; set; }
        private BaseTeam _team;

        public BaseTeam Team
        {
            get => _team;

            set
            {
                // A player must be on a valid team.
                if ( value != null && value != _team )
                {
                    _team = value;

                    // make sure our player loadouts are set
                    _team.OnPlayerSpawn( this );

                    if ( IsServer )
                    {
                        TeamID = _team.ID;

                        // You have to do this for now.
                        // Client.SetValue( "team", TeamID );
                    }
                }
            }
        }

        [ServerCmd( "fw_team_swap" )]
        public static void TeamSwapCommand()
        {
            var player = ConsoleSystem.Caller.Pawn as FortwarsPlayer;
            switch ( player.TeamID )
            {
                case Fortwars.Team.Invalid:
                case Fortwars.Team.Red:
                    player.TeamID = Fortwars.Team.Blue;
                    break;
                case Fortwars.Team.Blue:
                    player.TeamID = Fortwars.Team.Red;
                    break;
            }
            player.Respawn();
        }
    }
