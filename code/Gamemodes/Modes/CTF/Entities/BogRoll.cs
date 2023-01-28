namespace Fortwars;

public class BogRoll : ModelEntity
{
	public Team Team { get; set; }
	private readonly string _modelPath = "models/items/bogroll/bogroll_w.vmdl";

	public override void Spawn()
	{
		Model = Model.Load( _modelPath );

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		EnableAllCollisions = true;

		SetMaterialGroup( Team, this );
	}

	public static void SetMaterialGroup( Team team, ModelEntity entity )
	{
		switch ( team )
		{
			case Team.Invalid: break;
			case Team.Blue:
				entity.SetMaterialGroup( 0 );
				break;
			case Team.Red:
				entity.SetMaterialGroup( 1 );
				break;
			default:
				break;
		}
	}
}
