namespace Fortwars;

/// <summary>
/// This is where players spawn.
/// </summary>
[Library( "info_player_teamspawn" )]
[Title( "Team Spawn" ), Category( "Fortwars" )]
[EditorModel( "models/citizen/citizen.vmdl" )]
[HammerEntity]
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
