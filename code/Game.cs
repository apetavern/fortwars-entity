using Sandbox;
using System;
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

			var player = new FortwarsPlayer();
			player.Respawn();

			cl.Pawn = player;
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
