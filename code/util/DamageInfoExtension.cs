﻿using Sandbox;

namespace Fortwars
{
	public static class DamageInfoExtension
	{
		public static DamageInfo FromFall( float damage )
		{
			return new DamageInfo
			{
				Flags = DamageFlags.Fall,
				Damage = damage
			};
		}

		public static DamageInfo FromProjectile( float damage, Vector3 force, Entity attacker )
		{
			return new DamageInfo
			{
				Flags = DamageFlags.Blast,
				Damage = damage,
				Attacker = attacker,
				Force = force,
			};
		}
	}
}