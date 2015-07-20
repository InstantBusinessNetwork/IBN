using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data.Sql;
using System.Data;

namespace Mediachase.Ibn.Lists
{
	/// <summary>
	/// Represents List Single Record request handler.
	/// </summary>
	public class ListObjectRequestHandler: BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Properties
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ListRecordRequestHandler"/> class.
		/// </summary>
		public ListObjectRequestHandler()
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

		#region Delete
		/// <summary>
		/// Pres the delete inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			#region Remove references from IBN 4.7 objects
			SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure,
				"bus_cls_ListObject_Delete",
				SqlHelper.SqlParameter("@ClassName", SqlDbType.NVarChar, 250, context.GetTargetMetaClassName()),
				SqlHelper.SqlParameter("@ObjectId", SqlDbType.VarChar, 36, context.GetTargetPrimaryKeyId().Value.ToString()));
			#endregion
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
