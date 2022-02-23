using Sandbox;

namespace Fortwars
{
	public partial class BigHealthPickup : Pickup
	{
		[Net] int uses { get; set; } = 5;

		float ThrowSpeed = 100f;

		[Net] bool Landed { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/items/medkit/medkit_w.vmdl" );
			SetBodyGroup( 0, 1 );
			Scale = 0.4f;

			Components.Get<BobbingComponent>().NoPitch = true;
		}

		[Event.Tick.Server]
		public void OnTick()
		{
			if ( Landed )
			{
				SetAnimParameter( "deployed", true );
				Scale = 0.4f;
				return;
			}
			Velocity += ThrowSpeed * Rotation.Forward * Time.Delta;
			Velocity += Map.Physics.Gravity * 0.5f * Time.Delta;

			Rotation = Rotation.LookAt( -Velocity.Normal.WithZ( 0 ), Vector3.Up );

			var target = Position + Velocity * Time.Delta;
			var tr = Trace.Ray( Position, target ).Ignore( Owner ).Run();

			//DebugOverlay.Line( tr.StartPos, tr.EndPos );

			if ( tr.Hit )
			{
				SetAnimParameter( "deployed", true );
				Landed = true;
			}

			Position = target;
		}

		public override void StartTouch( Entity other )
		{
			if ( !Landed )
			{
				return;
			}
			base.StartTouch( other );

			if ( !IsServer )
				return;

			if ( other is not FortwarsPlayer player )
				return;

			if ( player.Health >= 100 )
				return;

			player.Health += 50f;
			player.Health = player.Health.Clamp( 0, 100 );

			uses--;

			if ( uses <= 0 )
			{
				this.Delete();
			}
		}
	}
}
