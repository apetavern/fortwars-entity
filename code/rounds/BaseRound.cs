// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public abstract partial class BaseRound : BaseNetworkable
{
	public virtual int RoundDuration => 0;
	public virtual string RoundName => "";
	[Net] public float RoundEndTime { get; set; }

	public float TimeLeft
	{
		get
		{
			if ( Game.IsClient )
			{
				return RoundEndTime - FortwarsGame.Instance.ServerTime;
			}

			return RoundEndTime - Time.Now;
		}
	}

	public void Start()
	{
		if ( Game.IsServer && RoundDuration > 0 )
		{
			RoundEndTime = (float)Math.Floor( Time.Now + RoundDuration );
		}

		OnStart();
	}

	public void Finish()
	{
		if ( Game.IsServer )
		{
			RoundEndTime = 0f;
		}

		OnFinish();
	}

	public virtual void OnPlayerSpawn( FortwarsPlayer player ) { }

	public virtual void OnPlayerKilled( FortwarsPlayer player ) { }

	public virtual void OnTick() { }

	public virtual void SetupInventory( FortwarsPlayer player )
	{
		player.Inventory.DeleteContents();
	}

	public virtual void OnSecond()
	{
		if ( Game.IsServer )
		{
			if ( skipRound || ( RoundEndTime > 0 && Time.Now >= RoundEndTime ) )
			{
				RoundEndTime = 0f;
				OnTimeUp();
				skipRound = false;
			}

			if ( Game.Clients.Count < FortwarsGame.Instance.MinPlayers && FortwarsGame.Instance.Round is not LobbyRound )
				FortwarsGame.Instance.ChangeRound( new LobbyRound() );

		}
	}

	bool skipRound = false;

	[ConCmd.Admin( "fw_round_skip" )]
	public static void SkipRound()
	{
		FortwarsGame.Instance.Round.skipRound = true;
	}

	[ConCmd.Admin( "fw_round_extend" )]
	public static void ExtendRound()
	{
		FortwarsGame.Instance.Round.RoundEndTime += 600;
	}

	protected virtual void OnStart() { }
	protected virtual void OnFinish() { }
	protected virtual void OnTimeUp() { }
}
