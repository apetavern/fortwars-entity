using Sandbox;

namespace Fortwars
{
	[Library( "func_wall_toggle" )]
	public partial class FuncWallToggle : ModelEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel(PhysicsMotionType.Static);
			CollisionGroup = CollisionGroup.Default;
			EnableAllCollisions = true;
			EnableTouch = true;
		}

		public void Show()
        {
			EnableAllCollisions = true;
			EnableDrawing = true;
        }

		public void Hide()
        {
			EnableAllCollisions = false;
			EnableDrawing = false;
        }
	}
}
