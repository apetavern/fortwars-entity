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
		/// Wood blocks per player
		/// </summary>
		[Property] public int WoodBlocksPerPlayer { get; set; } = 30;

		/// <summary>
		/// Steel blocks per player
		/// </summary>
		[Property] public int SteelBlocksPerPlayer { get; set; } = 15;

		public override void Spawn()
		{
			base.Spawn();

			Log.Trace( $"Spawned config {this}" );

			BuildRound.RoundLength = BuildTime;
			CombatRound.RoundLength = CombatTime;

			Transmit = TransmitType.Never;
		}
	}
}
