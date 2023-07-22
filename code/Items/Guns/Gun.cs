namespace Fortwars;

[Category( "Gun" ), Icon( "gavel" )]
public abstract partial class Gun : Item
{
	public struct Setup
	{
		public float MoveSpeedMultiplier;
	}

	public Setup Data => n_Data;
	[Net] private Setup n_Data { get; set; }

	protected abstract Setup Default { get; }

	protected Gun()
	{
		Transmit = TransmitType.Always;
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
	}
}
