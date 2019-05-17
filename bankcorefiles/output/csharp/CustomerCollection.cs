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
	//* ComponentCustomerCollection                                             *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ComponentCustomerItem items.
	/// </summary>
	public class ComponentCustomerCollection : ObservableCollection<ComponentCustomerItem>
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
		/// Create a new Instance of the ComponentCustomerCollection item.
		/// </summary>
		public ComponentCustomerCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentCustomerCollection Item.
		/// </summary>
		public ComponentCustomerCollection(bankEntities context)
		{
			mContext = context;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Add                                                                   *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentCustomerItem to the collection.
		/// </summary>
		public new void Add(ComponentCustomerItem componentcustomer)
		{
			bnkComponentCustomer ci = null;
			if(componentcustomer != null)
			{
				base.Add(componentcustomer);
				if(componentcustomer.EntityItem == null && mContext != null)
				{
					componentcustomer.HasPresetValues = true;
					ci = new bnkComponentCustomer();
					mContext.bnk.Add(ci);
					componentcustomer.EntityItem = ci;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddOrUpdate                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentCustomerItem to the collection, or update the existing one.
		/// </summary>
		public ComponentCustomerItem AddOrUpdate(ComponentCustomerItem componentcustomer)
		{
			bnkComponentCustomer ci = null;			// Internal Item.
			ComponentCustomerItem cx = null;			// Existing Item.
			if(componentcustomer != null)
			{
				cx = this.FirstOrDefault(r => r.ComponentCustomerID == componentcustomer.ComponentCustomerID);
				if(cx == null)
				{
					// Create new item.
					base.Add(componentcustomer);
					if(componentcustomer.EntityItem == null && mContext != null)
					{
						componentcustomer.HasPresetValues = true;
						ci = new bnkComponentCustomer();
						mContext.bnk.Add(ci);
						componentcustomer.EntityItem = ci;
					}
					cx = componentcustomer;
				}
				else
				{
					// Item already existed.
					componentcustomer.TransferProperties(cx);
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
		private Type mElementType = typeof(ComponentCustomerItem);
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
		/// Return a value indicating whether the specified componentcustomer exists in
		/// this collection.
		/// </summary>
		public bool Exists(int componentcustomerID)
		{
			bool rv = (this.First(r => r.ComponentCustomerID == componentcustomerID) != null);
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Expression                                                            *
		//*-----------------------------------------------------------------------*
		private System.Linq.Expressions.Expression mExpression =
			System.Linq.Expressions.Expression.New(typeof(ComponentCustomerCollection));
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
		/// Load all componentcustomers.
		/// </summary>
		public bool Load()
		{
			bankEntities cx = Context;
			bool rv = false;   // Return Value.

			this.Clear();
			cx.bnk.Load();
			foreach(bnkComponentCustomer ci in cx.bnkComponentCustomers)
			{
				this.Add(new ComponentCustomerItem(this, ci));
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
		/// Remove the specified componentcustomer from the collection.
		/// </summary>
		public new void Remove(ComponentCustomerItem componentcustomer)
		{
			ComponentCustomerItem ci = null;

			if(componentcustomer != null)
			{
				// Value is specified.
				ci = this.First(r => r.ComponentCustomerID == componentcustomer.ComponentCustomerID);
				if(ci != null)
				{
					// Member item found.
					if(ci.EntityItem != null && mContext != null)
					{
						mContext.bnk.Remove((bnkComponentCustomer)ci.EntityItem);
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
		public void SetItemModified(ComponentCustomerItem item)
		{
			if(item != null && item.EntityItem != null && mContext != null)
			{
				mContext.Entry(item.EntityItem).State = EntityState.Modified;
			}
		}
		//*-----------------------------------------------------------------------*
	}

	//*-------------------------------------------------------------------------*
	//* ComponentCustomerItem                                                 *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the componentcustomer.
	/// </summary>
	[DataContract]
	public class ComponentCustomerItem : TransientItem
	{
		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		private static List<string> ComponentCustomerItemPropertyNames =
			new List<string>(
			new string[] { CustomerID,CustomerTicket,Name,Address,City,State,ZipCode,TIN });

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
		/// Create a new Instance of the ComponentCustomerItem Item.
		/// </summary>
		public ComponentCustomerItem()
		{
			PropertyNames = ComponentCustomerItemPropertyNames;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentCustomerItem Item.
		/// </summary>
		public ComponentCustomerItem(ComponentCustomerCollection parent, bnkComponentCustomer componentcustomer) :
			this()
		{
			mParent = parent;
			EntityItem = componentcustomer;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CustomerID                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Customer ID of the componentcustomer.
		/// </summary>
		[DataMember]
		public int CustomerID
		{
			get { return mCustomerID; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CustomerTicket                                                        *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Customer Ticket of the componentcustomer.
		/// </summary>
		[DataMember]
		public Guid CustomerTicket
		{
			get { return mCustomerTicket; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Name                                                                  *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Name of the componentcustomer.
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
		/// Get the Address of the componentcustomer.
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
		/// Get the City of the componentcustomer.
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
		/// Get the State of the componentcustomer.
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
		/// Get the Zip Code of the componentcustomer.
		/// </summary>
		[DataMember]
		public string ZipCode
		{
			get { return mZipCode; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TIN                                                                   *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the TIN of the componentcustomer.
		/// </summary>
		[DataMember]
		public string TIN
		{
			get { return mTIN; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Parent                                                                *
		//*-----------------------------------------------------------------------*
		private ComponentCustomerCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection to which this item is
		/// attached.
		/// </summary>
		public ComponentCustomerCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*
	}
}
