namespace Fortwars
{
	partial class Crosshair
	{
		public struct CrosshairConfig
		{
			public enum CrosshairStyle
			{
				Static,
				Dynamic,
				Dot
			};

			public CrosshairStyle Style { get; set; }

			//
			// Scaling
			//
			public int Size { get; set; }
			public int Gap { get; set; }
			public bool Outline { get; set; }
			public int Thickness { get; set; }

			//
			// Color / opacity
			//
			public float Opacity { get; set; }
			public Color Color { get; set; }
			public Color OutlineColor { get; set; }

			public static CrosshairConfig Default => new CrosshairConfig()
			{
				Style = CrosshairStyle.Dynamic,
				Size = 64,
				Gap = 16,

				Thickness = 4,
				Opacity = 1.0f,
				Color = new Color( 1, 0, 1 ),

				Outline = true,
				OutlineColor = Color.Black
			};
		}
	}
}
