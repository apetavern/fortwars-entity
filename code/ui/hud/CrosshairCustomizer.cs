// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;
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

        ColorEditor color;
        ColorEditor outlineColor;

        DropDown style;

        public CrosshairCustomizer()
        {
            StyleSheet.Load( "/ui/hud/CrosshairCustomizer.scss" );
            Add.Label( "Crosshair", "subtitle" );

            style = AddFormElement( "Style", AddChild<DropDown>() );
            foreach ( var value in Enum.GetValues( typeof( CrosshairConfig.CrosshairStyle ) ) )
            {
                CrosshairConfig.CrosshairStyle style = (CrosshairConfig.CrosshairStyle)value;
                this.style.Options.Add( new Option( style.ToString(), style ) );
            }
            style.Selected = style.Options.First();

            size = AddFormElement( "Size", Add.SliderWithEntry( 0, 128, 2 ) );
            gap = AddFormElement( "Gap", Add.SliderWithEntry( 0, 32, 2 ) );
            outline = AddFormElement( "Outline", AddChild<Checkbox>() );
            thickness = AddFormElement( "Thickness", Add.SliderWithEntry( 0, 8, 1 ) );
            opacity = AddFormElement( "Opacity", Add.SliderWithEntry( 0, 1, 0.1f ) );

            color = AddFormElement( "Color", AddChild<ColorEditor>() );
            outlineColor = AddFormElement( "Outline Color", AddChild<ColorEditor>() );

            //
            // Events
            //
            style.AddEventListener( "value.changed", Apply );
            size.AddEventListener( "value.changed", Apply );
            gap.AddEventListener( "value.changed", Apply );
            outline.AddEventListener( "value.changed", Apply );
            thickness.AddEventListener( "value.changed", Apply );
            opacity.AddEventListener( "value.changed", Apply );
            color.AddEventListener( "value.changed", Apply );
            outlineColor.AddEventListener( "value.changed", Apply );

            //
            // Buttons
            //
            Add.Button( "Apply", Apply );
            Add.Button( "Close", () => Delete() );
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
            if ( ( Local.Pawn as FortwarsPlayer ).ActiveChild is not Carriable carriable )
                return;

            var config = CrosshairConfig.Default;

            config.Size = size.Value.CeilToInt();
            config.Gap = gap.Value.CeilToInt();
            config.Outline = outline.Checked;
            config.Thickness = thickness.Value.CeilToInt();
            config.Opacity = opacity.Value;

            config.OutlineColor = outlineColor.Value.ToColor();
            config.Color = color.Value.ToColor();

            Crosshair.Config = config;

            carriable.DestroyHudElements();
            carriable.CreateHudElements();
        }
    }
}
