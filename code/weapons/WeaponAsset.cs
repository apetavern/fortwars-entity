using Sandbox;
using System.ComponentModel;

namespace Fortwars;

[Library( "fwweapon" ), AutoGenerate]
public class WeaponAsset : Asset
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

	public enum HoldTypes
	{
		None,
		Pistol,
		Rifle,
		Shotgun,
		HoldItem,
		MeleePunch,
		MeleeWeapons
	}

	[Property, Category( "Meta" )]
	public HoldTypes HoldType { get; set; } = HoldTypes.Pistol;

	[Property, Category( "Meta" )]
	public bool UseRenderTarget { get; set; } = false;

	//
	// Shooting
	//
	public enum FireModes : int
	{
		Burst,
		Single,
		SemiAuto,
		BoltAction
	}

	[Property, Category( "Shooting" )]
	public FireModes FireMode { get; set; }

	[Property, Category( "Shooting" )]
	public float RPM { get; set; } = 600;

	[Property, Category( "Shooting" )]
	public float FirstShotDelay { get; set; }

	[Property, Category( "Shooting" )]
	public float ShotDelay { get; set; }

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
	public bool FirstShotAlwaysAccurate { get; set; } = true;

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
	public float AimAccuracyMult { get; set; } = 1.0f;

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

	[Property, Category( "Ammo" )]
	public float PostReloadTime { get; set; } = 0.5f;

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
	// View kick
	//
	[Property, Category( "View Kick" )]
	public float KickbackSize { get; set; } = 1f;

	[Property, Category( "View Kick" )]
	public float KickbackLength { get; set; } = 1f;

	[Property, Category( "View Kick" )]
	public float KickbackSpeed { get; set; } = 1f;

	[Property, Category( "View Kick" )]
	public float KickbackRotation { get; set; } = 0.6f;

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
	[Property, Category( "Projectile" )]
	public bool UseProjectile { get; set; } = false;

	[Property, Category( "Projectile" ), ResourceType( "vmdl" )]
	public string ProjectileModel { get; set; }

	[Property, Category( "Projectile" )]
	public float ProjectileSpeed { get; set; } = 1;
}
