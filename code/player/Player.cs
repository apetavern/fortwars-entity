namespace Fortwars;

public partial class Player : AnimatedEntity
{
	[BindComponent]
	public PlayerController Controller { get; }

	[BindComponent]
	public Inventory Inventory { get; }

	public PlayerCamera PlayerCamera { get; protected set; }

	public ClothingContainer Clothing = new();
	public ClothingContainer ClientClothing = new();

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

		Health = 100;
		LifeState = LifeState.Alive;
		EnableAllCollisions = true;
		EnableDrawing = true;

		Components.Create<PlayerController>();

		Components.RemoveAny<PlayerControllerMechanic>();

		Components.Create<WalkMechanic>();

		Components.Create<Inventory>();

		ClientRespawn( To.Single( Client ) );

		DressPlayerClothing();
	}

	[ClientRpc]
	public void ClientRespawn()
	{
		PlayerCamera = new PlayerCamera();
	}

	public override void Simulate( IClient client )
	{
		Rotation = LookInput.WithPitch( 0f ).ToRotation();

		Controller?.Simulate( client );
	}

	public override void FrameSimulate( IClient client )
	{
		Rotation = LookInput.WithPitch( 0f ).ToRotation();

		PlayerCamera?.Update( this );
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
}
