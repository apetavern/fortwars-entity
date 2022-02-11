using Sandbox;

namespace Fortwars
{
	/// <summary>
	/// Players capture the flag in this area.
	/// </summary>
	[Library( "func_flagzone" )]
	[Hammer.Solid]
	[Hammer.RenderFields]
	[Hammer.VisGroup( Hammer.VisGroup.Dynamic )]
	public partial class FuncFlagzone : BrushEntity
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

			if ( Game.Instance.Round is not CombatRound )
				return;

			if ( other is Player )
				Game.Instance.OnPlayerTouchFlagzone( other as FortwarsPlayer, Team );
		}

		public override void EndTouch( Entity other )
		{
			base.EndTouch( other );
		}
	}
}
