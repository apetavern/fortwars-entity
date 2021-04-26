using Sandbox;

namespace Fortwars
{
	[Library( "func_wall_toggle" )]
	public partial class FuncWallToggle : Entity
	{
		public override void Spawn()
		{
			base.Spawn();

			Log.Info( "Spwaned func_wall_toggle" );
		}
	}
}
