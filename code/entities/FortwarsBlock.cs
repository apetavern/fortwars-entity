using Sandbox;
using System;

namespace Fortwars
{
	public partial class FortwarsBlock : Prop
	{
		[Net] public BlockType BlockType { get; set; }
		[Net] public BlockMaterial BlockMaterial { get; set; } = BlockMaterial.Metal;

		[Net] public Team TeamID { get; set; }

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
				BlockType = BlockType.FromModelName( model.Name );

				Health += BlockType.AdditionalHealth;
				Log.Trace( Health );
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

				float t = MathF.Abs( Health / 100f - 1f );
				RenderColor = Color.Lerp( Color.White, Color.Black, t );
			}
		}
	}
}
