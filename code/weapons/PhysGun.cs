using Fortwars;
using Sandbox;
using Sandbox.Joints;
using System;
using System.Linq;

[Library( "physgun", Title = "Manipulator" )]
public partial class PhysGun : Carriable, IUse
{
	public override string ViewModelPath => "models/weapons/manipulator/manipulator_v.vmdl";

	protected PhysicsBody holdBody;
	protected PhysicsBody velBody;
	protected WeldJoint holdJoint;
	protected WeldJoint velJoint;

	protected PhysicsBody heldBody;
	protected Vector3 heldPos;
	protected Rotation heldRot;

	protected float holdDistance;
	protected bool grabbing;

	protected virtual float MinTargetDistance => 0.0f;
	protected virtual float MaxTargetDistance => 512.0f;
	protected virtual float LinearFrequency => 20.0f;
	protected virtual float LinearDampingRatio => 1.0f;
	protected virtual float AngularFrequency => 20.0f;
	protected virtual float AngularDampingRatio => 1.0f;
	protected virtual float TargetDistanceSpeed => 50.0f;
	protected virtual float RotateSpeed => 0.2f;
	protected virtual float RotateSnapAt => 45.0f;

	[Net] public bool BeamActive { get; set; }
	[Net] public Entity GrabbedEntity { get; set; }
	[Net] public int GrabbedBone { get; set; }
	[Net] public Vector3 GrabbedPos { get; set; }
	[Net] public bool CanGrab { get; set; }

	public PhysicsBody HeldBody => heldBody;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/manipulator/manipulator_w.vmdl" );

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	float DesiredDialPos;

