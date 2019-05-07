//	StringCatalog.cs
//	Catalog of string collections.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
using System.Collections.Generic;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	StringCatalog																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of StringItem Items.
	/// </summary>
	public class StringCatalog : List<StringCollectionItem>
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
		//*	AddUniqueCollection																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a unique collection name, if needed, and add the value to the
		/// selected list.
		/// </summary>
		/// <param name="name">
		/// Name of the new collection.
		/// </param>
		/// <param name="value">
		/// Initial value to add.
		/// </param>
		/// <returns>
		/// Newly created string collection, if the name was unique. Otherwise,
		/// a reference to the previously existing collection of the same name.
		/// </returns>
		public StringCollectionItem AddUniqueCollection(string name, string value)
		{
			StringCollectionItem result = null;

			foreach(StringCollectionItem item in this)
			{
				if(item.Name == name)
				{
					result = item;
					break;
				}
			}
			if(result == null)
			{
				result = new StringCollectionItem();
				result.Name = name;
				this.Add(result);
			}
			result.Values.Add(value);
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	StringCollectionItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// String collection with identifying information.
	/// </summary>
	public class StringCollectionItem
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
		//*	Values																																*
		//*-----------------------------------------------------------------------*
		private StringCollection mValues = new StringCollection();
		/// <summary>
		/// Get a reference to the collection of values on this item.
		/// </summary>
		public StringCollection Values
		{
			get { return mValues; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
