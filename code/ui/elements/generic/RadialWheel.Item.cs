// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

namespace Fortwars
{
    public partial class RadialWheel
    {
        public struct Item
        {
            public string Icon { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public Item( string name, string description, string icon = "question_mark" )
            {
                Name = name;
                Description = description;
                Icon = icon;
            }
        }
    }
}
