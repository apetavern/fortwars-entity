namespace Fortwars;
public class NoclipMechanic : PlayerControllerMechanic
{
	protected override void OnActivate()
	{
		base.OnActivate();
		
		if ( Controller.TryGetMechanic<AirMoveMechanic>( out var am ) )
			am.Enabled = false;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( Controller.TryGetMechanic<AirMoveMechanic>( out var am ) )
			am.Enabled = true;
	}

	protected override void Simulate()
	{
		base.Simulate();
		var fwd = Player.MoveInput.x.Clamp( -1f, 1f );
		var left = Player.MoveInput.y.Clamp( -1f, 1f );
		var rotation = Player.EyeRotation;

		var vel = ( rotation.Forward * fwd ) + ( rotation.Left * left );

		if ( Input.Down( "jump" ) )
		{
			vel += Vector3.Up * 1;
		}

		vel = vel.Normal * 2000;

		if ( Input.Down( "run" ) )
			vel *= 5.0f;

		if ( Input.Down( "duck" ) )
			vel *= 0.2f;

		Velocity += vel * Time.Delta;

		if ( Velocity.LengthSquared > 0.01f )
		{
			Position += Velocity * Time.Delta;
		}

		Velocity = Velocity.Approach( 0, Velocity.Length * Time.Delta * 5.0f );
		GroundEntity = null;
	}
}
