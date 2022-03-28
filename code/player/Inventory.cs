// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System;
using System.Linq;

namespace Fortwars;

public partial class Inventory : BaseInventory
{
	public Inventory( Player player ) : base( player )
	{

	}

	public override Entity DropActive()
	{
		if ( Active is FortwarsWeapon weapon )
			return null;

		return base.DropActive();
	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		if ( List.Count >= 3 && ent is not BogRoll )
			return false;

		return base.Add( ent, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.GetType() == t );
	}

	[AdminCmd( "fw_inventory_give" )]
	public static void GiveCommand( string itemName )
	{
		var player = ConsoleSystem.Caller.Pawn;
		var item = Library.Create<Entity>( itemName );
		( player as FortwarsPlayer ).Inventory.Add( item );
		Log.Info( $"Gave {player.Client.Name} 1x {itemName}" );
	}
}
