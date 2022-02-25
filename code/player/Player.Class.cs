namespace Fortwars
{
	partial class FortwarsPlayer
	{
		public Class Class { get; set; } = new AssaultClass();

		public void AssignClass( Class newClass )
		{
			Class?.Cleanup( Inventory as Inventory );
			Class = newClass;

			this.Reset();
			Game.Instance.Round.SetupInventory( this );
		}
	}
}
