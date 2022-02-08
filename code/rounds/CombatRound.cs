using Sandbox;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars
{
	public class CombatRound : BaseRound
	{
		public override string RoundName => "Combat";
		public override int RoundDuration => 300;

		protected override void OnStart()
		{
			Log.Info( "Started Combat Round" );

			if ( Host.IsServer )
			{
				Player.All.OfType<FortwarsPlayer>().ToList().ForEach( ( player ) =>
				{
					SetupInventory( player );
					Game.Instance.MoveToSpawnpoint( player );
				} );
			}

			foreach ( var wall in Entity.All.OfType<FuncWallToggle>() )
				wall.Hide();
		}

		public override void SetupInventory( Player player )
		{
			base.SetupInventory( player );
			player.Inventory.Add( FortwarsWeapon.FromPath( "/data/weapons/aiax50.fwweapon" ), true );
		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Combat Round" );
		}

		protected override void OnTimeUp()
		{
			Game.Instance.ChangeRound( new BuildRound() );
		}

		public override void OnPlayerKilled( Player player )
		{
			_ = StartRespawnTimer( player );

			base.OnPlayerKilled( player );
		}

		private async Task StartRespawnTimer( Player player )
		{
			await Task.Delay( 1000 );

			player.Respawn();
		}

		public override void OnPlayerSpawn( Player player )
		{
			base.OnPlayerSpawn( player );
		}
	}
}
