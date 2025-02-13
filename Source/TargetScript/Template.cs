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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	TemplateCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TemplateItem Items.
	/// </summary>
	public class TemplateCollection : List<TemplateItem>
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
	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TemplateItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a string-based line collection template.
	/// </summary>
	public class TemplateItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	IncludeLines																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process insert on the specified lines.
		/// </summary>
		/// <param name="template">
		/// Template being processed.
		/// </param>
		/// <param name="lines">
		/// Lines to inspect for Include statement.
		/// </param>
		/// <param name="projectPath">
		/// Current base project path.
		/// </param>
		private static void IncludeLines(TemplateItem template,
			StringCollection lines, string projectPath)
		{
			int eCount = 0;
			int eIndex = 0;
			string element = "";
			StringCollection elements = null;
			string filename = "";
			FileInfo ifile = null;
			string insert = "";
			MatchCollection matches = null;

			eCount = lines.Count;
			for(eIndex = 0; eIndex < eCount; eIndex++)
			{
				//	Get the current line.
				element = lines[eIndex];
				matches =
					Regex.Matches(element, @"(?i:\{Include\((?<f>[^\)]+)\)\})");
				if(matches.Count > 0)
				{
					//	Inserts were found. In this version, no other entries
					//	will be present on this line.
					foreach(Match match in matches)
					{
						filename = Tools.GetValue(match, "f");
						if(Tools.IsRelative(filename))
						{
							filename = Path.Combine(projectPath, filename);
						}
						ifile = new FileInfo(filename);
						if(ifile.Exists)
						{
							insert = File.ReadAllText(ifile.FullName);
							elements =
								JsonConvert.DeserializeObject<StringCollection>(insert);
							IncludeLines(template, elements, projectPath);
						}
						else
						{
							Console.WriteLine(
								string.Format(
								"Error could not read file: [{0}]...",
								ifile.FullName));
						}
					}
				}
				else
				{
					//	No inserts present on the line.
					template.Lines.Add(element);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Lines																																	*
		//*-----------------------------------------------------------------------*
		private StringCollection mLines = new StringCollection();
		/// <summary>
		/// Get a reference to the collection of lines on this item.
		/// </summary>
		public StringCollection Lines
		{
			get { return mLines; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of this item.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the source content and return a populated object containing
		/// the deserialized information.
		/// </summary>
		/// <param name="name">
		/// Name of the new template.
		/// </param>
		/// <param name="lines">
		/// Collection of string source lines representing the raw information to
		/// be parsed.
		/// </param>
		/// <param name="projectPath">
		/// Base project path.
		/// </param>
		/// <returns>
		/// Reference to a new template item.
		/// </returns>
		public static TemplateItem Parse(string name, StringCollection lines,
			string projectPath)
		{
			int eCount = 0;
			int eIndex = 0;
			string element = "";
			StringCollection elements = null;
			string filename = "";
			//int iCount = 0;
			FileInfo ifile = null;
			//int iIndex = 0;
			string insert = "";
			MatchCollection matches = null;
			TemplateItem result = new TemplateItem();

			result.mName = name;
			eCount = lines.Count;
			for(eIndex = 0; eIndex < eCount; eIndex++)
			{
				//	Get the current line.
				element = lines[eIndex];
				matches =
					Regex.Matches(element, @"(?i:\{Include\((?<f>[^\)]+)\)\})");
				if(matches.Count > 0)
				{
					//	Inserts were found. In this version, no other entries will
					//	be present on this line.
					foreach(Match match in matches)
					{
						filename = Tools.GetValue(match, "f");
						if(Tools.IsRelative(filename))
						{
							filename = Path.Combine(projectPath, filename);
						}
						ifile = new FileInfo(filename);
						if(ifile.Exists)
						{
							insert = File.ReadAllText(ifile.FullName);
							elements =
								JsonConvert.DeserializeObject<StringCollection>(insert);
							IncludeLines(result, elements, ifile.DirectoryName);
						}
						else
						{
							Console.WriteLine(
								string.Format(
								"Error could not read file: [{0}]...",
								ifile.FullName));
						}
					}
				}
				else
				{
					//	No inserts present on the line.
					result.Lines.Add(element);
				}
			}

			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
