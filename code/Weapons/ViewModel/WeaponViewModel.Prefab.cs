namespace Fortwars;

public partial class WeaponViewModel
{
	//// General
	[Prefab, Net] public float Weight { get; set; }
	[Prefab, Net] public float WeightReturnForce { get; set; }
	[Prefab, Net] public float WeightDamping { get; set; }
	[Prefab, Net] public float AccelerationDamping { get; set; }
	[Prefab, Net] public float VelocityScale { get; set; }
	[Prefab, Net] public float RotationalPivotForce { get; set; }
	[Prefab, Net] public float RotationalScale { get; set; }

	//// Walking & Bob
	[Prefab, Net] public Vector3 WalkCycleOffset { get; set; }
	[Prefab, Net] public Vector2 BobAmount { get; set; }

	//// Global
	[Prefab, Net] public float GlobalLerpPower { get; set; }
	[Prefab, Net] public Vector3 GlobalPositionOffset { get; set; }
	[Prefab, Net] public Angles GlobalAngleOffset { get; set; }

	//// Crouching
	[Prefab, Net] public Vector3 CrouchPositionOffset { get; set; }
	[Prefab, Net] public Angles CrouchAngleOffset { get; set; }

	//// Avoidance
	/// <summary>
	/// The max position offset when avoidance comes into play.
	/// Avoidance is when something is in the way of the weapon.
	/// </summary>
	[Prefab, Net] public Vector3 AvoidancePositionOffset { get; set; }

	/// <summary>
	/// The max angle offset when avoidance comes into play.
	/// Avoidance is when something is in the way of the weapon.
	/// </summary>
	[Prefab, Net] public Angles AvoidanceAngleOffset { get; set; }

	//// Sprinting
	[Prefab, Net] public Vector3 SprintPositionOffset { get; set; }
	[Prefab, Net] public Angles SprintAngleOffset { get; set; }

	/// Aim Down Sight
	[Prefab, Net] public Vector3 AimPositionOffset { get; set; }
	[Prefab, Net] public Angles AimAngleOffset { get; set; }
	[Prefab, Net] public float AimFovOffset { get; set; }

}
