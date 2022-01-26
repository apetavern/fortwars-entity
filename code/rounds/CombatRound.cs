using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fortwars
{
	public class CombatRound : BaseRound
	{
		public override string RoundName => "Combat";
		public override int RoundDuration => 120;

		protected override void OnStart()
		{
			Log.Info("Started Combat Round");

			if (Host.IsServer)
			{
				foreach (var player in Player.All)
					(player as FortwarsPlayer)?.Respawn();
			}

			foreach (var wall in Entity.All.OfType<FuncWallToggle>())
				wall.Hide();
		}

		protected override void OnFinish()
		{
			Log.Info("Finished Combat Round");
		}

		protected override void OnTimeUp()
		{
			Game.Instance.ChangeRound(new BuildRound());
		}

		public override void OnPlayerKilled(Player player)
		{
			_ = StartRespawnTimer(player);

			base.OnPlayerKilled(player);
		}

		private async Task StartRespawnTimer(Player player)
		{
			await Task.Delay(1000);

			player.Respawn();
		}

		public override void OnPlayerSpawn(Player player)
		{
			base.OnPlayerSpawn(player);
		}
	}
}
