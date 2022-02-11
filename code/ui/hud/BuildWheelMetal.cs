using Sandbox;
using System.Threading.Tasks;

namespace Fortwars
{
	public partial class BuildWheelMetal : RadialWheel
	{
		public override Item[] Items => new Item[]
		{
			new ("metal3x2", "A wide panel good for defences", "/ui/models/blocks/fw_3x2.png"),
			new ("metal1x2", "A medium panel good for entrances", "/ui/models/blocks/fw_1x2.png"),
			new ("metal1x4", "A tall panel good for ledges", "/ui/models/blocks/fw_1x4.png"),
			new ("metal1x1x1", "A thicc block good for climbing", "/ui/models/blocks/fw_1x1x1.png"),
			new ("metal1x2x1", "A thicc block good for cover", "/ui/models/blocks/fw_1x2x1.png"),
		};

		public BuildWheelMetal() : base()
		{
			VirtualCursor.OnClick += OnClick;

			BindClass( "active", () => Input.Down( InputButton.Menu ) && Input.Down(InputButton.Use) && Game.Instance.Round is BuildRound );
		}

		private void OnClick()
		{
			ConsoleSystem.Run( $"fw_spawn {GetCurrentItem()?.Name}" );
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
	}
}
