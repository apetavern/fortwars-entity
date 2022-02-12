using Sandbox;

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
	}
}
