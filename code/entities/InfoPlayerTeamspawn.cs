using Sandbox;

namespace Fortwars
{
	/// <summary>
	/// This is where players spawn.
	/// </summary>
	[Library( "info_player_teamspawn" )]
	[Hammer.EntityTool( "Team Spawn", "FortWars" )]
	[Hammer.EditorModel( "models/citizen/citizen.vmdl" )]
	public partial class InfoPlayerTeamspawn : Entity
	{
		[Property]
		public Team Team { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			Transmit = TransmitType.Never;
		}
	}
}
