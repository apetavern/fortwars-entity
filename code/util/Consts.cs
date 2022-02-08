using System.Collections.Generic;

namespace Fortwars
{
	public static class Consts
	{
		// Class Strings

		// Assault
		public static string AssaultName = "Assault";
		public static string AssaultDescription =
			"With high damage and a strong resolve, " +
			"the Assault class will help your team " +
			"clear enemies with relative ease.";
		public static string AssaultIconPath = "/textures/icons/assault.png";
		public static List<string> AssaultLoadout = new()
		{
			"/some/path/to/rpg"
		};

		// Medic
		public static string MedicName = "Medic";
		public static string MedicDescription =
			"Nobody knows how to keep the team alive " +
			"like a medic! This class will help ensure your " +
			"buddies stay in tip-top shape.";
		public static string MedicIconPath = "/textures/icons/medic.png";

		// Support
		public static string SupportName = "Support";
		public static string SupportDescription =
			"Specialization is for suckers! With support, " +
			"you can help your team in as many creative ways " +
			"as you can think of.";
		public static string SupportIconPath = "/textures/icons/support.png";

		// Engineer
		public static string EngineerName = "Engineer";
		public static string EngineerDescription =
			"The engineer is the king of defense. Fortify " +
			"and repair to victory with your enhanced tools " +
			"and gigantic brain!";
		public static string EngineerPath = "/textures/icons/engineer.png";
	}
}
