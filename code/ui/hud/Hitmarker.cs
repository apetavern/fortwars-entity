// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using System.Threading.Tasks;

namespace Fortwars;

public partial class Hitmarker : Panel
{
	public static Hitmarker Instance { get; private set; }
	private static HitmarkerInstance currentHitmarkerInstance;

	public Hitmarker()
	{
		Instance = this;
		StyleSheet.Load( "/ui/hud/Hitmarker.scss" );
	}

	public void OnHit( float amount, bool isKill, bool isBlock = false )
	{
		currentHitmarkerInstance?.Delete();
		currentHitmarkerInstance = new HitmarkerInstance( amount, this );
		currentHitmarkerInstance.SetClass( "kill", isKill );
		currentHitmarkerInstance.SetClass( "block", isBlock );
	}

	public class HitmarkerInstance : Panel
	{
		public HitmarkerInstance( float amount, Panel parent )
		{
			Parent = parent;
			_ = KillAfterTime();
		}

		async Task KillAfterTime()
		{
			await Task.Delay( 75 );
			Delete();
		}
	}
}
