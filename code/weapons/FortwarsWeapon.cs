using Sandbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fortwars;

public partial class FortwarsWeapon : Carriable
{
	//
	// Realtime variables
	//
	private float spread = 0;
	private Vector2 recoil = 0;
	private TimeSince TimeSinceReload { get; set; }
	public bool IsAiming => Input.Down( InputButton.Attack2 );

	//
	// Networked variables
	//
	[Net, Local] public WeaponAsset WeaponAsset { get; set; }

	[Net, Predicted] public TimeSince TimeSincePrimaryAttack { get; set; }

	[Net] public int CurrentClip { get; set; }
	[Net] public int ReserveAmmo { get; set; }

	[Net, Predicted] private bool IsReloading { get; set; }

	/// <summary>
	/// Load a weapon from a specified path.
	/// </summary>
	public static FortwarsWeapon FromPath( string assetPath )
	{
		var weaponAsset = Asset.FromPath<WeaponAsset>( assetPath );

		var weapon = new FortwarsWeapon()
		{
			WeaponAsset = weaponAsset
		};

		weapon.SetModel( weaponAsset.WorldModel );
		weapon.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		weapon.CurrentClip = weaponAsset.MaxAmmo;
		weapon.ReserveAmmo = weaponAsset.ExtraAmmo;

		return weapon;
	}

	public override void Spawn()
	{
		base.Spawn();

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	public override void Simulate( Client player )
	{
		spread = spread.LerpTo( 0, Time.Delta * WeaponAsset.SpreadChangeTime );

		if ( spread.AlmostEqual( 0 ) )
			spread = 0;

		ViewModelEntity?.SetAnimBool( "aiming", IsAiming );

		if ( IsReloading )
		{
			if ( WeaponAsset.UseProjectile )
			{
				OnProjectileReload();
			}
			else if ( TimeSinceReload > WeaponAsset.ReloadTime )
			{
				OnReloadFinish();
			}
			else
			{
				return;
			}
		}

		if ( CanReload() )
		{
			Reload();
		}

		if ( !Owner.IsValid() )
			return;

		if ( CanPrimaryAttack() )
		{
			TimeSincePrimaryAttack = 0;
			AttackPrimary();
		}
	}

	ScopeRenderTarget SniperScopePanel;
	public override void CreateHudElements()
	{
		base.CreateHudElements();

		if ( WeaponAsset.UseRenderTarget )
		{
			SniperScopePanel = new ScopeRenderTarget();
			SniperScopePanel.Parent = Local.Hud;
		}
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		SniperScopePanel?.Delete();
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( WeaponAsset.ViewModel ) )
			return;

		ViewModelEntity = new ViewModel( this );
		ViewModelEntity.Position = Position;
		ViewModelEntity.Owner = Owner;
		ViewModelEntity.EnableViewmodelRendering = true;
		ViewModelEntity.SetModel( WeaponAsset.ViewModel );
	}

	public float GetTuckDist()
	{
		if ( !Owner.IsValid() )
			return -1;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 32 )
			.Ignore( this )
			.Ignore( Owner )
			.Run();

		if ( !trace.Entity.IsValid() )
			return -1;

