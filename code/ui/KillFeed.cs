
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Fortwars
{
	public partial class KillFeedEntry : Panel
	{
		public Label Left { get; internal set; }
		public Label Right { get; internal set; }
		public Panel Icon { get; internal set; }

		public KillFeedEntry()
		{
			Left = Add.Label( "", "left" );
			Icon = Add.Panel( "icon" );
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

			StyleSheet.Load( "/ui/KillFeed.scss" );
		}

		[ClientRpc]
		public static void AddEntry( long lsteamid, string left, long rsteamid, string right, string method )
		{
			if ( Current == null )
				return;

			Log.Info( $"{left} killed {right} using {method}" );

			var e = Current.AddChild<KillFeedEntry>();

			e.AddClass( method );

			e.Left.Text = left;
			e.Left.SetClass( "me", lsteamid == (Local.Client?.PlayerId) );

			e.Right.Text = right;
			e.Right.SetClass( "me", rsteamid == (Local.Client?.PlayerId) );
		}
	}

}
