using Sandbox;
using System;

namespace Fortwars
{
	public partial class FortwarsBlock : Prop
	{
		[Net] public BlockMaterial BlockMaterial { get; set; } = BlockMaterial.Metal;

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

				float t = MathF.Abs( ( Health / MaxHealth ) - 1f );
				RenderColor = Color.Lerp( Color.White, Color.Black, t );
			}
		}

		//TODO: Override OnKilled for custom gibs, in the Prop class it creates gibs and decides if it should explode or not.
		//Gibs are defined in Prop.DoGibs() which is private on Prop so we need to copy it over to do custom gibs.
	}
}
