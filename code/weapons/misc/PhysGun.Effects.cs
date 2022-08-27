// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Fortwars;
using Sandbox;
using Sandbox.Component;
using System.Linq;

public partial class PhysGun
{
    Particles Beam;
    Particles EndNoHit;

    Vector3 lastBeamPos;
    ModelEntity lastGrabbedEntity;

    [Event.Frame]
    public void OnFrame()
    {
        UpdateEffects();
    }

    protected virtual void KillEffects()
    {
        Beam?.Destroy( true );
        Beam = null;
        BeamActive = false;

        EndNoHit?.Destroy( false );
        EndNoHit = null;

        if ( lastGrabbedEntity.IsValid() )
        {
            foreach ( var child in lastGrabbedEntity.Children.OfType<ModelEntity>() )
            {
                if ( child is Player )
                    continue;

                if ( child.Components.TryGet<Glow>( out var childglow ) )
                {
                    childglow.Active = false;
                }
            }

            if ( lastGrabbedEntity.Components.TryGet<Glow>( out var glow ) )
            {
                glow.Active = false;
            }

            lastGrabbedEntity = null;
        }
    }

    float DesiredDialPos;

    bool DidFreeze;

    public void UpdateViewmodel()
    {
        bool rotating = Input.Down( InputButton.Use );

        ViewModelEntity?.SetAnimParameter( "fire", BeamActive );

		if ( Parent is not null )
		{
			(Parent as AnimatedEntity).SetAnimParameter( "useleftik", GetAttachment( "lhand_ik" ).HasValue );

			if ( GetAttachment( "lhand_ik" ).HasValue )
			{
				Transform attachment = GetAttachment( "lhand_ik" ).Value;
				( Parent as AnimatedEntity ).SetAnimParameter( "left_hand_ik.position", attachment.Position + Parent.Velocity * Time.Delta );
				( Parent as AnimatedEntity ).SetAnimParameter( "left_hand_ik.rotation", attachment.Rotation );
			}

			( Parent as AnimatedEntity ).SetAnimParameter( "gunup", 0f );
			( Parent as AnimatedEntity ).SetAnimParameter( "gundown", 1f );
		}

		if ( GrabbedEntity.IsValid() && rotating )
        {
            ViewModelEntity?.SetAnimParameter( "joystickFB", MathX.LerpTo( ViewModelEntity.GetAnimParameterFloat( "joystickFB" ), -( Input.MouseDelta.y * RotateSpeed ), Time.Delta ) );
            ViewModelEntity?.SetAnimParameter( "joystickLR", MathX.LerpTo( ViewModelEntity.GetAnimParameterFloat( "joystickLR" ), Input.MouseDelta.x * RotateSpeed, Time.Delta ) );
            ViewModelEntity?.SetAnimParameter( "snap", Input.Down( InputButton.Run ) );
        }
        else
        {
            ViewModelEntity?.SetAnimParameter( "joystickFB", MathX.LerpTo( ViewModelEntity.GetAnimParameterFloat( "joystickFB" ), 0f, 0.1f ) );
            ViewModelEntity?.SetAnimParameter( "joystickLR", MathX.LerpTo( ViewModelEntity.GetAnimParameterFloat( "joystickLR" ), 0f, 0.1f ) );
            ViewModelEntity?.SetAnimParameter( "snap", false );
        }

        if ( GrabbedEntity.IsValid() )
        {
            DesiredDialPos = 0.5f;
            if ( !rotating )
            {
                DesiredDialPos += Rand.Float() * 0.1f;
            }
            else
            {
                DesiredDialPos += Rand.Float() * 0.15f * ( 1 + ( Input.MouseDelta.Length / 20f ) );
            }

            DidFreeze = false;
        }
        else
        {
            if ( !Input.Down( InputButton.PrimaryAttack ) && !DidFreeze )
            {
                ViewModelEntity?.SetAnimParameter( "freeze", true );
                DidFreeze = true;
            }
            DesiredDialPos = 0f;
        }

        ViewModelEntity?.SetAnimParameter( "cangrab", CanGrab );

        ViewModelEntity?.SetAnimParameter( "dialpos", MathX.LerpTo( ViewModelEntity.GetAnimParameterFloat( "dialpos" ), DesiredDialPos, 0.5f ) );
    }

    protected virtual void UpdateEffects()
    {
        var owner = Owner;

        if ( owner == null || !BeamActive || !( ( owner as FortwarsPlayer ).ActiveChild == this ) )
        {
            KillEffects();
            return;
        }

        var startPos = owner.EyePosition;
        var dir = owner.EyeRotation.Forward;

        var tr = Trace.Ray( startPos, startPos + dir * MaxTargetDistance )
            .UseHitboxes()
            .Ignore( owner, false )
            .HitLayer( CollisionLayer.Debris )
            .Run();

        if ( Beam == null )
        {
            Beam = Particles.Create( "particles/physgun_beam.vpcf", tr.EndPosition );
        }

        Beam.SetEntityAttachment( 0, EffectEntity, "muzzle", true );

        if ( GrabbedEntity.IsValid() && !GrabbedEntity.IsWorld )
        {
            var physGroup = GrabbedEntity.PhysicsGroup;

            if ( physGroup != null && GrabbedBone >= 0 )
            {
                var physBody = physGroup.GetBody( GrabbedBone );
                if ( physBody != null )
                {
                    Beam.SetPosition( 1, physBody.Transform.PointToWorld( GrabbedPos ) );
                }
            }
            else
            {
                Beam.SetEntity( 1, GrabbedEntity, GrabbedPos, true );
            }

            lastBeamPos = GrabbedEntity.Position + GrabbedEntity.Rotation * GrabbedPos;

            EndNoHit?.Destroy( false );
            EndNoHit = null;

            if ( GrabbedEntity is ModelEntity modelEnt )
            {
                lastGrabbedEntity = modelEnt;

                var glow = modelEnt.Components.GetOrCreate<Glow>();
                glow.Active = true;
                glow.RangeMin = 0;
                glow.RangeMax = 1000;
                glow.Color = new Color( 0.1f, 1.0f, 1.0f, 1.0f );

                foreach ( var child in lastGrabbedEntity.Children.OfType<ModelEntity>() )
                {
                    if ( child is Player )
                        continue;

                    glow = child.Components.GetOrCreate<Glow>();
                    glow.Active = true;
                    glow.RangeMin = 0;
                    glow.RangeMax = 1000;
                    glow.Color = new Color( 0.1f, 1.0f, 1.0f, 1.0f );
                }
            }
        }
        else
        {
            lastBeamPos = tr.EndPosition; // Vector3.Lerp( lastBeamPos, tr.EndPos, Time.Delta * 10 );
            Beam.SetPosition( 1, lastBeamPos );

            if ( EndNoHit == null )
                EndNoHit = Particles.Create( "particles/physgun_end_nohit.vpcf", lastBeamPos );

            EndNoHit.SetPosition( 0, lastBeamPos );
        }
    }
}
