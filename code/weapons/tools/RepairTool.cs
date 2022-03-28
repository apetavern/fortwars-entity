// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;

namespace Fortwars
{
    [Library( "repairtool", Title = "Repair Tool" )]
    public partial class RepairTool : MeleeWeapon
    {
        public override float PrimaryRate => 2.0f;
        public override string ViewModelPath => "models/weapons/amhammer/amhammer_v.vmdl";

        public override void Spawn()
        {
            base.Spawn();

            SetModel( "models/weapons/amhammer/amhammer_w.vmdl" );
        }

        public override void Simulate( Client player )
        {
            if ( !Owner.IsValid() )
                return;

            if ( CanPrimaryAttack() )
            {
                using ( LagCompensation() )
                {
                    TimeSincePrimaryAttack = 0;
                    AttackPrimary();
                }
            }
        }

        public override void AttackPrimary()
        {
            var player = Owner as FortwarsPlayer;
            player.SetAnimParameter( "b_attack", true );
            foreach ( var tr in TraceHit( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 128f ) )
            {
                ViewModelEntity?.SetAnimParameter( "hit", tr.Hit );

                if ( tr.Entity is FortwarsBlock block && block.TeamID == player.TeamID && block.IsValid )
                {
                    block?.Heal( 50, tr.EndPosition );
                    continue;
                }

                tr.Entity.TakeDamage( DamageInfo.FromBullet( tr.EndPosition, -tr.Normal * 10f, 10 ) );
            }

            ViewModelEntity?.SetAnimParameter( "fire", true );
        }

        public override void SimulateAnimator( PawnAnimator anim )
        {
            anim.SetAnimParameter( "holdtype", (int)HoldTypes.HoldItem );
            anim.SetAnimParameter( "holdtype_handedness", (int)HoldHandedness.RightHand );
            anim.SetAnimParameter( "holdtype_pose_hand", 0.07f );
            anim.SetAnimParameter( "holdtype_attack", 1 );
        }
    }
}
