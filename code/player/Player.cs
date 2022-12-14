// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.ComponentModel;

namespace Fortwars;

public partial class FortwarsPlayer : AnimatedEntity
{
	[Net] public string Killer { get; set; }
	[Net] public string SkinMaterialPath { get; set; }

	public DamageInfo LastDamage { get; private set; }
	public ClothingContainer Clothing = new();
	public ClothingContainer CleanClothing = new();

	[ConVar.Server( "fw_time_between_spawns", Help = "How long do players need to wait between respawns", Min = 1, Max = 30 )]
	public static int TimeBetweenSpawns { get; set; } = 10;

	/// <summary>
	/// Position a player should be looking from in world space.
	/// </summary>
	[Browsable( false )]
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	[Browsable( false )]
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Rotation EyeLocalRotation { get; set; }

	/// <summary>
	/// Override the aim ray to use the player's eye position and rotation.
	/// </summary>
	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );

	public bool IsSpectator
	{
		get => Team == null;
	}

	public FortwarsPlayer()
	{
		Inventory = new Inventory( this );
	}

	public FortwarsPlayer( IClient cl ) : this()
	{
		// Load clothing from client data
		CleanClothing.LoadFromClient( cl );

		// Get skin material.. this is a bit shit
		SkinMaterialPath = CleanClothing.Clothing.Select( x => x.SkinMaterial ).Where( x => !string.IsNullOrWhiteSpace( x ) ).FirstOrDefault();
	}

	public void Respawn()
	{
		// assign random team
		if ( Team == null )
		{
			int team = Game.Clients.Count % 2;
			if ( team == 0 )
				Team = FortwarsGame.Instance.BlueTeam;
			else
				Team = FortwarsGame.Instance.RedTeam;
		}

		SetModel( "models/playermodel/playermodel.vmdl" );

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
			return;
		}

		Controller = new FortwarsWalkController();

		EnableAllCollisions = true;
		EnableDrawing = true;

		// Draw clothes etc
		foreach ( var child in Children )
			child.EnableDrawing = true;

		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		DressPlayerClothing();

		FortwarsGame.Instance.Round.SetupInventory( this );

		InSpawnRoom = true;

		Game.AssertServer();

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;

		CreateHull();

		GameManager.Current?.MoveToSpawnpoint( this );
		ResetInterpolation();
	}

	public void DressPlayerClothing()
	{
		Clothing.ClearEntities();

		Clothing = new ClothingContainer();

		foreach ( var item in CleanClothing.Clothing )
		{
			Clothing.Clothing.Add( item );
		}


		foreach ( var item in Class.Cosmetics )
		{
			Clothing cloth = ResourceLibrary.Get<Clothing>( item );
			Clothing.Clothing.RemoveAll( x => !x.CanBeWornWith( cloth ) );
			Clothing.Clothing.Add( cloth );
		}

		Clothing.DressEntity( this );
	}

	public override void OnKilled()
	{
		BecomeRagdollOnClient( Position,
					 Rotation,
					 Velocity,
					 LastDamage.Force.Normal * 1024 );

		GameManager.Current?.OnKilled( this );

		timeSinceDied = 0;
		LifeState = LifeState.Dead;

		Client?.AddInt( "deaths", 1 );

		RespawnTimer = TimeBetweenSpawns;

		Inventory.DropActive();

		//
		// Delete any items we didn't drop
		//
		Inventory.DeleteContents();

		Controller = null;

		EnableAllCollisions = false;
		EnableDrawing = false;

		// Don't draw clothes etc
		foreach ( var child in Children )
			child.EnableDrawing = false;
	}

	[Net] public TimeUntil RespawnTimer { get; set; }

	public override void Simulate( IClient cl )
	{
		if ( LifeState == LifeState.Dead )
		{
			if ( RespawnTimer <= 0 && Game.IsServer )
			{
				Respawn();
			}

			return;
		}

		var controller = GetActiveController();
		controller?.Simulate( cl, this );

		SimulateActiveChild( cl, ActiveChild );
		SimulateDeployables( cl );
	}

	public override void FrameSimulate( IClient cl )
	{
		UpdateCamera();
	}

	private void SimulateDeployables( IClient cl )
	{
		Entity.All
			.OfType<Deployable>()
			.Where( x => x.Owner == this )
			.ToList()
			.ForEach( x => x.Simulate( cl ) );
	}

	public void Reset()
	{
		Game.AssertServer();

		DressPlayerClothing();

		Health = 100;
		FortwarsGame.Instance.MoveToSpawnpoint( this );
	}

	public override void TakeDamage( DamageInfo info )
	{
		LastDamage = info;

		bool isHeadshot = info.Hitbox.HasTag( "head" );

		if ( isHeadshot )
			info.Damage *= 2.0f;

		if ( info.Attacker is FortwarsPlayer attacker && attacker != this )
		{
			if ( attacker.TeamID == TeamID )
				return; // No team damage

			Killer = attacker.Client.Name;

			// Note - sending this only to the attacker!
			attacker.DidDamage( To.Single( attacker ), info.Position, isHeadshot, info.Damage, ( (float)Health ).LerpInverse( 100, 0 ) );
		}
		else
		{
			Killer = null;
		}

		if ( LifeState == LifeState.Dead )
			return;

		base.TakeDamage( info );

		this.ProceduralHitReaction( info );

		//
		// Add a score to the killer
		//
		if ( LifeState == LifeState.Dead && info.Attacker != null )
		{
			if ( info.Attacker.Client != null && info.Attacker != this )
			{
				info.Attacker.Client.AddInt( "kills" );
			}
		}

		if ( info.HasTag( "blast" ) )
		{
			Deafen( To.Single( Client ), info.Damage.LerpInverse( 0, 60 ) );
		}

		if ( info.Weapon.IsValid() || info.Attacker.IsValid() )
			TookDamage( To.Single( Client ), info.Weapon.IsValid() ? info.Weapon.Position : info.Position );
	}

	private void UpdateCamera()
	{
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.Position = EyePosition;
		Camera.FieldOfView = Game.Preferences.FieldOfView;
		Camera.FirstPersonViewer = this;
		Camera.ZNear = 1f;
		Camera.ZFar = 5000.0f;
	}

	[ClientRpc]
	public void DidDamage( Vector3 pos, bool isHeadshot, float amount, float healthinv )
	{
		Sound.FromScreen( "dm.ui_attacker" ).SetVolume( 0.5f );
		Hitmarker.Instance.OnHit( amount, isHeadshot );
	}

	[ClientRpc]
	public void TookDamage( Vector3 pos )
	{
		DamageIndicator.Current?.OnHit( pos );
	}

	TimeSince timeSinceLastFootstep = 0;

	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( LifeState != LifeState.Alive )
			return;

		if ( !Game.IsClient )
			return;

		if ( timeSinceLastFootstep < 0.2f )
			return;

		// Don't play if sliding
		if ( Controller is FortwarsWalkController { DuckSlide: { IsActiveSlide: true } } )
			return;

		// These are super quiet unless we bump them up. This might be due to some volume
		// bug in-engine, I don't know, I don't care.
		volume *= 5f;

		timeSinceLastFootstep = 0;

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) return;

		tr.Surface.DoFootstep( this, tr, foot, volume );
	}

	/// <summary>
	/// The PlayerController takes player input and moves the player. This needs
	/// to match between client and server. The client moves the local player and
	/// then checks that when the server moves the player, everything is the same.
	/// This is called prediction. If it doesn't match the player resets everything
	/// to what the server did, that's a prediction error.
	/// You should really never manually set this on the client - it's replicated so
	/// that setting the class on the server will automatically network and set it
	/// on the client.
	/// </summary>
	[Net, Predicted]
	public PawnController Controller { get; set; }

	/// <summary>
	/// This is used for noclip mode
	/// </summary>
	[Net, Predicted]
	public PawnController DevController { get; set; }

	[Net, Predicted] public Entity ActiveChild { get; set; }
	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Entity ActiveChildInput { get; set; }
	[ClientInput] public Angles ViewAngles { get; set; }
	public Angles OriginalViewAngles { get; private set; }

	/// <summary>
	/// Player's inventory for entities that can be carried. See <see cref="BaseCarriable"/>.
	/// </summary>
	public IBaseInventory Inventory { get; protected set; }

	/// <summary>
	/// Return the controller to use. Remember any logic you use here needs to match
	/// on both client and server. This is called as an accessor every tick.. so maybe
	/// avoid creating new classes here or you're gonna be making a ton of garbage!
	/// </summary>
	public virtual PawnController GetActiveController()
	{
		if ( DevController != null ) return DevController;

		return Controller;
	}

	TimeSince timeSinceDied;

	/// <summary>
	/// Applies flashbang-like ear ringing effect to the player.
	/// </summary>
	/// <param name="strength">Can be approximately treated as duration in seconds.</param>
	[ClientRpc]
	public void Deafen( float strength )
	{
		Audio.SetEffect( "flashbang", strength, velocity: 20.0f, fadeOut: 4.0f * strength );
	}

	/// <summary>
	/// Create a physics hull for this player. The hull stops physics objects and players passing through
	/// the player. It's basically a big solid box. It also what hits triggers and stuff.
	/// The player doesn't use this hull for its movement size.
	/// </summary>
	public virtual void CreateHull()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );
		EnableHitboxes = true;
	}

	/// <summary>
	/// Called from the gamemode, clientside only.
	/// </summary>
	public override void BuildInput()
	{
		OriginalViewAngles = ViewAngles;
		InputDirection = Input.AnalogMove;

		if ( Input.StopProcessing )
			return;

		var look = Input.AnalogLook;

		if ( ViewAngles.pitch > 90f || ViewAngles.pitch < -90f )
		{
			look = look.WithYaw( look.yaw * -1f );
		}

		var viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -89f, 89f );
		viewAngles.roll = 0f;
		ViewAngles = viewAngles.Normal;

		ActiveChild?.BuildInput();

		GetActiveController()?.BuildInput();
	}

	/// <summary>
	/// A generic corpse entity
	/// </summary>
	public ModelEntity Corpse { get; set; }

	/// <summary>
	/// Allows override of footstep sound volume.
	/// </summary>
	/// <returns>The new footstep volume, where 1 is full volume.</returns>
	public virtual float FootstepVolume()
	{
		return Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f ) * 0.2f;
	}

	/// <summary>
	/// This isn't networked, but it's predicted. If it wasn't then when the prediction system
	/// re-ran the commands LastActiveChild would be the value set in a future tick, so ActiveEnd
	/// and ActiveStart would get called multiple times and out of order, causing all kinds of pain.
	/// </summary>
	[Predicted]
	Entity LastActiveChild { get; set; }

	/// <summary>
	/// Simulated the active child. This is important because it calls ActiveEnd and ActiveStart.
	/// If you don't call these things, viewmodels and stuff won't work, because the entity won't
	/// know it's become the active entity.
	/// </summary>
	public virtual void SimulateActiveChild( IClient cl, Entity child )
	{
		if ( LastActiveChild != child )
		{
			OnActiveChildChanged( LastActiveChild, child );
			LastActiveChild = child;
		}

		if ( !LastActiveChild.IsValid() )
			return;

		if ( LastActiveChild.IsAuthority )
		{
			LastActiveChild.Simulate( cl );
		}
	}

	/// <summary>
	/// Called when the Active child is detected to have changed
	/// </summary>
	public virtual void OnActiveChildChanged( Entity previous, Entity next )
	{
		if ( previous is BaseCarriable previousBc )
		{
			previousBc?.ActiveEnd( this, previousBc.Owner != this );
		}

		if ( next is BaseCarriable nextBc )
		{
			nextBc?.ActiveStart( this );
		}
	}

	public override void OnChildAdded( Entity child )
	{
		Inventory?.OnChildAdded( child );
	}

	public override void OnChildRemoved( Entity child )
	{
		Inventory?.OnChildRemoved( child );
	}
}
