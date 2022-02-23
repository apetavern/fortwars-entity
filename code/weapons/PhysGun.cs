using Fortwars;
using Sandbox;
using System;
using System.Linq;

[Library( "physgun", Title = "Manipulator" )]
public partial class PhysGun : Carriable, IUse
{
	public override string ViewModelPath => "models/weapons/manipulator/manipulator_v.vmdl";

	protected PhysicsBody holdBody;
	protected PhysicsBody velBody;
	protected FixedJoint holdJoint;
	protected FixedJoint velJoint;

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

	public bool CheckCanGrab( Player owner, Vector3 EyePosition, Rotation EyeRotation, Vector3 eyeDir )
	{
		if ( GrabbedEntity.IsValid() )
			return true;

		var tr = Trace.Ray( EyePosition, EyePosition + eyeDir * MaxTargetDistance )
			.UseHitboxes()
			.Ignore( owner, false )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld || tr.StartedSolid )
			return false;

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
			return false;

		if ( rootEnt is not FortwarsBlock )
			return false;

		if ( (rootEnt as FortwarsBlock).TeamID != (owner as FortwarsPlayer).TeamID )
			return false;

		if ( rootEnt.Owner != owner )
			return false;

		return true;
	}

	public override void Simulate( Client client )
	{
		UpdateViewmodel();

		if ( Owner is not Player owner ) return;

		var EyePosition = owner.EyePosition;
		var eyeDir = owner.EyeRotation.Forward;
		var EyeRotation = Rotation.From( new Angles( 0.0f, owner.EyeRotation.Angles().yaw, 0.0f ) );

		CanGrab = CheckCanGrab( owner, EyePosition, EyeRotation, eyeDir );

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

			if ( !grabbing )
				grabbing = true;
		}

		bool grabEnabled = grabbing && Input.Down( InputButton.Attack1 );

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
						UpdateGrab( EyePosition, EyeRotation, eyeDir );
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
			}
		}

		if ( BeamActive )
		{
			Input.MouseWheel = 0;
		}
	}

	private static bool IsBodyGrabbed( PhysicsBody body ) => All.OfType<PhysGun>().Any( x => x?.HeldBody?.PhysicsGroup == body?.PhysicsGroup );

	private void TryStartGrab( Player owner, Vector3 EyePosition, Rotation EyeRotation, Vector3 eyeDir )
	{
		if ( !CanGrab ) return;

		var tr = Trace.Ray( EyePosition, EyePosition + eyeDir * MaxTargetDistance )
			.UseHitboxes()
			.Ignore( owner, false )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		var rootEnt = tr.Entity.Root;
		var body = tr.Body;

		// Unfreeze
		if ( body.BodyType == PhysicsBodyType.Static )
		{
			body.BodyType = PhysicsBodyType.Dynamic;
		}

		if ( IsBodyGrabbed( body ) )
			return;

		GrabInit( body, EyePosition, tr.EndPosition, EyeRotation );

		GrabbedEntity = rootEnt;
		GrabbedPos = body.Transform.PointToLocal( tr.EndPosition );
		GrabbedBone = body.GroupIndex;

		Client?.Pvs.Add( GrabbedEntity );
	}

	private void UpdateGrab( Vector3 EyePosition, Rotation EyeRotation, Vector3 eyeDir )
	{
		MoveTargetDistance( Input.MouseWheel * TargetDistanceSpeed );

		bool rotating = Input.Down( InputButton.Use );
		bool snapping = false;

		if ( rotating )
		{
			DoRotate( EyeRotation, Input.MouseDelta * RotateSpeed );
			snapping = Input.Down( InputButton.Run );
		}

		GrabMove( EyePosition, eyeDir, EyeRotation, snapping );
	}

	private void Activate()
	{
		if ( !IsServer )
			return;

		if ( !holdBody.IsValid() )
		{
			holdBody = new PhysicsBody( Map.Physics )
			{
				BodyType = PhysicsBodyType.Keyframed
			};
		}

		if ( !velBody.IsValid() )
		{
			velBody = new PhysicsBody( Map.Physics )
			{
				BodyType = PhysicsBodyType.Static,
				AutoSleep = false
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

	private void GrabInit( PhysicsBody body, Vector3 startPos, Vector3 grabPos, Rotation rot )
	{
		if ( !body.IsValid() )
			return;

		GrabEnd();

		grabbing = true;
		heldBody = body;
		holdDistance = Vector3.DistanceBetween( startPos, grabPos );
		holdDistance = holdDistance.Clamp( MinTargetDistance, MaxTargetDistance );
		heldRot = rot.Inverse * heldBody.Rotation;

		holdBody.Position = grabPos;
		holdBody.Rotation = heldBody.Rotation;

		velBody.Position = grabPos;
		velBody.Rotation = heldBody.Rotation;

		heldBody.Sleeping = false;
		heldBody.AutoSleep = false;

		holdJoint = PhysicsJoint.CreateFixed( new PhysicsPoint( holdBody ), new PhysicsPoint( heldBody, heldPos ) );
		holdJoint.SpringLinear = new PhysicsSpring( LinearFrequency, LinearDampingRatio );
		holdJoint.SpringAngular = new PhysicsSpring( AngularFrequency, AngularDampingRatio );

		velJoint = PhysicsJoint.CreateFixed( new PhysicsPoint( holdBody ), new PhysicsPoint( velBody ) );
		velJoint.SpringLinear = new PhysicsSpring( LinearFrequency, LinearDampingRatio );
		velJoint.SpringAngular = new PhysicsSpring( AngularFrequency, AngularDampingRatio );
	}

	private void GrabEnd()
	{
		holdJoint?.Remove();
		holdJoint = null;

		velJoint?.Remove();
		velJoint = null;

		if ( heldBody.IsValid() )
		{
			heldBody.AutoSleep = true;
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

		TraceResult walltr = Trace.Ray( heldBody.Transform.PointToWorld( heldPos ), startPos + dir * holdDistance ).Ignore( heldBody.GetEntity() ).Run();

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
