using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fortwars
{
	public struct BlockType
	{
		public BlockType( int additionalHealth )
		{
			AdditionalHealth = additionalHealth;
		}

		public int AdditionalHealth { get; set; }

		public static Dictionary<string, BlockType> BlockTypes { get; } = new()
		{
			//
			// 2D blocks
			//
			{ "1x2", new BlockType( 10 ) },
			{ "1x4", new BlockType( 20 ) },
			{ "3x2", new BlockType( 30 ) },

			//
			// 3D Blocks
			//
			{ "1x1x1", new BlockType( 30 ) },
			{ "1x2x1", new BlockType( 40 ) },
		};

		public static BlockType FromModelName( string modelName )
		{
			var regex = new Regex( @"\d\d?x\d\d?(x\d\d?)?" );
			var regexMatch = regex.Match( modelName );
			if ( !regexMatch.Success )
				return new BlockType();

			if ( BlockTypes.TryGetValue( regexMatch.Value, out var blockType ) )
				return blockType;

			return new BlockType();
		}
	}
}
