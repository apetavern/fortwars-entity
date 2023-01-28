namespace Fortwars;

public class ThrowComponent : WeaponComponent
{
	public virtual InputButton ThrowButton => InputButton.PrimaryAttack;

	protected Particles TracerParticle { get; set; }

	private bool _thrown = false;

	public override void Simulate( IClient client, Player player )
	{
		base.Simulate( client, player );

		if ( Input.Pressed( ThrowButton ) )
		{
			if ( CanThrow() )
			{
				Throw();
			}
		}
	}

	protected bool CanThrow()
	{
		return !_thrown;
	}

	private void Throw()
	{
		if ( Game.IsClient )
			return;

		_thrown = true;
		var throwDir = ( Player.EyeRotation.Forward + ( Vector3.Up / 3f ) ).Normal;

		var flag = new BogRoll()
		{
			Position = Player.EyePosition + ( Player.EyeRotation.Forward * 50f ),
			Velocity = throwDir * 600f,
		};

		var inv = Player.Inventory;
		inv.SetActiveWeapon( inv.GetWeaponFromSlot( inv.LastActiveWeaponSlot ) );
		inv.RemoveWeapon( Weapon );

		( GamemodeSystem.Instance as CaptureTheFlag ).OnWeaponThrown( Player, flag );
	}
}
