// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class MapVote : Panel
{
	public MapVote()
	{
		StyleSheet.Load( "ui/hud/MapVote.scss" );

		List<string> maps = FortwarsGame.GetMaps();

		List<string> fullMaplist = FortwarsGame.GetMaps();

		if ( maps.Contains( Game.Server.MapIdent ) )
		{
			maps.Remove( Game.Server.MapIdent );
		}

		for ( int i = 0; i < maps.Count; i++ )
		{
			string mapName = maps[i];
			var mapPanel = MapVotePanel.FromPackage( mapName, fullMaplist.IndexOf( mapName ) );
			AddChild( mapPanel );
		}

		/*for ( int i = 0; i < 4; i++ )
	{
		int selectedMap = Game.Random.Int( maps.Count - 1 );
		string mapName = maps[selectedMap];
		var mapPanel = MapVotePanel.FromPackage( mapName, fullMaplist.IndexOf( mapName ) );
		maps.RemoveAt( selectedMap );
		AddChild( mapPanel );
	}*/
	}

	public override void Tick()
	{
		var game = FortwarsGame.Instance;
		if ( game == null ) return;

		SetClass( "show", game.Round is VoteRound );
	}
}

public class MapVotePanel : Panel
{
	private Label VoteCount { get; set; }
	private int Index { get; set; }

	public static MapVotePanel FromPackage( string packageName, int index )
	{
		var packageTask = Package.Fetch( packageName, true ).ContinueWith( t =>
		{
			var package = t.Result;
			return new MapVotePanel( package.Title, package.Thumb, index );
		} );

		return packageTask.Result;
	}

	public MapVotePanel( string mapName, string backgroundImage, int index )
	{
		AddClass( "vote-panel" );

		VoteCount = Add.Label( "0", "vote-count" );
		Add.Label( "VOTES", "vote-subtext" );
		Add.Label( mapName, "map-name" );

		Style.BackgroundImage = Texture.Load( backgroundImage );

		AddEventListener( "onclick", () =>
		{
			if ( HasClass( "disabled" ) )
				return;
			FortwarsGame.VoteMap( index );
			Sound.FromScreen( "vote_confirm" );
			_ = SetClickClass();
		} );

		Index = index;
	}

	public override void Tick()
	{
		base.Tick();
		int votes = 0;

		foreach ( var mapVote in FortwarsGame.Instance?.MapVotes )
		{
			SetClass( "disabled", mapVote.SteamId == Game.LocalClient.SteamId );
			SetClass( "voted-for", mapVote.SteamId == Game.LocalClient.SteamId && mapVote.MapIndex == Index );

			if ( mapVote.MapIndex == Index )
				votes++;
		}

		VoteCount.Text = votes.ToString();
	}
	private async Task SetClickClass()
	{
		AddClass( "clicked" );
		await Task.Delay( 50 );
		RemoveClass( "clicked" );
	}
}
