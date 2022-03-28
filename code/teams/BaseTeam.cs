// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

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

        public virtual Color Color => Color.Gray;

        public virtual void OnPlayerSpawn( FortwarsPlayer player ) { }
    }
}
