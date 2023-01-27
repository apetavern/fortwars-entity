namespace Fortwars;

[Library( "info_flag_spawn" )]
[Title( "Flag Spawn" ), Category( "Fortwars" )]
[EditorModel( "models/rust_props/small_junk/toilet_paper.vmdl" )]
[HammerEntity]
public partial class InfoFlagSpawn : ModelEntity
{
	[Net, Property, Change( nameof(OnTeamChanged) )]
	public Team Team { get; set; }

	private static readonly Model FlagModel = Model.Load( "models/items/bogroll/bogroll_w.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		PhysicsEnabled = false;
		UsePhysicsCollision = false;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Model = FlagModel;
	}

	protected void OnTeamChanged()
	{
		switch ( Team )
		{
			case Team.Invalid: break;
			case Team.Blue: 
				SetMaterialGroup( 0 );
				break;
			case Team.Red:
				SetMaterialGroup( 1 );
				break;
			default:
				break;
		}
	}
}
