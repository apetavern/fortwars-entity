using Sandbox;
using System;

namespace Fortwars
{
	public partial class FortwarsBlock : Prop
	{
		[Net] public Team TeamID { get; set; }

		public override void Spawn()
		{
			base.Spawn();
			Health += 90f; //Smarter system required. Should the block know what type it is? Probably.
		}

		public void OnTeamIDChanged()
		{
			//More teams maybe in the future?
			switch ( TeamID )
			{
				case Team.Invalid: //Do we need a white block type maybe?
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
			if ( IsServer ) //Only do it on server so it doesn't get added to twice.
			{
				if ( model.Name.Contains( "2" ) ) //Most blocks
				{
					Health += 10f;
				}

				if ( model.Name.Contains( "3" ) ) //3x2 block
				{
					Health += 30f;
				}

				int XCount = model.Name.Split( 'x' ).Length - 1;

				if ( XCount > 1 ) //Any 3D block that isn't a panel
				{
					Health += 30f;
				}

				if ( model.Name.Contains( "4" ) ) //1x4 block
				{
					Health += 20f;
				}
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

				RenderColor = Color.Lerp( Color.White, Color.Black, MathF.Abs( Health / 100f - 1f ) );
			}
		}
	}
}
