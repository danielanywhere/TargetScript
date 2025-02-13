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
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	ConfigurationCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ConfigurationItem Items.
	/// </summary>
	public class ConfigurationCollection : List<ConfigurationItem>
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
		//*	EliminateNull																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Eliminate all null values, converting any voilators found to empty
		/// string.
		/// </summary>
		public void EliminateNull()
		{
			foreach(ConfigurationItem item in this)
			{
				foreach(ConfigurationEntryItem entry in item.Entries)
				{
					if(entry.Description == null)
					{
						entry.Description = "";
					}
					if(entry.Name == null)
					{
						entry.Name = "";
					}
					if(entry.Value == null)
					{
						entry.Value = "";
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Exists																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indiicating whether the specified name exists in
		/// configuration.
		/// </summary>
		/// <param name="name">
		/// Name to check for.
		/// </param>
		/// <returns>
		/// Value indicating whether the specified name was found in the
		/// collection.
		/// </returns>
		public bool Exists(string name)
		{
			string nLower = "";
			bool result = false;

			if(name?.Length > 0)
			{
				nLower = name.ToLower();
				foreach(ConfigurationItem item in this)
				{
					if(item.Entries.Exists(x => x.Name.ToLower() == nLower))
					{
						result = true;
						break;
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
		/// Return the value of the specified configuration item.
		/// </summary>
		/// <param name="name">
		/// Name of the configuration item to find.
		/// </param>
		/// <returns>
		/// Value of the first matching configuration item, if found.
		/// </returns>
		/// <remarks>
		/// Configuration names are not case-sensitive.
		/// </remarks>
		public string GetValue(string name)
		{
			string result = "";
			string vLower = "";

			if(name?.Length > 0)
			{
				vLower = name.ToLower();
				foreach(ConfigurationItem item in this)
				{
					foreach(ConfigurationEntryItem entry in item.Entries)
					{
						if(entry.Name.ToLower() == vLower)
						{
							result = entry.Value;
							break;
						}
					}
					if(result.Length > 0)
					{
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the corresponding value of the named configuration entry.
		/// </summary>
		/// <param name="name">
		/// Name to match.
		/// </param>
		/// <param name="value">
		/// Value to set on the matching name.
		/// </param>
		public void SetValue(string name, string value)
		{
			bool bFound = false;
			int cCount = 0;
			int cIndex = 0;
			ConfigurationItem cItem = null;
			ConfigurationEntryItem eItem = null;
			string eValue = "";
			string nLower = "";

			if(this.Count == 0)
			{
				//	If a collection didn't exist, create one first.
				cItem = new ConfigurationItem();
				this.Add(cItem);
			}
			if(name?.Length > 0)
			{
				nLower = name.ToLower();
				eValue = (value?.Length > 0 ? value : "");
				cCount = this.Count;
				for(cIndex = 0; cIndex < cCount; cIndex++)
				{
					cItem = this[cIndex];
					if(cItem.Entries.Exists(x => x.Name.ToLower() == nLower))
					{
						bFound = true;
						eItem = cItem.Entries.First(x => x.Name.ToLower() == nLower);
						eItem.Value = eValue;
						break;
					}
				}
				if(!bFound)
				{
					//	If the item didn't exist, place it in the closest collection.
					cItem = this[0];
					eItem = new ConfigurationEntryItem();
					eItem.Name = name;
					eItem.Value = value;
					cItem.Entries.Add(eItem);
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ConfigurationEntryCollection																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ConfigurationEntryItem Items.
	/// </summary>
	public class ConfigurationEntryCollection : List<ConfigurationEntryItem>
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
	//*	ConfigurationEntryItem																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Single entry within the configuration group.
	/// </summary>
	public class ConfigurationEntryItem
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
		//*	Description																														*
		//*-----------------------------------------------------------------------*
		private string mDescription = "";
		/// <summary>
		/// Get/Set the description of this entry.
		/// </summary>
		public string Description
		{
			get { return mDescription; }
			set { mDescription = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of this entry.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the value of this entry.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ConfigurationItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Single entry within the configuration values table.
	/// </summary>
	public class ConfigurationItem
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
		//*	Entries																																*
		//*-----------------------------------------------------------------------*
		private ConfigurationEntryCollection mEntries =
			new ConfigurationEntryCollection();
		/// <summary>
		/// Get a reference to the collection of entries on this configuration.
		/// </summary>
		public ConfigurationEntryCollection Entries
		{
			get { return mEntries; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of this entry.
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
		/// Name of the new configuration page.
		/// </param>
		/// <param name="lines">
		/// Collection of string source lines representing the raw information to
		/// be parsed.
		/// </param>
		/// <param name="projectPath">
		/// Current project path.
		/// </param>
		/// <returns>
		/// Newly created configuration item.
		/// </returns>
		public static ConfigurationItem Parse(string name, StringCollection lines,
			string projectPath)
		{
			int eCount = 0;
			int eIndex = 0;
			string element = "";
			ConfigurationEntryCollection elements = null;
			ConfigurationEntryItem entry = null;
			string filename = "";
			int iCount = 0;
			FileInfo ifile = null;
			int iIndex = 0;
			string insert = "";
			MatchCollection matches = null;
			ConfigurationItem result = new ConfigurationItem();
			string[] sArray = new string[0];

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
					//	Inserts were found. In this version, that means no other entries will
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
								JsonConvert.DeserializeObject<ConfigurationEntryCollection>(insert);
							iCount = elements.Count;
							for(iIndex = 0; iIndex < iCount; iIndex++)
							{
								result.Entries.Add(elements[iIndex]);
							}
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
					element = element.Trim();
					if(element.StartsWith("["))
					{
						//	Collection of entries.
						elements =
							JsonConvert.
							DeserializeObject<ConfigurationEntryCollection>(element);
						iCount = elements.Count;
						for(iIndex = 0; iIndex < iCount; iIndex++)
						{
							result.Entries.Add(elements[iIndex]);
						}
					}
					else if(element.StartsWith("{"))
					{
						//	One entry object.
						entry =
							JsonConvert.DeserializeObject<ConfigurationEntryItem>(element);
						result.Entries.Add(entry);
					}
					else if(element.IndexOf(":") > -1)
					{
						//	Name / Value.
						entry = new ConfigurationEntryItem();
						sArray = element.Split(new char[] { ':' });
						if(sArray.Length > 1)
						{
							entry.Name = sArray[0].Replace("\"", "").Trim();
							entry.Value = sArray[1].Replace("\"", "").Trim();
						}
						result.Entries.Add(entry);
					}
				}
			}

			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
