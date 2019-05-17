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
	/// Web API 2 Controller for transient account objects.
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
			mObjectNames = new AccountCollection(mDB);
			mObjectNames.Load();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ObjectNames                                                           *
		//*-----------------------------------------------------------------------*
		private AccountCollection mObjectNames = null;
		/// <summary>
		/// Get a reference to the collection of accounts driven by this
		/// interface.
		/// </summary>
		public AccountCollection ObjectNames
		{
			get { return mObjectNames; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DeleteAccount                                                         *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// DELETE: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// Delete the specified account.
		/// </remarks>
		[ResponseType(typeof(AccountItem))]
		public IHttpActionResult DeleteAccount(int id)
		{
			AccountItem ci = mObjectNames.First(r => r.AccountID == id);
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
		//* GetAccount                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// GET: api/ObjectNames/5
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the specified account.
		/// </para>
		/// <para>
		/// Even with a single entry, it is important to return an array due to
		/// the fact that the Kendo UI DataSource will only bind to an array.
		/// </para>
		/// </remarks>
		[ResponseType(typeof(AccountItem[]))]
		public IHttpActionResult GetAccount(int id)
		{
			AccountItem ci = mObjectNames.First(r => r.AccountID == id);
			AccountItem[] ro = new AccountItem[] { ci };
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
		/// Return all accounts.
		/// </remarks>
		public IQueryable<AccountItem> GetObjectNames()
		{
			mObjectNames.Load();
			return mObjectNames.AsQueryable<AccountItem>();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Lookup                                                                *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the identifying information for a single account record.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Return the ID and default text of specified account.
		/// </para>
		/// </remarks>
		public IDTextItem Lookup(int id)
		{
			AccountItem ci = mObjectNames.First(r => r.AccountID == id);
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
		/// Return the default Field and default text value for all accounts.
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
		//* PostAccount                                                           *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// POST: api/ObjectNames
		/// </summary>
		/// <remarks>
		/// Use an HTTP POST to store information about  account.
		/// JavaScriptSerializer is found in System.Web.Extensions.
		/// </remarks>
		[ResponseType(typeof(AccountItem))]
		public IHttpActionResult PostAccount(AccountItem account)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}


			account = mObjectNames.AddOrUpdate(account);
			mObjectNames.SaveChanges();

			return CreatedAtRoute(DefaultApi,
				new { id = account.AccountID }, account);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PutAccount                                                            *
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// PUT: api/ObjectNames/5
		/// </summary>
		[ResponseType(typeof(void))]
		public IHttpActionResult PutAccount(int id, AccountItem account)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if(id != account.AccountID || !mObjectNames.Exists(id))
			{
				return BadRequest();
			}

			account = mObjectNames.AddOrUpdate(account);
			mObjectNames.SaveChanges();

			return StatusCode(HttpStatusCode.NoContent);
		}
		//*-----------------------------------------------------------------------*
		}
	}
}
