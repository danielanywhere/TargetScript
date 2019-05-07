//	Component.cs
//	Component Collection / Component Item pair.
//	Manages the component metadata pages.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
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

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ComponentEntryFollower																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Tracking class for component / entry pairs.
	/// </summary>
	public class ComponentEntryFollower
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
		/// Create a new Instance of the ComponentEntryFollower Item.
		/// </summary>
		public ComponentEntryFollower()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentEntryFollower Item.
		/// </summary>
		/// <param name="component">
		/// A reference to the component being tracked.
		/// </param>
		/// <param name="entry">
		/// A reference to the currently selected attribute collection for the
		/// component.
		/// </param>
		public ComponentEntryFollower(ComponentItem component,
			AttributeCollection entry)
		{
			mComponent = component;
			mEntry = entry;
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
		//*	Entry																																	*
		//*-----------------------------------------------------------------------*
		private AttributeCollection mEntry = null;
		/// <summary>
		/// Get/Set a reference to the collection of attributes being followed in
		/// this instance.
		/// </summary>
		public AttributeCollection Entry
		{
			get { return mEntry; }
			set { mEntry = value; }
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
		//*	EntryExists																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified entry exists in the
		/// fields.
		/// </summary>
		/// <param name="name">
		/// Name of the entry to search for.
		/// </param>
		/// <returns>
		/// A value indicating whether an attribute of the specified name was found
		/// in at least one of the metadata fields.
		/// </returns>
		public bool EntryExists(string name)
		{
			string nLower = "";
			bool result = false;

			if(name?.Length > 0)
			{
				nLower = name.ToLower();
				foreach(AttributeCollection collection in mAttributes)
				{
					//	The base object doesn't count as an entry, but the definition of
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
		//*	GetEntryCount																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the count of non-object entries in the collection of attributes.
		/// </summary>
		/// <returns>
		/// The count of non-base entries in the catalog.
		/// </returns>
		public int GetEntryCount()
		{
			int result = 0;
			foreach(AttributeCollection collection in mAttributes)
			{
				//	The base object doesn't count as an entry, but the definition of
				//	the object itself.
				if(!collection.Exists(x =>
					x.Name.ToLower() == "datatype" &&
					x.Value.ToLower() == "baseobject"))
				{
					result++;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetEntryNames																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return all known entry names from the collections.
		/// </summary>
		/// <returns>
		/// Delimited list of all metadata field names found in the catalog.
		/// </returns>
		public string GetEntryNames()
		{
			StringBuilder result = new StringBuilder();
			foreach(AttributeCollection collection in mAttributes)
			{
				//	The base object doesn't count as an entry, but the definition of
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
		//*	GetElementCount																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the count of all elements in the catalog.
		/// </summary>
		/// <returns>
		/// The count of all elements found in the catalog.
		/// </returns>
		public int GetElementCount()
		{
			int result = 0;
			return result;
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
						//	One entry object.
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
