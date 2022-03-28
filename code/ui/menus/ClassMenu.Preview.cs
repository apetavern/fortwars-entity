// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace Fortwars;

partial class ClassMenu
{
	public class ClassPreviewPanel : Panel
	{
		private ScenePanel renderScene;

		private Rotation CameraRot => Rotation.From( 0, 210, 0 );
		private Vector3 CameraPos => new Vector3( 70, 40, 48 );
		SceneModel citizen;

		public ClassPreviewPanel()
		{
			Style.FlexWrap = Wrap.Wrap;
			Style.JustifyContent = Justify.Center;
			Style.AlignItems = Align.Center;
			Style.AlignContent = Align.Center;
			Style.Padding = 0;

			var world = new SceneWorld();
			citizen = new SceneModel( world, "models/citizen/citizen.vmdl", Transform.Zero );

			List<SceneLight> sceneLights = new();
			sceneLights.Add( new SceneLight( world, Vector3.Up * 150.0f, 200.0f, Color.White * 5.0f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 75.0f + Vector3.Forward * 100.0f, 200, Color.White * 15.0f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 75.0f + Vector3.Backward * 100.0f, 200, Color.White * 15f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 75.0f + Vector3.Right * 100.0f, 200, Color.White * 15.0f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 100.0f + Vector3.Up, 200, Color.White * 15.0f ) );

			renderScene = Add.ScenePanel( world, CameraPos, CameraRot, 75 );
			renderScene.Style.Width = Length.Percent( 100 );
			renderScene.Style.Height = Length.Percent( 100 );
			renderScene.AmbientColor = new Color( .25f, .15f, .15f ) * 2.0f;
		}

		Vector3 lookPos, headPos, aimPos;

		public void Animate()
		{
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

			citizen.SetAnimParameter( "b_grounded", true );
			citizen.SetAnimParameter( "aim_eyes", lookPos );
			citizen.SetAnimParameter( "aim_head", headPos );
			citizen.SetAnimParameter( "aim_body", aimPos );
			citizen.SetAnimParameter( "aim_body_weight", 1.0f );
		}

		public override void Tick()
		{
			base.Tick();

			renderScene.CameraPosition = CameraPos;
			renderScene.CameraRotation = CameraRot;

			Animate();

			citizen.Update( RealTime.Delta );
		}
	}
}
