using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	public partial class VoteRound : BaseRound
	{
		public override string RoundName => "Voting";
		public override int RoundDuration => 12;

		protected override void OnStart()
		{
			Log.Info( "Started Vote Round" );
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
				Global.ChangeLevel( Game.GetMaps()[0] );
			}

			var votedMap = sortedMapVotePairs.First();
			Global.ChangeLevel( Game.GetMaps()[votedMap.Key] );
		}
	}
}
