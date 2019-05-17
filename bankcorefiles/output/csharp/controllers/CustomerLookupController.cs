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
	//* CustomerLookupsController                                               *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Web API 2 Controller for transient customer lookup objects.
	/// </summary>
	public class CustomerLookupsController : ApiController
	{
		//*************************************************************************
		//* Private                                                               *
		//*************************************************************************
		private ObjectNamesController mEntity = new ObjectNamesController();

		//*************************************************************************
		//* Protected                                                             *
		//*************************************************************************
		//*************************************************************************
		//* Public                                                                *
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* GetCustomerLookup                                                     *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/CustomerLookups/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of the specified customer.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array for
		/// Ajax client.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(IDTextItem[]))]
		public IHttpActionResult GetCustomerLookup(int id)
		{
			IDTextItem di = mEntity.Lookup(id);
			IHttpActionResult rv = null;

			if(di != null)
			{
				rv = Ok(di);
			}
			else
			{
				rv = NotFound();
			}
			return rv;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCustomerLookups                                                    *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/CustomerLookups
		/// </summary>
		/// <remarks>
		/// Return all customer lookups.
		/// </remarks>
		public IQueryable<IDTextItem> GetCustomerLookups()
		{
			return mEntity.Lookups().AsQueryable<IDTextItem>();
		}
		//*-----------------------------------------------------------------------*
	}
}

