using System.Linq;
using Sandbox;

namespace Fortwars
{
	public partial class FortwarsPlayer
	{
		[Event.Tick.Client]
		public static void OnClientTick()
		{
			Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( player =>
			{
				if ( player.IsLocalPawn )
					return;

				player.GlowActive = true;
				player.GlowState = GlowStates.On;

				switch ( player.TeamID )
				{
					case Fortwars.Team.Invalid:
						player.GlowActive = false;
						break;
					case Fortwars.Team.Red:
						player.GlowColor = Color.Red;
						break;
					case Fortwars.Team.Blue:
						player.GlowColor = Color.Blue;
						break;
				}
			} );
		}
	}
}
