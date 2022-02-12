using Sandbox;
using System.Linq;

namespace Fortwars
{
	public partial class FortwarsPlayer
	{
		[Event.Tick.Client]
		public static void OnClientTick()
		{
			var localPlayer = Local.Pawn as FortwarsPlayer;

			Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( player =>
			{
				if ( player.IsLocalPawn )
					return;

				if ( player.TeamID != localPlayer.TeamID )
				{
					player.GlowActive = true;
					player.GlowState = GlowStates.On;
					player.GlowColor = Color.Red;
				}
				else
				{
					player.GlowActive = false;
					player.GlowState = GlowStates.Off;
				}
			} );
		}
	}
}
