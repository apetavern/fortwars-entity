using Sandbox;
using System;

namespace Fortwars
{
	public partial class FortwarsBlock : Prop
	{
		[Net] public BlockMaterial BlockMaterial { get; set; } = BlockMaterial.Wood;

		[Net] public Team TeamID { get; set; }

		[Net] public float MaxHealth { get; set; }

		public override void Spawn()
		{
			base.Spawn();
			Health = BlockMaterial.BaseHealth;
		}

		public void OnTeamIDChanged()
		{
			// More teams maybe in the future?
			switch ( TeamID )
			{
				case Team.Invalid: // Do we need a white block type maybe?
					break;
				case Team.Blue:
					SetMaterialGroup( 0 );
					break;
				case Team.Red:
					SetMaterialGroup( 1 );
					break;
				default:
					break;
			}
		}

		public override void OnNewModel( Model model )
		{
			base.OnNewModel( model );

			if ( IsServer )
			{
				var blockNode = model.GetData<BlockNode>();

				// This shouldn't ever be the case, but we'll catch it here just in case
				if ( blockNode == null )
					MaxHealth = BlockMaterial.BaseHealth;
				else
					MaxHealth = BlockMaterial.BaseHealth + blockNode.AdditionalHealth;

				Health = MaxHealth;
			}
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( Game.Instance.Round is not BuildRound )
			{
				base.TakeDamage( info );
				UpdateRenderColor();
			}
			if ( info.Attacker is FortwarsPlayer attacker )
			{
				this.DidDamage( To.Single( attacker ), info.Position, info.Damage, ((float)Health).LerpInverse( 100, 0 ) );
			}
		}

		[ClientRpc]
		public void DidDamage( Vector3 pos, float amount, float healthinv )
		{
			Sound.FromScreen( "dm.ui_attacker" )
				.SetPitch( 1 + healthinv * 1 );

			Hitmarker.Instance.OnHit( amount, false, true );
		}

		public void Heal( float amount, Vector3 position )
		{
			if ( Health + amount < MaxHealth )
			{
				var healParticles = Particles.Create( "particles/heal.vpcf", position );
				healParticles.SetPositionComponent( 1, 0, 2f );
			}

			Health += amount;
			Health = Health.Clamp( 0, MaxHealth );

			UpdateRenderColor();
		}


		private void UpdateRenderColor()
		{
			float t = MathF.Abs( (Health / MaxHealth) - 1f );
			RenderColor = Color.Lerp( Color.White, Color.Black, t );
		}

		//TODO: Override OnKilled for custom gibs, in the Prop class it creates gibs and decides if it should explode or not.
		//Gibs are defined in Prop.DoGibs() which is private on Prop so we need to copy it over to do custom gibs.
	}
}
