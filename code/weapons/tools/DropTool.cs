// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars;

[Library( "droptool", Title = "Drop tool" )]
public partial class DropTool : Carriable
{
    public virtual float PrimaryRate => 2.0f;

    public float DropTimeDelay => 1f;
    public override string ViewModelPath => "models/items/medkit/medkit_v.vmdl";

    public override void Spawn()
    {
        base.Spawn();

        SetModel( "models/items/medkit/medkit_w.vmdl" );
        Scale = 0.25f;

        TimeSinceLastDrop = DropTimeDelay;
    }

    [Net, Predicted]
    public TimeSince TimeSincePrimaryAttack { get; set; }

    [Net, Predicted]
    public TimeSince TimeSinceLastDrop { get; set; }

    [Net] bool Dropped { get; set; }

    public override void Simulate( Client player )
    {
        if ( !Owner.IsValid() )
            return;

        if ( CanPrimaryAttack() )
        {
            using ( LagCompensation() )
            {
                TimeSincePrimaryAttack = 0;
                AttackPrimary();
            }
        }

        if ( IsServer )
        {
            if ( TimeSinceLastDrop < DropTimeDelay )
            {
                Dropped = false;
            }

            if ( TimeSincePrimaryAttack > 0.4f && Dropped )
            {
                using ( LagCompensation() )
                {
                    TimeSinceLastDrop = 0;
                    DoDrop();
                }
            }
        }

        if ( IsClient )
        {
            if ( TimeSincePrimaryAttack > 0.6f )
            {
                ViewModelEntity?.SetAnimParameter( "noammo", true );
            }

            if ( TimeSinceLastDrop > DropTimeDelay )
            {
                ViewModelEntity?.SetAnimParameter( "noammo", false );
            }
        }
    }

    public virtual void SpawnPickup()
    {
    }

	protected void ThrowProjectile<T>( T projectile ) where T : Entity
	{
		// This offset is here to make it look like the projectile is being
		// thrown from the player's hands, rather than directly out of
		// their face.
		var offset = Vector3.Down * 32f;
		
		projectile.Rotation = Owner.EyeRotation;
		projectile.Position = Owner.EyePosition + offset;
		projectile.Velocity = projectile.Rotation.Forward * 256f + projectile.Rotation.Up * 256f + Owner.Velocity;

		projectile.Owner = Owner;
	}

    public void DoDrop()
    {
        if ( IsServer )
        {
            All.OfType<Pickup>().Where( e => e.Client == Client ).ToList().ForEach( e => e.Delete() );
        }

        SpawnPickup();

        Dropped = false;
    }

    public virtual bool CanPrimaryAttack()
    {
        if ( !Owner.IsValid() || !Input.Down( InputButton.PrimaryAttack ) ) return false;

        var rate = PrimaryRate;
        if ( rate <= 0 ) return true;

        return TimeSincePrimaryAttack > ( 1 / rate );
    }

    public virtual void AttackPrimary()
    {
        var player = Owner as FortwarsPlayer;
        player.SetAnimParameter( "b_attack", true );

        Dropped = true;

        ViewModelEntity?.SetAnimParameter( "fire", true );
    }

    public virtual IEnumerable<TraceResult> TraceHit( Vector3 start, Vector3 end, float radius = 2.0f )
    {
        var tr = Trace.Ray( start, end )
                .UseHitboxes()
                .WorldOnly()
				.Ignore(Owner)
				.Ignore(this)
                .Size( radius )
                .Run();

        yield return tr;
    }

    public override void ActiveStart( Entity ent )
    {
        base.ActiveStart( ent );
    }

    public override void SimulateAnimator( PawnAnimator anim )
    {
        if ( TimeSinceLastDrop < DropTimeDelay )
        {
            EnableDrawing = false;
            anim.SetAnimParameter( "holdtype", (int)HoldTypes.None );
            anim.SetAnimParameter( "holdtype_handedness", (int)HoldHandedness.TwoHands );
            anim.SetAnimParameter( "holdtype_pose_hand", 0f );
            anim.SetAnimParameter( "holdtype_attack", 1 );
        }
        else
        {
            EnableDrawing = true;
            anim.SetAnimParameter( "holdtype", (int)HoldTypes.HoldItem );
            anim.SetAnimParameter( "holdtype_handedness", (int)HoldHandedness.TwoHands );
            anim.SetAnimParameter( "holdtype_pose_hand", 0f );
            anim.SetAnimParameter( "holdtype_attack", 1 );
        }
    }
}
