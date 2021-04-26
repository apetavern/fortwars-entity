using Sandbox;

namespace Fortwars
{
	[Library( "info_player_teamspawn" )]
	public partial class InfoPlayerTeamspawn : Entity
	{
		[HammerProp( "team" )]
		public int Team { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Never;
		}
	}
}
