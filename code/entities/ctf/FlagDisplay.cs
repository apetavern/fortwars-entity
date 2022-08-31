// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

public partial class FlagDisplay : ModelEntity, IShowIcon
{
	[Net, Change( "OnTeamChange" )] public Team Team { get; set; }

	#region Icons
	private FortwarsPlayer GetCarrier()
	{
		FortwarsPlayer carrier = null;

		switch ( Team )
		{
			case Team.Invalid:
				carrier = null;
				break;
			case Team.Red:
				carrier = Game.Instance.RedFlagCarrier;
				break;
			case Team.Blue:
				carrier = Game.Instance.BlueFlagCarrier;
				break;
		}

		return carrier;
	}

	Vector3 IShowIcon.IconWorldPosition()
	{
		var carrier = GetCarrier();

		if ( carrier == null || !carrier.IsValid )
			return Position;
		else
			return carrier.Position;
	}

	bool IShowIcon.DrawIcon() => !GetCarrier()?.IsLocalPawn ?? true;

	string IShowIcon.CustomClassName() => Team.ToString();
	string IShowIcon.NonDiegeticIcon() => "flag";
	string IShowIcon.SpatialIcon() => "flag";

	#endregion

	public override void Spawn()
	{
		base.Spawn();

		PhysicsEnabled = false;
		UsePhysicsCollision = false;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		SetModel( "models/items/bogroll/bogroll_w.vmdl" );
		Scale = 1.5f;

		var bobbingComponent = Components.Create<BobbingComponent>();
		bobbingComponent.PositionOffset = Vector3.Up * 10f;
	}

	public void OnTeamChange()
	{
		switch ( Team )
		{
			case Team.Invalid:
				break;
			case Team.Red:
				SetMaterialGroup( 1 );
				break;
			case Team.Blue:
				SetMaterialGroup( 0 );
				break;
			default:
				break;
		}
	}
}
