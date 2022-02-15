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

	public void UpdateViewmodel()
	{
		bool rotating = Input.Down( InputButton.Use );

		ViewModelEntity?.SetAnimBool( "fire", BeamActive );

		if ( GrabbedEntity.IsValid() && rotating )
		{
			ViewModelEntity?.SetAnimFloat( "joystickFB", MathX.LerpTo( ViewModelEntity.GetAnimFloat( "joystickFB" ), -(Input.MouseDelta.y * RotateSpeed), Time.Delta ) );
			ViewModelEntity?.SetAnimFloat( "joystickLR", MathX.LerpTo( ViewModelEntity.GetAnimFloat( "joystickLR" ), (Input.MouseDelta.x * RotateSpeed), Time.Delta ) );
			ViewModelEntity?.SetAnimBool( "snap", Input.Down( InputButton.Run ) );
		}
		else
		{
			ViewModelEntity?.SetAnimFloat( "joystickFB", MathX.LerpTo( ViewModelEntity.GetAnimFloat( "joystickFB" ), 0f, 0.1f ) );
			ViewModelEntity?.SetAnimFloat( "joystickLR", MathX.LerpTo( ViewModelEntity.GetAnimFloat( "joystickLR" ), 0f, 0.1f ) );
			ViewModelEntity?.SetAnimBool( "snap", false );
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
				DesiredDialPos += Rand.Float() * 0.15f * (1 + (Input.MouseDelta.Length / 20f));
			}
		}
		else
		{
			DesiredDialPos = 0f;
		}

		ViewModelEntity?.SetAnimBool( "cangrab", CanGrab );

		ViewModelEntity?.SetAnimFloat( "dialpos", MathX.LerpTo( ViewModelEntity.GetAnimFloat( "dialpos" ), DesiredDialPos, 0.5f ) );
	}

	protected virtual void UpdateEffects()
	{
		var owner = Owner;

		if ( owner == null || !BeamActive || !IsActiveChild() )
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
			Beam = Particles.Create( "particles/physgun_beam.vpcf", tr.EndPos );
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
			lastBeamPos = tr.EndPos;// Vector3.Lerp( lastBeamPos, tr.EndPos, Time.Delta * 10 );
			Beam.SetPosition( 1, lastBeamPos );

			if ( EndNoHit == null )
				EndNoHit = Particles.Create( "particles/physgun_end_nohit.vpcf", lastBeamPos );

			EndNoHit.SetPosition( 0, lastBeamPos );
		}
	}
}
