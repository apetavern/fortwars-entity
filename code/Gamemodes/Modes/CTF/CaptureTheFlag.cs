namespace Fortwars;

public partial class CaptureTheFlag : Gamemode
{
	public override string GamemodeName => "Capture the Flag";

	private const string BogRollPath = "data/weapons/ctf/bogroll.fwweapon";

	/// <summary>
	/// Define the Game State's possible in Capture the Flag mode.
	/// </summary>
	public enum GameState
	{
		Lobby,
		Countdown,
		Build,
		Combat,
		GameOver
	}

	public override string GetGameStateLabel()
	{
		return CurrentState switch
		{
			GameState.Lobby => "Waiting for players",
			GameState.Countdown => "Game starting soon",
			GameState.Build => "Build Phase",
			GameState.Combat => "Combat Phase",
			GameState.GameOver => "Game Over!",
			_ => null,
		};
	}

	public override float GetTimeRemaining()
	{
		return TimeUntilNextState;
	}

	/// <summary>
	/// How long the countdown lasts before the game starts.
	/// </summary>
	public static float CountdownDuration => 15f;

	/// <summary>
	/// How long the build phase lasts.
	/// </summary>
	public static float BuildPhaseDuration => 5f; // 90

	/// <summary>
	/// How long the combat phase lasts.
	/// </summary>
	public static float CombatPhaseDuration => 300f; // 180

	/// <summary>
	/// How long the game over phase lasts before the game is restarted.
	/// </summary>
	public static float GameOverDuration => 10f;

	public override IEnumerable<Team> Teams
	{
		get
		{
			yield return Team.Red;
			yield return Team.Blue;
		}
	}

	[Net]
	public IDictionary<Team, int> Scores { get; set; }

	/// <summary>
	/// The Red Flag Carrier is a member of Team Blue.
	/// </summary>
	[Net]
	public Player RedFlagCarrier { get; set; }

	/// <summary>
	/// The Blue Flag Carrier is a member of Team Red.
	/// </summary>
	[Net]
	public Player BlueFlagCarrier { get; set; }

	[Net]
	public BogRoll RedFlag { get; set; }

	[Net]
	public BogRoll BlueFlag { get; set; }

	[Net]
	public GameState CurrentState { get; set; }

	[Net]
	public TimeUntil TimeUntilNextState { get; set; }

	public override bool AllowDamage => CurrentState == GameState.Combat;

	internal override void Initialize()
	{
		_ = GameLoop();

		Scores = null;
		foreach ( var team in Teams )
			Scores.Add( team, 0 );
	}

	protected async Task GameLoop()
	{
		CurrentState = GameState.Lobby;
		await WaitForPlayers();

		CurrentState = GameState.Countdown;
		await WaitAsync( CountdownDuration );

		CurrentState = GameState.Build;
		RespawnAllPlayers();
		await WaitAsync( BuildPhaseDuration );

		CurrentState = GameState.Combat;
		RespawnAllPlayers();
		await WaitAsync( CombatPhaseDuration );

		var winningTeam = Scores.MaxBy( v => v.Value ).Key;
		Log.Info( $"Fortwars CTF: {winningTeam} Team Wins!" );

		CurrentState = GameState.GameOver;
		await WaitAsync( GameOverDuration );
	}

	private async Task WaitForPlayers()
	{
		while ( PlayerCount < MinimumPlayers )
		{
			await Task.DelayRealtimeSeconds( 1f );
		}
	}

	private async Task WaitAsync( float time )
	{
		TimeUntilNextState = time;
		await Task.DelayRealtimeSeconds( time );
	}

	private static void RespawnAllPlayers()
	{
		foreach ( var client in Game.Clients )
		{
			if ( client.Pawn is not Player player )
				return;

			player.Respawn();
		}
	}

	internal override void OnClientJoined( IClient client )
	{
		base.OnClientJoined( client );

		// Assign the client a team.
		var teamComponent = client.Components.GetOrCreate<TeamComponent>();
		if ( teamComponent != null )
		{
			teamComponent.Team = TeamSystem.GetTeamWithFewestPlayers();
		}

		Log.Info( $"Fortwars: Assigned {client} to team {teamComponent.Team}" );
	}

