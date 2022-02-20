using Sandbox;
using System.Threading.Tasks;

namespace Fortwars
{
	public partial class BuildWheel : RadialWheel
	{
		public override Item[] Items => new Item[]
		{
			new ("3x2", "A wide panel good for defences", "/ui/icons/wood/fw_3x2.png"),
			new ("1x2", "A medium panel good for entrances", "/ui/icons/wood/fw_1x2.png"),
			new ("1x4", "A tall panel good for ledges", "/ui/icons/wood/fw_1x4.png"),
			new ("1x1x1", "A thicc block good for climbing", "/ui/icons/wood/fw_1x1x1.png"),
			new ("1x2x1", "A thicc block good for cover", "/ui/icons/wood/fw_1x2x1.png"),
		};

		public BuildWheel() : base()
		{
			VirtualCursor.OnClick += OnClick;

			BindClass( "active", () => Input.Down( InputButton.Menu ) && !Input.Down( InputButton.Use ) && Game.Instance.Round is BuildRound );

			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices );//Listen for speech
		}

		private void OnClick()
		{
			if ( !HasClass( "active" ) )
			{
				return;
			}

			Game.Spawn( GetCurrentItem()?.Name );
			_ = ApplyShrinkEffect();
		}

		protected override void OnChange()
		{
			_ = ApplyShrinkEffect();
			PlaySound( "ui_tick" );
		}

		private async Task ApplyShrinkEffect()
		{
			Wrapper.AddClass( "shrink" );
			await Task.DelaySeconds( 0.050f );
			Wrapper.RemoveClass( "shrink" );
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
						ConsoleSystem.Run( $"fw_spawn steel_1x2" );
					}

					if ( output.ToLower().Contains( "one by four" ) )
					{
						ConsoleSystem.Run( $"fw_spawn steel_1x4" );
					}
					if ( output.ToLower().Contains( "three by two" ) )
					{
						ConsoleSystem.Run( $"fw_spawn steel_3x2" );
					}

					if ( output.ToLower().Contains( "tall box" ) )
					{
						ConsoleSystem.Run( $"fw_spawn steel_1x2x1" );
					}
					else if ( output.ToLower().Contains( "box" ) )
					{
						ConsoleSystem.Run( $"fw_spawn steel_1x1x1" );
					}
				}
				else
				{
					if ( output.ToLower().Contains( "one by two" ) )
					{
						ConsoleSystem.Run( $"fw_spawn 1x2" );
					}

					if ( output.ToLower().Contains( "one by four" ) )
					{
						ConsoleSystem.Run( $"fw_spawn 1x4" );
					}
					if ( output.ToLower().Contains( "three by two" ) )
					{
						ConsoleSystem.Run( $"fw_spawn 3x2" );
					}

					if ( output.ToLower().Contains( "tall box" ) )
					{
						ConsoleSystem.Run( $"fw_spawn 1x2x1" );
					}
					else if ( output.ToLower().Contains( "box" ) )
					{
						ConsoleSystem.Run( $"fw_spawn 1x1x1" );
					}
				}

			}

			Log.Trace( output );

			SpeechRecognition.Listen( ( output ) => SpawnBlock( output ), choices );//Not sure why but this is needed...
		}
	}
}
