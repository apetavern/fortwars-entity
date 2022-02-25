using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Fortwars
{
	public partial class SpeechRecognitionComponent : EntityComponent
	{
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
		};

		bool wantstostop;

		[Event.Tick.Client]
		void Tick()
		{
			if ( Game.Instance.Round is BuildRound )
			{
				if ( Input.Pressed( InputButton.Attack2 ) )
				{
					SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices.Keys.ToArray() ); //Listen for speech
				}

				if ( Input.Released( InputButton.Attack2 ) )
				{
					if ( !wantstostop )
					{
						wantstostop = true;
					}
					else
					{
						SpeechRecognition.Stop(); //Only fires second mouse2 release while it was still listening, reduces amount of accidental activations
						wantstostop = false;
					}
				}
			}
		}

		public void SpawnBlock( string output )
		{
			if ( choices.ContainsKey( output ) )
				Game.Spawn( choices[output] );

			if ( wantstostop )
			{
				SpeechRecognition.Stop();
				wantstostop = false;
			}
			else
			{
				SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices.Keys.ToArray() ); //Not sure why but this is needed...
			}
		}
	}
}
