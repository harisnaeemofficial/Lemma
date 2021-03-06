﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lemma.Console
{
	public static class ConsoleParser
	{
		public class ParseResult
		{
			public class ParseToken
			{
				public enum TokenType
				{
					CmdOrVar,
					Argument,
					ConVar
				}

				public TokenType Type;
				public string Value;
			}

			public string EntireString;
			public ParseToken[] ParsedResult;
		}

		private enum ParseState
		{
			FindingToken,
			ParsingToken
		}

		//Rather simple parser for right now--first token is going to be the name, any subsequent tokens will be arguments.
		public static ParseResult Parse(string input)
		{
			ParseResult ret = new ParseResult();
			ret.EntireString = input;

			List<ParseResult.ParseToken> tokens = new List<ParseResult.ParseToken>();

			string acceptableForNewToken = "\"'abcdefghijklmnopqrstuvwxyz1234567890_-%";
			string acceptableForCurToken = "\"'abcdefghijklmnopqrstuvwxyz1234567890_-!@#$%^&*()<>?,./|\\][}{ ";

			ParseResult.ParseToken curToken = new ParseResult.ParseToken();
			bool parsingInQuotes = false;
			string tokenStr = "";
			ParseState curState = ParseState.FindingToken;

			bool alreadyFoundName = false;

			foreach (char c in input)
			{
				bool append = true;
				switch (curState)
				{
					case ParseState.FindingToken:
						if (acceptableForNewToken.Contains(c.ToString().ToLower()))
						{
							if (c == '"')
							{
								parsingInQuotes = true;
								append = false;
							}
							curState = ParseState.ParsingToken;
						}
						break;

						case ParseState.ParsingToken:
						if (acceptableForCurToken.Contains(c.ToString().ToLower()))
						{
							if (c == '"' && parsingInQuotes)
							{
								parsingInQuotes = false;
								append = false;
								curState = ParseState.FindingToken;
							}
							else if (c == ' ' && !parsingInQuotes)
							{
								parsingInQuotes = false;
								append = false;
								curState = ParseState.FindingToken;
							}

							if (curState == ParseState.FindingToken)
							{
								curToken.Type = alreadyFoundName
									? ParseResult.ParseToken.TokenType.Argument
									: ParseResult.ParseToken.TokenType.CmdOrVar;
								alreadyFoundName = true;
								curToken.Value = tokenStr;

								tokenStr = "";
								tokens.Add(curToken);
								curToken = new ParseResult.ParseToken();
							}
							
						}
						break;
				}
				if (append && curState == ParseState.ParsingToken)
					tokenStr += c;
			}

			if (!string.IsNullOrEmpty(tokenStr))
			{
				curToken.Type = alreadyFoundName
									? ParseResult.ParseToken.TokenType.Argument
									: ParseResult.ParseToken.TokenType.CmdOrVar;
				curToken.Value = tokenStr;

				tokens.Add(curToken);
			}

			
			for(int i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];
				if (token.Type == ParseResult.ParseToken.TokenType.Argument && token.Value.Length >= 3)
				{
					if (token.Value[0] == '%' && token.Value[token.Value.Length - 1] == '%')
					{
						var conVar = token.Value.Substring(1, token.Value.Length - 2);
						if (Console.IsConVar(conVar))
						{
							token.Value = Console.GetConVarValue(conVar, token.Value);
						}
					}
				}
			}

			ret.ParsedResult = tokens.ToArray();

			return ret;
		}
	}
}
