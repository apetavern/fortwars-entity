// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

namespace Fortwars;
partial class FortwarsPlayer
{
	[ClientRpc]
	private void BecomeRagdollOnClient( Vector3 position, Rotation rotation, Vector3 velocity, Vector3 force )
	{
		var ent = new ModelEntity();
		ent.Position = position;
		ent.Rotation = rotation;
		ent.Scale = Scale;
		ent.PhysicsEnabled = true;
		ent.UsePhysicsCollision = true;
		ent.EnableAllCollisions = true;
		ent.Tags.Add( "debris" );
		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.CopyBodyGroups( this );
		ent.CopyMaterialGroup( this );
		ent.TakeDecalsFrom( this );
		ent.EnableHitboxes = true;
		ent.EnableAllCollisions = true;
		ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		ent.RenderColor = RenderColor;
		ent.PhysicsGroup.Velocity = velocity;

		if ( Game.LocalPawn == this )
		{
			//ent.EnableDrawing = false; wtf
		}

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			var model = e.GetModelName();

			var clothing = new ModelEntity();
			clothing.SetModel( model );
			clothing.SetParent( ent, true );
			clothing.RenderColor = e.RenderColor;
			clothing.CopyBodyGroups( e );
			clothing.CopyMaterialGroup( e );
		}

		ent.PhysicsGroup.ApplyImpulse( force );
		Corpse = ent;

		ent.DeleteAsync( 10.0f );
	}
}
