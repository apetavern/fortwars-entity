// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public static class Bobbing
{
	public static Vector3 CalcBobbingOffset( ref float currentBob, Vector3 velocity, Entity entity )
	{
		// Bob up and down based on our walk movement
		var forwardVel = velocity.Cross( Vector3.Up );
		var speed = forwardVel.Length.LerpInverse( 0, 400 );
		var speed2 = forwardVel.Length.LerpInverse( 200, 400 );
		var left = Vector3.Left;
		var up = Vector3.Up;

		bool applyBob = entity.GroundEntity != null;

		if ( applyBob )
			currentBob += Time.Delta * 25.0f * speed;

		// Reset if we're not really moving
		if ( speed < 0.1f )
		{
			currentBob = currentBob.LerpTo( 0, 5 * Time.Delta );
		}

		// Limit to 1 cycle
		// https://www.desmos.com/calculator/8ued619kst
		currentBob = currentBob.UnsignedMod( MathF.PI * 4f );

		float sprintMul = 2.0f * speed2;
		float speedMul = speed + sprintMul;

		if ( entity is FortwarsPlayer { Controller: FortwarsWalkController { DuckSlide.IsActiveSlide: true } } )
			speedMul *= 0.25f;

		var offset = up * MathF.Sin( currentBob ) * speedMul;
		offset += left * MathF.Sin( currentBob * 0.5f ) * speedMul;

		return offset;
	}
}
