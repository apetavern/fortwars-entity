using Sandbox;
using System;

namespace Fortwars
{
	public partial class BobbingComponent : EntityComponent
	{
		[Net] public Vector3 CenterOffset { get; set; }

		[Net] float RandomOffset { get; set; }

		public BobbingComponent()
		{
			RandomOffset = Rand.Float( 0, 360 );
		}

		[Event.PreRender]
		public virtual void OnPreRender()
		{
			if ( Entity is not ModelEntity { SceneObject: SceneObject sceneObject } )
				return;

			if ( sceneObject == null )
				return;

			sceneObject.Rotation = Rotation.From( 45, (Time.Now * 90f) + RandomOffset, 0 );

			// actual origin is off-center, let's just center that
			Vector3 centerOffset = CenterOffset * sceneObject.Rotation;

			// bob up and down
			Vector3 bobbingOffset = Vector3.Up * MathF.Sin( Time.Now * 2f );
			sceneObject.Position = Entity.Position + (centerOffset + bobbingOffset) * Entity.Scale;
		}
	}
}
