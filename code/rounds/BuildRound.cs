// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class BuildRound : BaseRound
{
	public static int RoundLength = 150;

	public override string RoundName => "Build";
	public override int RoundDuration => RoundLength;

	protected override void OnStart()
	{
		Log.Info( "Started Build Round" );

		if ( Game.IsServer )
		{
			Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( ( player ) =>
			{
				SetupInventory( player );
				player.Reset();
			} );
		}

		foreach ( var wall in Entity.All.OfType<FuncWallToggle>() )
			wall.Show();
	}

	public override void SetupInventory( FortwarsPlayer player )
	{
		base.SetupInventory( player );

		player.Inventory.Add( new PhysGun(), true );
		_ = player.GiveLoadout( new() { player.Class?.Gadget }, player.Inventory as Inventory );
	}

	protected override void OnFinish()
	{
		Log.Info( "Finished Build Round" );
	}

	protected override void OnTimeUp()
	{
		FortwarsGame.Instance.ChangeRound( new CombatRound() );
	}

	public override void OnPlayerKilled( FortwarsPlayer player )
	{
		base.OnPlayerKilled( player );
	}

	public override void OnPlayerSpawn( FortwarsPlayer player )
	{
		base.OnPlayerSpawn( player );
	}
}