	internal override void OnPlayerKilled( Player player )
	{
		player.LifeState = LifeState.Respawning;
		player.TimeUntilRespawn = 5f;
	}

	/// <summary>
	/// PrepareLoadout for the CTF gamemode will load in the player's primary and secondary,
	/// plus the equipment from their chosen class.
	/// </summary>
	/// <param name="player">The player whose loadout we are preparing.</param>
	/// <param name="inventory">The inventory to add the items to.</param>
	internal override void PrepareLoadout( Player player, Inventory inventory )
	{
		switch ( CurrentState )
		{
			case GameState.Lobby:
				GiveCombatLoadout( player, inventory );
				break;
			case GameState.Build:
				GiveBuildLoadout( player, inventory );
				break;
			case GameState.Combat:
				GiveCombatLoadout( player, inventory );
				break;
			default:
				GiveCombatLoadout( player, inventory );
				break;
		}
	}

	private void GiveBuildLoadout( Player player, Inventory inventory )
	{
		// TODO: Give player a phygun.
	}

	private void GiveCombatLoadout( Player player, Inventory inventory )
	{
		/*		inventory.AddWeapon(
					WeaponAsset.CreateInstance( WeaponAsset.FromPath( player.SelectedPrimaryWeapon ) ), true );
				inventory.AddWeapon(
					WeaponAsset.CreateInstance( WeaponAsset.FromPath( player.SelectedSecondaryWeapon ) ), false );

				if ( player.Class.Equipment == null )
					return;

				inventory.AddWeapon(
					WeaponAsset.CreateInstance( player.Class.Equipment ), false );*/
	}

	internal override void OnWeaponDropped( Player player, Weapon weapon )
	{
		if ( Game.IsClient )
			return;

		/*		if ( weapon.WeaponAsset.Name != "Bog Roll" )
					return;*/

		var team = player.Client.Components.Get<TeamComponent>().Team;
		var dropPos = player.EyePosition + ( player.EyeRotation.Forward * 50f );

		if ( team == Team.Red )
		{
			BlueFlag = new BogRoll()
			{
				Position = dropPos,
				Team = Team.Blue,
			};
			BlueFlagCarrier = null;
		}
		else
		{
			RedFlag = new BogRoll()
			{
				Position = dropPos,
				Team = Team.Red,
			};
			RedFlagCarrier = null;
		}
	}

	public void OnWeaponThrown( Player player, BogRoll flag )
	{
		var team = player.Client.Components.Get<TeamComponent>().Team;
		if ( team == Team.Red )
		{
			BlueFlag = flag;
			BlueFlagCarrier = null;
		}
		else
		{
			RedFlag = flag;
			RedFlagCarrier = null;
		}

	}

	internal override void MoveToSpawnpoint( IClient client )
	{
		var clientTeam = client.Components.Get<TeamComponent>().Team;
		var spawnpoints = All.OfType<InfoPlayerTeamspawn>().Where( x => x.Team == clientTeam );
		var randomSpawn = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		if ( randomSpawn == null )
		{
			Log.Warning( "Couldn't find spawnpoint!" );
			return;
		}

		client.Pawn.Position = randomSpawn.Position;
		client.Pawn.Rotation = randomSpawn.Rotation;
	}

