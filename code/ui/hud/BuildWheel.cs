using Sandbox;
using System.Threading.Tasks;

namespace Fortwars
{
	public partial class BuildWheel : RadialWheel
	{
		public override Item[] Items => new Item[]
		{
			new ("3x2", "A wide panel good for defences", "crop_5_4"),
			new ("1x2", "A medium panel good for entrances", "crop_portrait"),
			new ("1x4", "A tall panel good for ledges", "crop_7_5"),
			new ("1x1x1", "A thicc block good for climbing", "view_in_ar"),
			new ("1x2x1", "A thicc block good for cover", "view_in_ar"),
		};

		public BuildWheel() : base()
		{
			VirtualCursor.OnClick += OnClick;
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
			await Task.DelaySeconds( 0.150f );
			Wrapper.RemoveClass( "shrink" );
		}
	}
}
