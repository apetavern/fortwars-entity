using Sandbox;

namespace Fortwars
{
	[Library( "info_flag_spawn" )]
	public partial class InfoFlagSpawn : Entity
	{
		[Property( "team" )]
		public int Team { get; set; }

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
