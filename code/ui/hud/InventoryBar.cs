// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;

public class InventoryBar : Panel
{
	readonly List<InventoryIcon> slots = new();

	public InventoryBar()
	{
		StyleSheet.Load( "ui/hud/inventorybar.scss" );
		for ( int i = 0; i < 3; i++ )
		{
			var icon = new InventoryIcon( i + 1, this );
			slots.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		var player = Game.LocalPawn;
		if ( player == null ) return;
		if ( ( player as FortwarsPlayer ).Inventory == null ) return;

		for ( int i = 0; i < slots.Count; i++ )
		{
			UpdateIcon( ( player as FortwarsPlayer ).Inventory.GetSlot( i ), slots[i], i );
		}
	}

	private static void UpdateIcon( Entity ent, InventoryIcon inventoryIcon, int i )
	{
		if ( ent == null )
		{
			inventoryIcon.Clear();
			return;
		}

		inventoryIcon.TargetEnt = ent;

		if ( ent is FortwarsWeapon weapon && weapon != null && weapon.WeaponAsset != null )
		{
			inventoryIcon.Label.Text = weapon.WeaponAsset.WeaponName;
			inventoryIcon.AmmoLabel.Text = $"{weapon.CurrentClip} / {weapon.ReserveAmmo}";
			inventoryIcon.AmmoLabel.SetClass( "visible", true );
		}
		else
		{
			inventoryIcon.Label.Text = DisplayInfo.For( ent ).Name;
			inventoryIcon.AmmoLabel.Text = $"";
			inventoryIcon.AmmoLabel.SetClass( "visible", false );
		}

		var player = Game.LocalPawn;
		if ( player == null ) return;

		inventoryIcon.SetClass( "active", ( player as FortwarsPlayer ).ActiveChild == ent );
	}

	[Event.Client.BuildInput]
	public void ProcessClientInput()
	{
		var player = Game.LocalPawn as FortwarsPlayer;
		if ( player == null )
			return;

		var inventory = player.Inventory;
		if ( inventory == null )
			return;

		if ( player.ActiveChild is PhysGun physgun && physgun.BeamActive )
		{
			return;
		}

		if ( Input.Pressed( InputButton.Slot1 ) ) SetActiveSlot( inventory, 0 );
		if ( Input.Pressed( InputButton.Slot2 ) ) SetActiveSlot( inventory, 1 );
		if ( Input.Pressed( InputButton.Slot3 ) ) SetActiveSlot( inventory, 2 );

		if ( Input.MouseWheel != 0 ) SwitchActiveSlot( inventory, -Input.MouseWheel );
	}

	private static void SetActiveSlot( IBaseInventory inventory, int i )
	{
		var player = Game.LocalPawn as FortwarsPlayer;
		if ( player == null )
			return;

		var ent = inventory.GetSlot( i );
		if ( ( player as FortwarsPlayer ).ActiveChild == ent )
			return;

		if ( ent == null )
			return;

		player.ActiveChild = ent;
	}

	private static void SwitchActiveSlot( IBaseInventory inventory, int idelta )
	{
		var count = inventory.Count();
		if ( count == 0 ) return;

		var slot = inventory.GetActiveSlot();
		var nextSlot = slot + idelta;

		while ( nextSlot < 0 ) nextSlot += count;
		while ( nextSlot >= count ) nextSlot -= count;

		SetActiveSlot( inventory, nextSlot );
	}
}

public class InventoryIcon : Panel
{
	public Entity TargetEnt;
	public Label Label;
	public Label AmmoLabel;

	public InventoryIcon( int i, Panel parent )
	{
		Parent = parent;
		Label = Add.Label( "empty", "item-name" );
		AmmoLabel = Add.Label( "0 / 0", "ammo" );

		var inputButton = (InputButton)Enum.Parse( typeof( InputButton ), $"Slot{i}" );
		Add.InputHint( inputButton, inputButton, "", "slot-number" );
	}

	public void Clear()
	{
		Label.Text = "";
		AmmoLabel.SetClass( "visible", false );
		SetClass( "active", false );
	}
}
