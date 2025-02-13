/*
 * Copyright (c). 2018 - 2025 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	Tools																																		*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Tools and utilities for use inside of TargetScript.
	/// </summary>
	public class Tools
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	AppendRepeat																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append the provided string to the caller's builder, repeating the
		/// pattern the specified number of times.
		/// </summary>
		/// <param name="target">
		/// Reference to the object to which the pattern will be appended.
		/// </param>
		/// <param name="pattern">
		/// Pattern to append.
		/// </param>
		/// <param name="count">
		/// Count of instances to append.
		/// </param>
		public static void AppendRepeat(StringBuilder target,
			string pattern, int count)
		{
			int index = 0;
			if(target != null && pattern != null && pattern.Length > 0 &&
				count > 0)
			{
				for(index = 0; index < count; index ++)
				{
					target.Append(pattern);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DelimitedToArray																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return an array of trimmed values from the caller's delimited string.
		/// </summary>
		/// <param name="value">
		/// Delimited string value.
		/// </param>
		/// <param name="delimiter">
		/// Delimiter character to use. Default is comma.
		/// </param>
		/// <returns>
		/// Array of trimmed values, if string was presented. Otherwise, a zero
		/// length array.
		/// </returns>
		public static string[] DelimitedToArray(string value, char delimiter = ',')
		{
			int count = 0;
			int index = 0;
			char[] pattern = new char[] { delimiter };
			string[] result = new string[0];

			if(value?.Length > 0)
			{
				result = value.Split(pattern);
				count = result.Length;
				for(; index < count; index ++)
				{
					result[index] = result[index].Trim();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	EscapeCharacters																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Escape any characters that need to be escaped in a regular expression
		/// pattern.
		/// </summary>
		/// <param name="value">
		/// String of characters that may need to be escaped for inclusion in
		/// regular expressions.
		/// </param>
		/// <returns>
		/// String whose characters are compatible for literal inclusion within
		/// regular expression strings.
		/// </returns>
		public static string EscapeCharacters(string value)
		{
			string result = "";
			if(value?.Length > 0)
			{
				result = Regex.Replace(value,
					ResourceMain.ToolsEscapePattern,
					ResourceMain.ToolsEscapeReplacement);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetBraceItemPattern																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a regular expression item list pattern that allows the user to
		/// specify which brace types are allowed.
		/// </summary>
		/// <param name="keyName">
		/// Name of the group key for which matches will be retrieved.
		/// </param>
		/// <param name="brace">
		/// Composite brace specification of one or more types.
		/// </param>
		/// <param name="prefix">
		/// Optional regular expression pattern prefix.
		/// </param>
		/// <param name="suffix">
		/// Optional regular expression pattern suffix.
		/// </param>
		/// <returns>
		/// Regular expression pattern used for enumerating through
		/// braced values.
		/// </returns>
		public static string GetBraceItemPattern(string keyName,
			BraceEnumType brace, string prefix = "", string suffix = "")
		{
			StringBuilder result = new StringBuilder();

			if(keyName?.Length > 0 && brace != BraceEnumType.None)
			{
				if((brace & BraceEnumType.Apostrophe) != 0)
				{
					if(result.Length > 0)
					{
						result.Append("|");
					}
					result.Append("('(?<");
					result.Append(keyName);
					result.Append(">.*)')");
				}
				if((brace & BraceEnumType.Curly) != 0)
				{
					if(result.Length > 0)
					{
						result.Append("|");
					}
					result.Append(@"(\{(?<");
					result.Append(keyName);
					result.Append(@">.*)\})");
				}
				if((brace & BraceEnumType.Parenthesis) != 0)
				{
					if(result.Length > 0)
					{
						result.Append("|");
					}
					result.Append(@"(\((?<");
					result.Append(keyName);
					result.Append(@">.*)\))");
				}
				if((brace & BraceEnumType.Quote) != 0)
				{
					if(result.Length > 0)
					{
						result.Append("|");
					}
					result.Append("(\\\"(?<");
					result.Append(keyName);
					result.Append(">.*)\\\")");
				}
				if((brace & BraceEnumType.Square) != 0)
				{
					if(result.Length > 0)
					{
						result.Append("|");
					}
					result.Append(@"(\[(?<");
					result.Append(keyName);
					result.Append(@">.*)\])");
				}
				if(result.Length > 0)
				{
					if(prefix?.Length > 0)
					{
						result.Insert(0, prefix);
					}
					result.Insert(0, @"^\s*");
					if(suffix?.Length > 0)
					{
						result.Append(suffix);
					}
					result.Append(@"\s*$");
				}
			}
			return result.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetBraceType																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the type of bracing currently surrounding the caller's value.
		/// </summary>
		/// <param name="value">
		/// The enclosed variable.
		/// </param>
		/// <returns>
		/// Type of brace, if any, surrounding the caller's string.
		/// </returns>
		public static BraceEnumType GetBraceType(string value)
		{
			BraceEnumType result = BraceEnumType.None;
			if(value?.Length > 1)
			{
				if(value.StartsWith("'") && value.EndsWith("'"))
				{
					result = BraceEnumType.Apostrophe;
				}
				else if(value.StartsWith("{") && value.EndsWith("}"))
				{
					result = BraceEnumType.Curly;
				}
				else if(value.StartsWith("(") && value.EndsWith(")"))
				{
					result = BraceEnumType.Parenthesis;
				}
				else if(value.StartsWith("\"") && value.EndsWith("\""))
				{
					result = BraceEnumType.Quote;
				}
				else if(value.StartsWith("[") && value.EndsWith("]"))
				{
					result = BraceEnumType.Square;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetParameterNameValueList																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return an array of string arrays representing the name:value pairs
		/// found in the caller's parameter specification.
		/// </summary>
		/// <param name="paramList">
		/// CSS-style list in the pattern of name:value; ...; name:value.
		/// </param>
		/// <returns>
		/// Array of string arrays representing all of the name:value pairs found.
		/// </returns>
		public static string[][] GetParameterNameValueList(string paramList)
		{
			int iCount = 0;
			int iIndex = 0;
			char[] pattern = new char[] { ':' };
			int pCount = 0;
			int pIndex = 0;
			string[] pItem = new string[0];
			string[] pItems = new string[0];
			string[] pReplace = new string[0];
			string[][] result = new string[0][];

			if(paramList?.Length > 0)
			{
				pItems = paramList.Split(new char[] { ';' });
				result = new string[pItems.Length][];
				pCount = pItems.Length;
				for(pIndex = 0; pIndex < pCount; pIndex ++)
				{
					pItem = pItems[pIndex].Trim().Split(pattern);
					if(pItem.Length > 1)
					{
						iCount = pItem.Length;
						for(iIndex = 0; iIndex < iCount; iIndex ++)
						{
							pItem[iIndex] = pItem[iIndex].Trim();
						}
						result[pIndex] = pItem;
					}
					else if(pItem.Length > 0)
					{
						pReplace = new string[2];
						pReplace[0] = "param";
						pReplace[1] = pItem[0].Trim();
						result[pIndex] = pReplace;
					}
					else
					{
						pReplace = new string[] { "", "" };
						result[pIndex] = pReplace;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the captured group value of a given name from the supplied
		/// Regular Expression Match.
		/// </summary>
		/// <param name="match">
		/// The regular expression match containing the capture group to retrieve.
		/// </param>
		/// <param name="name">
		/// Name of the group to retrieve.
		/// </param>
		/// <returns>
		/// Value found in the caller's match, if present. Otherwise, an empty
		/// string.
		/// </returns>
		public static string GetValue(Match match, string name)
		{
			string result = "";
			string value = "";
			if(match != null && match.Groups[name] != null)
			{
				value = match.Groups[name].Value;
				if(value != null)
				{
					result = value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	HasBraces																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the caller's phrase includes
		/// one or more of the specified brace types.
		/// </summary>
		/// <param name="value">
		/// Value to inspect.
		/// </param>
		/// <param name="brace">
		/// Brace types to check.
		/// </param>
		/// <returns>
		/// A value indicating whether the phrase includes one or more of the
		/// specified brace types.
		/// </returns>
		public static bool HasBraces(string value, BraceEnumType brace)
		{
			bool result = false;
			if(value?.Length > 1 && brace != BraceEnumType.None)
			{
				if((brace & BraceEnumType.Apostrophe) != 0)
				{
					result |= (value.IndexOf("'") > -1);
				}
				if(!result && (brace & BraceEnumType.Curly) != 0)
				{
					result |= (value.IndexOf("{") > -1 && value.IndexOf("}") > -1);
				}
				if(!result && (brace & BraceEnumType.Parenthesis) != 0)
				{
					result |= (value.IndexOf("(") > -1 && value.IndexOf(")") > -1);
				}
				if(!result && (brace & BraceEnumType.Quote) != 0)
				{
					result |= (value.IndexOf("\"") > -1);
				}
				if(!result && (brace & BraceEnumType.Square) != 0)
				{
					result |= (value.IndexOf("[") > -1 && value.IndexOf("]") > -1);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsBraced																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the caller's phrase in enclosed
		/// within one of the specified brace types.
		/// </summary>
		/// <param name="value">
		/// Value to inspect.
		/// </param>
		/// <param name="brace">
		/// Brace types to check.
		/// </param>
		/// <returns>
		/// A value indicating whether the caller's phrase is fully enclosed
		/// within one of the specified brace types.
		/// </returns>
		public static bool IsBraced(string value, BraceEnumType brace)
		{
			bool result = false;
			if(value?.Length > 1 && brace != BraceEnumType.None)
			{
				if((brace & BraceEnumType.Apostrophe) != 0)
				{
					result = (value.StartsWith("'") && value.EndsWith("'"));
				}
				if(!result && (brace & BraceEnumType.Curly) != 0)
				{
					result = (value.StartsWith("{") && value.EndsWith("}"));
				}
				if(!result && (brace & BraceEnumType.Parenthesis) != 0)
				{
					result = (value.StartsWith("(") && value.EndsWith(")"));
				}
				if(!result && (brace & BraceEnumType.Quote) != 0)
				{
					result = (value.StartsWith("\"") && value.EndsWith("\""));
				}
				if(!result && (brace & BraceEnumType.Square) != 0)
				{
					result = (value.StartsWith("[") && value.EndsWith("]"));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsRelative																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified filename is relative.
		/// </summary>
		/// <param name="filename">
		/// Name of the file to inspect.
		/// </param>
		/// <returns>
		/// A value indicating whether the filename is relative (true) or
		/// absolute (false).
		/// </returns>
		public static bool IsRelative(string filename)
		{
			bool result = false;
			if(filename?.Length > 0)
			{
				result = (filename.IndexOf(":") < 0 && filename.IndexOf(@"\\") < 0);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReduceDensity																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Reduce the number of spaces and repeated characters found in the
		/// caller's value.
		/// </summary>
		/// <param name="value">
		/// The phrase to reduce.
		/// </param>
		/// <returns>
		/// An abbreviated, shortened version of the caller's string.
		/// </returns>
		public static string ReduceDensity(string value)
		{
			string result = "";

			if(value?.Length > 0)
			{
				result = value;
				result = Regex.Replace(result, @"\t", " ");
				result = Regex.Replace(result, @"\s{2,}", " ");
				result = Regex.Replace(result, @"(?<f>.)\1{2,}", "${f}${f}");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReduceHighLevelCommands																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Reduce higher level commands in the expression to make it primitive.
		/// </summary>
		/// <param name="expression">
		/// The expression to parse.
		/// </param>
		/// <returns>
		/// Caller's expression with any higher level commands reduced to primitive
		/// elements.
		/// </returns>
		public static string ReduceHighLevelCommands(string expression)
		{
			int cBegin = 0;						//	Current begin.
			int cEnd = 0;							//	Current end.
			string cExpression = "";	//	Current expression.
			MatchCollection cMatches = null;    //	Command matches.
			int pEnd = 0;							//	Previous end.
			MatchCollection pMatches = null;    //	Parameter matches.
			string pValue = "";				//	Parameters value.
			string pValues = "";			//	Parameter values.
			StringBuilder result = new StringBuilder();
			StringBuilder statement = new StringBuilder();
			string subject = "";      //	Subject value.
			int tEnd = 0;							//	Text end.

			if(expression?.Length > 0)
			{
				cExpression = expression;
				tEnd = cExpression.Length - 1;
				//	Command: IN(x)
				//	a IN(b, c, d) -> (a = b) OR (a = c) OR (a = d)
				cMatches = Regex.Matches(cExpression,
					ResourceMain.ToolsReduceCommandInPattern);
				if(cMatches.Count > 0)
				{
					if(result.Length > 0)
					{
						result.Remove(0, result.Length);
					}
					pEnd = 0;
					foreach(Match cMatch in cMatches)
					{
						//	Each IN command.
						cBegin = cMatch.Index;
						cEnd = cBegin + cMatch.Length - 1;
						if(cBegin > pEnd)
						{
							//	Append unaltered content to the result.
							result.Append(cExpression.Substring(pEnd, cBegin - pEnd));
						}
						subject = GetValue(cMatch, "subject");
						pValues = GetValue(cMatch, "params");
						if(statement.Length > 0)
						{
							statement.Remove(0, statement.Length);
						}
						pMatches = Regex.Matches(pValues,
							ResourceMain.ToolsReduceCommandParamPattern);
						foreach(Match pMatch in pMatches)
						{
							//	Each parameter.
							pValue = GetValue(pMatch, "param");
							if(statement.Length > 0)
							{
								statement.Append(" OR ");
							}
							statement.Append("(");
							statement.Append(subject);
							statement.Append(" = ");
							statement.Append(pValue);
							statement.Append(")");
						}
						result.Append(statement);
						pEnd = cEnd + 1;
					}
					if(pEnd < tEnd)
					{
						//	Append any unaltered trailing text.
						result.Append(cExpression.Substring(pEnd, tEnd - pEnd));
					}
					cExpression = result.ToString();
				}
			}
			return cExpression;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RemoveBraces																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the specified types of braces from the caller's expression.
		/// </summary>
		/// <param name="expression">
		/// Expression to inspect.
		/// </param>
		/// <param name="brace">
		/// Type of brace set to remove.
		/// </param>
		/// <returns>
		/// The caller's expression with the specified types of braces removed.
		/// </returns>
		public static string RemoveBraces(string expression,
			BraceEnumType brace)
		{
			string result = "";

			if(expression?.Length > 0 && brace != BraceEnumType.None)
			{
				result = expression;
				if((brace & BraceEnumType.Apostrophe) > 0)
				{
					result = result.Replace("'", "");
				}
				if((brace & BraceEnumType.Curly) > 0)
				{
					result = result.Replace("{", "").Replace("}", "");
				}
				if((brace & BraceEnumType.Parenthesis) > 0)
				{
					result = result.Replace("(", "").Replace(")", "");
				}
				if((brace & BraceEnumType.Quote) > 0)
				{
					result = result.Replace("\"", "");
				}
				if((brace & BraceEnumType.Square) > 0)
				{
					result = result.Replace("[", "").Replace("]", "");
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ReplaceFirst																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Replace the first occurrence of the specified pattern with the
		/// caller's replacement.
		/// </summary>
		/// <param name="text">
		/// Source value to inspect.
		/// </param>
		/// <param name="find">
		/// The pattern to find.
		/// </param>
		/// <param name="replace">
		/// The content to replace on the found text.
		/// </param>
		/// <returns>
		/// Caller's expression with the first match of the specified find text
		/// replaced with the replacement value.
		/// </returns>
		public static string ReplaceFirst(string text, string find,
			string replace)
		{
			int index = 0;
			string result = "";

			if(text?.Length > 0)
			{
				result = text;
				index = result.IndexOf(find);
				if(index > -1)
				{
					result = text.Substring(0, index) +
						replace +
						text.Substring(index + find.Length);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SortDelimited																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Sort the contents of the delimited string in the specified order.
		/// </summary>
		/// <param name="list">
		/// Delimited list to sort.
		/// </param>
		/// <param name="ascending">
		/// Value indicating whether to sort in ascending order (true), or
		/// descending order (false).
		/// </param>
		/// <returns>
		/// Caller's delimited string, where elements have been sorted in the
		/// specified direction.
		/// </returns>
		public static string SortDelimited(string list, bool ascending)
		{
			int count = 0;
			string iItem = "";
			int index = 0;
			StringCollection items = null;
			StringBuilder result = new StringBuilder();
			string[] values = new string[0];

			if(list?.Length > 0)
			{
				values = list.Split(new char[] { ',' });
				items = new StringCollection();
				foreach(string value in values)
				{
					items.Add(value.Trim());
				}
				items.Sort();
				if(ascending)
				{
					foreach(string item in items)
					{
						if(result.Length > 0)
						{
							result.Append(",");
						}
						result.Append(item);
					}
				}
				else
				{
					count = items.Count;
					for(index = count - 1; index > -1; index --)
					{
						iItem = items[index];
						if(result.Length > 0)
						{
							result.Append(",");
						}
						result.Append(iItem);
					}
				}
			}
			return result.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StripBraces																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the outer bracing on a name or value.
		/// </summary>
		/// <param name="value">
		/// The value to process.
		/// </param>
		/// <returns>
		/// The caller's value with all brace types stripped.
		/// </returns>
		public static string StripBraces(string value)
		{
			string result = "";

			if(value?.Length > 0)
			{
				result = Regex.Replace(value,
					ResourceMain.ToolsStripBracesPattern,
					ResourceMain.ToolsStripBracesReplacement);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*
}
