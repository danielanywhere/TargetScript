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
	//* EmployeeLookupsController                                               *
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Web API 2 Controller for transient employee lookup objects.
	/// </summary>
	public class EmployeeLookupsController : ApiController
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
		//* GetEmployeeLookup                                                     *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/EmployeeLookups/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of the specified employee.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array for
		/// Ajax client.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(IDTextItem[]))]
		public IHttpActionResult GetEmployeeLookup(int id)
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
		//* GetEmployeeLookups                                                    *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/EmployeeLookups
		/// </summary>
		/// <remarks>
		/// Return all employee lookups.
		/// </remarks>
		public IQueryable<IDTextItem> GetEmployeeLookups()
		{
			return mEntity.Lookups().AsQueryable<IDTextItem>();
		}
		//*-----------------------------------------------------------------------*
	}
}

