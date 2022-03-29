// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Fortwars;

public partial class KillFeedEntry : Panel
{
	public Label Left { get; internal set; }
	public Label Right { get; internal set; }
	public Label Method { get; internal set; }

	public KillFeedEntry()
	{
		Left = Add.Label( "", "left" );
		Method = Add.Label( "", "method" );
		Right = Add.Label( "", "right" );

		_ = RunAsync();
	}

	async Task RunAsync()
	{
		await Task.Delay( 4000 );
		Delete();
	}

}

public partial class KillFeed : Panel
{
	public static KillFeed Current;

	public KillFeed()
	{
		Current = this;

		StyleSheet.Load( "/ui/hud/KillFeed.scss" );
	}

	[ClientRpc]
	public static void AddEntry( long lsteamid, string left, long rsteamid, string right, string method, string leftClass = "", string rightClass = "" )
	{
		if ( Current == null )
			return;

		Log.Info( $"{left} killed {right} using {method}" );

		var e = Current.AddChild<KillFeedEntry>();

		e.AddClass( method );
		e.SetClass( "me", lsteamid == ( Local.Client?.PlayerId ) || rsteamid == ( Local.Client?.PlayerId ) );

		e.Left.Text = left;
		e.Left.AddClass( leftClass );

		e.Method.Text = $"[{method}]";

		e.Right.Text = right;
		e.Right.AddClass( rightClass );
	}
}

