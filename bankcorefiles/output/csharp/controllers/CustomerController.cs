using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using BankViewModel;

namespace BankWEB
{
	//*-------------------------------------------------------------------------*
	//* ObjectNamesController                                                   *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Web API 2 Controller for transient customer objects.
	/// </summary>
	public class ObjectNamesController : ApiController
	{
		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		private bankEntities mDB = new bankEntities();

		//*************************************************************************
		//* Protected                                                             *
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* Dispose                                                               *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// When disposing, also dispose of the context.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				mDB.Dispose();
			}
			base.Dispose(disposing);
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* _Constructor                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new Instance of the ObjectNamesController Item.
		/// </summary>
		public ObjectNamesController()
		{
			mObjectNames = new CustomerCollection(mDB);
			mObjectNames.Load();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ObjectNames                                                           *
		//*-----------------------------------------------------------------------*
		private CustomerCollection mObjectNames = null;
		/// <summary>
		/// Get a reference to the collection of customers driven by this
		/// interface.
		/// </summary>
		public CustomerCollection ObjectNames
		{
			get { return mObjectNames; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DeleteCustomer                                                        *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// DELETE: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// Delete the specified customer.
		/// </remarks>
		[ResponseType(typeof(CustomerItem))]
		public IHttpActionResult DeleteCustomer(int id)
		{
			CustomerItem ci = mObjectNames.First(r => r.CustomerID == id);
			if(ci == null)
			{
				return NotFound();
			}

			mObjectNames.Remove(ci);
			mObjectNames.SaveChanges();

			return Ok(ci);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCustomer                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the specified customer.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array due to
		/// the fact that the Kendo UI DataSource will only bind to an array.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(CustomerItem[]))]
		public IHttpActionResult GetCustomer(int id)
		{
			CustomerItem ci = mObjectNames.First(r => r.CustomerID == id);
			CustomerItem[] ro = new CustomerItem[] { ci };
			if(ci == null)
			{
				return NotFound();
			}

			return Ok(ro);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetObjectNames                                                        *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/ObjectNames
		/// </summary>
		/// <remarks>
		/// Return all customers.
		/// </remarks>
		public IQueryable<CustomerItem> GetObjectNames()
		{
			mObjectNames.Load();
			return mObjectNames.AsQueryable<CustomerItem>();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Lookup                                                                *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the identifying information for a single customer record.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of specified customer.
		/// </para>
		/// </remarks>
		public IDTextItem Lookup(int id)
		{
			CustomerItem ci = mObjectNames.First(r => r.CustomerID == id);
			IDTextItem di = IDTextItem.Assign(ci, , );

			return di;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Lookups                                                               *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the collection of ID lookups for this entity.
		/// </summary>
		/// <remarks>
		/// Return the default Field and default text value for all customers.
		/// </remarks>
		public IDTextCollection Lookups()
		{
			IDTextCollection rv = new IDTextCollection();
			if(mObjectNames.Count() == 0)
			{
				mObjectNames.Load();
			}
			rv.AddRange(mObjectNames, , );
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PostCustomer                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// POST: api/ObjectNames
		/// </summary>
		/// <remarks>
		/// Use an HTTP POST to store information about  customer.
		/// JavaScriptSerializer is found in System.Web.Extensions.
		/// </remarks>
		[ResponseType(typeof(CustomerItem))]
		public IHttpActionResult PostCustomer(CustomerItem customer)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			customer = mObjectNames.AddOrUpdate(customer);
			mObjectNames.SaveChanges();

			return CreatedAtRoute(DefaultApi,
				new { id = customer.CustomerID }, customer);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PutCustomer                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// PUT: api/ObjectNames/5
		/// </summary>
		[ResponseType(typeof(void))]
		public IHttpActionResult PutCustomer(int id, CustomerItem customer)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if(id != customer.CustomerID || !mObjectNames.Exists(id))
			{
				return BadRequest();
			}

			customer = mObjectNames.AddOrUpdate(customer);
			mObjectNames.SaveChanges();

			return StatusCode(HttpStatusCode.NoContent);
		}
		//*-----------------------------------------------------------------------*
		}
	}
}
