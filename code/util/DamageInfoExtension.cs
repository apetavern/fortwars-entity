// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public static class DamageInfoExtension
{
	public static DamageInfo FromFall( float damage, Entity attacker )
	{
		return new DamageInfo
		{
			Flags = DamageFlags.Fall,
			Damage = damage,
			Attacker = attacker
		};
	}

	public static DamageInfo FromProjectile( float damage, Vector3 position, Vector3 force, Entity attacker )
	{
		return new DamageInfo
		{
			Flags = DamageFlags.Blast,
			Position = position,
			Damage = damage,
			Attacker = attacker,
			Force = force,
		};
	}
}
