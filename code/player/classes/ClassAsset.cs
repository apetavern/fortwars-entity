// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.Text.Json.Serialization;

namespace Fortwars;

[GameResource( "Class Definition", "fwclass", "Describes a FortWars class", Icon = "⚒️", IconBgColor = "#fe71dc", IconFgColor = "black" )]
public class ClassAsset : GameResource
{
	//
	// Meta
	//
	[Category( "Meta" )]
	public string ClassName { get; set; } = "My class";

	[Category( "Meta" )]
	public string ShortDescription { get; set; } = "My class";

	[Category( "Meta" ), ResourceType( "png" )]
	public string IconPath { get; set; }

	[Category( "Meta" )]
	public bool IsSelectable { get; set; } = true;

	[Category( "Meta" ), ResourceType( "clothing" )]
	public List<string> Cosmetics { get; set; } = new();

	//
	// FortwarsGameplay
	//
	[Category( "FortwarsGameplay" )]
	public float SpeedMultiplier { get; set; } = 1.0f;

	[Category( "FortwarsGameplay" )]
	public float JumpMultiplier { get; set; } = 1.0f;

	[Category( "FortwarsGameplay" )]
	public bool TakesFallDamage { get; set; } = true;

	[Category( "FortwarsGameplay" )]
	public bool UsesWeaponGadget { get; set; }

	[ShowIf( "UsesWeaponGadget", true ), Category( "FortwarsGameplay" ), ResourceType( "fwweapon" )]
	public string WeaponGadget { get; set; }

	[HideIf( "UsesWeaponGadget", true ), Category( "FortwarsGameplay" )]
	public string LibraryGadget { get; set; }

	[HideInEditor, JsonIgnore]
	public string Gadget => UsesWeaponGadget ? "fw:" + WeaponGadget : LibraryGadget;

	//
	// Preview
	//
	[Category( "Preview" ), ResourceType( "vmdl" )]
	public string PreviewWeapon { get; set; }

	[Category( "Preview" )]
	public CitizenAnimationHelper.HoldTypes PreviewHoldType { get; set; } = CitizenAnimationHelper.HoldTypes.Pistol;

	[Category( "Preview" )]
	public CitizenAnimationHelper.Hand PreviewHoldHandedness { get; set; } = CitizenAnimationHelper.Hand.Both;

	[Category( "Preview" )]
	public float PreviewHandPose { get; set; }

	[Category( "Preview" )]
	public float PreviewHoldPose { get; set; }

	public static ClassAsset FromPath( string path )
	{
		return ResourceLibrary.Get<ClassAsset>( path );
	}

	public static ClassAsset Default => FromPath( "data/classes/assault.fwclass" );

	private async void AssignLoadout( List<string> items, Inventory inventory )
	{
		for ( int i = 0; i < items.Count; i++ )
		{
			string itemPath = items[i];
			inventory.Add( ItemUtils.GetItem( itemPath ), i == 0 );
			await Task.Delay( 100 ); //Gotta wait between each weapon added so OnChildAdded gets fired in the correct order...
		}
	}

	public virtual void Cleanup( Inventory inventory )
	{
		inventory.DeleteContents();
	}
}
