namespace Fortwars;

[Category( "Fortwars" )]
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

	public override void StartTouch( Entity other )
	{
		Log.Info( $"Fortwars CTF: Touched BogRoll belonging to {Team}" );
		if ( GamemodeSystem.Instance is not CaptureTheFlag ctf )
			return;

		if ( ctf.CurrentState != CaptureTheFlag.GameState.Combat )
			return;

		if ( other is not Player player )
			return;

		ctf.OnTouchFlag( player, Team, fromGround: true );
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
