using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	public partial class MessageFeed : Panel
	{
		public static MessageFeed Instance { get; set; }
		public MessageFeed()
		{
			Instance = this;
			StyleSheet.Load( "/ui/hud/MessageFeed.scss" );
		}

		[ClientCmd( "fw_message_add", CanBeCalledFromServer = true )]
		public static void AddMessage( string icon, string title, string message )
		{
			var control = Instance.Add.Message( icon, title, message );
			control.SetClass( "kill", true );
		}
	}
}
