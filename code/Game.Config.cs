using Sandbox;

namespace Fortwars
{
	partial class Game : Sandbox.Game
	{
		[Net] public int WoodPerPlayer { get; set; } = 50;
		[Net] public int SteelPerPlayer { get; set; } = 35;
		[Net] public int RubberPerPlayer { get; set; } = 20;
	}
}
