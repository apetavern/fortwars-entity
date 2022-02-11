using Sandbox;
using System;

namespace Fortwars
{
	/// <summary>
	/// Bob the entity's scene object up and down, and rotate it, over time
	/// </summary>
	public partial class BobbingComponent : EntityComponent
	{
		[Net] public Vector3 CenterOffset { get; set; }

		[Net] public Vector3 PositionOffset { get; set; }

		[Net] float RandomOffset { get; set; }

		[Net] public bool NoPitch { get; set; }

		public BobbingComponent()
		{
			// random offsets help make things look less uniform, adds variety
			// this is mostly for ents that spawn on map load
			RandomOffset = Rand.Float( 0, 360 );
		}

		[Event.PreRender]
		public virtual void OnPreRender()
		{
			if ( Entity is not ModelEntity { SceneObject: SceneObject sceneObject } )
				return;

			if ( !sceneObject.IsValid() )
				return;

			sceneObject.Rotation = Rotation.From( NoPitch ? 0 : 45, (Time.Now * 90f) + RandomOffset, 0 );

			// actual origin is off-center, let's just center that
			Vector3 centerOffset = CenterOffset * sceneObject.Rotation;

			// bob up and down
			Vector3 bobbingOffset = Vector3.Up * MathF.Sin( Time.Now * 2f );
			sceneObject.Position = Entity.Position + (centerOffset + bobbingOffset + PositionOffset) * Entity.Scale;
		}
	}
}
