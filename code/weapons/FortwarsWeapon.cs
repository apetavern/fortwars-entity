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

		Log.Trace( $"Loaded weapon {assetPath}" );
		Log.Trace( weaponAsset.WeaponName );

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
		recoil = Vector2.Lerp( recoil, 0, Time.Delta * 25 );
		spread = spread.LerpTo( 0, Time.Delta * WeaponAsset.SpreadChangeTime );

		if ( spread.AlmostEqual( 0 ) )
			spread = 0;

		DrawDebug();

		if ( IsReloading )
		{
			if ( TimeSinceReload > WeaponAsset.ReloadTime )
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
		var trace = Trace.Ray( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 32 )
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

	public virtual void OnReloadFinish()
	{
		var amount = Math.Min( ReserveAmmo, WeaponAsset.MaxAmmo - CurrentClip );

		if ( amount > 0 )
		{
			ReserveAmmo -= amount;
			CurrentClip += amount;
		}

		IsReloading = false;
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

		var rate = WeaponAsset.RPM / 60f;
		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	private void DrawDebug()
	{
		var vals = new (string, object)[]
		{
			( "Spread", spread ),
			( "Recoil", recoil ),
			( "XHair size", GetCrosshairSize() ),
		};

		Vector2 offset = new Vector2( 100, 250 );

		for ( int i = 0; i < vals.Length; i++ )
		{
			(string, object) valTuple = vals[i];
			DebugOverlay.ScreenText( offset, i, Color.White, $"{valTuple.Item1,16}: {valTuple.Item2,16}" );
		}
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
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet()
	{
		if ( !TakeAmmo() )
			return;

		recoil += new Vector2( WeaponAsset.RecoilY, WeaponAsset.RecoilX );
		spread += WeaponAsset.SpreadShotIncrease;

		ShootEffects();
		PlaySound( WeaponAsset.FireSound );

		var forward = Owner.EyeRot.Forward;

		if ( TimeSincePrimaryAttack < 3 || !WeaponAsset.FirstShotAlwaysAccurate )
		{
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * (WeaponAsset.Spread + spread) * 0.25f;
			forward = forward.Normal;
		}

		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * WeaponAsset.Range, 1f ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100, CalcDamage( tr.Distance, false ) )
				.UsingTraceResult( tr )
				.WithAttacker( Owner )
				.WithWeapon( this );

			tr.Entity.TakeDamage( damageInfo );

			TracerEffects( tr.EndPos );
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

			ShootBullet();
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

	public virtual ModelEntity GetEffectModel()
	{
		ModelEntity effectModel = ViewModelEntity;

		if ( (IsLocalPawn && !Owner.IsFirstPersonMode) || !IsLocalPawn )
		{
			effectModel = EffectEntity;
		}

		return effectModel;
	}

	[ClientRpc]
	protected void TracerEffects( Vector3 end )
	{
		ModelEntity firingViewModel = GetEffectModel();

		if ( firingViewModel == null ) return;

		var effectEntity = firingViewModel;
		var muzzle = effectEntity.GetAttachment( "muzzle" );
		var tracer = Particles.Create( "particles/tracer_large.vpcf" );

		tracer.SetPosition( 1, muzzle.GetValueOrDefault().Position );
		tracer.SetPosition( 2, end );
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/flash_medium.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin(
				WeaponAsset.KickbackLength,
				WeaponAsset.KickbackSpeed,
				WeaponAsset.KickbackSize,
				WeaponAsset.KickbackRotation
			);
		}

		(ViewModelEntity as ViewModel)?.OnFire();
		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void BuildInput( InputBuilder inputBuilder )
	{
		base.BuildInput( inputBuilder );

		inputBuilder.ViewAngles += recoil;
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
		return (256 * (spread + WeaponAsset.Spread)).Clamp( 16, 512 );
	}
}
