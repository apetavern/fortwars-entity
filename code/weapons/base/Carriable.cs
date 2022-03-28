// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author support@apetavern.com

using Fortwars;
using Sandbox;

public partial class Carriable : BaseCarriable
{
    public override void Spawn()
    {
        base.Spawn();

        CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
        SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it
    }

    public override void CreateViewModel()
    {
        Host.AssertClient();

        if ( string.IsNullOrEmpty( ViewModelPath ) )
            return;

        ViewModelEntity = new ViewModel();
        ViewModelEntity.Position = Position;
        ViewModelEntity.Owner = Owner;
        ViewModelEntity.EnableViewmodelRendering = true;
        ViewModelEntity.SetModel( ViewModelPath );
    }

    public override void CreateHudElements()
    {
        base.CreateHudElements();

        if ( Local.Hud == null ) return;

        CrosshairPanel = new Crosshair();
        CrosshairPanel.Parent = Local.Hud;
        CrosshairPanel.AddClass( ClassInfo.Name );
    }

    public override void DestroyHudElements()
    {
        base.DestroyHudElements();

        CrosshairPanel?.Delete();
    }
}
