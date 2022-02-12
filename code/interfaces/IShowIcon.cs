namespace Fortwars
{
	/// <summary>
	/// Put this on things where you want to have a UI icon
	/// </summary>
	public interface IShowIcon
	{
		/// <summary>
		/// Path for the world panel icon
		/// </summary>
		string SpatialIcon();

		/// <summary>
		/// Path for the compass icon
		/// </summary>
		string NonDiegeticIcon();

		/// <summary>
		/// Give this icon a class name (for tinting etc)
		/// </summary>
		string CustomClassName();

		/// <summary>
		/// Position to place the icon (will be calculated for compass)
		/// </summary>
		Vector3 IconWorldPosition();

		/// <summary>
		/// Should we draw the icon?
		/// </summary>
		bool DrawIcon();
	}
}
