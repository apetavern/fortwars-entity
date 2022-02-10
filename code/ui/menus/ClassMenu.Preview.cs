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

			public void Animate()
			{
				citizen.Update( Time.Delta );
				citizen.SetAnimBool( "grounded", true );
				citizen.SetAnimFloat( "incline", 0f );
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
