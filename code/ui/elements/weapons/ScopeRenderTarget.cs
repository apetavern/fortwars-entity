using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	public class ScopeRenderTarget : Panel
	{
		Texture colorTexture;
		Texture depthTexture;

		private Vector2 textureSize;
		private float fieldOfView = 25f;
		const float baseFov = 50f;

		public ScopeRenderTarget()
		{
			textureSize = Screen.Size / 2f;

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

			if ( SceneWorld.Current == null )
				return;

			if ( player.ActiveChild is not FortwarsWeapon weapon )
				return;

			var sceneObject = weapon.ViewModelEntity.SceneObject;

			if ( sceneObject == null )
				return;

			if ( weapon.IsAiming )
				fieldOfView = fieldOfView.LerpTo( weapon.WeaponAsset.AimFovMult * baseFov, 10 * Time.Delta );
			else
				fieldOfView = fieldOfView.LerpTo( baseFov, 10 * Time.Delta );

			Render.DrawScene( colorTexture,
					depthTexture,
					textureSize,
					SceneWorld.Current,
					CurrentView.Position,
					CurrentView.Rotation.Angles(),
					fieldOfView,
					zNear: 1,
					zFar: 25000 );

			Render.Set( "ScopeRT", colorTexture );
			sceneObject.SetValue( "ScopeRT", colorTexture );
		}
	}
}
