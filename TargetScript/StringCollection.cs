//	StringCollection.cs
//	Specialized string collections.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
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
