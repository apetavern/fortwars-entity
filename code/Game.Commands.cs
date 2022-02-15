using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	partial class Game
	{
		[AdminCmd( "recreatehud" )]
		public static void RecreateHud()
		{
			hud?.Delete();
			hud = new();
		}

		[AdminCmd( "fw_give_ammo" )]
		public static void GiveAmmo( int amount )
		{
			var owner = ConsoleSystem.Caller;
			var player = owner.Pawn;

			if ( player.ActiveChild is not FortwarsWeapon weapon )
				return;

			weapon.ReserveAmmo += amount;
		}

		[AdminCmd( "fw_cleanup" )]
		public static void Cleanup()
		{
			foreach ( var entry in Instance.buildLogEntries )
			{
				entry.Block.Delete();
			}

			Instance.buildLogEntries.Clear();

			Log.Trace( $"Deleted blocks" );
		}

		[ServerCmd( "fw_spawn" )]
		public static void Spawn( string blockName )
		{
			var owner = ConsoleSystem.Caller;
			var player = owner.Pawn as FortwarsPlayer;

			if ( Instance.Round is not BuildRound )
				return;

			if ( ConsoleSystem.Caller == null )
				return;

			if ( !player.IsValid() || player.LifeState != LifeState.Alive )
				return;

			if ( blockName.Contains( "steel" ) && BlockMaterial.Steel.GetRemainingCount( owner ) <= 0 )
				return;

			if ( !blockName.Contains( "steel" ) && BlockMaterial.Wood.GetRemainingCount( owner ) <= 0 )
				return;

			//
			// Stop the player spawning too many blocks too quickly
			//
			{
				int delay = 0;
				var playerLogs = Instance.buildLogEntries.ToList().Where( x => x.Player == player );
				foreach ( var entry in playerLogs.TakeLast( 3 ) )
				{
					delay += (Time.Tick - entry.Tick);
				}
				delay = (delay / 3f).CeilToInt();

				if ( delay < Global.TickRate * 3 && playerLogs.Count() > 3 )
				{
					MessageFeed.AddMessage( To.Single( player ), "clear", "Can't build", "You're building too fast! Slow down!" );
					return;
				}
			}

			var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 500 )
				.UseHitboxes()
				.Ignore( player )
				.Size( 2 )
				.Run();

			var ent = new FortwarsBlock();
			ent.Position = tr.EndPos;
			ent.Rotation = Rotation.From( new Angles( 0, player.EyeRotation.Yaw(), 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );

			if ( blockName.Contains( "steel" ) )
			{
				ent.BlockMaterial = BlockMaterial.Steel;
				ent.SetModel( $"models/blocks/steel/fw_{blockName.Split( '_' )[1]}.vmdl" );
			}
			else
			{
				ent.SetModel( $"models/blocks/wood/fw_{blockName}.vmdl" );
			}

			ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
			ent.TeamID = player.TeamID;
			ent.Owner = player;

			ent.OnTeamIDChanged();

			// Drop to floor
			if ( ent.PhysicsBody != null && ent.PhysicsGroup.BodyCount == 1 )
			{
				var p = ent.PhysicsBody.FindClosestPoint( tr.EndPos );

				var delta = p - tr.EndPos;
				ent.PhysicsBody.Position -= delta;
				ent.PhysicsBody.BodyType = PhysicsBodyType.Static;
			}

			Instance.buildLogEntries.Add( new BuildLogEntry( Time.Tick, ent, player ) );
		}
	}
}
