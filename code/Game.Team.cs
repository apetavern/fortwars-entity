using Sandbox;

namespace Fortwars
{
	partial class Game
	{
		// shit tier singleton hack since we can't init these in Game ctor
		// CreatePlayer is called before Game ctor
		private RedTeam _redTeam;
		private BlueTeam _blueTeam;

		public RedTeam RedTeam
		{
			get
			{
				if ( _redTeam == null )
					_redTeam = new RedTeam();
				return _redTeam;
			}
		}
		public BlueTeam BlueTeam
		{
			get
			{
				if ( _blueTeam == null )
					_blueTeam = new BlueTeam();
				return _blueTeam;
			}
		}
	}
}
