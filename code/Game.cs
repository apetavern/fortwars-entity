// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

global using Editor;
global using Sandbox;
global using Sandbox.Diagnostics;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using Sandbox.Utility;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

namespace Fortwars;

partial class FortwarsGame : Sandbox.GameManager
{
	public static FortwarsGame Instance
	{
		get => Current as FortwarsGame;
	}

	// shit hack, ideally Time.Now should be synced with server
	[Net] public float ServerTime { get; private set; }

	private static FortwarsHUD hud;

	public FortwarsGame()
	{
		if ( Game.IsServer )
		{
			hud = new FortwarsHUD();
			Game.TickRate = 30;
		}

		ChangeRound( new LobbyRound() );
	}

	public async Task StartSecondTimer()
	{
		while ( true )
		{
			await Task.DelaySeconds( 1 );
			OnSecond();
		}
	}

	public override void PostLevelLoaded()
	{
		_ = StartSecondTimer();

		base.PostLevelLoaded();
	}

	public override void ClientJoined( IClient cl )
	{
		base.ClientJoined( cl );

		var player = new FortwarsPlayer( cl );
		player.Respawn();

		cl.Pawn = player;
	}

	public override void ClientDisconnect( IClient cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		All.OfType<FortwarsBlock>().ToList().ForEach( x =>
		{
			if ( x.Owner == cl )
				x.Delete();
		} );
	}

	public override void OnKilled( Entity pawn )
	{
		Round?.OnPlayerKilled( pawn as FortwarsPlayer );

		var player = pawn as FortwarsPlayer;

		if ( player.ActiveChild is BogRoll )
			player.Inventory.DropActive();

		PlayerDropFlag( player );

		Log.Info( $"{pawn.Name} was killed" );

		if ( pawn.LastAttacker != null )
		{
			if ( pawn.LastAttacker is FortwarsPlayer attackPlayer )
			{
				var method = pawn.LastAttackerWeapon?.ClassName;
				if ( pawn.LastAttackerWeapon is FortwarsWeapon weapon )
					method = weapon.WeaponAsset.WeaponName;

				KillFeed.AddEntry(
					attackPlayer.Client.SteamId,
					attackPlayer.Client.Name,
					pawn.Client.SteamId,
					pawn.Client.Name,
					method,
					attackPlayer.Team.GetCssClass(),
					player.Team.GetCssClass() );
			}
			else
			{
				KillFeed.AddEntry(
					pawn.LastAttacker.NetworkIdent,
					pawn.LastAttacker.ToString(),
					pawn.Client.SteamId,
					pawn.Client.Name,
					"killed",
					"",
					player.Team.GetCssClass() );
			}
		}
		else
		{
			KillFeed.AddEntry(
				0,
				"Suicide",
				pawn.Client.SteamId,
				pawn.Client.Name,
				"killed",
				"",
				player.Team.GetCssClass() );
		}

		base.OnKilled( pawn );
	}

	private void OnSecond()
	{
		// TODO: Validate this
		// this is shite, need Time.Now to reflect server time clientside
		if ( Game.IsServer )
		{
			ServerTime = Time.Now;
		}

		Round?.OnSecond();
	}

	private void OnRoundChange( BaseRound lastRound, BaseRound newRound )
	{
		lastRound?.Finish();
		newRound?.Start();
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		Log.Info( $"Finding spawnpoint for {pawn.Name} on TeamID: {(int)( pawn as FortwarsPlayer ).TeamID}" );

		var spawnpoints = All.OfType<InfoPlayerTeamspawn>().Where( x => x.Team == ( pawn as FortwarsPlayer ).TeamID );
		var randomSpawn = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		if ( randomSpawn == null )
		{
			Log.Warning( "Couldn't find spawnpoint!" );
			return;
		}

		pawn.Position = randomSpawn.Position;
		pawn.Rotation = randomSpawn.Rotation;
	}
}
