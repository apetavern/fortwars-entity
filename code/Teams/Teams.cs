namespace Fortwars;

public enum Team
{
	Invalid = -1,
	Red,
	Blue
}

public partial class TeamSystem
{
	public static Team GetTeam( IClient client )
	{
		return client?.Components.Get<TeamComponent>()?.Team ?? Team.Invalid;
	}

	public static IEnumerable<Team> GetTeams()
	{
		return GamemodeSystem.Instance?.Teams;
	}

	public static Team GetTeamWithFewestPlayers()
	{
		return GetTeams()
			.Where( t => t != Team.Invalid )
			.MinBy( t => t.Count() );
	}
}

public static class TeamExtensions
{
	public static IEnumerable<IClient> GetClients( this Team team )
	{
		return Game.Clients.Where( x => TeamSystem.GetTeam( x ) == team );
	}

	public static int Count( this Team team )
	{
		return GetClients( team ).Count();
	}
}
