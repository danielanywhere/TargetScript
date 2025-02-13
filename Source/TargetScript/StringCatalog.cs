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
