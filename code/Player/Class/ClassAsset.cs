namespace Fortwars;

[GameResource( "Class Asset", "fwclass", "Describes a Fortwars class",
	Icon = "⚒️", IconBgColor = "#fe71dc", IconFgColor = "black" )]
public class ClassAsset : GameResource
{
	//
	// Meta
	//
	[Category( "Meta" )]
	public string ClassName { get; set; } = "My class";

	[Category( "Meta" )]
	public string ShortDescription { get; set; } = "My class";

	[Category( "Meta" ), ResourceType( "png" )]
	public string IconPath { get; set; }

	[Category( "Meta" )]
	public bool IsSelectable { get; set; } = true;

	[Category( "Meta" ), ResourceType( "clothing" )]
	public List<string> Cosmetics { get; set; } = new();

	//
	// Gameplay
	//
	[Category( "Gameplay" )]
	public float JumpMultiplier { get; set; } = 1.0f;

	[Category( "Gameplay" )]
	public float SpeedMultiplier { get; set; } = 1.0f;

	[Category( "Gameplay" )]
	public float HealthMultiplier { get; set; } = 1.0f;

	[Category( "Gameplay" ), ResourceType( "prefab" )]
	public Prefab Equipment { get; set; }

	public static ClassAsset Default => FromPath( "data/classes/assault.fwclass" );

	public static ClassAsset FromPath( string path )
	{
		return ResourceLibrary.Get<ClassAsset>( path );
	}
}
