using Sandbox;

namespace Fortwars
{
	/// <summary>
	/// Overrides game settings for this map
	/// </summary>
	[Library( "fw_config" )]
	[Hammer.EntityTool( "Map Config", "FortWars" )]
	[Hammer.EditorSprite( "materials/editor/fw_config.vmat" )]
	public partial class ConfigEntity : Entity
	{
		/// <summary>
		/// Build round time in seconds
		/// </summary>
		[Property] public int BuildTime { get; set; } = 150;

		/// <summary>
		/// Combat round time in seconds
		/// </summary>
		[Property] public int CombatTime { get; set; } = 300;

		/// <summary>
		/// Maximum number of rounds per game
		/// </summary>
		[Property] public int MaxRounds { get; set; } = 3;

		/// <summary>
		/// Wood blocks per player
		/// </summary>
		[Property] public int WoodBlocksPerPlayer { get; set; } = 30;

		/// <summary>
		/// Steel blocks per player
		/// </summary>
		[Property] public int SteelBlocksPerPlayer { get; set; } = 15;
	}
}
