using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	public class ScopeRenderTarget : Panel
	{
		Texture colorTexture;
		Texture depthTexture;

		private Rect viewport;
		private float fieldOfView = 25f;
		const float baseFov = 50f;

		private RenderAttributes renderAttributes;

		public ScopeRenderTarget()
		{
			renderAttributes = new();
			viewport = new Rect( Vector2.Zero, Screen.Size / 2f );

			colorTexture = Texture.CreateRenderTarget()
						 .WithSize( (int)viewport.width, (int)viewport.height )
						 .WithScreenFormat()
						 .WithScreenMultiSample()
						 .Create();

			depthTexture = Texture.CreateRenderTarget()
						 .WithSize( (int)viewport.width, (int)viewport.height )
						 .WithDepthFormat()
						 .WithScreenMultiSample()
						 .Create();
		}

		public override void OnDeleted()
		{
			colorTexture.Dispose();
			depthTexture.Dispose();

			base.OnDeleted();
		}

		public override void DrawBackground( ref RenderState state )
		{
			var player = Local.Pawn;
			if ( player == null )
				return;

			if ( (player as FortwarsPlayer).ActiveChild is not FortwarsWeapon weapon )
				return;

			var sceneObject = weapon.ViewModelEntity.SceneObject;
			sceneObject.Flags.ViewModelLayer = true;

			if ( sceneObject == null )
				return;

			if ( !weapon.IsAiming )
				return;

			Render.Draw.DrawScene( colorTexture,
					depthTexture,
					Map.Scene,
					renderAttributes,
					viewport,
					CurrentView.Position,
					CurrentView.Rotation,
					fieldOfView,
					zNear: 32,
					zFar: 25000 );

			Render.Attributes.Set( "ScopeRT", colorTexture );
			sceneObject.Attributes.Set( "ScopeRT", colorTexture );
		}
	}
}
