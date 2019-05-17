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
	//* BranchLookupsController                                                 *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Web API 2 Controller for transient branch lookup objects.
	/// </summary>
	public class BranchLookupsController : ApiController
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
		//* GetBranchLookup                                                       *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/BranchLookups/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of the specified branch.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array for
		/// Ajax client.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(IDTextItem[]))]
		public IHttpActionResult GetBranchLookup(int id)
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
		//* GetBranchLookups                                                      *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/BranchLookups
		/// </summary>
		/// <remarks>
		/// Return all branch lookups.
		/// </remarks>
		public IQueryable<IDTextItem> GetBranchLookups()
		{
			return mEntity.Lookups().AsQueryable<IDTextItem>();
		}
		//*-----------------------------------------------------------------------*
	}
}

