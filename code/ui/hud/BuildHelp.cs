using Fortwars.UI.Elements.Generic;
using Sandbox.UI;

namespace Fortwars.UI
{
	public class BuildHelp : Panel
	{
		public BuildHelp()
		{
			{
				var row = Add.Panel( "row" );
				row.Add.InputHint( Sandbox.InputButton.Menu, "Build Menu" );
			}
			{
				var row = Add.Panel( "row" );
				row.Add.InputHint( Sandbox.InputButton.Use, "Rotate" );
			}
			{
				var row = Add.Panel( "row" );
				row.Add.InputHint( Sandbox.InputButton.Attack1, "Move" );
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
