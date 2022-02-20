using Sandbox;
using System;

namespace Fortwars
{
	public partial class SpeechRecognitionComponent : EntityComponent
	{
		public SpeechRecognitionComponent()
		{
			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices );//Listen for speech
		}

		string[] choices = new string[] {
		"Spawn One By Two",
		"Spawn One By Four",
		"Spawn Three By Two",
		"Spawn Box",
		"Spawn Tall Box",
		"Spawn Steel One By Two",
		"Spawn Steel One By Four",
		"Spawn Steel Three By Two",
		"Spawn Steel Box",
		"Spawn Steel Tall Box",
		};

		public void SpawnBlock( string output )
		{
			if ( output.ToLower().Contains( "spawn" ) )
			{
				if ( output.ToLower().Contains( "steel" ) )
				{
					if ( output.ToLower().Contains( "one by two" ) )
					{
						Game.Spawn( "steel_1x2" );
					}

					if ( output.ToLower().Contains( "one by four" ) )
					{
						Game.Spawn( "steel_1x4" );
					}
					if ( output.ToLower().Contains( "three by two" ) )
					{
						Game.Spawn( "steel_3x2" );
					}

					if ( output.ToLower().Contains( "tall box" ) )
					{
						Game.Spawn( "steel_1x2x1" );
					}
					else if ( output.ToLower().Contains( "box" ) )
					{
						Game.Spawn( "steel_1x1x1" );
					}
				}
				else
				{
					if ( output.ToLower().Contains( "one by two" ) )
					{
						Game.Spawn( "1x2" );
					}

					if ( output.ToLower().Contains( "one by four" ) )
					{
						Game.Spawn( "1x4" );
					}
					if ( output.ToLower().Contains( "three by two" ) )
					{
						Game.Spawn( "3x2" );
					}

					if ( output.ToLower().Contains( "tall box" ) )
					{
						Game.Spawn( "1x2x1" );
					}
					else if ( output.ToLower().Contains( "box" ) )
					{
						Game.Spawn( "1x1x1" );
					}
				}

			}

			Log.Trace( output );

			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices );//Not sure why but this is needed...
		}
	}
}
