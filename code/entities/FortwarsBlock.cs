using Sandbox;
using System;

namespace Fortwars
{
	public partial class FortwarsBlock : Prop
	{
		public override void Spawn()
		{
			base.Spawn();
			Health = 100f; //Smarter system required. Should the block know what type it is? Probably.
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
		}

		public override void TakeDamage( DamageInfo info )
		{
			base.TakeDamage( info );
			
			RenderColor = Color.Lerp( Color.White, Color.Black, MathF.Abs(Health / 100f - 1f) );
		}
	}
}
