using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Business.Directory
{
	/// <summary>
	/// Represents DirectoryPrincipal request handler.
	/// </summary>
	public class DirectoryPrincipalRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryPrincipalRequestHandler"/> class.
		/// </summary>
		public DirectoryPrincipalRequestHandler()
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
			if (metaClassName == DirectoryPrincipalEntity.ClassName)
			{
				DirectoryPrincipalEntity retVal = new DirectoryPrincipalEntity();
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
		#endregion

		#endregion
	}
}