		return trace.Distance;
	}

	public virtual bool CanReload()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Reload ) ) return false;
		if ( ReserveAmmo <= 0 ) return false;

		return true;
	}

	public virtual void Reload()
	{
		if ( IsReloading )
			return;

		if ( CurrentClip >= WeaponAsset.MaxAmmo )
			return;

		TimeSinceReload = 0;
		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool( "b_reload", true );

		StartReloadEffects();
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", (int)WeaponAsset.HoldType );
		anim.SetParam( "aimat_weight", 1.0f );
		anim.SetParam( "holdtype_handedness", 0 );
	}

	private void OnProjectileReload()
	{
		var amount = Math.Min( ReserveAmmo, WeaponAsset.MaxAmmo - CurrentClip );

		if ( amount <= 0 )
		{
			ViewModelEntity?.SetAnimBool( "endreload", true );
		}

		if ( TimeSinceReload < WeaponAsset.ReloadTime )
			return;

		if ( amount > 0 )
		{
			ReserveAmmo--;
			CurrentClip++;
			TimeSinceReload = 0;
		}
		else
		{
			IsReloading = false;
		}
	}

	private void OnReloadFinish()
	{
		var amount = Math.Min( ReserveAmmo, WeaponAsset.MaxAmmo - CurrentClip );

		if ( amount > 0 )
		{
			ReserveAmmo -= amount;
			CurrentClip += amount;
			IsReloading = false;
		}
	}

	public virtual bool CanPrimaryAttack()
	{
		if ( !Owner.IsValid() )
			return false;

		if ( WeaponAsset.FireMode == WeaponAsset.FireModes.SemiAuto )
		{
			if ( !Input.Down( InputButton.Attack1 ) )
				return false;
		}
		else
		{
			if ( !Input.Pressed( InputButton.Attack1 ) )
				return false;
		}

		if ( Owner is FortwarsPlayer { Controller: FortwarsWalkController { IsSprinting: true } } || GetTuckDist() != -1 )
			return false;

		var rate = WeaponAsset.RPM / 60f;
		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual void AttackPrimary()
	{
		_ = FireBullets();
	}

	public virtual bool TakeAmmo()
	{
		if ( CurrentClip <= 0 )
			return false;

		CurrentClip--;
		return true;
	}

	/// <summary>
	/// Shoot a single projectile
	/// </summary>
	public virtual void ShootProjectile()
	{
		if ( !TakeAmmo() )
			return;

		ViewModelEntity?.SetAnimBool( "endreload", true );
		IsReloading = false;

		recoil += new Vector2( WeaponAsset.RecoilY, WeaponAsset.RecoilX );
		spread += WeaponAsset.SpreadShotIncrease;

		ShootEffects();
		PlaySound( WeaponAsset.FireSound );

		if ( !IsServer )
			return;

		var projectile = new Projectile();
		projectile.Rotation = Owner.EyeRotation;
		projectile.Position = Owner.EyePosition;
		projectile.SetModel( WeaponAsset.ProjectileModel );
		projectile.Speed = WeaponAsset.ProjectileSpeed;
		projectile.Weapon = this;

		projectile.Velocity = projectile.Rotation.Forward * WeaponAsset.ProjectileSpeed; // Initial velocity

		projectile.Owner = Owner;
	}

	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet()
	{
		if ( !TakeAmmo() )
			return;

		ShootEffects();
		PlaySound( WeaponAsset.FireSound );

		var forward = Owner.EyeRotation.Forward;

		if ( !IsAiming )
		{
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * (WeaponAsset.Spread + spread) * 0.25f;
			forward = forward.Normal;
		}

		recoil += new Vector2( WeaponAsset.RecoilY, WeaponAsset.RecoilX );
		spread += WeaponAsset.SpreadShotIncrease;

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * WeaponAsset.Range, 1f ) )
		{
			TracerEffects( tr.EndPos );
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			float damage = CalcDamage( tr.Distance, false );

			if ( tr.Entity is FortwarsBlock )
				damage *= WeaponAsset.BuildingDamageMultiplier;

			var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100, damage )
				.UsingTraceResult( tr )
				.WithAttacker( Owner )
				.WithWeapon( this );

			tr.Entity.TakeDamage( damageInfo );
		}
	}

	private async Task FireBullets()
	{
		//
		// Seed rand using the tick, so bullet cones match on client and server
		//
		Rand.SetSeed( Time.Tick );

		for ( int i = 0; i < WeaponAsset.ShotCount; ++i )
		{
			if ( i == 0 && WeaponAsset.FirstShotDelay > 0 )
			{
				await Task.DelayRealtimeSeconds( WeaponAsset.FirstShotDelay );
			}
			else if ( i != 0 && WeaponAsset.ShotDelay > 0 )
			{
				await Task.DelayRealtimeSeconds( WeaponAsset.ShotDelay );
			}

			if ( WeaponAsset.UseProjectile )
			{
				ShootProjectile();
			}
			else
			{
				ShootBullet();
			}
		}
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimBool( "reload", true );
	}

	public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		using var _ = LagCompensation();

		bool InWater = Physics.TestPointContents( start, CollisionLayer.Water );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				.HitLayer( CollisionLayer.Water, !InWater )
				.Ignore( Owner )
				.Ignore( this )
				.Size( radius )
				.Run();

		yield return tr;
	}

	[ClientRpc]
	protected void TracerEffects( Vector3 end )
	{
		var tracer = Particles.Create( WeaponAsset.TracerParticles );
		tracer.SetPosition( 1, EffectEntity.GetAttachment( "muzzle" )?.Position ?? default );
		tracer.SetPosition( 2, end );
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( WeaponAsset.FireParticles, EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			//new Sandbox.ScreenShake.Perlin(
			//	WeaponAsset.KickbackLength * (IsAiming ? 0.5f : 1.0f),
			//	WeaponAsset.KickbackSpeed * (IsAiming ? 0.5f : 1.0f),
			//	WeaponAsset.KickbackSize * (IsAiming ? 0.5f : 1.0f),
			//	WeaponAsset.KickbackRotation * (IsAiming ? 0.5f : 1.0f)
			//);
		}

		(ViewModelEntity as ViewModel)?.OnFire( IsAiming );

		if ( !IsAiming )
			ViewModelEntity?.SetAnimBool( "fire", true );

		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void BuildInput( InputBuilder inputBuilder )
	{
		base.BuildInput( inputBuilder );

		inputBuilder.ViewAngles += recoil * Time.Delta * 100f;

		recoil = Vector2.Lerp( recoil, 0, Time.Delta * 25 );

		if ( IsAiming )
			inputBuilder.ViewAngles = Angles.Lerp( inputBuilder.OriginalViewAngles, inputBuilder.ViewAngles, WeaponAsset.AimFovMult );
	}

	private float CalcDamage( float distance, bool isHeadshot )
	{
		float baseDamage = WeaponAsset.MaxDamage.LerpTo( WeaponAsset.MinDamage, distance / WeaponAsset.DamageDropOffDistance );
		if ( isHeadshot )
			return WeaponAsset.HeadshotMultiplier * baseDamage;

		return baseDamage;
	}

	public float GetCrosshairSize()
	{
		return (384 * (spread + WeaponAsset.Spread)).Clamp( 16, 512 );
	}
}
