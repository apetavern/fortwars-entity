namespace Fortwars
{
	partial class FortwarsPlayer
	{
		public Class Class { get; set; }

		public void AssignClass( Class newClass )
		{
			Class?.Cleanup( Inventory as Inventory );
			Class = newClass;
			Class.AssignLoadout( Inventory as Inventory );
		}

	}
}
