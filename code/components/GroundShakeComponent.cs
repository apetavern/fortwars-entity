// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Linq;

namespace Fortwars;

/// <summary>
/// Shake the ground when these objects hit something
/// </summary>
public class GroundShakeComponent : EntityComponent<FortwarsBlock>
{
	private TimeSince timeSinceLastShake;
	private bool wasShakingLastFrame;

	// when we multiply shake strength with mass, what should we scale it by
	private float massScale => 0.0025f;

	private float strengthScale => 0.01f;
	private float strengthMax => 10;

	[Event.Frame]
	public void FrameUpdate()
	{
		var velocity = Entity.Velocity;

		if ( !Entity.IsValid() || !Entity.PhysicsBody.IsValid() )
			return;

		// do a sweep to check if we're colliding with the world
		var tr = Trace.Sweep( Entity.PhysicsBody, Entity.Transform ).WorldOnly().Run();

		if ( timeSinceLastShake < 0.2f )
			return;

		if ( tr.Hit && velocity.Length > 1 && !wasShakingLastFrame )
		{
			var shakeStrength = Vector3.DistanceBetween( CurrentView.Position, Entity.PhysicsBody.MassCenter ).LerpInverse( 512, 0 );
			shakeStrength *= velocity.Length;
			shakeStrength *= Entity.PhysicsBody.Mass * massScale;
			shakeStrength *= strengthScale;
			shakeStrength = shakeStrength.Clamp( 0, strengthMax );

			_ = new Sandbox.ScreenShake.Perlin( 0.5f, 1f, shakeStrength );
			timeSinceLastShake = 0;
			wasShakingLastFrame = true;
		}
		else
		{
			wasShakingLastFrame = false;
		}
	}

	[Event.Frame]
	public static void SystemUpdate()
	{
		foreach ( var entity in Sandbox.Entity.All.OfType<FortwarsBlock>() )
		{
			if ( !entity.IsValid() )
			{
				var existingGroundShake = entity.Components.Get<GroundShakeComponent>();
				existingGroundShake?.Remove();
				continue;
			}

			entity.Components.GetOrCreate<GroundShakeComponent>();
		}
	}
}
