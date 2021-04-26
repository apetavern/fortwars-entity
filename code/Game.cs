using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars
{
	[Library( "fortwars", Title = "Fortwars" )]
	partial class Game : Sandbox.Game
	{
		public static Game Instance
		{
			get => Current as Game;
		}

		// shit hack, ideally Time.Now should be synced with server
		[Net] public float ServerTime { get; private set; }

		public Game()
		{
			if ( IsServer )
				new UI.FortwarsHUD();

			_ = StartTickTimer();

			// todo: start in Lobby round
			ChangeRound( new BuildRound() );
		}

		public override Player CreatePlayer() => new FortwarsPlayer();



		public async Task StartSecondTimer()
		{
			while ( true )
			{
				await Task.DelaySeconds( 1 );
				OnSecond();
			}
		}

		public async Task StartTickTimer()
		{
			while ( true )
			{
				await Task.NextPhysicsFrame();
				OnTick();
			}
		}

		public override void PostLevelLoaded()
		{
			_ = StartSecondTimer();

			base.PostLevelLoaded();
		}

		private void OnSecond()
		{
			// this is shite, need Time.Now to reflect server time clientside
			if ( IsServer )
			{
				ServerTime = Time.Now;
				NetworkDirty( "ServerTime", NetVarGroup.Net );
			}

			Round?.OnSecond();
		}

		private void OnTick()
		{
			Round?.OnTick();

			if ( IsClient )
			{
				// We have to hack around this for now until we can detect changes in net variables.
				if ( _lastRound != Round )
				{
					_lastRound?.Finish();
					_lastRound = Round;
					Round.Start();
				}
			}
		}

		public override void PlayerKilled( Player player )
		{
			Round?.OnPlayerKilled( player as Player );

			Log.Info($"{player.Name} was killed");

			if (player.LastAttacker != null)
			{
				if (player.LastAttacker is Player attackPlayer)
				{
					KillFeed.AddEntry(attackPlayer.SteamId, attackPlayer.Name, player.SteamId, player.Name, player.LastAttackerWeapon?.ClassInfo?.Name);
				}
				else
				{
					KillFeed.AddEntry((ulong)player.LastAttacker.NetworkIdent, player.LastAttacker.ToString(), player.SteamId, player.Name, "killed");
				}
			}
			else
			{
				KillFeed.AddEntry((ulong)0, "", player.SteamId, player.Name, "died");
			}

			base.PlayerKilled( player );
		}

		public override void PlayerRespawn( Player player )
		{
			Log.Info( $"Finding spawnpoint for {player.Name} on TeamID: {(int)(player as FortwarsPlayer).TeamID}" );

			var spawnpoints = Entity.All.OfType<InfoPlayerTeamspawn>().Where( x => x.Team == (int)(player as FortwarsPlayer).TeamID);
			var randomSpawn = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			if ( randomSpawn == null )
			{
				Log.Warning( "Couldn't find spawnpoint!" );
				return;
			}

			player.WorldPos = randomSpawn.WorldPos;
			player.WorldRot = randomSpawn.WorldRot;
		}

		[ServerCmd( "spawn" )]
		public static void Spawn( string modelname )
		{
			var owner = ConsoleSystem.Caller;

			if ( ConsoleSystem.Caller == null )
				return;

			var tr = Trace.Ray( owner.EyePos, owner.EyePos + owner.EyeRot.Forward * 500 )
				.UseHitboxes()
				.Ignore( owner )
				.Size( 2 )
				.Run();

			var ent = new Prop();
			ent.WorldPos = tr.EndPos;
			ent.WorldRot = Rotation.From( new Angles( 0, owner.EyeAng.yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
			ent.SetModel( modelname );

			// Drop to floor
			if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
			{
				var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

				var delta = p - tr.EndPos;
				ent.PhysicsBody.Pos -= delta;
				//DebugOverlay.Line( p, tr.EndPos, 10, false );
			}

		}
	}
}
