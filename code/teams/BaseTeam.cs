using Sandbox;
using System;

namespace Fortwars
{
	public enum Team
	{
		Invalid = -1,
		Red,
		Blue
	}

	public abstract class BaseTeam
	{
		public virtual Team ID => Team.Invalid;

		public virtual string Name => "";

		public virtual void OnPlayerSpawn( FortwarsPlayer player ) { }
	}
}
