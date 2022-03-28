// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars;

public class ShopMenu : Panel
{
	public ShopMenu()
	{
		StyleSheet.Load( "ui/menus/ShopMenu.scss" );

		AddClass( "menu" );

		var top = Add.Panel( "top" );
		top.Add.Label( "SHOP", "title" );

		var main = Add.Panel( "main" );

		//
		// Left container - categories
		//
		{
			var leftContainer = main.Add.Panel( "left-container" );

			var categories = new (string, string)[]
			{
					( "Upgrades", "upgrade" ),
					( "Weapons", "clear" ),
					( "Refills", "refresh" ),
					( "Materials", "construction" ),
			};

			foreach ( var item in categories )
			{
				leftContainer.Add.ButtonWithIcon( item.Item1, item.Item2, "category" );
			}
		}

		//
		// Item container
		//
		{
			var itemContainer = main.Add.Panel( "item-container" );

			var items = new ShopItem[]
			{
			//
			//
			new ShopItem( "MP5", "Gun", 1000 ),
				new ShopItem( "HK SMG2", "Gun", 2500 ),
				new ShopItem( "Rocket Launcher", "Gun", 5000 ),

			//
			//
			new ShopItem( "Grenade", "Gun", 100 ),

			//
			//
			new ShopItem( "Ammo Refill", "Gun", 100 ),

			//
			//
			new ShopItem( "10 Wood", "Gun", 100 ),
				new ShopItem( "10 Metal", "Gun", 250 ),
				new ShopItem( "10 Rubber", "Gun", 500 ),
			};

			foreach ( var item in items )
			{
				var itemPanel = new ShopUpgradePanel( item );
				itemPanel.Parent = itemContainer;
			}
		}

		//
		// Item info
		//
		{
			var itemInfo = main.Add.Panel( "item-info" );
			itemInfo.Add.Image( "tools/images/common/generic_hud_warning.png", "icon" );
			itemInfo.Add.Label( "My cool item", "subtitle" );
			itemInfo.Add.Label( "$20000", "cost" );
			itemInfo.Add.Label( "Lorem ipsum dolor sit amet, consectetur adipiscing elit.", "description" );

			itemInfo.Add.Button( "BUY", "buy-button" );
		}
	}
}

public struct ShopItem
{
	public ShopItem( string name, string description, int cost )
	{
		Name = name;
		Description = description;
		Cost = cost;
	}

	public string Name { get; set; }
	public string Description { get; set; }
	public int Cost { get; set; }

}

public class ShopItemPanel : Panel
{
	public ShopItemPanel( ShopItem item )
	{
		Add.Image( "tools/images/common/generic_hud_warning.png", "icon" );

		var itemMeta = Add.Panel( "item-meta" );
		itemMeta.Add.Label( item.Name, "subtitle" );
		itemMeta.Add.Label( "$20000", "cost" );
	}
}

public class ShopUpgradePanel : Panel
{
	public ShopUpgradePanel( ShopItem item )
	{
		Add.Image( "tools/images/common/generic_hud_warning.png", "icon" );

		var itemMeta = Add.Panel( "item-meta" );
		itemMeta.Add.Label( item.Name, "subtitle" );
		itemMeta.AddChild<UpgradeBar>();
	}

	class UpgradeBar : Panel
	{
		public UpgradeBar()
		{
			for ( int i = 0; i < 5; i++ )
			{
				var section = Add.Panel( "upgrade-section" );
				section.Add.Label( $"${i * 1000}", "requirement" );
			}
		}
	}
}
