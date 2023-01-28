namespace Fortwars;

public class BogRollComponent : WeaponComponent
{
	protected override void OnStart( Player player )
	{
		var team = player.Client.Components.Get<TeamComponent>().Team;
		BogRoll.SetMaterialGroup( TeamSystem.GetOpposingTeam( team ), Weapon );
	}

	public override void OnGameEvent( string eventName )
	{
		if ( eventName == "inv.switchweapon" )
		{
			var inv = Player.Inventory;

			if ( inv.ActiveWeapon != Weapon )
				return;

			inv.RemoveWeapon( Weapon );
			GamemodeSystem.Instance.OnWeaponDropped( Player, Weapon );
		}
	}
}
