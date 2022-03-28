// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

    [Library( "fwclass_medic" )]
    public class MedicClass : Class
    {
        public override string Name => "Medic";
        public override string Description =>
            "Nobody knows how to keep the team alive " +
            "like a medic! This class will help ensure your " +
            "buddies stay in tip-top shape.";
        public override string IconPath => "/textures/icons/medic.png";

        public override List<string> BuildLoadout => new()
        {
            "medkittool"
        };

        public override List<string> CombatLoadout => new()
        {
            "fw:data/weapons/uts15.fwweapon",
            "fw:data/weapons/usp.fwweapon",
            "medkittool"
        };
    }
