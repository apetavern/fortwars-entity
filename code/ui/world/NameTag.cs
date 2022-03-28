// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars;

    /// <summary>
    /// A nametag panel in the world
    /// </summary>
    public class NameTag : WorldPanel
    {
        public Panel avatar;
        public Label nameLabel;
        public Panel healthPanel;

        public NameTag( string title, long? steamid )
        {
            StyleSheet.Load( "/ui/world/NameTag.scss" );

            var inner = Add.Panel( "inner" );
            nameLabel = inner.Add.Label( title, "title" );

            if ( steamid != null )
            {
                avatar = inner.Add.Panel( "avatar" );
                avatar.Style.SetBackgroundImage( $"avatar:{steamid}" );
            }

            healthPanel = Add.Panel( "health-bar" );

            Vector2 size = new Vector2( 750, 150 );
            PanelBounds = new Rect( -( size / 2 ).x, -( size / 2 ).y, size.x, size.y );
        }
    }
