namespace Fortwars
{
	/// <summary>
	/// Defines data for each block type
	/// </summary>
	[ModelDoc.GameData( "fw_block_node" )]
	public class BlockNode
	{
		public string Name { get; set; }
		public int AdditionalHealth { get; set; }
	}
}
