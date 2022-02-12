using Sandbox;
using System;
using System.Linq;

namespace Fortwars
{
	public struct BlockMaterial : IEquatable<BlockMaterial>
	{
		public enum Type
		{
			Wood,
			Rubber,
			Metal
		}

		public Type ID { get; set; }
		public int BaseHealth { get; set; }
		public int PlayerLimit { get; set; }

		public int GetBuiltCount( Client client )
		{
			var blockMaterial = this;
			return Entity.All.OfType<FortwarsBlock>().Where( e => e.Client == client && e.BlockMaterial == blockMaterial ).Count();
		}

		public int GetRemainingCount( Client client ) => PlayerLimit - GetBuiltCount( client );

		/// <summary>
		/// Basic, has lower health but you can spawn lots of them
		/// </summary>
		public static readonly BlockMaterial Wood = new BlockMaterial( Type.Wood, 500, 25 );

		/// <summary>
		/// Basically just bounce pads, you can't have many of these but they're fun
		/// </summary>
		public static readonly BlockMaterial Rubber = new BlockMaterial( Type.Rubber, 1000, 5 );

		/// <summary>
		/// Ol' reliable, plenty of health but use them wisely
		/// </summary>
		public static readonly BlockMaterial Steel = new BlockMaterial( Type.Metal, 1000, 10 );

		public BlockMaterial( Type id, int baseHealth, int playerLimit )
		{
			ID = id;
			BaseHealth = baseHealth;
			PlayerLimit = playerLimit;
		}

		public static bool operator ==( BlockMaterial a, BlockMaterial b ) => a.Equals( b );
		public static bool operator !=( BlockMaterial a, BlockMaterial b ) => !(a == b);

		public bool Equals( BlockMaterial other )
		{
			if ( other.ID == this.ID )
				return true;

			return false;
		}

		public override bool Equals( object obj )
		{
			return obj is BlockMaterial && Equals( (BlockMaterial)obj );
		}
	}
}
