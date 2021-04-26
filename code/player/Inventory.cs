using Sandbox;
using System;
using System.Linq;

namespace Fortwars
{
	public partial class Inventory : BaseInventory
	{
		public Inventory( Player player ) : base( player )
		{

		}

		public override bool Add( Entity ent, bool makeActive = false )
		{
			/*var player = Owner as DeathmatchPlayer;
			var weapon = ent as BaseDmWeapon;
			var notices = !player.SupressPickupNotices;

			//
			// We don't want to pick up the same weapon twice
			// But we'll take the ammo from it Winky Face
			//
			if ( weapon != null && IsCarryingType( ent.GetType() ) )
			{
				var ammo = weapon.AmmoClip;
				var ammoType = weapon.AmmoType;

				if ( ammo > 0 )
				{
					player.GiveAmmo( ammoType, ammo );

					if ( notices )
					{
						Sound.FromWorld( "dm.pickup_ammo", ent.WorldPos );
						PickupFeed.OnPickup( player, $"+{ammo} {ammoType}" );
					}
				}

				ItemRespawn.Taken( ent );

				// Despawn it
				ent.Delete();
				return false;
			}

			if ( weapon != null && notices )
			{
				Sound.FromWorld( "dm.pickup_weapon", ent.WorldPos );
				PickupFeed.OnPickup( player, $"{ent.ClassInfo.Title}" );
			}


			ItemRespawn.Taken( ent );*/
			return base.Add( ent, makeActive );
		}

		public bool IsCarryingType( Type t )
		{
			return List.Any( x => x.GetType() == t );
		}
	}
}
