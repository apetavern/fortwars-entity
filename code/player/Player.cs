// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;

namespace Fortwars
{
    public partial class FortwarsPlayer : Sandbox.Player
    {
        [Net] public string Killer { get; set; }

        public DamageInfo LastDamage { get; private set; }
        public Clothing.Container Clothing = new();

        [ServerVar( "fw_time_between_spawns", Help = "How long do players need to wait between respawns", Min = 1, Max = 30 )]
        public static int TimeBetweenSpawns { get; set; } = 10;

        public bool IsSpectator
        {
            get => Team == null;
        }

        public FortwarsPlayer()
        {
            Inventory = new Inventory( this );
        }

        public FortwarsPlayer( Client cl ) : this()
        {
            // Load clothing from client data
            Clothing.LoadFromClient( cl );
        }

        public override void Respawn()
        {
            // assign random team
            if ( Team == null )
            {
                int team = Client.All.Count % 2;
                if ( team == 0 )
                    Team = Game.Instance.BlueTeam;
                else
                    Team = Game.Instance.RedTeam;

                // ChatBox.AddInformation( To.Everyone, $"{Name} has joined {Team.Name}", $"avatar:{Client.PlayerId}" );
            }

            SetModel( "models/citizen/citizen.vmdl" );

            // Allow Team class to dress the player
            if ( Team != null )
            {
                Team.OnPlayerSpawn( this );
            }

            if ( IsSpectator )
            {
                EnableAllCollisions = false;
                EnableDrawing = false;

                Controller = null;
                CameraMode = new SpectateRagdollCamera();

                base.Respawn();

                return;
            }

            Controller = new FortwarsWalkController();
            Animator = new StandardPlayerAnimator();
            CameraMode = new FirstPersonCamera();

            EnableAllCollisions = true;
            EnableDrawing = true;

            // Draw clothes etc
            foreach ( var child in Children )
                child.EnableDrawing = true;

            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = true;

            Clothing.DressEntity( this );

            Game.Instance.Round.SetupInventory( this );

            InSpawnRoom = true;

            base.Respawn();
        }

        public override void OnKilled()
        {
            BecomeRagdollOnClient( Position,
                         Rotation,
                         Velocity,
                         LastDamage.Flags,
                         LastDamage.Position,
                         LastDamage.Force,
                         GetHitboxBone( LastDamage.HitboxIndex ) );

            base.OnKilled();
            RespawnTimer = TimeBetweenSpawns;

            Inventory.DropActive();

            //
            // Delete any items we didn't drop
            //
            Inventory.DeleteContents();

            Controller = null;
            CameraMode = new SpectateRagdollCamera();

            EnableAllCollisions = false;
            EnableDrawing = false;

            // Don't draw clothes etc
            foreach ( var child in Children )
                child.EnableDrawing = false;
        }

        [Net] public TimeUntil RespawnTimer { get; set; }

        public override void Simulate( Client cl )
        {
            if ( LifeState == LifeState.Dead )
            {
                if ( RespawnTimer <= 0 && IsServer )
                {
                    Respawn();
                }

                return;
            }


            // HACK: remove this when https://github.com/Facepunch/sbox-issues/issues/1641 gets fixed
            if ( CameraMode is SpectateRagdollCamera )
                CameraMode = new FirstPersonCamera();

            var controller = GetActiveController();
            controller?.Simulate( cl, this, GetActiveAnimator() );

            if ( Input.ActiveChild != null )
            {
                ActiveChild = Input.ActiveChild;
            }

            SimulateActiveChild( cl, ActiveChild );

            if ( LifeState != LifeState.Alive )
                return;

            TickPlayerUse();
        }

        protected override void TickPlayerUse()
        {
            // This is serverside only
            if ( !Host.IsServer ) return;

            // Turn prediction off
            using ( Prediction.Off() )
            {
                if ( Input.Pressed( InputButton.Use ) )
                {
                    Using = FindUsable();

                    if ( Using == null )
                    {
                        if ( ActiveChild is not PhysGun )
                            UseFail();
                        return;
                    }
                }

                if ( !Input.Down( InputButton.Use ) )
                {
                    StopUsing();
                    return;
                }

                if ( !Using.IsValid() )
                    return;

                // If we move too far away or something we should probably ClearUse()?

                //
                // If use returns true then we can keep using it
                //
                if ( Using is IUse use && use.OnUse( this ) )
                    return;

                StopUsing();
            }
        }

        public void Reset()
        {
            Host.AssertServer();

            Health = 100;
            Game.Instance.MoveToSpawnpoint( this );
        }

        public override void TakeDamage( DamageInfo info )
        {
            LastDamage = info;

            if ( (HitboxIndex)info.HitboxIndex == HitboxIndex.Head )
                info.Damage *= 2.0f;

            if ( info.Attacker is FortwarsPlayer attacker && attacker != this )
            {
                if ( attacker.TeamID == this.TeamID )
                    return; // No team damage

                Killer = attacker.Client.Name;

                // Note - sending this only to the attacker!
                attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, ( (float)Health ).LerpInverse( 100, 0 ) );
            }

            base.TakeDamage( info );

            if ( info.Weapon.IsValid() || info.Attacker.IsValid() )
                TookDamage( To.Single( Client ), info.Weapon.IsValid() ? info.Weapon.Position : info.Position );
        }

        [ClientRpc]
        public void DidDamage( Vector3 pos, float amount, float healthinv )
        {
            Sound.FromScreen( "dm.ui_attacker" )
                .SetPitch( 1 + healthinv * 1 );

            Hitmarker.Instance.OnHit( amount, false );
        }

        [ClientRpc]
        public void TookDamage( Vector3 pos )
        {
            DamageIndicator.Current?.OnHit( pos );
        }
    }
}
