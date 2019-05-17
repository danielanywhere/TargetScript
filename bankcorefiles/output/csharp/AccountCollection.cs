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
	//* ComponentAccountCollection                                              *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ComponentAccountItem items.
	/// </summary>
	public class ComponentAccountCollection : ObservableCollection<ComponentAccountItem>
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
		/// Create a new Instance of the ComponentAccountCollection item.
		/// </summary>
		public ComponentAccountCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentAccountCollection Item.
		/// </summary>
		public ComponentAccountCollection(bankEntities context)
		{
			mContext = context;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Add                                                                   *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentAccountItem to the collection.
		/// </summary>
		public new void Add(ComponentAccountItem componentaccount)
		{
			bnkComponentAccount ci = null;
			if(componentaccount != null)
			{
				base.Add(componentaccount);
				if(componentaccount.EntityItem == null && mContext != null)
				{
					componentaccount.HasPresetValues = true;
					ci = new bnkComponentAccount();
					mContext.bnk.Add(ci);
					componentaccount.EntityItem = ci;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddOrUpdate                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentAccountItem to the collection, or update the existing one.
		/// </summary>
		public ComponentAccountItem AddOrUpdate(ComponentAccountItem componentaccount)
		{
			bnkComponentAccount ci = null;			// Internal Item.
			ComponentAccountItem cx = null;			// Existing Item.
			if(componentaccount != null)
			{
				cx = this.FirstOrDefault(r => r.ComponentAccountID == componentaccount.ComponentAccountID);
				if(cx == null)
				{
					// Create new item.
					base.Add(componentaccount);
					if(componentaccount.EntityItem == null && mContext != null)
					{
						componentaccount.HasPresetValues = true;
						ci = new bnkComponentAccount();
						mContext.bnk.Add(ci);
						componentaccount.EntityItem = ci;
					}
					cx = componentaccount;
				}
				else
				{
					// Item already existed.
					componentaccount.TransferProperties(cx);
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
		private Type mElementType = typeof(ComponentAccountItem);
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
		/// Return a value indicating whether the specified componentaccount exists in
		/// this collection.
		/// </summary>
		public bool Exists(int componentaccountID)
		{
			bool rv = (this.First(r => r.ComponentAccountID == componentaccountID) != null);
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Expression                                                            *
		//*-----------------------------------------------------------------------*
		private System.Linq.Expressions.Expression mExpression =
			System.Linq.Expressions.Expression.New(typeof(ComponentAccountCollection));
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
		/// Load all componentaccounts.
		/// </summary>
		public bool Load()
		{
			bankEntities cx = Context;
			bool rv = false;   // Return Value.

			this.Clear();
			cx.bnk.Load();
			foreach(bnkComponentAccount ci in cx.bnkComponentAccounts)
			{
				this.Add(new ComponentAccountItem(this, ci));
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
		/// Remove the specified componentaccount from the collection.
		/// </summary>
		public new void Remove(ComponentAccountItem componentaccount)
		{
			ComponentAccountItem ci = null;

			if(componentaccount != null)
			{
				// Value is specified.
				ci = this.First(r => r.ComponentAccountID == componentaccount.ComponentAccountID);
				if(ci != null)
				{
					// Member item found.
					if(ci.EntityItem != null && mContext != null)
					{
						mContext.bnk.Remove((bnkComponentAccount)ci.EntityItem);
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
		public void SetItemModified(ComponentAccountItem item)
		{
			if(item != null && item.EntityItem != null && mContext != null)
			{
				mContext.Entry(item.EntityItem).State = EntityState.Modified;
			}
		}
		//*-----------------------------------------------------------------------*
	}

	//*-------------------------------------------------------------------------*
	//* ComponentAccountItem                                                  *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the componentaccount.
	/// </summary>
	[DataContract]
	public class ComponentAccountItem : TransientItem
	{
		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		private static List<string> ComponentAccountItemPropertyNames =
			new List<string>(
			new string[] { AccountID,AccountTicket,AccountStatus,BalanceAvailable,BalancePending,BranchID,CustomerID,DateClosed,DateLastActivity,DateOpened,EmployeeID });

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
		/// Create a new Instance of the ComponentAccountItem Item.
		/// </summary>
		public ComponentAccountItem()
		{
			PropertyNames = ComponentAccountItemPropertyNames;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentAccountItem Item.
		/// </summary>
		public ComponentAccountItem(ComponentAccountCollection parent, bnkComponentAccount componentaccount) :
			this()
		{
			mParent = parent;
			EntityItem = componentaccount;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AccountID                                                             *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Account ID of the componentaccount.
		/// </summary>
		[DataMember]
		public int AccountID
		{
			get { return mAccountID; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AccountTicket                                                         *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Ticket of the componentaccount.
		/// </summary>
		[DataMember]
		public Guid AccountTicket
		{
			get { return mAccountTicket; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AccountStatus                                                         *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Status of the componentaccount.
		/// </summary>
		[DataMember]
		public string AccountStatus
		{
			get { return mAccountStatus; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* BalanceAvailable                                                      *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Balance Available of the componentaccount.
		/// </summary>
		[DataMember]
		public decimal BalanceAvailable
		{
			get { return mBalanceAvailable; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* BalancePending                                                        *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Balance Pending of the componentaccount.
		/// </summary>
		[DataMember]
		public decimal BalancePending
		{
			get { return mBalancePending; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* BranchID                                                              *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Branch of the componentaccount.
		/// </summary>
		[DataMember]
		public int BranchID
		{
			get { return mBranchID; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CustomerID                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Customer of the componentaccount.
		/// </summary>
		[DataMember]
		public int CustomerID
		{
			get { return mCustomerID; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DateClosed                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Date Closed of the componentaccount.
		/// </summary>
		[DataMember]
		public DateTime DateClosed
		{
			get { return mDateClosed; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DateLastActivity                                                      *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Date Last Activity of the componentaccount.
		/// </summary>
		[DataMember]
		public DateTime DateLastActivity
		{
			get { return mDateLastActivity; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DateOpened                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Date Opened of the componentaccount.
		/// </summary>
		[DataMember]
		public DateTime DateOpened
		{
			get { return mDateOpened; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EmployeeID                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Employee of the componentaccount.
		/// </summary>
		[DataMember]
		public int EmployeeID
		{
			get { return mEmployeeID; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Parent                                                                *
		//*-----------------------------------------------------------------------*
		private ComponentAccountCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection to which this item is
		/// attached.
		/// </summary>
		public ComponentAccountCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*
	}
}
