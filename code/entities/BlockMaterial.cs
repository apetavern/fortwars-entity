﻿using System;

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

		/// <summary>
		/// Basic, has lower health but you can spawn lots of them
		/// </summary>
		public static readonly BlockMaterial Wood = new BlockMaterial( Type.Wood, 50, 50 );

		/// <summary>
		/// Basically just bounce pads, you can't have many of these but they're fun
		/// </summary>
		public static readonly BlockMaterial Rubber = new BlockMaterial( Type.Rubber, 100, 5 );

		/// <summary>
		/// Ol' reliable, plenty of health but use them wisely
		/// </summary>
		public static readonly BlockMaterial Metal = new BlockMaterial( Type.Metal, 100, 25 );

		public BlockMaterial( Type id, int baseHealth, int playerLimit )
		{
			ID = id;
			BaseHealth = baseHealth;
			PlayerLimit = playerLimit;
		}

		public bool Equals( BlockMaterial other )
		{
			if ( other.ID == this.ID )
				return true;

			return false;
		}
	}
}