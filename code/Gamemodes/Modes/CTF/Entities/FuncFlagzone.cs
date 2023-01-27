namespace Fortwars;

[Library( "func_flagzone" )]
[Solid]
[RenderFields]
[VisGroup( VisGroup.Dynamic )]
[HammerEntity]
public partial class FuncFlagzone : BrushEntity
{
	[Property]
	public Team Team { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		EnableSolidCollisions = false;
		EnableTouch = true;

		Tags.Add( "trigger" );

		Transmit = TransmitType.Never;
	}

	public override void StartTouch( Entity other )
	{
		if ( other.IsWorld )
			return;

		if ( GamemodeSystem.Instance is not CaptureTheFlag ctf )
			return;

		if ( ctf.CurrentState != CaptureTheFlag.GameState.Combat )
			return;

		if ( other is not Player player )
			return;

		ctf.OnTouchFlagzone( player, Team );
	}
}
