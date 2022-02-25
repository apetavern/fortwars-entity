using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	public class ScopeRenderTarget : Panel
	{
		Texture colorTexture;
		Texture depthTexture;
		RenderAttributes renderAttributes;

		private Vector2 textureSize;
		private float fieldOfView = 25f;
		const float baseFov = 50f;

		public ScopeRenderTarget()
		{
			textureSize = Screen.Size / 2f;
			renderAttributes.Set( "textureSize", textureSize );

			colorTexture = Texture.CreateRenderTarget()
						 .WithSize( (int)textureSize.x, (int)textureSize.y )
						 .WithScreenFormat()
						 .WithScreenMultiSample()
						 .Create();

			depthTexture = Texture.CreateRenderTarget()
						 .WithSize( (int)textureSize.x, (int)textureSize.y )
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

			var sceneWorld = Map.Scene;

			if ( sceneWorld == null )
				return;

			if ( (player as FortwarsPlayer).ActiveChild is not FortwarsWeapon weapon )
				return;

			var sceneObject = weapon.ViewModelEntity.SceneObject;

			if ( sceneObject == null )
				return;

			if ( weapon.IsAiming )
				fieldOfView = fieldOfView.LerpTo( weapon.WeaponAsset.AimFovMult * baseFov, 10 * Time.Delta );
			else
				fieldOfView = fieldOfView.LerpTo( baseFov, 10 * Time.Delta );

			Render.Draw.DrawScene( colorTexture,
					depthTexture,
					sceneWorld,
					renderAttributes,
					Box.Rect,
					CurrentView.Position,
					CurrentView.Rotation.Angles().ToRotation(),
					fov: fieldOfView,
					zNear: 1,
					zFar: 25000 );

			Render.Attributes.Set( "ScopeRT", colorTexture );
			sceneObject.Attributes.Set( "ScopeRT", colorTexture );
		}
	}
}
