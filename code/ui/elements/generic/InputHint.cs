// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Fortwars;

[UseTemplate]
public partial class InputHint : Panel
{
	// @ref
	public Image Glyph { get; set; }
	public InputButton Button { get; set; }
	public InputButton gamepadButton { get; set; }
	public string Text { get; set; }
	public Label ActionLabel { get; set; }

	public InputHint()
	{
		StyleSheet.Load( "/ui/elements/generic/InputHint.scss" );

		BindClass( "noaction", () => string.IsNullOrEmpty( Text ) );
	}

	public override void SetProperty( string name, string value )
	{
		base.SetProperty( name, value );

		if ( name == "btn" )
		{
			SetButton( Enum.Parse<InputButton>( value, true ) );
		}
	}

	public void SetButton( InputButton button )
	{
		Button = button;
	}

	public override void SetContent( string value )
	{
		base.SetContent( value );

		ActionLabel.SetText( value );
		Text = value;
	}

	public override void Tick()
	{
		base.Tick();

		ActionLabel.SetText( Text );

		Texture glyphTexture = null;

		if ( !Input.UsingController )
		{
			glyphTexture = Input.GetGlyph( Button, InputGlyphSize.Small, GlyphStyle.Knockout.WithSolidABXY().WithNeutralColorABXY() );
		}
		else
		{
			glyphTexture = Input.GetGlyph( gamepadButton, InputGlyphSize.Small, GlyphStyle.Knockout.WithSolidABXY().WithNeutralColorABXY() );
		}

		if ( glyphTexture != null )
		{
			Glyph.Texture = glyphTexture;
			Glyph.Style.Width = glyphTexture.Width;
			Glyph.Style.Height = glyphTexture.Height;
		}
		else
		{
			Glyph.Texture = Texture.Load( FileSystem.Mounted, "/tools/images/common/generic_hud_warning.jpg" );
		}
	}
}

public static class InputHintExtensions
{
	public static InputHint InputHint( this PanelCreator self, InputButton button, InputButton gamepadButton, string action = null, string classname = null )
	{
		var control = self.panel.AddChild<InputHint>();

		control.Button = button;

		control.gamepadButton = gamepadButton;

		if ( action != null )
			control.Text = action;

		if ( classname != null )
			control.AddClass( classname );

		return control;
	}
}
