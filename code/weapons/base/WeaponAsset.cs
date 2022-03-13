using Sandbox;
using System;
using System.ComponentModel;

namespace Fortwars;

[Library( "fwweapon" ), AutoGenerate]
public partial class WeaponAsset : Asset
{
	//
	// Meta
	//
	[Property, Category( "Meta" )]
	public string WeaponName { get; set; } = "My weapon";

	[Property, Category( "Meta" ), ResourceType( "vmdl" )]
	public string WorldModel { get; set; }

	[Property, Category( "Meta" ), ResourceType( "vmdl" )]
	public string ViewModel { get; set; }

	[Property, Category( "Meta" ), Range( 0.1f, 1.0f )]
	public float MovementSpeedMultiplier { get; set; } = 1.0f;

	public enum InventorySlots
	{
		Primary,
		Secondary,
		Equipment,

		Flag
	}

	[Property, Category( "Meta" )]
	public InventorySlots InventorySlot { get; set; } = InventorySlots.Primary;

	[Property, Category( "Meta" )]
	public HoldTypes HoldType { get; set; } = HoldTypes.Pistol;

	[Property, Category( "Meta" )]
	public float ProceduralViewmodelStrength { get; set; } = 1.0f;

	[Property, Category( "Meta" )]
	public float AimedProceduralViewmodelStrength { get; set; } = 0.1f;

	//
	// TODO: No flag support in inspector?
	//
	public struct WeaponFlags
	{
		public bool AutomaticFire { get; set; }
		public bool UseProjectile { get; set; }
		public bool ContinuousLoading { get; set; }
		public bool UseRenderTarget { get; set; }
		public bool ShotgunAmmo { get; set; }
	}

	[Property( Title = "Weapon Flags" ), Category( "Meta" )]
	public WeaponFlags Flags { get; set; }


	//
	// Shooting
	//

	[Property, Category( "Shooting" )]
	public float RPM { get; set; } = 600;

	[Property, Category( "Shooting" )]
	public int Range { get; set; } = 16384;

	[Property, Category( "Shooting" )]
	public int ShotCount { get; set; } = 1;

	[Property, Category( "Shooting" )]
	public float RecoilX { get; set; } = 0.2f;

	[Property, Category( "Shooting" )]
	public float RecoilY { get; set; } = 1.0f;

	//
	// Accuracy
	//

	[Property, Category( "Accuracy" )]
	public float Spread { get; set; } = 0.1f;

	[Property, Category( "Accuracy" )]
	public float SpreadShotIncrease { get; set; } = 0.05f;

	[Property, Category( "Accuracy" )]
	public float SpreadChangeTime { get; set; } = 0.01f;

	//
	// ADS
	//
	[Property, Category( "ADS" )]
	public Vector3 AimPosition { get; set; }

	[Property, Category( "ADS" )]
	public Angles AimRotation { get; set; }

	[Property, Category( "ADS" )]
	public float AimFovMult { get; set; } = 0.5f;

	//
	// Ammo
	//
	[Property, Category( "Ammo" )]
	public int MaxAmmo { get; set; } = 30;

	[Property, Category( "Ammo" )]
	public int ExtraAmmo { get; set; } = 90;

	[Property, Category( "Ammo" )]
	public float ReloadTime { get; set; } = 3.0f;

	//
	// Damage
	//
	[Property, Category( "Damage" )]
	public float MaxDamage { get; set; } = 20f;

	[Property, Category( "Damage" )]
	public float MinDamage { get; set; } = 10f;

	[Property, Category( "Damage" )]
	public int DamageDropOffDistance { get; set; } = 256;

	[Property, Category( "Damage" )]
	public float HeadshotMultiplier { get; set; } = 1.5f;

	[Property, Category( "Damage" )]
	public float BuildingDamageMultiplier { get; set; } = 1.0f;

	//
	// Effects
	//
	[Property, Category( "Effects" ), FGDType( "sound" )]
	public string FireSound { get; set; } = "rust_pistol.shoot";

	[Property, Category( "Effects" ), ResourceType( "vpcf" )]
	public string FireParticles { get; set; } = "particles/flash_medium.vpcf";

	[Property, Category( "Effects" ), ResourceType( "vpcf" )]
	public string TracerParticles { get; set; } = "particles/tracer_large.vpcf";

	//
	// Projectile
	//

	[Property, Category( "Projectile" ), ResourceType( "vmdl" )]
	public string ProjectileModel { get; set; }

	[Property, Category( "Projectile" )]
	public float ProjectileSpeed { get; set; } = 1;
}
