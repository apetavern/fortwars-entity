namespace Fortwars;

public partial class Player : AnimatedEntity
{
	[BindComponent]
	public PlayerController Controller { get; }

	[BindComponent]
	public PlayerAnimator Animator { get; }

	[BindComponent]
	public Inventory Inventory { get; }

	public PlayerCamera PlayerCamera { get; protected set; }

	public ClothingContainer Clothing = new();
	public ClothingContainer ClientClothing = new();

	[Net]
	public TimeUntil TimeUntilRespawn { get; set; }

	private static readonly Model CitizenModel = Model.Load( "models/playermodel/playermodel.vmdl" );

	[Net]
	public string SkinMaterialPath { get; set; }

	public Player()
	{

	}

	public Player( IClient client ) : this()
	{
		ClientClothing.LoadFromClient( client );

		SkinMaterialPath = ClientClothing.Clothing
			.Select( x => x.SkinMaterial )
			.Where( x => !string.IsNullOrWhiteSpace( x ) )
			.FirstOrDefault();
	}

	public override void Spawn()
	{
		Model = CitizenModel;

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableLagCompensation = true;
		EnableHitboxes = true;

		Tags.Add( "player" );
	}

	public void Respawn()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

		Health = 100 * Class.HealthMultiplier;
		LifeState = LifeState.Alive;
		EnableAllCollisions = true;
		EnableDrawing = true;

		SetupController();

		Components.Create<PlayerAnimator>();
		Components.Create<Inventory>();

		ClientRespawn( To.Single( Client ) );

		GamemodeSystem.Instance.MoveToSpawnpoint( this );

		DressPlayerClothing();
	}

	[ClientRpc]
	public void ClientRespawn()
	{
		PlayerCamera = new PlayerCamera();
	}

	public override void Simulate( IClient client )
	{
		if ( LifeState == LifeState.Dead )
		{
			if ( TimeUntilRespawn <= 0 && Game.IsServer )
			{
				Respawn();
			}

			return;
		}

		Rotation = LookInput.WithPitch( 0f ).ToRotation();

		Controller?.Simulate( client );
		Animator?.Simulate( client );
	}

	public override void FrameSimulate( IClient client )
	{
		Rotation = LookInput.WithPitch( 0f ).ToRotation();

		Controller?.FrameSimulate( client );

		PlayerCamera?.Update( this );
	}

	public override void OnKilled()
	{
		// TODO: Consult active gamemode for death timer / mechanics.
		LifeState = LifeState.Dead;
		TimeUntilRespawn = 5f;

		EnableAllCollisions = false;
		EnableDrawing = false;

		Controller.Remove();
		Animator.Remove();
		Inventory.Remove();

		Children.OfType<ModelEntity>()
			.ToList()
			.ForEach( x => x.EnableDrawing = false );
	}

	public void DressPlayerClothing()
	{
		Clothing.ClearEntities();
		Clothing = new ClothingContainer();

		foreach ( var clothingItem in ClientClothing.Clothing )
		{
			Clothing.Clothing.Add( clothingItem );
		}

		Clothing.DressEntity( this );
	}

	[ConCmd.Admin( "fw_kill" )]
	public static void KillPlayer()
	{
		if ( ConsoleSystem.Caller is not IClient caller )
			return;

		if ( caller.Pawn is not Player player )
			return;

		if ( player.LifeState == LifeState.Dead )
			return;

		player.Health = 0f;
		player.OnKilled();
	}

	[ConCmd.Admin( "fw_set_health" )]
	public static void SetHealth( int health )
	{
		if ( ConsoleSystem.Caller is not IClient caller )
			return;

		if ( caller.Pawn is not Player player )
			return;

		if ( player.LifeState == LifeState.Dead )
			return;

		player.Health = health;
	}
}
