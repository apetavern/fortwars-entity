
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace Fortwars.UI
{
    public class Vitals : Panel
    {
        public Label Health;
        public List<Panel> Segments;

        public Vitals()
        {
            var segmentsContainer = Add.Panel( "segments" );
            Segments = new List<Panel>();
            for ( int i = 0; i < 14; i++ )
                Segments.Add( segmentsContainer.Add.Panel() );

            Health = Add.Label( "100", "health" );
        }

        public override void Tick()
        {
            var player = Local.Pawn;
            if ( player == null ) return;

            var col = ColorConvert.HSLToRGB( (int)(360f * player.Health), 1.0f, 0.5f );

            Health.Text = $"{player.Health:n0}";
            Health.Style.FontColor = col;
            Health.Style.Dirty(); // todo: don't call every frame?

            var activeSegments = (int)MathF.Round( (player.Health / 100.0f) * Segments.Count );

            // kinda shit but fuck it
            for ( int i = 0; i < Segments.Count; i++ )
            {
                var segment = Segments[i];

                if ( i < activeSegments )
                {
                    segment.Style.BackgroundColor = col;
                    segment.Style.Opacity = 1.0f;
                    segment.Style.Dirty();
                    continue;
                }

                segment.Style.BackgroundColor = Color.Black;
                segment.Style.Opacity = 0.35f;
                segment.Style.Dirty();
            }
        }
    }
}
