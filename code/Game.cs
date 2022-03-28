// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars
{
	partial class Game : Sandbox.Game
	{
		public static Game Instance
		{
			get => Current as Game;
		}

		// shit hack, ideally Time.Now should be synced with server
		[Net] public float ServerTime { get; private set; }

		private static FortwarsHUD hud;

		public Game()
		{
			if ( IsServer )
				hud = new FortwarsHUD();

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

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var player = new FortwarsPlayer( cl );
			player.Respawn();

			cl.Pawn = player;
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );

			Entity.All.OfType<FortwarsBlock>().ToList().ForEach( x =>
			{
				if ( x.Owner == cl )
					x.Delete();
			} );
		}

		public override void OnKilled( Entity pawn )
		{
			Round?.OnPlayerKilled( pawn as Player );

			var player = pawn as FortwarsPlayer;

			if ( player.ActiveChild is BogRoll )
				player.Inventory.DropActive();

			PlayerDropFlag( player );

			Log.Info( $"{pawn.Name} was killed" );

			if ( pawn.LastAttacker != null )
			{
				if ( pawn.LastAttacker is Player attackPlayer )
				{
					if ( pawn.LastAttackerWeapon is FortwarsWeapon weapon )
						KillFeed.AddEntry( attackPlayer.Client.PlayerId, attackPlayer.Client.Name, pawn.Client.PlayerId, pawn.Client.Name, weapon.WeaponAsset.WeaponName );
					else
						KillFeed.AddEntry( attackPlayer.Client.PlayerId, attackPlayer.Client.Name, pawn.Client.PlayerId, pawn.Client.Name, pawn.LastAttackerWeapon?.ClassInfo?.Name );
				}
				else
				{
					KillFeed.AddEntry( pawn.LastAttacker.NetworkIdent, pawn.LastAttacker.ToString(), pawn.Client.PlayerId, pawn.Client.Name, "killed" );
				}
			}
			else
			{
				if ( player.LastDamage.Flags.HasFlag( DamageFlags.Fall ) )
				{
					KillFeed.AddEntry( 0, "Fall Damage", pawn.Client.PlayerId, pawn.Client.Name, "killed" );
				}
				else
				{
					KillFeed.AddEntry( 0, "", pawn.Client.PlayerId, pawn.Client.Name, "died" );
				}
			}

			base.OnKilled( pawn );
		}

		private void OnSecond()
		{
			// TODO: Validate this
			// this is shite, need Time.Now to reflect server time clientside
			if ( IsServer )
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

			var spawnpoints = Entity.All.OfType<InfoPlayerTeamspawn>().Where( x => x.Team == ( pawn as FortwarsPlayer ).TeamID );
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
}
