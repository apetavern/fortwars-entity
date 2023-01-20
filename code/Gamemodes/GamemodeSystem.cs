namespace Fortwars;

public class GamemodeSystem
{
	[ConVar.Replicated( "fortwars_gamemode" )]
	public static string SelectedGamemode { get; set; } = "";

	private static Gamemode _instance;

	public static Gamemode Instance
	{
		get
		{
			if ( Game.IsServer )
				return _instance;

			if ( !_instance.IsValid() )
				_instance = Entity.All.FirstOrDefault( x => x is Gamemode ) as Gamemode;

			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	protected static Gamemode FetchGamemodeEntity()
	{
		var gamemode = Entity.All.FirstOrDefault( x => x is Gamemode ) as Gamemode;

		if ( !gamemode.IsValid() && !string.IsNullOrEmpty( SelectedGamemode ) )
		{
			Log.Info( $"Fortwars: Attempting to find gamemode from Type - {SelectedGamemode}" );
			var gamemodeEntity = TypeLibrary.Create<Gamemode>( SelectedGamemode );
			if ( gamemodeEntity.IsValid() )
			{
				Log.Info( $"Fortwars: Found gamemode from Type - {SelectedGamemode}" );
				return gamemodeEntity;
			}
		}
		else
		{
			Log.Info( "Fortwars: Creating default gamemode - CTF" );
			var gamemodeEntity = TypeLibrary.Create<Gamemode>( "CaptureTheFlag" );
			return gamemodeEntity;
		}

		return gamemode;
	}

	public static void SetupGamemode()
	{
		Instance = FetchGamemodeEntity();

		if ( Instance.IsValid() )
			Instance.Initialize();
	}
}
