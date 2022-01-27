using Sandbox;

namespace Fortwars
{
	partial class FortwarsPlayer
	{
		[Net] public Team TeamID { get; set; }
		private BaseTeam _team;

		public BaseTeam Team
		{
			get => _team;

			set
			{
				// A player must be on a valid team.
				if ( value != null && value != _team )
				{
					_team = value;

					// make sure our player loadouts are set
					_team.OnPlayerSpawn( this );

					if ( IsServer )
					{
						TeamID = _team.ID;

						// You have to do this for now.
						// Client.SetValue( "team", TeamID );
					}
				}
			}
		}
	}
}
