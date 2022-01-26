using Sandbox;

namespace Fortwars
{
	[Library( "info_player_teamspawn" )]
	public partial class InfoPlayerTeamspawn : Entity
	{
		[Property( "team" )]
		public int Team { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Never;
		}
	}
}
