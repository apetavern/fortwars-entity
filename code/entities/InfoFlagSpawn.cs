using Sandbox;

namespace Fortwars
{
	[Library( "info_flag_spawn" )]
	public partial class InfoFlagSpawn : Entity
	{
		[Property( "team" )]
		public int Team { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			var flag = new FlagDisplay();
			flag.Position = Position;

			// make sure our clients know where to render flags
			// todo: probably better as a ClientRpc
			Transmit = TransmitType.Never;
		}
	}
}
