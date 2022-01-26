using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars.UI
{
	public class BuildHelp : Panel
	{
		public BuildHelp()
		{
			{
				var row = Add.Panel( "row" );
				row.Add.Label( "Q", "key" );
				row.Add.Label( "Build Menu", "desc" );
			}
			{
				var row = Add.Panel( "row" );
				row.Add.Label( "E", "key" );
				row.Add.Label( "Rotate", "desc" );
			}
			{
				var row = Add.Panel( "row" );
				row.Add.Label( "LMB", "key" );
				row.Add.Label( "Move", "desc" );
			}
		}

		public override void Tick()
		{
			var game = Game.Instance;
			if ( game == null ) return;

			var round = game.Round;
			if ( round == null ) return;
		}
	}
}
