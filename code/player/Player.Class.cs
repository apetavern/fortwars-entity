// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.Collections.Generic;

namespace Fortwars;

partial class FortwarsPlayer
{
	public ClassAsset Class { get; set; } = ClassAsset.Default;

	[Net] public string SelectedClass { get; set; } = "data/classes/assault.fwclass";
	[Net] public string SelectedPrimary { get; set; } = "fw:data/weapons/ksr1.fwweapon";
	[Net] public string SelectedSecondary { get; set; } = "fw:data/weapons/trj.fwweapon";

	public bool CanChangeClass => InSpawnRoom || LifeState != LifeState.Alive;
	[Net, Predicted] public bool InSpawnRoom { get; set; }

	public async Task GiveLoadout( List<string> items, Inventory inventory )
	{
		for ( int i = 0; i < items.Count; i++ )
		{
			string itemPath = items[i];
			Log.Trace( itemPath );
			inventory.Add( ItemUtils.GetItem( itemPath ), i == 0 );
			await Task.Delay( 100 ); //Gotta wait between each weapon added so OnChildAdded gets fired in the correct order...
		}
	}

	public void AssignLoadout( string newClassName, string newPrimaryName, string newSecondaryName )
	{
		if ( !CanChangeClass )
		{
			MessageFeed.AddMessage(
				To.Single( Client ),
				"clear",
				"Go to spawn to change loadout." );

			return;
		}

		bool wasLoadoutChanged = false;

		if ( SelectedClass != newClassName )
		{
			wasLoadoutChanged = true;
			SelectedClass = newClassName;
		}

		if ( SelectedPrimary != newPrimaryName )
		{
			WeaponAsset weapon;
			if ( ( weapon = ItemUtils.GetWeaponAsset( newPrimaryName ) ) == null
				|| weapon.InventorySlot != WeaponAsset.InventorySlots.Primary )
				return;

			wasLoadoutChanged = true;
			SelectedPrimary = newPrimaryName;
		}

		if ( SelectedSecondary != newSecondaryName )
		{
			WeaponAsset weapon;
			if ( ( weapon = ItemUtils.GetWeaponAsset( newSecondaryName ) ) == null
				|| weapon.InventorySlot != WeaponAsset.InventorySlots.Secondary )
				return;

			wasLoadoutChanged = true;
			SelectedSecondary = newSecondaryName;
		}

		if ( !wasLoadoutChanged )
			return;

		Class?.Cleanup( Inventory as Inventory );
		Class = ClassAsset.FromPath( SelectedClass );

		if ( Class == null )
			return;

		Reset();
		FortwarsGame.Instance.Round.SetupInventory( this );
	}

	public override void StartTouch( Entity other )
	{
		if ( Game.IsClient ) return;

		if ( other is PickupTrigger )
		{
			StartTouch( other.Parent );
			return;
		}

		Inventory?.Add( other, Inventory.Active == null );

		if ( other is FuncSpawnArea )
			InSpawnRoom = true;
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is FuncSpawnArea )
			InSpawnRoom = false;
	}
}
