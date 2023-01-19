namespace Fortwars;

public partial class Gamemode : Entity
{
	public virtual bool AllowMovement => true;
	public virtual bool AllowDamage => true;
	public virtual bool AllowFriendlyFire => false;

	/// <summary>
	/// Specify the list of teams that are supported in this mode.
	/// </summary>
	public virtual IEnumerable<Team> Teams => null;

	public override void Spawn()
	{
		Transmit = TransmitType.Always;
	}

	internal virtual void Initialize() { }

	internal virtual void OnPlayerKilled( Player player ) { }
}
