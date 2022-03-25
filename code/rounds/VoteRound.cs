using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	public partial class VoteRound : BaseRound
	{
		public override string RoundName => "Voting";
		public override int RoundDuration => 20;

		protected override void OnStart()
		{
			Log.Info( "Started Vote Round" );

			Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( player =>
			{
				if ( player.LifeState != LifeState.Alive )
					player.RespawnTimer = 0;
			} );
		}

		public override void OnTick()
		{
			base.OnTick();

			// Keep players alive during this round
			Entity.All.OfType<FortwarsPlayer>().ToList().ForEach( player => player.Health = 100 );
		}

		protected override void OnTimeUp()
		{
			var game = Game.Instance;
			if ( game == null ) return;

			Dictionary<int, int> voteCount = new();

			foreach ( var vote in game.MapVotes )
			{
				if ( !voteCount.ContainsKey( vote.MapIndex ) )
					voteCount.Add( vote.MapIndex, 0 );

				voteCount[vote.MapIndex]++;
			}

			var sortedMapVotePairs =
				from entry in voteCount
				orderby entry.Value descending
				select entry;

			if ( sortedMapVotePairs.Count() == 0 )
			{
				Global.ChangeLevel( Rand.FromList( Game.GetMaps() ) );
				return;
			}

			var votedMap = sortedMapVotePairs.First();
			Global.ChangeLevel( Game.GetMaps()[votedMap.Key] );
		}
	}
}
