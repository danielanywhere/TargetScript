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
	/// Web API 2 Controller for transient branch objects.
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
			mObjectNames = new BranchCollection(mDB);
			mObjectNames.Load();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ObjectNames                                                           *
		//*-----------------------------------------------------------------------*
		private BranchCollection mObjectNames = null;
		/// <summary>
		/// Get a reference to the collection of branches driven by this
		/// interface.
		/// </summary>
		public BranchCollection ObjectNames
		{
			get { return mObjectNames; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DeleteBranch                                                          *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// DELETE: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// Delete the specified branch.
		/// </remarks>
		[ResponseType(typeof(BranchItem))]
		public IHttpActionResult DeleteBranch(int id)
		{
			BranchItem ci = mObjectNames.First(r => r.BranchID == id);
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
		//* GetBranch                                                             *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the specified branch.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array due to
		/// the fact that the Kendo UI DataSource will only bind to an array.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(BranchItem[]))]
		public IHttpActionResult GetBranch(int id)
		{
			BranchItem ci = mObjectNames.First(r => r.BranchID == id);
			BranchItem[] ro = new BranchItem[] { ci };
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
		/// Return all branches.
		/// </remarks>
		public IQueryable<BranchItem> GetObjectNames()
		{
			mObjectNames.Load();
			return mObjectNames.AsQueryable<BranchItem>();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Lookup                                                                *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the identifying information for a single branch record.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of specified branch.
		/// </para>
		/// </remarks>
		public IDTextItem Lookup(int id)
		{
			BranchItem ci = mObjectNames.First(r => r.BranchID == id);
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
		/// Return the default Field and default text value for all branches.
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
		//* PostBranch                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// POST: api/ObjectNames
		/// </summary>
		/// <remarks>
		/// Use an HTTP POST to store information about  branch.
		/// JavaScriptSerializer is found in System.Web.Extensions.
		/// </remarks>
		[ResponseType(typeof(BranchItem))]
		public IHttpActionResult PostBranch(BranchItem branch)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			branch = mObjectNames.AddOrUpdate(branch);
			mObjectNames.SaveChanges();

			return CreatedAtRoute(DefaultApi,
				new { id = branch.BranchID }, branch);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PutBranch                                                             *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// PUT: api/ObjectNames/5
		/// </summary>
		[ResponseType(typeof(void))]
		public IHttpActionResult PutBranch(int id, BranchItem branch)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if(id != branch.BranchID || !mObjectNames.Exists(id))
			{
				return BadRequest();
			}

			branch = mObjectNames.AddOrUpdate(branch);
			mObjectNames.SaveChanges();

			return StatusCode(HttpStatusCode.NoContent);
		}
		//*-----------------------------------------------------------------------*
		}
	}
}
