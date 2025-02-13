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
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	ComponentCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ComponentItem Items.
	/// </summary>
	public class ComponentCollection : List<ComponentItem>
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
		//*	_Indexer																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the item in the collection specified by name.
		/// </summary>
		/// <param name="name">
		/// Name of the component to retrieve.
		/// </param>
		public ComponentItem this[string name]
		{
			get
			{
				string nLower = "";
				ComponentItem result = null;
				if(name?.Length > 0)
				{
					nLower = name.ToLower();
					result = this.FirstOrDefault(x => x.Name.ToLower() == nLower);
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ComponentExists																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified component is loaded in
		/// the collection.
		/// </summary>
		/// <param name="name">
		/// Name of the component to search for.
		/// </param>
		/// <returns>
		/// A value indicating whether a component with the specified name was
		/// found.
		/// </returns>
		public bool ComponentExists(string name)
		{
			string nLower = "";
			bool result = false;

			if(name?.Length > 0)
			{
				nLower = name.ToLower();
				if(this.Exists(x => x.Name.ToLower() == nLower))
				{
					result = true;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetElement																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the specified element from the component collection.
		/// </summary>
		/// <param name="component">
		/// Reference to the component in which to find the element. If null, the
		/// first matching element in all components will be found.
		/// </param>
		/// <param name="elementName">
		/// Name of the element to retrieve.
		/// </param>
		/// <returns>
		/// Reference to the selected element, if found. Otherwise, null.
		/// </returns>
		public AttributeCollection GetElement(ComponentItem component,
			string elementName)
		{
			AttributeCollection result = null;

			if(elementName?.Length > 0)
			{
				if(component != null)
				{
					//	Specific component is selected.
					result = component.Attributes.
						GetCollection("[Name]=" + elementName);
				}
				else
				{
					//	Component is unknown. Select the first matching element.
					foreach(ComponentItem item in this)
					{
						result = item.Attributes.GetCollection("[Name]=" + elementName);
						if(result != null)
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ComponentElementFollower																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Tracking class for component / element pairs.
	/// </summary>
	public class ComponentElementFollower
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ComponentElementFollower Item.
		/// </summary>
		public ComponentElementFollower()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentElementFollower Item.
		/// </summary>
		/// <param name="component">
		/// A reference to the component being tracked.
		/// </param>
		/// <param name="element">
		/// A reference to the currently selected attribute collection for the
		/// component.
		/// </param>
		public ComponentElementFollower(ComponentItem component,
			AttributeCollection element)
		{
			mComponent = component;
			mElement = element;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Component																															*
		//*-----------------------------------------------------------------------*
		private ComponentItem mComponent = null;
		/// <summary>
		/// Get/Set a reference to the component to follow.
		/// </summary>
		public ComponentItem Component
		{
			get { return mComponent; }
			set { mComponent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Element																																*
		//*-----------------------------------------------------------------------*
		private AttributeCollection mElement = null;
		/// <summary>
		/// Get/Set a reference to the collection of attributes being followed in
		/// this instance.
		/// </summary>
		public AttributeCollection Element
		{
			get { return mElement; }
			set { mElement = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ComponentItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a specific component.
	/// </summary>
	public class ComponentItem
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
		//*	Attributes																														*
		//*-----------------------------------------------------------------------*
		private AttributeCatalog mAttributes = new AttributeCatalog();
		/// <summary>
		/// Get a reference to the collection of attributes available on this
		/// component.
		/// </summary>
		public AttributeCatalog Attributes
		{
			get { return mAttributes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ElementExists																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified element exists in the
		/// fields.
		/// </summary>
		/// <param name="name">
		/// Name of the element to search for.
		/// </param>
		/// <returns>
		/// A value indicating whether an attribute of the specified name was found
		/// in at least one of the metadata fields.
		/// </returns>
		public bool ElementExists(string name)
		{
			string nLower = "";
			bool result = false;

			if(name?.Length > 0)
			{
				nLower = name.ToLower();
				foreach(AttributeCollection collection in mAttributes)
				{
					//	The base object doesn't count as an element, but the definition of
					//	the object itself.
					if(!collection.Exists(x =>
						x.Name.ToLower() == "datatype" &&
						x.Value.ToLower() == "baseobject") &&
						collection.Exists(x =>
						x.Name.ToLower() == nLower))
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
		//*	GetElementCount																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the count of all elements in the attribute catalog.
		/// </summary>
		/// <returns>
		/// The count of all metadata elements found in the catalog.
		/// </returns>
		public int GetElementCount()
		{
			int result = 0;

			foreach(AttributeCollection collection in mAttributes)
			{
				if(collection.Exists(x => x.Name.ToLower() == "name"))
				{
					result += collection.Count(x =>
					(x.Name.ToLower() != "datatype" ||
					x.Value.ToLower() != "baseobject"));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetElementNames																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return all known element names from the collections.
		/// </summary>
		/// <returns>
		/// Delimited list of all metadata field names found in the catalog.
		/// </returns>
		public string GetElementNames()
		{
			StringBuilder result = new StringBuilder();
			foreach(AttributeCollection collection in mAttributes)
			{
				//	The base object doesn't count as an element, but the definition of
				//	the object itself.
				if(!collection.Exists(x =>
					x.Name.ToLower() == "datatype" &&
					x.Value.ToLower() == "baseobject") &&
					collection.Exists(x =>
					x.Name.ToLower() == "name"))
				{
					if(result.Length > 0)
					{
						result.Append(",");
					}
					result.Append(collection.GetValue("name"));
				}
			}
			return result.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetValueForExpression																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of an attribute from a collection in the catalog for
		/// a matching value in the expression on the collection.
		/// </summary>
		/// <param name="returnAttributeName">
		/// Name of the attribute for which the value will be returned.
		/// </param>
		/// <param name="expression">
		/// The expression to match.
		/// </param>
		public string GetValueForExpression(string returnAttributeName,
			string expression)
		{
			return
				mAttributes.GetValueForExpression(returnAttributeName, expression);
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
		/// Name of the new component item.
		/// </param>
		/// <param name="lines">
		/// Collection of string source lines representing the raw information to
		/// be parsed.
		/// </param>
		/// <param name="projectPath">
		/// Current project path.
		/// </param>
		/// <returns>
		/// Newly created component item.
		/// </returns>
		public static ComponentItem Parse(string name, StringCollection lines,
			string projectPath)
		{
			AttributeItem attribute = null;
			//AttributeCollection attributes = null;
			int eCount = 0;
			int eIndex = 0;
			string element = "";
			AttributeCatalog elements = null;
			AttributeCollection entry = null;
			string filename = "";
			int iCount = 0;
			FileInfo ifile = null;
			int iIndex = 0;
			string insert = "";
			MatchCollection matches = null;
			ComponentItem result = new ComponentItem();
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
							//attributes = new AttributeCollection();
							//result.Attributes.Add(attributes);
							insert = File.ReadAllText(ifile.FullName);
							elements =
								JsonConvert.DeserializeObject<AttributeCatalog>(insert);
							iCount = elements.Count;
							for(iIndex = 0; iIndex < iCount; iIndex++)
							{
								result.Attributes.Add(elements[iIndex]);
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
							DeserializeObject<AttributeCatalog>(element);
						iCount = elements.Count;
						for(iIndex = 0; iIndex < iCount; iIndex++)
						{
							result.Attributes.Add(elements[iIndex]);
						}
					}
					else if(element.StartsWith("{"))
					{
						//	One element object.
						entry =
							JsonConvert.DeserializeObject<AttributeCollection>(element);
						result.Attributes.Add(entry);
					}
					else if(element.IndexOf(":") > -1)
					{
						//	Name / Value.
						entry = new AttributeCollection();
						sArray = element.Split(new char[] { ':' });
						if(sArray.Length > 1)
						{
							attribute = new AttributeItem();
							entry.Add(attribute);
							attribute.Name = sArray[0].Replace("\"", "").Trim();
							attribute.Value = sArray[1].Replace("\"", "").Trim();
						}
						result.Attributes.Add(entry);
					}
				}
			}

			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
