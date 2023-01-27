namespace Fortwars;

public partial class TeamComponent : EntityComponent
{
	[Net, Change( nameof( OnTeamChanged ) )]
	private Team _team { get; set; }

	public Team Team
	{
		get => _team;
		set
		{
			var prev = _team;
			_team = value;
			OnTeamChanged( prev, _team );
		}
	}

	protected void OnTeamChanged( Team prev, Team team )
	{
		// do something
	}
}
