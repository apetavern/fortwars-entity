using Sandbox;
using Sandbox.Joints;
using System;
using System.Linq;

[Library( "physgun" )]
public partial class PhysGun : Carriable, IPlayerControllable, IFrameUpdate, IPlayerInput
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	protected PhysicsBody holdBody;
	protected WeldJoint holdJoint;

	protected PhysicsBody heldBody;
	protected Vector3 heldPos;
	protected Rotation heldRot;

	protected float holdDistance;
	protected bool grabbing;

	protected virtual float MinTargetDistance => 0.0f;
	protected virtual float MaxTargetDistance => 10000.0f;
	protected virtual float LinearFrequency => 20.0f;
	protected virtual float LinearDampingRatio => 1.0f;
	protected virtual float AngularFrequency => 20.0f;
	protected virtual float AngularDampingRatio => 1.0f;
	protected virtual float TargetDistanceSpeed => 50.0f;
	protected virtual float RotateSpeed => 0.2f;
	protected virtual float RotateSnapDegree => 45.0f;

	[Net] public bool BeamActive { get; set; }
	[Net] public Entity GrabbedEntity { get; set; }
	[Net] public int GrabbedBone { get; set; }
	[Net] public Vector3 GrabbedPos { get; set; }

	public PhysicsBody HeldBody => heldBody;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	public void OnPlayerControlTick( Player owner )
	{
		if ( owner == null ) return;

		var input = owner.Input;
		var eyePos = owner.EyePos;
		var eyeDir = owner.EyeRot.Forward;
		var eyeRot = Rotation.From( new Angles( 0.0f, owner.EyeAng.yaw, 0.0f ) );

		if ( !grabbing && input.Pressed( InputButton.Attack1 ) )
		{
			grabbing = true;
		}

		bool grabEnabled = grabbing && input.Down( InputButton.Attack1 );

		BeamActive = grabEnabled;

		if ( IsServer )
		{
			using ( Prediction.Off() )
			{
				if ( !holdBody.IsValid() )
					return;

				if ( grabEnabled )
				{
					if ( heldBody.IsValid() )
					{
						UpdateGrab( input, eyePos, eyeRot, eyeDir );
					}
					else
					{
						TryStartGrab( owner, eyePos, eyeRot, eyeDir );
					}
				}
				else if ( grabbing )
				{
					GrabEnd();
				}
			}
		}

		if ( BeamActive )
		{
			owner.Input.MouseWheel = 0;
		}
	}

	private static bool IsBodyGrabbed( PhysicsBody body )
	{
		// There for sure is a better way to deal with this
		if ( All.OfType<PhysGun>().Any( x => x?.HeldBody?.PhysicsGroup == body?.PhysicsGroup ) ) return true;

		return false;
	}

	private void TryStartGrab( Player owner, Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir )
	{
		var tr = Trace.Ray( eyePos, eyePos + eyeDir * MaxTargetDistance )
			.UseHitboxes()
			.Ignore( owner )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !tr.Hit || !tr.Body.IsValid() || tr.Entity.IsWorld ) return;

		var rootEnt = tr.Entity.Root;
		var body = tr.Body;

		if ( tr.Entity.Parent.IsValid() )
		{
			if ( rootEnt.IsValid() && rootEnt.PhysicsGroup != null )
			{
				body = rootEnt.PhysicsGroup.GetBody( 0 );
			}
		}

		if ( !body.IsValid() )
			return;

		//
		// Don't move keyframed 
		//
		if ( body.BodyType == PhysicsBodyType.Keyframed )
			return;

		if ( IsBodyGrabbed( body ) )
			return;

		GrabInit( body, eyePos, tr.EndPos, eyeRot );

		GrabbedEntity = rootEnt;
		GrabbedPos = body.Transform.PointToLocal( tr.EndPos );
		GrabbedBone = tr.Entity.PhysicsGroup.GetBodyIndex( body );
	}

	private void UpdateGrab( UserInput input, Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir )
	{
		MoveTargetDistance( input.MouseWheel * TargetDistanceSpeed );

		if ( input.Down( InputButton.Use ) )
		{
			EnableAngularSpring( true );
			DoRotate( eyeRot, input.MouseDelta * RotateSpeed );
		}
		else
		{
			EnableAngularSpring( false );
		}

		GrabMove( eyePos, eyeDir, eyeRot, input.Down( InputButton.Run ) );
	}

	private void EnableAngularSpring( bool enabled )
	{
		if ( holdJoint.IsValid() )
		{
			holdJoint.AngularDampingRatio = enabled ? AngularDampingRatio : 0.0f;
			holdJoint.AngularFrequency = enabled ? AngularFrequency : 0.0f;
		}
	}

	private void Activate()
	{
		if ( !IsServer )
			return;

		if ( !holdBody.IsValid() )
		{
			holdBody = PhysicsWorld.AddBody();
			holdBody.BodyType = PhysicsBodyType.Keyframed;
		}
	}

	private void Deactivate()
	{
		if ( IsServer )
		{
			GrabEnd();

			holdBody?.Remove();
			holdBody = null;
		}

		KillEffects();
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		Activate();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		Deactivate();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Deactivate();
	}

	public override void OnCarryDrop( Entity dropper )
	{
	}

	private void GrabInit( PhysicsBody body, Vector3 startPos, Vector3 grabPos, Rotation rot )
	{
		if ( !body.IsValid() )
			return;

		GrabEnd();

		grabbing = true;
		heldBody = body;
		holdDistance = Vector3.DistanceBetween( startPos, grabPos );
		holdDistance = holdDistance.Clamp( MinTargetDistance, MaxTargetDistance );
		heldPos = heldBody.Transform.PointToLocal( grabPos );
		heldRot = rot.Inverse * heldBody.Rot;

		holdBody.Pos = grabPos;
		holdBody.Rot = heldBody.Rot;

		heldBody.Wake();
		heldBody.EnableAutoSleeping = false;
		heldBody.BodyType = PhysicsBodyType.Dynamic;

		holdJoint = PhysicsJoint.Weld
			.From( holdBody )
			.To( heldBody, heldPos )
			.WithLinearSpring( LinearFrequency, LinearDampingRatio, 0.0f )
			.WithAngularSpring( 0.0f, 0.0f, 0.0f )
			.Create();
	}

	private void GrabEnd()
	{
		if ( GrabbedEntity.IsValid() )
		{
			var freezeEffect = Particles.Create( "particles/physgun_freeze.vpcf" );
			freezeEffect.SetPos( 0, heldBody.Transform.PointToWorld( GrabbedPos ) );
		}

		if ( holdJoint.IsValid() )
		{
			holdJoint.Remove();
		}

		if ( heldBody.IsValid() )
		{
			heldBody.EnableAutoSleeping = true;
			heldBody.BodyType = PhysicsBodyType.Static;
		}

		heldBody = null;
		GrabbedEntity = null;
		grabbing = false;
	}

	private void GrabMove( Vector3 startPos, Vector3 dir, Rotation rot, bool snapAngles )
	{
		if ( !heldBody.IsValid() )
			return;

		holdBody.Pos = startPos + dir * holdDistance;
		holdBody.Rot = rot * heldRot;

		if (snapAngles)
        {
			var angles = holdBody.Rot.Angles();
			angles.yaw = MathF.Round(angles.yaw / RotateSnapDegree) * RotateSnapDegree;
			angles.pitch = MathF.Round(angles.pitch / RotateSnapDegree) * RotateSnapDegree;
			angles.roll = MathF.Round(angles.roll / RotateSnapDegree) * RotateSnapDegree;

			holdBody.Rot = Rotation.From(angles);
		}
	}

	private void MoveTargetDistance( float distance )
	{
		holdDistance += distance;
		holdDistance = holdDistance.Clamp( MinTargetDistance, MaxTargetDistance );
	}

	protected virtual void DoRotate( Rotation eye, Vector3 input )
	{
		var localRot = eye;
		localRot *= Rotation.FromAxis( Vector3.Up, input.x );
		localRot *= Rotation.FromAxis( Vector3.Right, input.y );
		localRot = eye.Inverse * localRot;

		heldRot = localRot * heldRot;
	}

	public void BuildInput( ClientInput owner )
	{
		if ( !GrabbedEntity.IsValid() )
			return;

		if ( !owner.Down( InputButton.Attack1 ) )
			return;

		if ( owner.Down( InputButton.Use ) )
		{
			owner.ViewAngles = owner.LastViewAngles;
		}
	}
}
