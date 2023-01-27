namespace Fortwars;

public class BogRoll : ModelEntity
{
	private readonly string _modelPath = "models/items/bogroll/bogroll_w.vmdl";

	public override void Spawn()
	{
		Model = Model.Load( _modelPath );

		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		EnableAllCollisions = true;
	}
}
