// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace Fortwars;

    [Library( "richlabel", Alias = new[] { "richtext" } )]
    public class RichLabel : Panel
    {
        private List<Label> labels = new();

        public string Text { get; set; }

        public RichLabel()
        {
            StyleSheet.Load( "ui/elements/generic/richlabel.scss" );
        }

        public override void SetContent( string value )
        {
            Text = value;
        }

        protected override void PostTemplateApplied()
        {
            base.PostTemplateApplied();
            Build();
        }

        public override void OnHotloaded()
        {
            base.OnHotloaded();
            Build();
        }

        private void Build()
        {
            Log.Trace( $"Building rich label for '{Text}'" );
            labels.ForEach( x => x.Delete() );
            labels.Clear();

            var parser = new RichLabelParser( Text );

            float huePerCharacter = 360f / (float)Text.Length;
            int index = 0;
            foreach ( var token in parser.Run() )
            {
                var label = new Label();
                label.Parent = this;

                label.Text = token.Text;
                label.AddClass( token.Class );

                labels.Add( label );
                index++;
            }
        }
    }
