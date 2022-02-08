using Sandbox.UI;

namespace Fortwars
{
	public class BuildHelp : Panel
	{
		public BuildHelp()
		{
			Add.InputHint( Sandbox.InputButton.Attack1, "Move" );
			Add.InputHint( Sandbox.InputButton.Attack2, "Freeze" );
			Add.InputHint( Sandbox.InputButton.Use, "Rotate" );
			Add.InputHint( Sandbox.InputButton.Run, "Snap" );
			Add.InputHint( Sandbox.InputButton.Menu, "Build Menu" );

			BindClass( "visible", () => Game.Instance.Round is BuildRound );
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
