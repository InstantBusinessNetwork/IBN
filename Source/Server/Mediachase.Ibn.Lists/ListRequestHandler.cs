using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Lists
{
	/// <summary>
	/// Represents List Info request handler.
	/// </summary>
	public class ListRequestHandler: BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ListInfoRequestHandler"/> class.
		/// </summary>
		public ListRequestHandler()
		{
		}
		#endregion

		#region Methods

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, Mediachase.Ibn.Data.PrimaryKeyId? primaryKeyId)
		{
			return base.CreateEntityObject(metaClassName, primaryKeyId);
		} 
		#endregion

		#region Create
		/// <summary>
		/// Creates the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void Create(BusinessContext context)
		{
			//base.Create(context);
			throw new NotImplementedException();
		}
		#endregion

		#region Delete
		/// <summary>
		/// Pres the delete inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);
		}


		/// <summary>
		/// Deletes the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void Delete(BusinessContext context)
		{
			// OZ: Comment base logic and call custom business logic ListManager.DeleteList
			//base.Delete(context);

			ListManager.DeleteList((int)context.GetTargetPrimaryKeyId().Value);

			context.SetResponse(new Response());
		}

		/// <summary>
		/// Posts the delete inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostDeleteInsideTransaction(BusinessContext context)
		{
			base.PostDeleteInsideTransaction(context);
		}
		#endregion

		#endregion

		
	}
}
