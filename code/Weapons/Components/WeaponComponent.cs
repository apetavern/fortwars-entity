namespace Fortwars;

public partial class WeaponComponent : EntityComponent<Weapon>
{
	protected Weapon Weapon => Entity;
	protected Player Player => Weapon.Owner as Player;

	[Net, Predicted]
	public bool IsActive { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSinceActivated { get; protected set; }

	public virtual string Name => displayInfo.Name.Replace( " ", "" );
	protected virtual bool UseLagCompensation => false;
	protected virtual bool EnableActivateEvents => true;

	DisplayInfo displayInfo;

	public WeaponComponent()
	{
		displayInfo = DisplayInfo.For( this );
	}

	public T GetComponent<T>() where T : WeaponComponent
	{
		return Weapon.GetComponent<T>();
	}

	public void RunGameEvent( string eventName )
	{
		Player?.RunGameEvent( eventName );
	}

	protected virtual bool CanStart( Player player )
	{
		return true;
	}

	protected virtual void OnStart( Player player )
	{
		TimeSinceActivated = 0;

		if ( EnableActivateEvents )
			RunGameEvent( $"{Name}.start" );
	}

	protected virtual void OnStop( Player player )
	{
		if ( EnableActivateEvents )
			RunGameEvent( $"{Name}.stop" );
	}

	public virtual void Simulate( IClient client, Player player )
	{
		var before = IsActive;

		if ( !IsActive && CanStart( player ) )
		{
			if ( UseLagCompensation )
			{
				using ( Sandbox.Entity.LagCompensation() )
				{
					OnStart( player );
					IsActive = true;
				}
			}
			else
			{
				OnStart( player );
				IsActive = true;
			}
		}
		else if ( before && !CanStart( player ) )
		{
			IsActive = false;
			OnStop( player );
		}
	}

	public virtual void Initialize( Weapon weapon ) { }

	public virtual void OnGameEvent( string eventName ) { }

	public virtual void BuildInput() { }

	public virtual void SimulateAnimator( Player player ) { }
}
