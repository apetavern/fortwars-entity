// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.Effects;

namespace Fortwars;

partial class FortwarsPlayer
{
	private static ScreenEffects postProcess;

	private static void SetupPostProcessing()
	{
		if ( !Host.IsClient )
			return;

		postProcess = Map.Camera.FindOrCreateHook<ScreenEffects>();
	}

	[Event.Frame]
	private static void UpdatePostProcessing()
	{
		if ( !Host.IsClient )
			return;

		if ( postProcess == null )
		{
			SetupPostProcessing();
		}

		postProcess.Enabled = true;

		postProcess.ChromaticAberration.Scale = 1.0f;
		postProcess.ChromaticAberration.Offset = Vector3.Up * 0.0005f;

		float vignette;
		if ( Local.Pawn is FortwarsPlayer { ActiveChild: FortwarsWeapon { IsAiming: true } } )
			vignette = 0.5f;
		else
			vignette = 0.0f;

		postProcess.Vignette.Intensity = postProcess.Vignette.Intensity.LerpTo( vignette, 10 * Time.Delta );
		
		postProcess.Vignette.Roundness = 1.0f;
		postProcess.Vignette.Smoothness = 1.0f;
		postProcess.Vignette.Color = Color.Black;

		if ( Local.Pawn is FortwarsPlayer player )
		{
			var healthT = player.Health.LerpInverse( 50f, 0f );

			if ( healthT > 0 )
			{
				postProcess.Vignette.Color = Color.Lerp( postProcess.Vignette.Color, Color.Red, healthT );
				postProcess.Vignette.Intensity = healthT;
			}

			// postProcess.Blur.Enabled = healthT > 0;
			// postProcess.Blur.Strength = healthT * 0.25f;

			postProcess.Pixelation = healthT * 0.25f;
			postProcess.Saturation = 1.0f - healthT;
			postProcess.MotionBlur.Scale = healthT;
			postProcess.MotionBlur.Samples = 32;
		}

		postProcess.Vignette.Intensity = 0.0f;
	}
}
