using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Business.Documents
{
	/// <summary>
	/// Êepresents document request handler.
	/// </summary>
	public class DocumentTemplateRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentTemplateRequestHandler"/> class.
		/// </summary>
		public DocumentTemplateRequestHandler()
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
			if (metaClassName == DocumentTemplateEntity.GetAssignedMetaClassName())
			{
				DocumentTemplateEntity retVal = new DocumentTemplateEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region Create
		protected override void PreCreate(BusinessContext context)
		{
			base.PreCreate(context);
		}
		#endregion

		#region Delete
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			// Call Base method
			base.PreDeleteInsideTransaction(context);
		}
		#endregion

		#region CustomMethod
		/// <summary>
		/// Pres the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCustomMethod(BusinessContext context)
		{
			base.PreCustomMethod(context);
		}

		/// <summary>
		/// Pres the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PreCustomMethodInsideTransaction(context);
		}

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void CustomMethod(BusinessContext context)
		{
			base.CustomMethod(context);
		}

		/// <summary>
		/// Posts the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PostCustomMethodInsideTransaction(context);
		}

		/// <summary>
		/// Posts the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethod(BusinessContext context)
		{
			base.PostCustomMethod(context);
		}
		#endregion

		#endregion
	}

}
