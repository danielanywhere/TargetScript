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
	//* AccountLookupsController                                                *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Web API 2 Controller for transient account lookup objects.
	/// </summary>
	public class AccountLookupsController : ApiController
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
		//* GetAccountLookup                                                      *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/AccountLookups/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of the specified account.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array for
		/// Ajax client.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(IDTextItem[]))]
		public IHttpActionResult GetAccountLookup(int id)
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
		//* GetAccountLookups                                                     *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/AccountLookups
		/// </summary>
		/// <remarks>
		/// Return all account lookups.
		/// </remarks>
		public IQueryable<IDTextItem> GetAccountLookups()
		{
			return mEntity.Lookups().AsQueryable<IDTextItem>();
		}
		//*-----------------------------------------------------------------------*
	}
}

