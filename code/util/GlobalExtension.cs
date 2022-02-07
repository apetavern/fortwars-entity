using Sandbox;
using Sandbox.Internal.Globals;

namespace Fortwars
{
	public static class GlobalExtension
	{
		public static bool CheatsEnabled( this Global global )
		{
			return ConsoleSystem.GetValue( "sv_cheats" ) == "1";
		}
	}
}
