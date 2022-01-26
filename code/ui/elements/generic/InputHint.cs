using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Fortwars.UI.Elements.Generic
{
	[UseTemplate]
	public partial class InputHint : Panel
	{
		// @ref
		public Image Glyph { get; set; }
		public InputButton Button { get; set; }
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

			Texture glyphTexture = Input.GetGlyph( Button, InputGlyphSize.Small, GlyphStyle.Knockout.WithSolidABXY().WithNeutralColorABXY() );
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
		public static InputHint InputHint( this PanelCreator self, InputButton button, string action = null, string classname = null )
		{
			var control = self.panel.AddChild<InputHint>();

			control.Button = button;

			if ( action != null )
				control.Text = action;

			if ( classname != null )
				control.AddClass( classname );

			return control;
		}
	}
}
