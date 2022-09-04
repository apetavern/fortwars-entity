// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

[Library( "jumppadtool", Title = "Jump Pad" )]
public partial class JumpPadTool : DropTool
{
	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/items/jumppad/jumppad.vmdl" );
		TimeSinceLastDrop = DropTimeDelay;
	}

	public override void SpawnPickup()
	{
		ThrowProjectile( new JumpPad() );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		if ( TimeSinceLastDrop < DropTimeDelay )
		{
			EnableDrawing = false;
			anim.SetAnimParameter( "holdtype", (int)HoldTypes.None );
			anim.SetAnimParameter( "holdtype_handedness", (int)HoldHandedness.TwoHands );
			anim.SetAnimParameter( "holdtype_pose_hand", 0f );
			anim.SetAnimParameter( "holdtype_attack", 1 );
		}
		else
		{
			EnableDrawing = true;
			anim.SetAnimParameter( "holdtype", (int)HoldTypes.HoldItem );
			anim.SetAnimParameter( "holdtype_handedness", (int)HoldHandedness.OverHead );
			anim.SetAnimParameter( "holdtype_pose_hand", 0f );
			anim.SetAnimParameter( "holdtype_attack", 1 );
		}
	}
}
