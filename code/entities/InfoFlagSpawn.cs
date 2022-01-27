using Sandbox;

namespace Fortwars
{
	/// <summary>
	/// This is where the flag spawns.
	/// </summary>
	[Library( "info_flag_spawn" )]
	[Hammer.EntityTool( "Flag Spawn", "FortWars" )]
	[Hammer.EditorModel( "models/rust_props/small_junk/toilet_paper.vmdl" )]
	public partial class InfoFlagSpawn : Entity
	{
		[Property]
		public Team Team { get; set; }

		private FlagDisplay flag;

		public override void Spawn()
		{
			base.Spawn();

			flag = new FlagDisplay();
			flag.Position = Position;

			// make sure our clients know where to render flags
			// todo: probably better as a ClientRpc
			Transmit = TransmitType.Never;
		}

		public void ShowFlag()
		{
			Host.AssertServer();

			flag.EnableDrawing = true;
		}

		public void HideFlag()
		{
			Host.AssertServer();

			flag.EnableDrawing = false;
		}
	}
}
