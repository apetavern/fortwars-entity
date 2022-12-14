// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

[Library( "fw_grenade", Title = "Grenade" )]
[EditorModel( "models/fw_grenade.vmdl" )]
[Title( "Grenade" )]
partial class Grenade : Carriable
{
	public static readonly Model WorldModel = Model.Load( "models/weapons/fraggrenade/fraggrenade_w.vmdl" );
	public override string ViewModelPath => "models/weapons/fraggrenade/fraggrenade_v.vmdl";

	public float PrimaryRate => 1.0f;
	public float SecondaryRate => 1.0f;
	public float ReloadTime => 1.0f;
	public int Count { get; private set; } = 3000;

	[Net, Predicted] private bool PlayerThrown { get; set; }
	[Net, Predicted] private TimeSince TimeSinceThrow { get; set; } = 0;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		//DebugOverlay.ScreenText( $"TimeSinceThrow:       {TimeSinceThrow}", 21 );

		if ( CanThrow() )
		{
			PlayerThrown = true;
			TimeSinceThrow = 0;

			if ( Game.IsClient )
				ViewModelEntity?.SetAnimParameter( "fire", true );
		}

		if ( PlayerThrown && TimeSinceThrow >= 0.3f )
		{
			PlayerThrown = false;
			Throw();
		}
	}

	public bool CanThrow()
	{
		return TimeSinceThrow > 2 && Input.Released( InputButton.PrimaryAttack );
	}

	public void Throw()
	{
		if ( Owner is not FortwarsPlayer player ) return;

		if ( Count <= 0 )
			return;

		Count--;

		// TODO
		// _ = new Sandbox.ScreenShake.Perlin( 1.0f, 0.5f, 4.0f, 0.6f );

		Game.SetRandomSeed( Time.Tick );

		if ( Game.IsServer )
		{
			using ( Prediction.Off() )
			{
				var grenade = new Throwable
				{
					Position = Owner.EyePosition + Owner.EyeRotation.Forward * 3.0f,
					Owner = Owner
				};

				grenade.PhysicsBody.Velocity = Owner.EyeRotation.Forward * 600.0f + Owner.EyeRotation.Up * 200.0f
					+ ( Owner.EyeRotation.Forward * Owner.Velocity.Length );

				grenade.Tags.Add( "debris" );

				_ = grenade.ExplodeAfterSeconds( 3.0f );
			}
		}
	}
	public override void SimulateAnimator( CitizenAnimationHelper animHelper )
	{
		animHelper.HoldType = CitizenAnimationHelper.HoldTypes.HoldItem;
		animHelper.Handedness = CitizenAnimationHelper.Hand.Right;
		// animHelper.Pose = 0.11f;
		// animHelper.AimBodyWeight = 1;
	}
}
