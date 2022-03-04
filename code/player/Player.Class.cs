using Sandbox;

namespace Fortwars
{
	partial class FortwarsPlayer
	{
		public Class Class { get; set; } = new AssaultClass();

		public bool CanChangeClass => InSpawnRoom || LifeState != LifeState.Alive;
		[Net, Predicted] public bool InSpawnRoom { get; set; }

		public void AssignClass( Class newClass )
		{
			if ( !CanChangeClass )
			{
				MessageFeed.AddMessage(
					To.Single( Client ),
					"clear",
					"Can't change class",
					"You can only change class in a spawn room." );

				return;
			}

			Class?.Cleanup( Inventory as Inventory );
			Class = newClass;

			this.Reset();
			Game.Instance.Round.SetupInventory( this );
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( other is FuncSpawnArea )
				InSpawnRoom = true;
		}

		public override void EndTouch( Entity other )
		{
			base.EndTouch( other );

			if ( other is FuncSpawnArea )
				InSpawnRoom = false;
		}
	}
}
