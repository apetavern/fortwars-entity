using Sandbox;

namespace Fortwars
{
	public class Spawner : Entity
	{
		protected TimeUntil timeUntilSpawn = 0;

		public void ResetSpawnTimer()
		{
			timeUntilSpawn = 10;
		}
	}

	public class Spawner<T> : Spawner where T : Pickup, new()
	{
		private T pickup;

		public override void Spawn()
		{
			base.Spawn();

			SpawnPickup();
		}

		private void SpawnPickup()
		{
			pickup = new();
			pickup.Position = Position;
			pickup.Spawner = this;
		}

		[Event.Tick.Server]
		public void OnServerTick()
		{
			if ( timeUntilSpawn < 0 && !pickup.IsValid() )
			{
				SpawnPickup();
			}
		}
	}
}
