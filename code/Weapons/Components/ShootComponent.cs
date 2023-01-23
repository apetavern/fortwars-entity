namespace Fortwars;

public class ShootComponent : WeaponComponent
{
	public virtual InputButton FireButton => InputButton.PrimaryAttack;

	protected override void OnStart( Player player )
	{
		base.OnStart( player );
	}

	public override void Simulate( IClient client, Player player )
	{
		base.Simulate( client, player );

		if ( Input.Down( FireButton ) )
		{
			StartFiring();
		}
	}

	public void StartFiring()
	{
		//
	}
}
