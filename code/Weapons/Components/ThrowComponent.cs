namespace Fortwars;

public class ThrowComponent : WeaponComponent
{
	public virtual string ThrowButton => InputAction.PrimaryAttack;

	protected Particles TracerParticle { get; set; }

	private int _charge = 0;
	private bool _thrown = false;

	public override void Simulate( IClient client, Player player )
	{
		base.Simulate( client, player );

		if ( Input.Down( ThrowButton ) )
		{
			if ( CanThrow() )
			{
				Charge();
			}
		}

		if ( Input.Released( ThrowButton ) )
		{
			if ( CanThrow() )
			{
				Throw();
			}

			_charge = 0;
		}
	}

	protected bool CanThrow()
	{
		return !_thrown;
	}

	private void Charge()
	{
		_charge++;
		_charge = _charge.Clamp( 0, 100 );
	}

	private void Throw()
	{
		if ( Game.IsClient )
			return;

		_thrown = true;
		var throwDir = ( Player.EyeRotation.Forward + ( Vector3.Up / 3f ) ).Normal;

		var team = Player.Client.Components.Get<TeamComponent>().Team;

		var flag = new BogRoll( TeamSystem.GetOpposingTeam( team ) )
		{
			Position = Player.EyePosition + ( Player.EyeRotation.Forward * 50f ),
			Velocity = throwDir * _charge * 10f,
		};

		var inv = Player.Inventory;
		inv.SetActiveWeapon( inv.GetWeaponFromSlot( inv.LastActiveWeaponSlot ) );
		inv.RemoveWeapon( Weapon );

		( GamemodeSystem.Instance as CaptureTheFlag ).OnWeaponThrown( Player, flag );
	}

	public override void SimulateAnimator( Player player )
	{
		/*		if ( Game.IsClient )
				{
					Weapon.ViewModelEntity.SetAnimParameter( "pull", _charge > 0 );
					Weapon.ViewModelEntity.SetAnimParameter( "pullamount", _charge / 100f );
				}*/

	}
}
