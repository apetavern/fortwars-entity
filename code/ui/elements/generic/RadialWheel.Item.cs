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
