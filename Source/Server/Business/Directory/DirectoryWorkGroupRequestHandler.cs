using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Business.Directory
{
	/// <summary>
	/// Represents DirectoryWorkGroup request handler.
	/// </summary>
	public class DirectoryWorkGroupRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryWorkGroupRequestHandler"/> class.
		/// </summary>
		public DirectoryWorkGroupRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == DirectoryWorkGroupEntity.ClassName)
			{
				DirectoryWorkGroupEntity retVal = new DirectoryWorkGroupEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region Create
		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);
		}

		protected override void PostCreateInsideTransaction(BusinessContext context)
		{
			base.PostCreateInsideTransaction(context);

			// Add SecureRole To Principal
			DirectoryManager.CreatePrincipal(DirectoryPrincipalType.WorkGroup,
				((CreateResponse)context.Response).PrimaryKeyId,
				((DirectoryWorkGroupEntity)context.Request.Target).Name);
		}
		#endregion

		#region Delete
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			// Remove SecureRole From Principal
			DirectoryManager.DeletePrincipal(context.GetTargetPrimaryKeyId().Value);
		}
		#endregion

		#region Update
		protected override void  PostUpdateInsideTransaction(BusinessContext context)
		{
			base.PostUpdateInsideTransaction(context);

			// Update Principal
			if(context.Request.Target.Properties.Contains(DirectoryWorkGroupEntity.FieldName))
			{
				DirectoryManager.UpdatePrincipal(context.GetTargetPrimaryKeyId().Value,
					(string)context.Request.Target[DirectoryWorkGroupEntity.FieldName]);
			}
		}
		#endregion


		#endregion
	}
}
