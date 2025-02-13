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
	//*	TrackingStringCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// String collection that can track whether its content is dirty.
	/// </summary>
	public class TrackingStringCollection : StringCollection
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
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new item to the collection.
		/// </summary>
		/// <param name="value">
		/// String to be added to the collection.
		/// </param>
		public new void Add(string value)
		{
			base.Add(value);
			mIsDirty = true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Clear																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the content of the collection and reset the dirty flag.
		/// </summary>
		public new void Clear()
		{
			base.Clear();
			mIsDirty = false;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsDirty																																*
		//*-----------------------------------------------------------------------*
		private bool mIsDirty = false;
		/// <summary>
		/// Get/Set a value indicating whether the content of this collection is
		/// dirty.
		/// </summary>
		public bool IsDirty
		{
			get { return mIsDirty; }
			set { mIsDirty = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RemoveAt																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the entry at the specified location.
		/// </summary>
		/// <param name="index">
		/// Index at which to remove the item.
		/// </param>
		public new void RemoveAt(int index)
		{
			base.RemoveAt(index);
			mIsDirty = true;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	StringCollection																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of string Items.
	/// </summary>
	public class StringCollection : List<string>
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
		//*	AddUnique																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an item to the collection if it is unique.
		/// </summary>
		/// <param name="value">
		/// Value to add.
		/// </param>
		public void AddUnique(string value)
		{
			if(!this.Exists(x => x == value))
			{
				this.Add(value);
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
