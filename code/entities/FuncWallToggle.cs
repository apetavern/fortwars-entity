using Sandbox;

namespace Fortwars
{
	[Library( "func_wall_toggle" )]
	[Hammer.Solid]
	[Hammer.RenderFields]
	[Hammer.VisGroup( Hammer.VisGroup.Dynamic )]
	public partial class FuncWallToggle : BrushEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
			CollisionGroup = CollisionGroup.Default;
			EnableAllCollisions = true;
			EnableTouch = true;
		}

		public void Show()
		{
			// TODO: Why does changing EnableAllCollisions do fuck all
			CollisionGroup = CollisionGroup.Default;
			EnableAllCollisions = true;
			EnableDrawing = true;
		}

		public void Hide()
		{
			// TODO: Ditto
			CollisionGroup = CollisionGroup.Never;
			EnableAllCollisions = false;
			EnableDrawing = false;
		}
	}
}
