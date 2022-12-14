// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using static Sandbox.CitizenAnimationHelper;

namespace Fortwars;

[Library( "jumppadtool", Title = "Jump Pad" )]
public partial class JumpPadTool : DropTool
{
	public override string ViewModelPath => "models/items/jumppad/jumppad_v.vmdl";

	public override float DropTimeDelay => 5f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/items/jumppad/jumppad_w.vmdl" );
		TimeSinceLastDrop = DropTimeDelay;
	}

	public override void SpawnPickup()
	{
		ThrowProjectile( new JumpPad() );
	}

	public override void SimulateAnimator( CitizenAnimationHelper animHelper )
	{
		if ( TimeSinceLastDrop < DropTimeDelay )
		{
			EnableDrawing = false;
			animHelper.HoldType = HoldTypes.None;
			animHelper.Handedness = Hand.Both;
			// anim.SetAnimParameter( "holdtype_pose_hand", 0f );
			// anim.SetAnimParameter( "holdtype_attack", 1 );
		}
		else
		{
			EnableDrawing = true;
			animHelper.HoldType = HoldTypes.HoldItem;
			animHelper.Handedness = Hand.Both;

			// anim.SetAnimParameter( "holdtype", (int)HoldTypes.HoldItem );
			// anim.SetAnimParameter( "holdtype_handedness", (int)HoldHandedness.OverHead );
			// anim.SetAnimParameter( "holdtype_pose_hand", 0f );
			// anim.SetAnimParameter( "holdtype_attack", 1 );

		}

		// anim.SetAnimParameter( "useleftik", false );
		// anim.SetAnimParameter( "gunup", 0f );
		// anim.SetAnimParameter( "gundown", 0f );
	}
}
