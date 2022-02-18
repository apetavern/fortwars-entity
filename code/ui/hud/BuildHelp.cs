using Sandbox.UI;

namespace Fortwars
{
	public class BuildHelp : Panel
	{
		public BuildHelp()
		{
			Add.InputHint( Sandbox.InputButton.Attack1, "Move" );
			Add.InputHint( Sandbox.InputButton.Use, "Rotate" );
			Add.InputHint( Sandbox.InputButton.Run, "Snap Rotation" );
			Add.InputHint( Sandbox.InputButton.Menu, "Build Menu" );

			BindClass( "visible", () => Game.Instance.Round is BuildRound );
		}
	}
}
