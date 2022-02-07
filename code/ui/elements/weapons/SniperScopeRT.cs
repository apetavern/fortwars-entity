using Sandbox;
using Sandbox.UI;

namespace Fortwars
{
	public class SniperScopeRT : Panel
	{
		Texture ColorTexture;
		Texture DepthTexture;

		public SniperScopeRT()
		{
			Vector2 textureSize = Screen.Size / 2f;

			ColorTexture = Texture.CreateRenderTarget().WithSize( (int)textureSize.x, (int)textureSize.y ).WithScreenFormat()
						   .WithScreenMultiSample()
						   .Create();

			DepthTexture = Texture.CreateRenderTarget().WithSize( (int)textureSize.x, (int)textureSize.y ).WithDepthFormat()
						   .WithScreenMultiSample()
						   .Create();
		}

		public override void OnDeleted()
		{
			Log.Trace( $"!!! DISPOSING OF TEXTURES !!!" );
			//
			// IF WE DON'T DISPOSE OF THESE PROPERLY THINGS *REALLY* FUCK UP
			// DON'T MESS WITH THIS
			//
			ColorTexture.Dispose();
			DepthTexture.Dispose();

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

			Render.DrawScene( ColorTexture,
					DepthTexture,
					new Vector2( 500, 500 ),
					SceneWorld.Current,
					CurrentView.Position,
					CurrentView.Rotation.Angles(),
					weapon.WeaponAsset.AimFovMult * 25f,
					zNear: 1,
					zFar: 25000 );

			Render.Set( "ScopeRT", ColorTexture );

			sceneObject.SetValue( "ScopeRT", ColorTexture );
		}
	}
}
