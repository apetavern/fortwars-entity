using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	public partial class SpeechRecognitionComponent : EntityComponent
	{
		public SpeechRecognitionComponent()
		{
			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices.Keys.ToArray() );//Listen for speech
		}

		Dictionary<string, string> choices = new Dictionary<string, string>{
			{ "Spawn One By Two","1x2" },
			{ "Spawn One By Four","1x4" },
			{ "Spawn Three By Two","3x2" },
			{ "Spawn Box","1x1x1" },
			{ "Spawn Tall Box","1x2x1" },
			{ "Spawn Steel One By Two","steel_1x2" },
			{ "Spawn Steel One By Four","steel_1x4" },
			{ "Spawn Steel Three By Two","steel_3x2" },
			{ "Spawn Steel Box","steel_1x1x1" },
			{ "Spawn Steel Tall Box","steel_1x2x1" },

		};

		public void SpawnBlock( string output )
		{
			if(choices.ContainsKey( output ) )
				Game.Spawn( choices[output] );

			Log.Trace( output );

			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices.Keys.ToArray() );//Not sure why but this is needed...
		}
	}
}
