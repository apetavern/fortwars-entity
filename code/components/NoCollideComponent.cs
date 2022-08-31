// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using System.Linq;
using System.Threading.Tasks;

namespace Fortwars;

/// <summary>
/// Don't collide with players when we're grabbing these
/// </summary>
public class NoCollideComponent : EntityComponent<FortwarsBlock>
{
	FortwarsBlock entity;

	protected override void OnActivate()
	{
		entity = Entity;

		if ( Host.IsServer )
		{
			entity.Tags.Add( "nocollide" );
		}
		else
		{
			entity.RenderColor = entity.RenderColor.WithAlpha( 0.5f );
		}
	}

	protected override void OnDeactivate()
	{
		_ = RemoveNoCollide();
	}

	private async Task RemoveNoCollide()
	{
		await Task.Delay( 500 );

		if ( Host.IsServer )
		{
			entity.Tags.Remove( "nocollide" );
		}
		else
		{
			entity.RenderColor = entity.RenderColor.WithAlpha( 1.0f );
		}
	}

	[Event.Tick.Server]
	public static void SystemUpdate()
	{
		foreach ( var entity in Sandbox.Entity.All.OfType<FortwarsBlock>() )
		{
			void Remove()
			{
				var existingNoCollide = entity.Components.Get<NoCollideComponent>();
				existingNoCollide?.Remove();
			}

			bool hasPhysgun = false;
			foreach ( var player in Sandbox.Entity.All.OfType<FortwarsPlayer>() )
			{
				if ( player.ActiveChild is PhysGun physgun )
				{
					if ( physgun.GrabbedEntity == entity )
						hasPhysgun = true;
				}
			}

			if ( !hasPhysgun )
			{
				Remove();
				continue;
			}

			entity.Components.GetOrCreate<NoCollideComponent>();
		}
	}
}
