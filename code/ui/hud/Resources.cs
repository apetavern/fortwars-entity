// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars
{
    public class Resources : Panel
    {
        public Resources()
        {
            StyleSheet.Load( "/ui/hud/Resources.scss" );

            AddResource( "/ui/icons/resources/wood.png", BlockMaterial.Wood );
            AddResource( "/ui/icons/resources/steel.png", BlockMaterial.Steel );
            // AddResource( "/ui/icons/resources/rubber.png", BlockMaterial.Rubber );
        }

        private void AddResource( string name, BlockMaterial resourceType )
        {
            var resource = new Resource( name, resourceType );
            resource.Parent = this;
        }

        class Resource : Panel
        {
            private Label count;
            private Label limit;

            private BlockMaterial resourceType;

            public Resource( string iconPath, BlockMaterial resourceType )
            {
                this.resourceType = resourceType;

                var top = Add.Panel( "top" );
                var bottom = Add.Panel( "bottom" );

                top.Add.Image( iconPath, "icon" );
                count = bottom.Add.Label( "20", "count" );
                limit = bottom.Add.Label( "20", "limit" );
            }

            public override void Tick()
            {
                base.Tick();

                count.Text = resourceType.GetRemainingCount( Local.Client ).ToString();
                limit.Text = resourceType.PlayerLimit.ToString();
            }
        }
    }
}
