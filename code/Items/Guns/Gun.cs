namespace Fortwars;

[Category( "Gun" ), Icon( "gavel" )]
public abstract partial class Gun : Item
{
	public struct Setup
	{
		public bool Automatic;
		public float MoveSpeedMultiplier;
		public int MagazineSize;
		public ProjectileType ProjectileType;
		public int Damage;
		public int RateOfFire;
	}

	public Setup Data => n_Data;
	[Net] private Setup n_Data { get; set; }

	protected abstract Setup Default { get; }

	protected Gun()
	{
		Transmit = TransmitType.Always;
	}

	public override void Spawn()
	{
		base.Spawn();

		n_SinceFired = 0;
		n_Data = Default;
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( "reload" ) ) { }

		const string fireAction = "attack1";
		if ( Data.Automatic ? Input.Down( fireAction ) : Input.Pressed( fireAction ) )
		{
			Shoot();
		}
	}

	// Shoot

	[Net, Predicted] private TimeSince n_SinceFired { get; set; }

	public void Shoot()
	{
		// Can't shoot, haven't chambered
		if ( n_SinceFired < 60f / Data.RateOfFire )
			return;

		n_SinceFired = 0;
		Log.Info( "Fired" );
	}

	// Reload

	[Net, Predicted] private TimeSince n_SinceReload { get; set; }

	public void Reload() { }
}
