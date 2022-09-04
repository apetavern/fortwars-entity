// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Collections.Generic;

namespace Fortwars;

[GameResource( "Class Definition", "fwclass", "Describes a FortWars class", Icon = "⚒️", IconBgColor = "#fe71dc", IconFgColor = "black" )]
public class ClassAsset : GameResource
{
	//
	// Meta
	//
	[Property, Category( "Meta" )]
	public string ClassName { get; set; } = "My class";

	[Property, Category( "Meta" )]
	public string ShortDescription { get; set; } = "My class";

	[Property, Category( "Meta" ), ResourceType( "png" )]
	public string IconPath { get; set; }

	[Property, Category( "Meta" ), ResourceType( "fwweapon" )]
	public string Gadget { get; set; }

	[Property, Category( "Meta" )]
	public float SpeedMultiplier { get; set; } = 1.0f;

	[Property, Category( "Meta" )]
	public float JumpMultiplier { get; set; } = 1.0f;

	[Property, Category( "Meta" )]
	public bool TakesFallDamage { get; set; } = true;

	[Property, Category( "Meta" ), ResourceType( "clothing" )]
	public List<string> Cosmetics { get; set; } = new();

	[Property, Category( "Meta" ), ResourceType( "vmdl" )]
	public string PreviewWeapon { get; set; }

	[Property, Category( "Meta" )]
	public HoldTypes PreviewHoldType { get; set; } = HoldTypes.Pistol;

	[Property, Category( "Meta" )]
	public HoldHandedness PreviewHoldHandedness { get; set; } = HoldHandedness.TwoHands;

	[Property, Category( "Meta" )]
	public bool IsSelectable { get; set; } = true;
}
