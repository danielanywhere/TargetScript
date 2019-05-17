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
	//* ComponentEmployeeCollection                                             *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ComponentEmployeeItem items.
	/// </summary>
	public class ComponentEmployeeCollection : ObservableCollection<ComponentEmployeeItem>
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
		/// Create a new Instance of the ComponentEmployeeCollection item.
		/// </summary>
		public ComponentEmployeeCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentEmployeeCollection Item.
		/// </summary>
		public ComponentEmployeeCollection(bankEntities context)
		{
			mContext = context;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Add                                                                   *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentEmployeeItem to the collection.
		/// </summary>
		public new void Add(ComponentEmployeeItem componentemployee)
		{
			bnkComponentEmployee ci = null;
			if(componentemployee != null)
			{
				base.Add(componentemployee);
				if(componentemployee.EntityItem == null && mContext != null)
				{
					componentemployee.HasPresetValues = true;
					ci = new bnkComponentEmployee();
					mContext.bnk.Add(ci);
					componentemployee.EntityItem = ci;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddOrUpdate                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new ComponentEmployeeItem to the collection, or update the existing one.
		/// </summary>
		public ComponentEmployeeItem AddOrUpdate(ComponentEmployeeItem componentemployee)
		{
			bnkComponentEmployee ci = null;			// Internal Item.
			ComponentEmployeeItem cx = null;			// Existing Item.
			if(componentemployee != null)
			{
				cx = this.FirstOrDefault(r => r.ComponentEmployeeID == componentemployee.ComponentEmployeeID);
				if(cx == null)
				{
					// Create new item.
					base.Add(componentemployee);
					if(componentemployee.EntityItem == null && mContext != null)
					{
						componentemployee.HasPresetValues = true;
						ci = new bnkComponentEmployee();
						mContext.bnk.Add(ci);
						componentemployee.EntityItem = ci;
					}
					cx = componentemployee;
				}
				else
				{
					// Item already existed.
					componentemployee.TransferProperties(cx);
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
		private Type mElementType = typeof(ComponentEmployeeItem);
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
		/// Return a value indicating whether the specified componentemployee exists in
		/// this collection.
		/// </summary>
		public bool Exists(int componentemployeeID)
		{
			bool rv = (this.First(r => r.ComponentEmployeeID == componentemployeeID) != null);
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Expression                                                            *
		//*-----------------------------------------------------------------------*
		private System.Linq.Expressions.Expression mExpression =
			System.Linq.Expressions.Expression.New(typeof(ComponentEmployeeCollection));
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
		/// Load all componentemployees.
		/// </summary>
		public bool Load()
		{
			bankEntities cx = Context;
			bool rv = false;   // Return Value.

			this.Clear();
			cx.bnk.Load();
			foreach(bnkComponentEmployee ci in cx.bnkComponentEmployees)
			{
				this.Add(new ComponentEmployeeItem(this, ci));
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
		/// Remove the specified componentemployee from the collection.
		/// </summary>
		public new void Remove(ComponentEmployeeItem componentemployee)
		{
			ComponentEmployeeItem ci = null;

			if(componentemployee != null)
			{
				// Value is specified.
				ci = this.First(r => r.ComponentEmployeeID == componentemployee.ComponentEmployeeID);
				if(ci != null)
				{
					// Member item found.
					if(ci.EntityItem != null && mContext != null)
					{
						mContext.bnk.Remove((bnkComponentEmployee)ci.EntityItem);
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
		public void SetItemModified(ComponentEmployeeItem item)
		{
			if(item != null && item.EntityItem != null && mContext != null)
			{
				mContext.Entry(item.EntityItem).State = EntityState.Modified;
			}
		}
		//*-----------------------------------------------------------------------*
	}

	//*-------------------------------------------------------------------------*
	//* ComponentEmployeeItem                                                 *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the componentemployee.
	/// </summary>
	[DataContract]
	public class ComponentEmployeeItem : TransientItem
	{
		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		private static List<string> ComponentEmployeeItemPropertyNames =
			new List<string>(
			new string[] { DisplayName,EmployeeID,EmployeeTicket,FirstName,LastName,DateStarted,DateEnded,Title,TIN });

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
		/// Create a new Instance of the ComponentEmployeeItem Item.
		/// </summary>
		public ComponentEmployeeItem()
		{
			PropertyNames = ComponentEmployeeItemPropertyNames;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new Instance of the ComponentEmployeeItem Item.
		/// </summary>
		public ComponentEmployeeItem(ComponentEmployeeCollection parent, bnkComponentEmployee componentemployee) :
			this()
		{
			mParent = parent;
			EntityItem = componentemployee;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DisplayName                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Display Name of the componentemployee.
		/// </summary>
		[DataMember]
		public string DisplayName
		{
			get
			{
				return ;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EmployeeID                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Employee ID of the componentemployee.
		/// </summary>
		[DataMember]
		public int EmployeeID
		{
			get { return mEmployeeID; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EmployeeTicket                                                        *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Ticket of the componentemployee.
		/// </summary>
		[DataMember]
		public Guid EmployeeTicket
		{
			get { return mEmployeeTicket; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FirstName                                                             *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the First Name of the componentemployee.
		/// </summary>
		[DataMember]
		public string FirstName
		{
			get { return mFirstName; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* LastName                                                              *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Last Name of the componentemployee.
		/// </summary>
		[DataMember]
		public string LastName
		{
			get { return mLastName; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DateStarted                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Date Started of the componentemployee.
		/// </summary>
		[DataMember]
		public DateTime DateStarted
		{
			get { return mDateStarted; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DateEnded                                                             *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Date Ended of the componentemployee.
		/// </summary>
		[DataMember]
		public DateTime DateEnded
		{
			get { return mDateEnded; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Title                                                                 *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the Title of the componentemployee.
		/// </summary>
		[DataMember]
		public string Title
		{
			get { return mTitle; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TIN                                                                   *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the TIN of the componentemployee.
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
		private ComponentEmployeeCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection to which this item is
		/// attached.
		/// </summary>
		public ComponentEmployeeCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*
	}
}
