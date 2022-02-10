using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

namespace Fortwars
{
	partial class ClassMenu
	{
		public class ClassPreviewPanel : Panel
		{
			private ScenePanel renderScene;

			private Rotation CameraRot => Rotation.From( 0, 210, 0 );
			private Vector3 CameraPos => new Vector3( 70, 40, 48 );

			AnimSceneObject citizen;

			public ClassPreviewPanel()
			{
				Build();
			}

			public override void OnHotloaded()
			{
				base.OnHotloaded();

				Build();
			}

			public override void Tick()
			{
				base.Tick();
				Animate();
			}

			Vector3 lookPos, headPos, aimPos;

			public void Animate()
			{
				citizen.Update( Time.Delta );

				// Get mouse position
				var mousePosition = Mouse.Position;

				// subtract what we think is about the player's eye position
				mousePosition.x -= Box.Rect.width * 0.475f;
				mousePosition.y -= Box.Rect.height * 0.3f;
				mousePosition /= ScaleToScreen;

				// convert it to an imaginary world position
				var worldpos = new Vector3( 200, mousePosition.x, -mousePosition.y );

				// convert that to local space for the model
				lookPos = citizen.Transform.PointToLocal( worldpos );
				headPos = Vector3.Lerp( headPos, citizen.Transform.PointToLocal( worldpos ), Time.Delta * 20.0f );
				aimPos = Vector3.Lerp( aimPos, citizen.Transform.PointToLocal( worldpos ), Time.Delta * 5.0f );

				citizen.SetAnimBool( "b_grounded", true );
				citizen.SetAnimVector( "aim_eyes", lookPos );
				citizen.SetAnimVector( "aim_head", headPos );
				citizen.SetAnimVector( "aim_body", aimPos );
				citizen.SetAnimFloat( "aim_body_weight", 1.0f );
			}

			public void Build()
			{
				renderScene?.Delete( true );

				Log.Trace( "Building render scene" );

				using ( SceneWorld.SetCurrent( new SceneWorld() ) )
				{
					citizen = new AnimSceneObject( Model.Load( "models/citizen/citizen.vmdl" ), Transform.Zero );

					Light.Point( Vector3.Up * 150.0f, 200.0f, Color.White * 5.0f );
					Light.Point( Vector3.Up * 75.0f + Vector3.Forward * 100.0f, 200, Color.White * 15.0f );
					Light.Point( Vector3.Up * 75.0f + Vector3.Backward * 100.0f, 200, Color.White * 15f );
					Light.Point( Vector3.Up * 75.0f + Vector3.Right * 100.0f, 200, Color.White * 15.0f );
					Light.Point( Vector3.Up * 100.0f + Vector3.Up, 200, Color.White * 15.0f );

					renderScene = Add.ScenePanel( SceneWorld.Current, CameraPos, CameraRot, 75 );
					renderScene.Style.Width = Length.Percent( 100 );
					renderScene.Style.Height = Length.Percent( 100 );
					renderScene.AmbientColor = new Color( .25f, .15f, .15f ) * 2.0f;
				}
			}
		}
	}
}
