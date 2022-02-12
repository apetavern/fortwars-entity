using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
	/// <summary>
	/// A nametag panel in the world
	/// </summary>
	public class NameTag : WorldPanel
	{
		public Panel Avatar;
		public Label NameLabel;

		internal NameTag( string title, long? steamid )
		{
			StyleSheet.Load( "/ui/world/NameTag.scss" );

			if ( steamid != null )
			{
				Avatar = Add.Panel( "avatar" );
				Avatar.Style.SetBackgroundImage( $"avatar:{steamid}" );
			}

			NameLabel = Add.Label( title, "title" );

			// this is the actual size and shape of the world panel
			PanelBounds = new Rect( -500, -100, 1000, 200 );
		}
	}
}
