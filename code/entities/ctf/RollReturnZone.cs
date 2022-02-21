using Sandbox;
using System.Collections.Generic;
using System;


namespace Fortwars
{

	public partial class RollReturnZone : ModelEntity
	{
		[Net] public Team Team { get; set; }

		[Net] public List<FortwarsPlayer> RedPlayers { get; set; }
		[Net] public List<FortwarsPlayer> BluePlayers { get; set; }

		public BogRoll AttachedRoll;

		ModelEntity visuals;

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/items/bogroll/roll_returnfield.vmdl" );

			visuals = new ModelEntity();//Why the fuck do I need this?
			visuals.SetModel( "models/items/bogroll/roll_returnfield.vmdl" );
			visuals.Position = Position;
			visuals.SetParent( this );

			SetupPhysicsFromModel( PhysicsMotionType.Static );
			CollisionGroup = CollisionGroup.Trigger;
			EnableSolidCollisions = false;
			EnableTouch = true;

			EnableDrawing = true;

			Transmit = TransmitType.Never;
		}

		public void AttachToRoll(BogRoll roll )
		{
			Team = roll.Team;
			switch ( Team )
			{
				case Team.Invalid:
					break;
				case Team.Red:
					RenderColor = Color.Red;
					break;
				case Team.Blue:
					RenderColor = Color.Blue;
					break;
				default:
					break;
			}
			visuals.RenderColor = RenderColor;
			AttachedRoll = roll;
		}

		[Event.Tick.Server]
		public void Tick()
		{
			if ( AttachedRoll != null )
			{
				Position = Trace.Ray( AttachedRoll.Position, AttachedRoll.Position - Vector3.Up * 10f ).Ignore( AttachedRoll ).Run().EndPos;

				if (Team == Team.Red )
				{
					if(RedPlayers.Count > 0 && BluePlayers.Count == 0 )
					{
						AttachedRoll.TimeSinceDropped += Time.Delta; //Return faster if team matches.
					}

					if ( BluePlayers.Count > 0 && RedPlayers.Count == 0)
					{
						AttachedRoll.TimeSinceDropped -= Time.Delta * 2f; // Timer goes back up to max if enemy team is in the return zone.
						AttachedRoll.TimeSinceDropped = Math.Clamp( AttachedRoll.TimeSinceDropped, 0f, 15f );
					}
				}

				if ( Team == Team.Blue )
				{
					if ( BluePlayers.Count > 0 && RedPlayers.Count == 0 )
					{
						AttachedRoll.TimeSinceDropped += Time.Delta;
					}

					if ( RedPlayers.Count > 0 && BluePlayers.Count == 0 )
					{
						AttachedRoll.TimeSinceDropped -= Time.Delta * 2f;
						AttachedRoll.TimeSinceDropped = Math.Clamp( AttachedRoll.TimeSinceDropped, 0f, 15f );
					}
				}
			}
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( other.IsWorld )
				return;

			if ( Game.Instance.Round is not CombatRound )
				return;

			if ( other is FortwarsPlayer player )
			{
				switch ( player.TeamID )
				{
					case Team.Invalid:
						break;
					case Team.Red:
						if ( !RedPlayers.Contains( player ) )
							RedPlayers.Add( player );
						break;
					case Team.Blue:
						if ( !BluePlayers.Contains( player ) )
							BluePlayers.Add( player );
						break;
					default:
						break;
				}
				
			}
		}

		public override void EndTouch( Entity other )
		{
			base.EndTouch( other );

			if ( other.IsWorld )
				return;

			if ( Game.Instance.Round is not CombatRound )
				return;

			if ( other is FortwarsPlayer player )
			{
				switch ( player.TeamID )
				{
					case Team.Invalid:
						break;
					case Team.Red:
						if(RedPlayers.Contains( player ))
							RedPlayers.Remove( player );
						break;
					case Team.Blue:
						if ( BluePlayers.Contains( player ) )
							BluePlayers.Remove( player );
						break;
					default:
						break;
				}

			}
		}
	}
}
