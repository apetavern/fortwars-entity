using Sandbox;

namespace Fortwars
{
	public partial class FlagDisplay : ModelEntity, IShowIcon
	{
		[Net] public Team Team { get; set; }

		#region Icons
		Vector3 IShowIcon.IconWorldPosition() => this.Position;
		string IShowIcon.CustomClassName() => Team.ToString();
		string IShowIcon.NonDiegeticIcon() => "flag";
		string IShowIcon.SpatialIcon() => "flag";
		#endregion

		public override void Spawn()
		{
			base.Spawn();

			MoveType = MoveType.Physics;
			CollisionGroup = CollisionGroup.Interactive;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			SetModel( "models/rust_props/small_junk/toilet_paper.vmdl" );
			Scale = 5.0f;

			var bobbingComponent = Components.Create<BobbingComponent>();
			bobbingComponent.CenterOffset = Vector3.Down * 3f;
		}
	}
}
