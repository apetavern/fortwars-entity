// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System;
using System.Collections.Generic;

namespace Fortwars;

public partial class FortwarsWeapon : Carriable
{
	//
	// Networked variables
	//
	[Net] public WeaponAsset WeaponAsset { get; set; }
	[Net] public int CurrentClip { get; set; }
	[Net] public int ReserveAmmo { get; set; }
	[Net, Predicted] public TimeSince TimeSincePrimaryAttack { get; set; }
	[Net, Predicted] private bool IsReloading { get; set; }

	//
	// Cvars
	//
	[ConVar.Replicated( "fw_weapon_debug_shots" )]
	public static bool DebugShots { get; set; } = false;

	[ConVar.Replicated( "fw_weapon_debug_spread" )]
	public static bool DebugSpread { get; set; } = false;

	//
	// Realtime variables
	//
	public float Bloom { get; set; }
	public Vector2 Recoil { get; set; }
	public float Inaccuracy { get; set; }
	public bool IsAiming => Input.Down( InputButton.SecondaryAttack );
	private TimeSince TimeSinceReload { get; set; }
	private FallbackScope scopePanel;

	/// <summary>
	/// Load a weapon from a specified path.
	/// </summary>
	public static FortwarsWeapon FromPath( string assetPath )
	{
		var weaponAsset = ResourceLibrary.Get<WeaponAsset>( assetPath );

		var weapon = new FortwarsWeapon()
		{
			WeaponAsset = weaponAsset,
			CurrentClip = weaponAsset.MaxAmmo,
			ReserveAmmo = weaponAsset.ExtraAmmo
		};

		weapon.SetModel( weaponAsset.WorldModel );
		weapon.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		return weapon;
	}

