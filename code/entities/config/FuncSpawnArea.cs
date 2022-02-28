using Sandbox;

namespace Fortwars
{
	/// <summary>
	/// Players capture the flag in this area.
	/// </summary>
	[Library( "func_spawn_area" )]
	[Hammer.Solid]
	[Hammer.RenderFields]
	[Hammer.VisGroup( Hammer.VisGroup.Dynamic )]
	public partial class FuncSpawnArea : BrushEntity
	{
		[Property]
		public Team Team { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel( PhysicsMotionType.Static );
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			Transmit = TransmitType.Never;
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( other.IsWorld )
				return;

			if ( other is FortwarsPlayer player && player.TeamID != Team )
				other.TakeDamage( DamageInfo.Generic( 10000f ) );
		}
	}
}