	public void OnTouchFlag( Player player, Team team, bool fromGround = false )
	{
		if ( Game.IsClient )
			return;

		var playerTeam = player.Client.Components.Get<TeamComponent>().Team;
		Log.Info( $"Fortwars CTF: {player.Client.Name} with team {playerTeam} touched flagzone {team}" );

		if ( fromGround )
		{
			if ( playerTeam == team )
			{
				ReturnFlag( playerTeam );
			}
			else
			{
				PickupFlag( player, playerTeam );
			}

			return;
		}

		// Player is touching their own flagzone.
		if ( playerTeam == team )
		{
			if ( !player.HasFlag )
				return;

			// Ensure the player's active weapon is a Bog Roll.
			/*			if ( player.ActiveWeapon.WeaponAsset.Name != "Bog Roll" )
							return;*/

			// Ensure the player's team flag is home.
			if ( !FlagIsHome( playerTeam ) )
				return;

			// Score for the player's team.
			ScoreFlag( player, playerTeam );
			return;
		}

		// Player is touching the enemy flagzone.
		if ( player.HasFlag )
			return;

		// If flag is not missing, pick it up.
		if ( !FlagIsHome( team ) )
			return;

		PickupFlag( player, playerTeam );

	}

	/// <summary>
	/// Whether the flag for given team is in the flagzone.
	/// </summary>
	/// <param name="team">The team whose flagzone is being touched.</param>
	/// <returns>True, if the flag is home. Otherwise, false.</returns>
	public bool FlagIsHome( Team team )
	{
		if ( team == Team.Red )
		{
			return RedFlagCarrier is null && RedFlag is null;
		}
		else if ( team == Team.Blue )
		{
			return BlueFlagCarrier is null && BlueFlag is null;
		}

		// How did we get here?
		return false;
	}

	private void PickupFlag( Player player, Team team )
	{
		// Assign the carrier based on their team.
		if ( team == Team.Red )
		{
			BlueFlagCarrier = player;
			BlueFlag?.Delete();
			BlueFlag = null;
		}
		else if ( team == Team.Blue )
		{
			RedFlagCarrier = player;
			RedFlag?.Delete();
			RedFlag = null;
		}

		Log.Info( $"Fortwars CTF: The flag for team {team} has been stolen by {player.Client.Name}." );

		// Add the flag to the player's inventory.
		/*		player.Inventory.AddWeapon(
					WeaponAsset.CreateInstance( WeaponAsset.FromPath( BogRollPath ) ), true );*/
	}

	private void ReturnFlag( Team team )
	{
		if ( team == Team.Red )
		{
			RedFlagCarrier = null;
			RedFlag?.Delete();
			RedFlag = null;
		}
		else
		{
			BlueFlagCarrier = null;
			BlueFlag?.Delete();
			BlueFlag = null;
		}
	}

	private void ScoreFlag( Player player, Team team )
	{
		if ( player == BlueFlagCarrier )
		{
			BlueFlagCarrier = null;
		}
		else if ( player == RedFlagCarrier )
		{
			RedFlagCarrier = null;
		}

		Scores[team] += 1;

		var inv = player.Inventory;
		var lastActiveWeapon = inv.LastActiveWeaponSlot;
		inv.RemoveWeapon( player.ActiveWeapon );
		inv.SetActiveWeapon( inv.GetWeaponFromSlot( lastActiveWeapon ) );
	}

	[GameEvent.Tick]
	public void Tick()
	{
		if ( !Debug )
			return;

		int i = 0;
		foreach ( var team in Teams )
		{
			DebugOverlay.ScreenText( $"{team}: {Scores[team]}", i++ );
		}
		DebugOverlay.ScreenText( $"RedFlagCarrier: {RedFlagCarrier}", i++ );
		DebugOverlay.ScreenText( $"BlueFlagCarrier: {BlueFlagCarrier}", i++ );
		DebugOverlay.ScreenText( $"RedFlag: {RedFlag}", i++ );
		DebugOverlay.ScreenText( $"BlueFlag: {BlueFlag}", i++ );
	}

	[ConCmd.Admin( "fw_ctf_set_state" )]
	public static void SetGameState( string state )
	{
		if ( GamemodeSystem.Instance is not CaptureTheFlag ctf )
			return;

		if ( Enum.TryParse<GameState>( state, true, out GameState newState ) )
		{
			ctf.CurrentState = newState;
			ctf.TimeUntilNextState = 500f;
		}
	}

	[ConVar.Replicated( "fw_debug_ctf" )]
	public static bool Debug { get; set; } = false;
}
