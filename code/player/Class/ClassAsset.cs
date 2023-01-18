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

	public static ClassAsset FromPath( string path )
	{
		return ResourceLibrary.Get<ClassAsset>( path );
	}
}
