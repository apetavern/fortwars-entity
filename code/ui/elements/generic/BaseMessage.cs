// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Fortwars;

public class BaseMessage : Panel
{
	Panel progress;

	TimeUntil remainingLifetime;

	public BaseMessage( string icon, string message )
	{
		StyleSheet.Load( "/ui/elements/generic/BaseMessage.scss" );

		progress = Add.Panel( "inner" );

		Add.Icon( icon, "icon" );
		Add.Label( message, "message" );

		remainingLifetime = 5;
	}

	public override void Tick()
	{
		base.Tick();
		progress.Style.Width = Length.Fraction( remainingLifetime / 5.0f );

		if ( remainingLifetime <= 0 )
			Delete();
	}
}

public static class BaseMessageExtensions
{
	public static BaseMessage Message( this PanelCreator self, string icon, string message )
	{
		var control = new BaseMessage( icon, message );
		control.Parent = self.panel;

		return control;
	}
}
