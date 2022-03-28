// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;

namespace Fortwars;

[Library( "medkittool", Title = "Medkit" )]
public partial class MedkitTool : DropTool
{
	public override void SpawnPickup()
	{
		base.SpawnPickup();

		var projectile = new BigHealthPickup();
		projectile.Rotation = Owner.EyeRotation.Angles().WithPitch( 0 ).ToRotation();
		projectile.Position = Owner.EyePosition - Vector3.Up * 15f;
		projectile.Velocity = projectile.Rotation.Forward * 250f;

		projectile.Owner = Owner;
	}
}
