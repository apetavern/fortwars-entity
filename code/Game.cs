using Fortwars.UI;
using FortWars;
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

		private FortwarsHUD hud;

		public Game()
		{
			if ( IsServer )
				hud = new UI.FortwarsHUD();

			_ = StartTickTimer();

			// todo: start in Lobby round
			ChangeRound( new BuildRound() );
		}

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

		public override void ClientJoined( Client cl )
		{
			base.ClientJoined( cl );

			var player = new FortwarsPlayer( cl );
			player.Respawn();

			cl.Pawn = player;
		}

		public override void OnKilled( Entity pawn )
		{
			Round?.OnPlayerKilled( pawn as Player );
			PlayerDropFlag( pawn as FortwarsPlayer );

			Log.Info( $"{pawn.Name} was killed" );

			if ( pawn.LastAttacker != null )
			{
				if ( pawn.LastAttacker is Player attackPlayer )
				{
					KillFeed.AddEntry( attackPlayer.Client.PlayerId, attackPlayer.Client.Name, pawn.Client.PlayerId, pawn.Client.Name, pawn.LastAttackerWeapon?.ClassInfo?.Name );
				}
				else
				{
					KillFeed.AddEntry( pawn.LastAttacker.NetworkIdent, pawn.LastAttacker.ToString(), pawn.Client.PlayerId, pawn.Client.Name, "killed" );
				}
			}
			else
			{
				KillFeed.AddEntry( 0, "", pawn.Client.PlayerId, pawn.Client.Name, "died" );
			}

			base.OnKilled( pawn );
		}

		private void OnSecond()
		{
			// this is shite, need Time.Now to reflect server time clientside
			if ( IsServer )
			{
				ServerTime = Time.Now;
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

		public override void MoveToSpawnpoint( Entity pawn )
		{
			Log.Info( $"Finding spawnpoint for {pawn.Name} on TeamID: {(int)(pawn as FortwarsPlayer).TeamID}" );

			var spawnpoints = Entity.All.OfType<InfoPlayerTeamspawn>().Where( x => x.Team == (pawn as FortwarsPlayer).TeamID );
			var randomSpawn = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			if ( randomSpawn == null )
			{
				Log.Warning( "Couldn't find spawnpoint!" );
				return;
			}

			pawn.Position = randomSpawn.Position;
			pawn.Rotation = randomSpawn.Rotation;
		}

		[ServerCmd( "recreatehud" )]
		public static void RecreateHud()
		{
			Instance.hud?.Delete();
			Instance.hud = new();
		}

		[ServerCmd( "give_ammo" )]
		public static void GiveAmmo( int amount )
		{
			var owner = ConsoleSystem.Caller;
			var player = owner.Pawn;

			if ( player.ActiveChild is not FortwarsWeapon weapon )
				return;

			weapon.ReserveAmmo += amount;
		}

		[ServerCmd( "spawn" )]
		public static void Spawn( string modelname )
		{
			var owner = ConsoleSystem.Caller;
			var player = owner.Pawn;

			if ( ConsoleSystem.Caller == null )
				return;

			var tr = Trace.Ray( player.EyePos, player.EyePos + player.EyeRot.Forward * 500 )
				.UseHitboxes()
				.Ignore( player )
				.Size( 2 )
				.Run();

			var ent = new Prop();
			ent.Position = tr.EndPos;
			ent.Rotation = Rotation.From( new Angles( 0, player.EyeRot.Yaw(), 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
			ent.SetModel( modelname );
			if ( (player as FortwarsPlayer).TeamID == Team.Red )
			{
				ent.SetMaterialGroup( 1 );
			}

			// Drop to floor
			if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
			{
				var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

				var delta = p - tr.EndPos;
				ent.PhysicsBody.Position -= delta;
				//DebugOverlay.Line( p, tr.EndPos, 10, false );
			}
		}
	}
}
