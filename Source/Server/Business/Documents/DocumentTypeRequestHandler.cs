using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.IBN.Business.Documents
{
	/// <summary>
	/// Êepresents document request handler.
	/// </summary>
	public class DocumentTypeRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentTypeRequestHandler"/> class.
		/// </summary>
		public DocumentTypeRequestHandler()
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
			if (metaClassName == DocumentTypeEntity.GetAssignedMetaClassName())
			{
				DocumentTypeEntity retVal = new DocumentTypeEntity();
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

			// TOD0 Check Document Type Name

			string cardName = context.Request.Target["Name"].ToString();

			if (!cardName.StartsWith("Document_"))
				throw new ArgumentException("Document Type Name should be start with Document_");
		}

		protected override void PostCreateInsideTransaction(BusinessContext context)
		{
			// Call Base method
			base.PostCreateInsideTransaction(context);

			#region Create a new Document Card
			PrimaryKeyId pkDocumentType = ((CreateResponse)context.Response).PrimaryKeyId;

			// Create a new Document Card
			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				// TODO: Check Card Name
				string cardName = context.Request.Target["Name"].ToString();
				string cardFriendlyName = context.Request.Target["FriendlyName"].ToString();
				string cardPluralName = cardFriendlyName;
				string tableName = "cls_Document_" + context.Request.Target["Name"].ToString();

				MetaClass newCard = DataContext.Current.MetaModel.CreateCardMetaClass(DataContext.Current.GetMetaClass(DocumentEntity.GetAssignedMetaClassName()),
					cardName, cardFriendlyName,
					cardPluralName, tableName);

				scope.SaveChanges();
			} 
			#endregion
		}
		#endregion

		#region Delete
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			// Call Base method
			base.PreDeleteInsideTransaction(context);

			#region Load Document Type
			// Load Document Type
			DocumentTypeEntity docType = (DocumentTypeEntity)BusinessManager.Load(DocumentTypeEntity.GetAssignedMetaClassName(),
				context.GetTargetPrimaryKeyId().Value);

			if (docType != null)
			{
				// Read Card Name
				string cardName = docType.Name;

				// Delete meta class
				using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
				{
					DataContext.Current.MetaModel.DeleteMetaClass(cardName);

					scope.SaveChanges();
				}
			} 
			#endregion
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