	public void UpdateViewmodel()
	{
		bool wantsToFreeze = Input.Pressed( InputButton.Attack2 );
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

		if ( GrabbedEntity.IsValid() && wantsToFreeze )
		{
			ViewModelEntity?.SetAnimBool( "freeze", true );
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

	public void UpdateCanGrab( Player owner, Vector3 EyePosition, Rotation EyeRotation, Vector3 eyeDir )
	{
		if ( GrabbedEntity.IsValid() )
		{
			CanGrab = true;
			return;
		}

		var tr = Trace.Ray( EyePosition, EyePosition + eyeDir * MaxTargetDistance )
			.UseHitboxes()
			.Ignore( owner, false )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld || tr.StartedSolid )
		{
			CanGrab = false;
			return;
		}

		var rootEnt = tr.Entity.Root;
		var body = tr.Body;

		if ( !body.IsValid() || tr.Entity.Parent.IsValid() )
		{
			if ( rootEnt.IsValid() && rootEnt.PhysicsGroup != null )
			{
				body = (rootEnt.PhysicsGroup.BodyCount > 0 ? rootEnt.PhysicsGroup.GetBody( 0 ) : null);
			}
		}

		if ( !body.IsValid() )
		{
			CanGrab = false;
			return;
		}

		if ( rootEnt is not FortwarsBlock )
		{
			CanGrab = false;
			return;
		}

		if ( (rootEnt as FortwarsBlock).TeamID != (owner as FortwarsPlayer).TeamID )
		{
			CanGrab = false;
			return;
		}

		if ( rootEnt.Owner != owner )
		{
			CanGrab = false;
			return;
		}

		CanGrab = true;
	}

	public override void Simulate( Client client )
	{
		UpdateViewmodel();

		if ( Owner is not Player owner ) return;

		var EyePosition = owner.EyePosition;
		var eyeDir = owner.EyeRotation.Forward;
		var EyeRotation = Rotation.From( new Angles( 0.0f, owner.EyeRotation.Angles().yaw, 0.0f ) );

		UpdateCanGrab( owner, EyePosition, EyeRotation, eyeDir );

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

			if ( !grabbing )
				grabbing = true;
		}

		bool grabEnabled = grabbing && Input.Down( InputButton.Attack1 );
		bool wantsToFreeze = Input.Pressed( InputButton.Attack2 );

		if ( GrabbedEntity.IsValid() && wantsToFreeze )
		{
			(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );
		}

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
						UpdateGrab( EyePosition, EyeRotation, eyeDir, wantsToFreeze );
					}
					else
					{
						TryStartGrab( owner, EyePosition, EyeRotation, eyeDir );
					}
				}
				else if ( grabbing )
				{
					GrabEnd();
				}

				if ( !grabbing && Input.Pressed( InputButton.Reload ) )
				{
					TryUnfreezeAll( owner, EyePosition, EyeRotation, eyeDir );
				}
			}
		}

		if ( BeamActive )
		{
			Input.MouseWheel = 0;
		}
	}

	private static bool IsBodyGrabbed( PhysicsBody body )
	{
		// There for sure is a better way to deal with this
		if ( All.OfType<PhysGun>().Any( x => x?.HeldBody?.PhysicsGroup == body?.PhysicsGroup ) ) return true;

		return false;
	}

	private void TryUnfreezeAll( Player owner, Vector3 EyePosition, Rotation EyeRotation, Vector3 eyeDir )
	{
		var tr = Trace.Ray( EyePosition, EyePosition + eyeDir * MaxTargetDistance )
			.UseHitboxes()
			.Ignore( owner, false )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld ) return;

		var rootEnt = tr.Entity.Root;
		if ( !rootEnt.IsValid() ) return;

		var physicsGroup = rootEnt.PhysicsGroup;
		if ( physicsGroup == null ) return;

		bool unfrozen = false;

		for ( int i = 0; i < physicsGroup.BodyCount; ++i )
		{
			var body = physicsGroup.GetBody( i );
			if ( !body.IsValid() ) continue;

			if ( body.BodyType == PhysicsBodyType.Static )
			{
				body.BodyType = PhysicsBodyType.Dynamic;
				unfrozen = true;
			}
		}

		if ( unfrozen )
		{
			var freezeEffect = Particles.Create( "particles/physgun_freeze.vpcf" );
			freezeEffect.SetPosition( 0, tr.EndPos );
		}
	}

	private void TryStartGrab( Player owner, Vector3 EyePosition, Rotation EyeRotation, Vector3 eyeDir )
	{
		var tr = Trace.Ray( EyePosition, EyePosition + eyeDir * MaxTargetDistance )
			.UseHitboxes()
			.Ignore( owner, false )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld || tr.StartedSolid ) return;

		var rootEnt = tr.Entity.Root;
		var body = tr.Body;

		if ( !body.IsValid() || tr.Entity.Parent.IsValid() )
		{
			if ( rootEnt.IsValid() && rootEnt.PhysicsGroup != null )
			{
				body = (rootEnt.PhysicsGroup.BodyCount > 0 ? rootEnt.PhysicsGroup.GetBody( 0 ) : null);
			}
		}

		if ( !body.IsValid() ) return;

		if ( rootEnt is not FortwarsBlock ) return;
		if ( (rootEnt as FortwarsBlock).TeamID != (owner as FortwarsPlayer).TeamID ) return; //Gotta make sure the block is actually our team's, can't fuck with enemy blocks.
		if ( rootEnt.Owner != Owner ) return;

		// Unfreeze
		if ( body.BodyType == PhysicsBodyType.Static )
		{
			body.BodyType = PhysicsBodyType.Dynamic;
		}

		if ( IsBodyGrabbed( body ) )
			return;

		GrabInit( body, EyePosition, tr.EndPos, EyeRotation );

		GrabbedEntity = rootEnt;
		GrabbedPos = body.Transform.PointToLocal( tr.EndPos );
		GrabbedBone = tr.Entity.PhysicsGroup.GetBodyIndex( body );

		Client?.Pvs.Add( GrabbedEntity );
	}

	private void UpdateGrab( Vector3 EyePosition, Rotation EyeRotation, Vector3 eyeDir, bool wantsToFreeze )
	{
		if ( wantsToFreeze )
		{
			if ( heldBody.BodyType == PhysicsBodyType.Dynamic )
			{
				heldBody.BodyType = PhysicsBodyType.Static;
			}

			if ( GrabbedEntity.IsValid() )
			{
				var freezeEffect = Particles.Create( "particles/physgun_freeze.vpcf" );
				freezeEffect.SetPosition( 0, heldBody.Transform.PointToWorld( GrabbedPos ) );
			}
			ViewModelEntity?.SetAnimBool( "freeze", true );
			GrabEnd();
			return;
		}

		MoveTargetDistance( Input.MouseWheel * TargetDistanceSpeed );

		bool rotating = Input.Down( InputButton.Use );
		bool snapping = false;

		if ( rotating )
		{
			EnableAngularSpring( Input.Down( InputButton.Run ) ? 100.0f : 0.0f );
			DoRotate( EyeRotation, Input.MouseDelta * RotateSpeed );

			snapping = Input.Down( InputButton.Run );
		}
		else
		{
			DisableAngularSpring();
		}

		GrabMove( EyePosition, eyeDir, EyeRotation, snapping );
	}

	private void EnableAngularSpring( float scale )
	{
		if ( holdJoint.IsValid )
		{
			holdJoint.AngularDampingRatio = AngularDampingRatio * scale;
			holdJoint.AngularFrequency = AngularFrequency * scale;
		}
	}

	private void DisableAngularSpring()
	{
		if ( holdJoint.IsValid )
		{
			holdJoint.AngularDampingRatio = 0.0f;
			holdJoint.AngularFrequency = 0.0f;
		}
	}

	private void Activate()
	{
		if ( !IsServer )
			return;

		if ( !holdBody.IsValid() )
		{
			holdBody = new PhysicsBody
			{
				BodyType = PhysicsBodyType.Keyframed
			};
		}

		if ( !velBody.IsValid() )
		{
			velBody = new PhysicsBody
			{
				BodyType = PhysicsBodyType.Static,
				EnableAutoSleeping = false
			};
		}
	}

	private void Deactivate()
	{
		if ( IsServer )
		{
			GrabEnd();

			holdBody?.Remove();
			holdBody = null;

			velBody?.Remove();
			velBody = null;
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
		heldRot = rot.Inverse * heldBody.Rotation;

		holdBody.Position = grabPos;
		holdBody.Rotation = heldBody.Rotation;

		velBody.Position = grabPos;
		velBody.Rotation = heldBody.Rotation;

		heldBody.Wake();
		heldBody.EnableAutoSleeping = false;

		holdJoint = PhysicsJoint.Weld
			.From( holdBody )
			.To( heldBody, heldPos )
			.WithLinearSpring( LinearFrequency, LinearDampingRatio, 0.0f )
			.WithAngularSpring( 0.0f, 0.0f, 0.0f )
			.Create();

		velJoint = PhysicsJoint.Weld
			.From( holdBody )
			.To( velBody )
			.WithLinearSpring( LinearFrequency, LinearDampingRatio, 0.0f )
			.WithAngularSpring( 0.0f, 0.0f, 0.0f )
			.Create();
	}

	private void GrabEnd()
	{
		if ( holdJoint.IsValid )
		{
			holdJoint.Remove();
		}

		if ( velJoint.IsValid )
		{
			velJoint.Remove();
		}

		if ( heldBody.IsValid() )
		{
			heldBody.EnableAutoSleeping = true;
			heldBody.BodyType = PhysicsBodyType.Static;
		}

		Client?.Pvs.Remove( GrabbedEntity );

		heldBody = null;
		GrabbedEntity = null;
		grabbing = false;
	}

	bool StopPushing;

	private void GrabMove( Vector3 startPos, Vector3 dir, Rotation rot, bool snapAngles )
	{
		if ( !heldBody.IsValid() )
			return;

		TraceResult walltr = Trace.Ray( heldBody.Transform.PointToWorld( heldPos ), startPos + dir * holdDistance ).Ignore( heldBody.Entity ).Run();

		StopPushing = walltr.Hit;

		if ( walltr.Hit )
		{
			DebugOverlay.Line( heldBody.Transform.PointToWorld( heldPos ), startPos + dir * holdDistance, Color.Red );
		}

		if ( !StopPushing )
		{
			holdBody.Position = startPos + dir * holdDistance;
		}

		if ( GrabbedEntity is Player player )
		{
			player.Velocity = velBody.Velocity;
			player.Position = holdBody.Position - heldPos;

			var controller = player.GetActiveController();
			if ( controller != null )
			{
				controller.Velocity = velBody.Velocity;
			}

			return;
		}

		holdBody.Rotation = rot * heldRot;

		if ( snapAngles )
		{
			var angles = holdBody.Rotation.Angles();

			holdBody.Rotation = Rotation.From(
				MathF.Round( angles.pitch / RotateSnapAt ) * RotateSnapAt,
				MathF.Round( angles.yaw / RotateSnapAt ) * RotateSnapAt,
				MathF.Round( angles.roll / RotateSnapAt ) * RotateSnapAt
			);
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

	public override void BuildInput( InputBuilder owner )
	{
		if ( !GrabbedEntity.IsValid() )
			return;

		if ( !owner.Down( InputButton.Attack1 ) )
			return;

		if ( owner.Down( InputButton.Use ) )
		{
			owner.ViewAngles = owner.OriginalViewAngles;
		}
	}

	public bool OnUse( Entity user )
	{
		throw new NotImplementedException();
	}

	public bool IsUsable( Entity user )
	{
		return Owner == null || HeldBody.IsValid();
	}
}
