// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;

namespace Fortwars;

    public partial class BigAmmoPickup : Pickup
    {
        [Net] int uses { get; set; } = 5;

        float ThrowSpeed = 100f;

        [Net] bool Landed { get; set; }

        public override void Spawn()
        {
            base.Spawn();

            SetModel( "models/items/medkit/medkit_w.vmdl" );
            SetBodyGroup( 0, 1 );
            SetMaterialGroup( "ammo" );
            Scale = 0.4f;

            Components.Get<BobbingComponent>().NoPitch = true;
        }

        [Event.Tick.Server]
        public void OnTick()
        {
            if ( Landed )
            {
                SetAnimParameter( "deployed", true );
                Scale = 0.4f;
                return;
            }
            Velocity += ThrowSpeed * Rotation.Forward * Time.Delta;
            Velocity += Map.Physics.Gravity * 0.5f * Time.Delta;

            Rotation = Rotation.LookAt( -Velocity.Normal.WithZ( 0 ), Vector3.Up );

            var target = Position + Velocity * Time.Delta;
            var tr = Trace.Ray( Position, target ).Ignore( Owner ).Run();

            if ( tr.Hit )
            {
                SetAnimParameter( "deployed", true );
                Landed = true;
            }

            Position = target;
        }

        public override void StartTouch( Entity other )
        {
            if ( !Landed )
            {
                return;
            }
            base.StartTouch( other );

            if ( !IsServer )
                return;

            if ( ( other as FortwarsPlayer ).ActiveChild is not FortwarsWeapon weapon )
                return;

            weapon.ReserveAmmo += weapon.WeaponAsset.MaxAmmo * 2;

            uses--;

            if ( uses <= 0 )
            {
                this.Delete();
            }
        }
    }
