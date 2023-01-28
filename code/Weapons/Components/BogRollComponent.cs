namespace Fortwars;

public class BogRollComponent : WeaponComponent
{
	protected override void OnStart( Player player )
	{
		var team = player.Client.Components.Get<TeamComponent>().Team;
		BogRoll.SetMaterialGroup( TeamSystem.GetOpposingTeam( team ), Weapon );
	}
}
