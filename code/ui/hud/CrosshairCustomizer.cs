using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using static Fortwars.Crosshair;

namespace Fortwars
{
	public class CrosshairCustomizer : Panel
	{
		SliderEntry size;
		SliderEntry gap;
		Checkbox outline;
		SliderEntry thickness;
		SliderEntry opacity;

		public CrosshairCustomizer()
		{
			StyleSheet.Load( "/ui/hud/CrosshairCustomizer.scss" );
			Add.Label( "Crosshair", "subtitle" );

			size = AddFormElement( "Size", Add.SliderWithEntry( 0, 64, 1 ) );
			gap = AddFormElement( "Gap", Add.SliderWithEntry( 0, 16, 1 ) );
			outline = AddFormElement( "Outline", AddChild<Checkbox>() );
			thickness = AddFormElement( "Thickness", Add.SliderWithEntry( 0, 4, 1 ) );
			opacity = AddFormElement( "Opacity", Add.SliderWithEntry( 0, 1, 0.1f ) );

			AddFormElement( "Color", Add.Slider( 0, 32, 1 ) );
			AddFormElement( "Outline Color", Add.Slider( 0, 32, 1 ) );
			Add.Button( "Apply", Apply );
		}

		private T AddFormElement<T>( string label, T formElement ) where T : Panel
		{
			var row = Add.Panel( "row" );
			row.Add.Label( label );
			formElement.Parent = row;

			return formElement;
		}

		public void Apply()
		{
			if ( Local.Pawn.ActiveChild is not Carriable carriable )
				return;

			var config = CrosshairConfig.Default;

			config.Size = size.Value.CeilToInt();
			config.Gap = gap.Value.CeilToInt();
			config.Outline = outline.Checked;
			config.Thickness = thickness.Value.CeilToInt();
			config.Opacity = opacity.Value;

			Crosshair.Config = config;

			carriable.DestroyHudElements();
			carriable.CreateHudElements();
		}
	}
}
