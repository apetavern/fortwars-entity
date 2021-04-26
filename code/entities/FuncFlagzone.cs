using Sandbox;

namespace Fortwars
{
	[Library("func_flagzone")]
	public partial class FuncFlagzone : ModelEntity
	{
		[HammerProp("team")]
		public int Team { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel(PhysicsMotionType.Static);
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			Transmit = TransmitType.Never;
		}

		public override void StartTouch(Entity other)
		{
			base.StartTouch(other);

			if (other.IsWorld)
				return;

			if (other is Player)
            {
				var player = other as Player;
				Log.Info($"{player.Name} StartTouch flag belonging to team {Team}");
			}
		}

		public override void EndTouch(Entity other)
		{
			base.EndTouch(other);

			if (other.IsWorld)
				return;

			if (other is Player)
			{
				var player = other as Player;
				Log.Info($"{player.Name} EndTouch flag belonging to team {Team}");
			}
		}

	}
}
