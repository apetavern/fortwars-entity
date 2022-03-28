// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars;

    public partial class FortwarsPlayer
    {
        public override void BuildInput( InputBuilder input )
        {
            base.BuildInput( input );

            if ( VirtualCursor.InUse )
            {
                // This is a bit of a shit way of doing things but nothing else
                // I tried seemed to work. pointer-events: auto; did nothing,
                // capturing the event manually here seems to be the best way of
                // solving things right now
                //
                // We could probably put in a feature request at some point
                // or even move this to VirtualCursor itself somewhere just to make
                // things a bit more modular?
                if ( input.Pressed( InputButton.Attack1 ) )
                {
                    VirtualCursor.OnClick?.Invoke();
                }
                input.ClearButton( InputButton.Attack1 );

                input.StopProcessing = true;
                input.ViewAngles = input.OriginalViewAngles;
            }

            if ( input.UsingController )
            {
                AimAssistInput( input, this );
            }
        }

        private struct PlayerAimAssistEntry
        {
            public PlayerAimAssistEntry( float distance, float angularDistance, Player player )
            {
                Distance = distance;
                AngularDistance = angularDistance;
                Player = player;
            }

            public float Distance { get; set; }
            public float AngularDistance { get; set; }
            public Player Player { get; set; }
        }

        [ServerVar( "fw_aimassist_distance_min", Min = 64, Max = 512 )]
        public static float AimAssistDistanceMin { get; set; } = 64f;

        [ServerVar( "fw_aimassist_distance_max", Min = 512, Max = 2048 )]
        public static float AimAssistDistanceMax { get; set; } = 512f;

        [ServerVar( "fw_aimassist_angle_min", Min = 0, Max = 15 )]
        public static float AimAssistAngleMin { get; set; } = 0f;

        [ServerVar( "fw_aimassist_angle_max", Min = 30, Max = 60 )]
        public static float AimAssistAngleMax { get; set; } = 30f;

        [ServerVar( "fw_aimassist_strength", Min = 1, Max = 10 )]
        public static float AimAssistStrength { get; set; } = 2.5f;

        public static void AimAssistInput( InputBuilder input, Player owner )
        {
            List<PlayerAimAssistEntry> players = new();
            foreach ( Player player in Entity.All.Where( x => x is FortwarsPlayer ) )
            {
                if ( player != owner )
                {
                    if ( player.Health <= 0 ) continue;

                    var centerOfMass = player.Position + Vector3.Up * 36f;

                    var tr = Trace.Ray( owner.EyePosition, centerOfMass ).Ignore( owner ).Ignore( player ).WorldOnly().Run();

                    if ( tr.Hit )
                        continue;

                    var aimDir = centerOfMass - owner.EyePosition;
                    var angDist = ( aimDir.EulerAngles.Normal - owner.EyeRotation.Angles().Normal ).Length;

                    players.Add( new PlayerAimAssistEntry( aimDir.Length, angDist, player ) );
                }
            }

            if ( players.Count > 0 )
            {
                players.Sort( ( a, b ) => a.Distance * a.AngularDistance > b.Distance * b.AngularDistance ? 1 : -1 );

                var dist = players[0].Distance;
                var angDist = players[0].AngularDistance;
                var target = players[0].Player;

                var centerOfMass = target.Position + Vector3.Up * 36f;
                var delta = centerOfMass - owner.EyePosition;

                var diff = input.ViewAngles - delta.EulerAngles;
                float distT = dist.LerpInverse( AimAssistDistanceMax, AimAssistDistanceMin );
                float t = ( ( input.AnalogLook.Length * 2 ) + ( input.AnalogMove.Length * 0.5f ) ) * angDist.LerpInverse( AimAssistAngleMax * distT, 0 ) * distT;

                var targetAngles = delta.EulerAngles;
                targetAngles = Angles.Lerp( input.ViewAngles, targetAngles, AimAssistStrength * Time.Delta * t * 4 );
                input.ViewAngles = input.ViewAngles.WithYaw( targetAngles.yaw );
            }
        }
    }
