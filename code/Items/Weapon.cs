namespace Fortwars;

[Prefab, Category( "Weapons" ), Obsolete]
public partial class Weapon : AnimatedEntity
{
	public Player Player => Owner as Player;

	public IEnumerable<WeaponComponent> WeaponComponents => Components.GetAll<WeaponComponent>( includeDisabled: true );


	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		foreach ( var component in WeaponComponents )
		{
			if ( !component.Enabled )
				continue;

			component.Simulate( cl, Player );
		}
	}
	
	[Prefab, Net] public Vector3 PositionOffset { get; set; } = Vector3.Zero;

	[Prefab, Net] public SoundEvent ShootSound { get; set; }

	[Prefab, Net] public ParticleSystem TracerParticle { get; set; }
}
