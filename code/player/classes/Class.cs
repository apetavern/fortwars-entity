namespace Fortwars
{
	public struct Class
	{
		public Class( string name, string description, string icon, string toolName )
		{
			Name = name;
			Description = description;
			Icon = icon;
			ToolName = toolName;
		}

		public string Name { get; set; }
		public string Description { get; set; }
		public string Icon { get; set; }
		public string ToolName { get; set; }

		public static Class[] Classes => new Class[]
		{
			new Class( "Assault", "Lorem ipsum dolor sit amet", "sports_martial_arts", "repairtool" ),
			new Class( "Medic", "Lorem ipsum dolor sit amet", "sports_martial_arts", "repairtool" ),
			new Class( "Support", "Lorem ipsum dolor sit amet", "sports_martial_arts", "repairtool" ),
			new Class( "Engineer", "Lorem ipsum dolor sit amet", "sports_martial_arts", "repairtool" ),
		};
	}
}
