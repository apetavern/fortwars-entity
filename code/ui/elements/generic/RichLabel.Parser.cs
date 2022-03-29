// Copyright (c) 2022 Ape Tavern, do not share, re-distribute or modify
// without permission of its author (insert_email_here)

using System.Collections.Generic;

namespace Fortwars;

public class RichLabelParser
{
	public struct Token
	{
		public string Text { get; set; }
		public string Class { get; set; }
	}

	private string text;
	public RichLabelParser( string text )
	{
		this.text = text;
	}

	//
	// Couldn't be fucked to make anything better
	// so this just uses the shitty old Quake syntax
	// for stuff.
	// Just use a caret and then a symbol:
	// - ^b: Bold
	// - ^1: Title (big text)
	// - ^2: Subtitle (med text)
	// - ^h: Highlight (make it pink)
	// - ^r: Reset & push to list
	//
	public List<Token> Run()
	{
		var tokenList = new List<Token>();

		Token currentToken = new();

		for ( int i = 0; i < text.Length; i++ )
		{
			char character = text[i];
			if ( character == '^' && i < text.Length - 1 )
			{
				// Read ahead
				char readAhead = text[++i];
				switch ( readAhead )
				{
					case 'b':
						currentToken.Class += " bold";
						break;
					case '1':
						currentToken.Class += " title";
						break;
					case '2':
						currentToken.Class += " subtitle";
						break;
					case 'h':
						currentToken.Class += " highlight";
						break;
					case 'r':
						tokenList.Add( currentToken );
						currentToken = new();
						break;
					default:
						currentToken.Text += $"{character}{readAhead}";
						break;
				}
			}
			else
			{
				currentToken.Text += character;
			}
		}

		tokenList.Add( currentToken );
		return tokenList;
	}
}
