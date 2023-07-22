namespace Fortwars;

[Prefab]
public class ShootComponent : WeaponComponent
{
	public virtual string FireButton => InputAction.PrimaryAttack;

	public TimeUntil TimeUntilCanFire { get; set; }
	protected Particles TracerParticle { get; set; }
	protected Particles ImpactTrailParticle { get; set; }

	protected bool IsFiring { get; set; } = false;

	public override void Simulate( IClient client, Player player )
	{
		base.Simulate( client, player );

		// Check player input for fire input and act accordingly.
		if ( Input.Down( FireButton ) )
		{
			if ( CanFire() )
				StartFiring();
		}
		else
		{
			if ( IsFiring )
				StopFiring();
		}

		if ( !IsFiring )
			return;

		// Continue firing logic.
		if ( TimeSinceActivated > 1f )
			Fire( player );
	}

	public void StartFiring()
	{
		if ( IsFiring )
			return;

		IsFiring = true;
	}

	public void StopFiring()
	{
		IsFiring = false;

		TracerParticle?.Destroy();
		TracerParticle = null;

		ImpactTrailParticle?.Destroy();
		ImpactTrailParticle = null;
	}

	protected override void OnStart( Player player )
	{
		base.OnStart( player );
	}

	protected override void OnDeactivate()
	{
		StopFiring();
	}

	protected bool CanFire()
	{
		if ( TimeUntilCanFire > 0 )
			return false;

		if ( Weapon.Tags.Has( "reloading" ) )
			return false;

		// TODO: CanFire case for ammo.

		return true;
	}

	private void Fire( Player player )
	{
		if ( player is null )
			return;

		TimeSinceActivated = 0;
		player.SetAnimParameter( "b_attack", true );

		Weapon.PlaySound( Weapon.ShootSound is not null ? Weapon.ShootSound.ResourcePath : "audio/weapons/aiax50/aiax50_fire.sound" );

		if ( !Game.IsClient )
			return;

		player.ViewModel.SetAnimParameter( "fire", true );

		if ( Weapon.TracerParticle is null )
			TracerParticle ??= Particles.Create( "particles/tracer.vpcf" );
		else
			TracerParticle ??= Particles.Create( Weapon.TracerParticle.ResourcePath );

		if ( TracerParticle != null || ImpactTrailParticle != null )
		{
			var xForm = player.ViewModel.GetAttachment( "muzzle" ) ?? Weapon.Transform;
			DebugOverlay.Sphere( xForm.Position, 2, Color.Random, 20 );
			TracerParticle?.SetPosition( 0, xForm.Position );

			var tr = Trace.Ray( Player.EyePosition, Player.EyePosition + Player.EyeRotation.Forward * 5000f )
				.WithAnyTags( "solid", "glass" )
				.Ignore( player )
				.Run();

			TracerParticle?.SetPosition( 1, tr.EndPosition );
			ImpactTrailParticle?.SetPosition( 0, tr.EndPosition + ( tr.Normal * 1f ) );
			ImpactTrailParticle?.SetForward( 0, tr.Normal );
		}
	}
}
