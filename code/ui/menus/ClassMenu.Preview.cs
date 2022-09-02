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
		private Rotation CameraRot => Rotation.From( 0, 210, 0 );
		private Vector3 CameraPos => new Vector3( 50, 30, 52 );

		public ClothingContainer Container = new();
		private List<SceneModel> clothes = new List<SceneModel>();
		private SceneModel citizen;
		private SceneModel weaponModel;
		private ScenePanel renderScene;

		public ClassPreviewPanel( string className )
		{
			Style.FlexWrap = Wrap.Wrap;
			Style.JustifyContent = Justify.Center;
			Style.AlignItems = Align.Center;
			Style.AlignContent = Align.Center;
			Style.Padding = 0;

			var world = new SceneWorld();
			citizen = new SceneModel( world, "models/playermodel/playermodel.vmdl", Transform.Zero );
			citizen.Position += Vector3.Up * 5f;

			List<SceneLight> sceneLights = new();
			sceneLights.Add( new SceneLight( world, Vector3.Up * 150.0f, 200.0f, Color.White * 5.0f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 75.0f + Vector3.Forward * 100.0f, 200, Color.White * 15.0f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 75.0f + Vector3.Backward * 100.0f, 200, Color.White * 15f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 75.0f + Vector3.Right * 100.0f, 200, Color.White * 15.0f ) );
			sceneLights.Add( new SceneLight( world, Vector3.Up * 100.0f + Vector3.Up, 200, Color.White * 15.0f ) );

			renderScene = Add.ScenePanel( world, CameraPos, CameraRot, 50 );
			renderScene.AmbientColor = new Color( .25f, .15f, .15f ) * 2.0f;

			Log.Trace( className );
			ShowClass( className );
		}

		Vector3 lookPos, headPos, aimPos;

		public void Animate()
		{
			// Get mouse position
			var mousePosition = Mouse.Position;

			// subtract what we think is about the player's eye position
			mousePosition.x -= Box.Rect.Width * 1.15f;
			mousePosition.y -= Box.Rect.Height * 0.35f;
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
			citizen.SetAnimParameter( "holdtype", (int)weaponHoldtype );
			citizen.SetAnimParameter( "holdtype_handedness", (int)weaponHoldHandedness );
			citizen.SetAnimParameter( "holdtype_pose_hand", handpose );

			/*citizen.SetAnimParameter( "useleftik", weaponModel.GetAttachment( "lhand_ik" ).HasValue ); //Uncomment this shit when https://github.com/Facepunch/sbox-issues/issues/2256 that issue has be resolved

			if ( weaponModel.GetAttachment( "lhand_ik" ).HasValue )
			{
				Transform attachment = weaponModel.GetAttachment( "lhand_ik" ).Value;
				citizen.SetAnimParameter( "left_hand_ik.position", attachment.Position );
				citizen.SetAnimParameter( "left_hand_ik.rotation", attachment.Rotation );
			}*/
		}

		HoldTypes weaponHoldtype;
		HoldHandedness weaponHoldHandedness;
		float handpose;

		public void ShowClass( Class classType )
		{
			foreach ( var model in clothes )
				model?.Delete();

			if ( weaponModel != null )
				weaponModel.Delete();

			Container.Deserialize( ConsoleSystem.GetValue( "avatar" ) );
			citizen.SetMaterialGroup( "Skin01" );
			
			foreach ( var item in classType.Cosmetics )
			{
				Clothing cloth = ResourceLibrary.Get<Clothing>( item );
				Container.Clothing.RemoveAll( x => !x.CanBeWornWith( cloth ) );
				Container.Clothing.Add( cloth );
			}

			clothes = Container.DressSceneObject( citizen );

			Transform bone = citizen.GetBoneWorldTransform( "hold_R" );
			if ( classType.PreviewWeapon.Contains( "medkit" ) )
				bone = citizen.GetBoneWorldTransform( "hold_L" );

			weaponModel = new SceneModel( citizen.World, classType.PreviewWeapon, bone );
			weaponHoldtype = classType.PreviewHoldType;
			weaponHoldHandedness = classType.PreviewHoldHandedness;
			handpose = classType.PreviewHandpose;

			citizen.AddChild( "weapon", weaponModel );
		}

		public void ShowClass( string className )
		{
			var classType = TypeLibrary.Create<Class>( className );
			ShowClass( classType );
		}

		void DressModel()
		{
			Container.Deserialize( ConsoleSystem.GetValue( "avatar" ) );

			citizen.SetMaterialGroup( "Skin01" );

			clothes = Container.DressSceneObject( citizen );
		}

		public override void Tick()
		{
			base.Tick();

			renderScene.CameraPosition = CameraPos;
			renderScene.CameraRotation = CameraRot;

			Animate();

			citizen.Update( RealTime.Delta );

			weaponModel?.Update( RealTime.Delta );

			foreach ( var item in clothes )
			{
				item.Update( RealTime.Delta );
			}

			if ( clothes.Count == 0 )
			{
				DressModel();
			}
		}
	}
}
