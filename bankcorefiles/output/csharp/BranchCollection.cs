using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BankViewModel
{
	//*-------------------------------------------------------------------------*
	//* ComponentBranchCollection                                               *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ComponentBranchItem items.
	/// </summary>
	public class ComponentBranchCollection : ObservableCollection<ComponentBranchItem>
	{
		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		//*************************************************************************
		//* Protected                                                             *
		//*************************************************************************
		//*************************************************************************
		//* Public                                                                *
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* _Constructor                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ComponentBranchCollection item.
		/// </summary>
		public ComponentBranchCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentBranchCollection Item.
		/// </summary>
		public ComponentBranchCollection(bankEntities context)
		{
			mContext = context;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Add                                                                   *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentBranchItem to the collection.
		/// </summary>
		public new void Add(ComponentBranchItem componentbranch)
		{
			bnkComponentBranch ci = null;
			if(componentbranch != null)
			{
				base.Add(componentbranch);
				if(componentbranch.EntityItem == null && mContext != null)
				{
					componentbranch.HasPresetValues = true;
					ci = new bnkComponentBranch();
					mContext.bnk.Add(ci);
					componentbranch.EntityItem = ci;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddOrUpdate                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentBranchItem to the collection, or update the existing one.
		/// </summary>
		public ComponentBranchItem AddOrUpdate(ComponentBranchItem componentbranch)
		{
			bnkComponentBranch ci = null;			// Internal Item.
			ComponentBranchItem cx = null;			// Existing Item.
			if(componentbranch != null)
			{
				cx = this.FirstOrDefault(r => r.ComponentBranchID == componentbranch.ComponentBranchID);
				if(cx == null)
				{
					// Create new item.
					base.Add(componentbranch);
					if(componentbranch.EntityItem == null && mContext != null)
					{
						componentbranch.HasPresetValues = true;
						ci = new bnkComponentBranch();
						mContext.bnk.Add(ci);
						componentbranch.EntityItem = ci;
					}
					cx = componentbranch;
				}
				else
				{
					// Item already existed.
					componentbranch.TransferProperties(cx);
				}
			}
			return cx;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Context                                                               *
		//*-----------------------------------------------------------------------*
		private bankEntities mContext = null;
		/// <summary>
		/// Get/Set a reference to the current data model content.
		/// </summary>
		public bankEntities Context
		{
			get
			{
				if(mContext == null)
				{
					mContext = new bankEntities();
				}
				return mContext;
			}
			set { mContext = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ElementType                                                           *
		//*-----------------------------------------------------------------------*
		private Type mElementType = typeof(ComponentBranchItem);
		/// <summary>
		/// Get the type of element returned when the expression tree associated
		/// with this instance is executed.
		/// </summary>
		public Type ElementType
		{
			get { return mElementType; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Exists                                                                *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified componentbranch exists in
		/// this collection.
		/// </summary>
		public bool Exists(int componentbranchID)
		{
			bool rv = (this.First(r => r.ComponentBranchID == componentbranchID) != null);
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Expression                                                            *
		//*-----------------------------------------------------------------------*
		private System.Linq.Expressions.Expression mExpression =
			System.Linq.Expressions.Expression.New(typeof(ComponentBranchCollection));
		/// <summary>
		/// Get the expression tree associated with this instance of IQueryable.
		/// </summary>
		public System.Linq.Expressions.Expression Expression
		{
			get { return mExpression; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Load                                                                  *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Load all componentbranches.
		/// </summary>
		public bool Load()
		{
			bankEntities cx = Context;
			bool rv = false;   // Return Value.

			this.Clear();
			cx.bnk.Load();
			foreach(bnkComponentBranch ci in cx.bnkComponentBranches)
			{
				this.Add(new ComponentBranchItem(this, ci));
			}
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Queryable                                                             *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get a reference to the queryable list for this collection.
		/// </summary>
		public IQueryable<CustomerItem> Queryable
		{
			get
			{
				return this.AsQueryable();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Remove                                                                *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the specified componentbranch from the collection.
		/// </summary>
		public new void Remove(ComponentBranchItem componentbranch)
		{
			ComponentBranchItem ci = null;

			if(componentbranch != null)
			{
				// Value is specified.
				ci = this.First(r => r.ComponentBranchID == componentbranch.ComponentBranchID);
				if(ci != null)
				{
					// Member item found.
					if(ci.EntityItem != null && mContext != null)
					{
						mContext.bnk.Remove((bnkComponentBranch)ci.EntityItem);
					}
					this.Remove(ci);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveChanges                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save all of the changes in the associated entity collection.
		/// </summary>
		public void SaveChanges()
		{
			bankEntities cx = Context;

			cx.SaveChanges();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetItemModified                                                       *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the background state to modified for the specified item.
		/// </summary>
		public void SetItemModified(ComponentBranchItem item)
		{
			if(item != null && item.EntityItem != null && mContext != null)
			{
				mContext.Entry(item.EntityItem).State = EntityState.Modified;
			}
		}
		//*-----------------------------------------------------------------------*
	}

	//*-------------------------------------------------------------------------*
	//* ComponentBranchItem                                                   *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the componentbranch.
	/// </summary>
	[DataContract]
	public class ComponentBranchItem : TransientItem
	{
		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		private static List<string> ComponentBranchItemPropertyNames =
			new List<string>(
			new string[] { BranchID,BranchTicket,Name,Address,City,State,ZipCode });

		//*************************************************************************
		//* Protected                                                             *
		//*************************************************************************
		//*************************************************************************
		//* Public                                                                *
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* _Constructor                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ComponentBranchItem Item.
		/// </summary>
		public ComponentBranchItem()
		{
			PropertyNames = ComponentBranchItemPropertyNames;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentBranchItem Item.
		/// </summary>
		public ComponentBranchItem(ComponentBranchCollection parent, bnkComponentBranch componentbranch) :
			this()
		{
			mParent = parent;
			EntityItem = componentbranch;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* BranchID                                                              *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Branch ID of the componentbranch.
		/// </summary>
		[DataMember]
		public int BranchID
		{
			get { return mBranchID; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* BranchTicket                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Ticket of the componentbranch.
		/// </summary>
		[DataMember]
		public Guid BranchTicket
		{
			get { return mBranchTicket; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Name                                                                  *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Name of the componentbranch.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return mName; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Address                                                               *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Address of the componentbranch.
		/// </summary>
		[DataMember]
		public string Address
		{
			get { return mAddress; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* City                                                                  *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the City of the componentbranch.
		/// </summary>
		[DataMember]
		public string City
		{
			get { return mCity; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* State                                                                 *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the State of the componentbranch.
		/// </summary>
		[DataMember]
		public string State
		{
			get { return mState; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ZipCode                                                               *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Zip Code of the componentbranch.
		/// </summary>
		[DataMember]
		public string ZipCode
		{
			get { return mZipCode; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Parent                                                                *
		//*-----------------------------------------------------------------------*
		private ComponentBranchCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection to which this item is
		/// attached.
		/// </summary>
		public ComponentBranchCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*
	}
}