	private void ShowSpreadPattern()
	{
		if ( IsServer )
			return;

		for ( int i = 0; i < 32; i++ )
		{
			var forward = Owner.EyeRotation.Forward;

			if ( !IsAiming )
			{
				forward += ( Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random ) * Inaccuracy * 0.25f;
				forward = forward.Normal;
			}

			foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * WeaponAsset.Range, 1f ) )
			{
				DebugOverlay.Line( tr.EndPosition, tr.EndPosition + tr.Normal * 4f, 1f, false );
			}
		}
	}

	public void ApplyInaccuracy()
	{
		float spread = Bloom + WeaponAsset.Spread;

		if ( Owner.GroundEntity == null )
			spread *= 3f;
		else
			spread *= 1.0f + Owner.Velocity.Length.LerpInverse( 0, 400 );

		Inaccuracy = Inaccuracy.LerpTo( spread, 25f * Time.Delta );
	}

	public override void Simulate( Client player )
	{
		Bloom = Bloom.LerpTo( 0, Time.Delta * WeaponAsset.SpreadChangeTime );
		if ( Bloom.AlmostEqual( 0 ) )
			Bloom = 0;

		ApplyInaccuracy();

		if ( DebugSpread )
			ShowSpreadPattern();

		ViewModelEntity?.SetAnimParameter( "aiming", IsAiming );

		if ( IsReloading )
		{
			if ( WeaponAsset.Flags.ContinuousLoading )
				OnContinuousReload();
			else if ( TimeSinceReload > WeaponAsset.ReloadTime )
				OnReloadFinish();
			else
				return;
		}

		if ( CanReload() )
			Reload();

		if ( !Owner.IsValid() )
			return;

		if ( CanPrimaryAttack() )
		{
			if ( ReserveAmmo > 0 && CurrentClip == 0 )
				Reload();

			TimeSincePrimaryAttack = 0;
			AttackPrimary();
		}
	}

	public override void CreateHudElements()
	{
		base.CreateHudElements();

		if ( WeaponAsset.Flags.UseRenderTarget )
		{
			scopePanel = new FallbackScope();
			scopePanel.Parent = Local.Hud;
		}
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		scopePanel?.Delete();
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
		if ( !Owner.IsValid() || !Input.Down( InputButton.Reload ) )
			return false;

		if ( ReserveAmmo <= 0 )
			return false;

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

		( Owner as AnimatedEntity ).SetAnimParameter( "b_reload", true );

		StartReloadEffects();
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)WeaponAsset.HoldType );
		anim.SetAnimParameter( "holdtype_handedness", (int)HoldHandedness.TwoHands );
	}

	private void OnContinuousReload()
	{
		var amount = Math.Min( ReserveAmmo, WeaponAsset.MaxAmmo - CurrentClip );

		if ( amount <= 0 )
			ViewModelEntity?.SetAnimParameter( "endreload", true );

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

		if ( WeaponAsset.Flags.AutomaticFire )
		{
			if ( !Input.Down( InputButton.PrimaryAttack ) )
				return false;
		}
		else
		{
			if ( !Input.Pressed( InputButton.PrimaryAttack ) )
				return false;
		}

		if ( Owner is FortwarsPlayer { Controller: FortwarsWalkController { IsSprinting: true } } || GetTuckDist() != -1 )
			return false;

		var rate = WeaponAsset.RPM / 60f;
		if ( rate <= 0 )
			return true;

		return TimeSincePrimaryAttack > ( 1 / rate );
	}

	public virtual void AttackPrimary()
	{
		FireBullets();
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

		ViewModelEntity?.SetAnimParameter( "endreload", true );
		IsReloading = false;

		ShootEffectsLocal( Vector3.Zero );
		PlaySound( WeaponAsset.FireSound ).SetVolume( 0.5f );

		if ( !IsServer )
			return;

		ShootEffectsRemote( Vector3.Zero );

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
		PlaySound( WeaponAsset.FireSound );

		var forward = Owner.EyeRotation.Forward;

		if ( !IsAiming )
		{
			forward += ( Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random ) * Inaccuracy * 0.25f;
			forward = forward.Normal;
		}

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * WeaponAsset.Range, 1f ) )
		{
			if ( DebugShots )
			{
				var color = IsServer ? Color.Blue : Color.Red;
				var size = IsServer ? new Vector3( 2f ) : new Vector3( 2.5f );

				DebugOverlay.Box( tr.EndPosition, -size, size, color, 5f, false );
			}

			tr.Surface.DoBulletImpact( tr );

			ShootEffectsLocal( tr.EndPosition );

			if ( !IsServer )
				continue;

			ShootEffectsRemote( tr.EndPosition );

			if ( !tr.Entity.IsValid() )
				continue;

			float damage = CalcDamage( tr.Distance, false );

			if ( tr.Entity is FortwarsBlock )
				damage *= WeaponAsset.BuildingDamageMultiplier;

			var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 32, damage )
				.UsingTraceResult( tr )
				.WithAttacker( Owner )
				.WithWeapon( this );

			if ( DebugShots )
			{
				DebugOverlay.Text( $"D: {damageInfo.Damage}\nF: {damageInfo.Force.Length}", tr.EndPosition, Color.White, 5f, float.MaxValue );
			}

			tr.Entity.TakeDamage( damageInfo );
		}
	}

	private void FireBullets()
	{
		//
		// Seed rand using the tick, so bullet cones match on client and server
		//
		Rand.SetSeed( Time.Tick );

		if ( WeaponAsset.Flags.ShotgunAmmo )
		{
			if ( !TakeAmmo() )
				return;
		}

		for ( int i = 0; i < WeaponAsset.ShotCount; ++i )
		{
			if ( WeaponAsset.Flags.UseProjectile )
			{
				ShootProjectile();
			}
			else
			{
				if ( !WeaponAsset.Flags.ShotgunAmmo )
				{
					if ( !TakeAmmo() )
						return;
				}

				ShootBullet();
			}
		}

		Bloom += WeaponAsset.SpreadShotIncrease;
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimParameter( "reload", true );
	}

	public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		using var _ = LagCompensation();

		bool InWater = Map.Physics.IsPointWater( start );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				.HitLayer( CollisionLayer.Water, !InWater )
				.Ignore( Owner )
				.Ignore( this )
				.Size( radius )
				.Run();

		yield return tr;
	}

	private void CreateTracerEffects( Vector3 end )
	{
		var tracer = Particles.Create( WeaponAsset.TracerParticles );
		tracer.SetPosition( 1, EffectEntity.GetAttachment( "muzzle" )?.Position ?? default );
		tracer.SetPosition( 2, end );
	}

	[ClientRpc]
	private void ShootEffectsLocal( Vector3 traceEnd )
	{
		if ( !IsLocalPawn )
			return;

		Particles.Create( WeaponAsset.FireParticles, EffectEntity, "muzzle" );

		( ViewModelEntity as ViewModel )?.OnFire( IsAiming );

		const float RecoilScaleFactor = 10f;
		Recoil += new Vector2( WeaponAsset.RecoilX, WeaponAsset.RecoilY ) * RecoilScaleFactor;

		CreateTracerEffects( traceEnd );
	}

	[ClientRpc]
	private void ShootEffectsRemote( Vector3 traceEnd )
	{
		if ( IsLocalPawn )
			return;

		CreateTracerEffects( traceEnd );
	}

	public override void BuildInput( InputBuilder inputBuilder )
	{
		base.BuildInput( inputBuilder );

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

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		Recoil = 0;
	}

	public float GetCrosshairSize() => 768 * Inaccuracy;
}
