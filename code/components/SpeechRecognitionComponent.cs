using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	public partial class SpeechRecognitionComponent : EntityComponent
	{
		public SpeechRecognitionComponent()
		{
			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ));//Listen for speech //, choices.Keys.ToArray() 
		}

		Dictionary<string, string> choices = new Dictionary<string, string>{
			{ "Spawn door","1x2" },
			{ "Spawn plank","1x4" },
			{ "Spawn wall","3x2" },
			{ "Spawn box","1x1x1" },
			{ "Spawn tall box","1x2x1" },
			{ "Spawn steel door","steel_1x2" },
			{ "Spawn steel plank","steel_1x4" },
			{ "Spawn steel wall","steel_3x2" },
			{ "Spawn steel box","steel_1x1x1" },
			{ "Spawn steel tall box","steel_1x2x1" },
			{ "Spahn door","1x2" },
			{ "Spahn plank","1x4" },
			{ "Spahn wall","3x2" },
			{ "Spahn box","1x1x1" },
			{ "Spahn tall box","1x2x1" },
			{ "Spahn steel door","steel_1x2" },
			{ "Spahn steel plank","steel_1x4" },
			{ "Spahn steel wall","steel_3x2" },
			{ "Spahn steel box","steel_1x1x1" },
			{ "Spahn steel tall box","steel_1x2x1" },
			{ "Spawned door","1x2" },
			{ "Spawned plank","1x4" },
			{ "Spawned wall","3x2" },
			{ "Spawned box","1x1x1" },
			{ "Spawned tall box","1x2x1" },
			{ "Spawned steel door","steel_1x2" },
			{ "Spawned steel plank","steel_1x4" },
			{ "Spawned steel wall","steel_3x2" },
			{ "Spawned steel box","steel_1x1x1" },
			{ "Spawned steel tall box","steel_1x2x1" },

		};

		public void SpawnBlock( string output )
		{
			if(choices.ContainsKey( output ) )
				Game.Spawn( choices[output] );

			Log.Trace( output );

			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ));//Not sure why but this is needed...//, choices.Keys.ToArray() 
		}
	}
}
