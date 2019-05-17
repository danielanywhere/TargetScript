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
	/// Web API 2 Controller for transient employee objects.
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
			mObjectNames = new EmployeeCollection(mDB);
			mObjectNames.Load();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ObjectNames                                                           *
		//*-----------------------------------------------------------------------*
		private EmployeeCollection mObjectNames = null;
		/// <summary>
		/// Get a reference to the collection of employees driven by this
		/// interface.
		/// </summary>
		public EmployeeCollection ObjectNames
		{
			get { return mObjectNames; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DeleteEmployee                                                        *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// DELETE: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// Delete the specified employee.
		/// </remarks>
		[ResponseType(typeof(EmployeeItem))]
		public IHttpActionResult DeleteEmployee(int id)
		{
			EmployeeItem ci = mObjectNames.First(r => r.EmployeeID == id);
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
		//* GetEmployee                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the specified employee.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array due to
		/// the fact that the Kendo UI DataSource will only bind to an array.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(EmployeeItem[]))]
		public IHttpActionResult GetEmployee(int id)
		{
			EmployeeItem ci = mObjectNames.First(r => r.EmployeeID == id);
			EmployeeItem[] ro = new EmployeeItem[] { ci };
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
		/// Return all employees.
		/// </remarks>
		public IQueryable<EmployeeItem> GetObjectNames()
		{
			mObjectNames.Load();
			return mObjectNames.AsQueryable<EmployeeItem>();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Lookup                                                                *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the identifying information for a single employee record.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of specified employee.
		/// </para>
		/// </remarks>
		public IDTextItem Lookup(int id)
		{
			EmployeeItem ci = mObjectNames.First(r => r.EmployeeID == id);
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
		/// Return the default Field and default text value for all employees.
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
		//* PostEmployee                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// POST: api/ObjectNames
		/// </summary>
		/// <remarks>
		/// Use an HTTP POST to store information about  employee.
		/// JavaScriptSerializer is found in System.Web.Extensions.
		/// </remarks>
		[ResponseType(typeof(EmployeeItem))]
		public IHttpActionResult PostEmployee(EmployeeItem employee)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			employee = mObjectNames.AddOrUpdate(employee);
			mObjectNames.SaveChanges();

			return CreatedAtRoute(DefaultApi,
				new { id = employee.EmployeeID }, employee);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PutEmployee                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// PUT: api/ObjectNames/5
		/// </summary>
		[ResponseType(typeof(void))]
		public IHttpActionResult PutEmployee(int id, EmployeeItem employee)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if(id != employee.EmployeeID || !mObjectNames.Exists(id))
			{
				return BadRequest();
			}

			employee = mObjectNames.AddOrUpdate(employee);
			mObjectNames.SaveChanges();

			return StatusCode(HttpStatusCode.NoContent);
		}
		//*-----------------------------------------------------------------------*
		}
	}
}
