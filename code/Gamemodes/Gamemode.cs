namespace Fortwars;

public partial class Gamemode : Entity
{
	public virtual string GamemodeName => "";

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

	public virtual string GetGameStateLabel()
	{
		return "";
	}

	internal virtual void Initialize() { }

	internal virtual void OnClientJoined( IClient client ) { }

	internal virtual void OnClientDisconnect( IClient cl, NetworkDisconnectionReason reason ) { }

	internal virtual void OnPlayerKilled( Player player ) { }

	internal virtual void MoveToSpawnpoint( Entity pawn ) { }
}
