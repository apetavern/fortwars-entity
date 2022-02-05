using Sandbox;

namespace Fortwars
{
	public partial class FlagDisplay : ModelEntity, IShowIcon
	{
		[Net,Change("OnTeamChange")] public Team Team { get; set; }

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
			CollisionGroup = CollisionGroup.Weapon;
			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			SetModel( "models/items/bogroll/bogroll_w.vmdl" );
			Scale = 1.5f;

			var bobbingComponent = Components.Create<BobbingComponent>();
			bobbingComponent.PositionOffset = Vector3.Up * 10f;
		}

		public void OnTeamChange()
		{
			switch ( Team )
			{
				case Team.Invalid:
					break;
				case Team.Red:
					SetMaterialGroup( 1 );
					break;
				case Team.Blue:
					SetMaterialGroup( 0 );
					break;
				default:
					break;
			}
		}
	}
}
