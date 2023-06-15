namespace Fortwars;

[Prefab, Category( "Weapons" )]
public partial class Weapon : AnimatedEntity
{
	public Player Player => Owner as Player;

	public Weapon()
	{
		Transmit = TransmitType.Always;
	}

	public override void Spawn()
	{
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableDrawing = false;
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
	}

	public void OnHolster( Player player )
	{
		EnableDrawing = false;
	}

	public void OnDeploy( Player player )
	{
		SetParent( player, true );
		Owner = player;

		EnableDrawing = true;
	}
}
