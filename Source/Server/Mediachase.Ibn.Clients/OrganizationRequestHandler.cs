using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core.Business.Mapping;

namespace Mediachase.Ibn.Clients
{
	/// <summary>
	/// Represents Organization Request Handler.
	/// </summary>
	public class OrganizationRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationRequestHandler"/> class.
		/// </summary>
		public OrganizationRequestHandler()
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
			if (metaClassName == OrganizationEntity.GetAssignedMetaClassName())
			{
				OrganizationEntity retVal = new OrganizationEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region CopyEntityObjectToMetaObject
		protected override void CopyEntityObjectToMetaObject(EntityObject target, Mediachase.Ibn.Data.Meta.MetaObject metaObject)
		{
			base.CopyEntityObjectToMetaObject(target, metaObject);

			if (metaObject.GetMetaType().Name == AddressEntity.GetAssignedMetaClassName())
				AddressRequestHandler.UpdateAddressName(metaObject);
		} 
		#endregion

		#region Create
		protected override void PreCreate(BusinessContext context)
		{
			// TODO: Check CreateRequest parameters here

			base.PreCreate(context);
		}
		#endregion

		#region Delete
		protected override void  PreDeleteInsideTransaction(BusinessContext context)
		{
			// Call Base method
			base.PreDeleteInsideTransaction(context);

			#region Delete Assigned Addresses
			EntityObject[] addresses = BusinessManager.List(AddressEntity.GetAssignedMetaClassName(),
						new FilterElement[] { FilterElement.EqualElement("OrganizationId", context.GetTargetPrimaryKeyId()) });

			foreach (AddressEntity address in addresses)
			{
				DeleteRequest request = new DeleteRequest(address);
				request.Parameters.Add(AddressRequestParameters.Delete_SkipDefaultAddressCheck, null);

				BusinessManager.Execute(request);
			}
			#endregion

			#region Process RelatedContactAction
			// Read RelatedContactAction from request parameters
			RelatedContactAction relContactAction = context.Request.Parameters.GetValue<RelatedContactAction>(OrganizationRequestParameters.Delete_RelatedContactAction, RelatedContactAction.None);

			switch (relContactAction)
			{
				// Detach all assigned contacts
				case RelatedContactAction.Detach:
					EntityObject[] detachedContacts = BusinessManager.List(ContactEntity.GetAssignedMetaClassName(),
						new FilterElement[] { FilterElement.EqualElement("OrganizationId", context.GetTargetPrimaryKeyId()) });

					foreach (ContactEntity contact in detachedContacts)
					{
						contact.OrganizationId = null;

						BusinessManager.Update(contact);
					}
					break;
				// Delete all assigned actions
				case RelatedContactAction.Delete:
					EntityObject[] deletedContacts = BusinessManager.List(ContactEntity.GetAssignedMetaClassName(),
						new FilterElement[] { FilterElement.EqualElement("OrganizationId", context.GetTargetPrimaryKeyId()) });

					foreach (ContactEntity contact in deletedContacts)
					{
						BusinessManager.Execute(new DeleteRequest(contact));
					}
					break;
			} 
			#endregion

			#region Remove references from IBN 4.7 objects
			SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure,
				"bus_cls_Organization_Delete",
				SqlHelper.SqlParameter("@OrgUid", SqlDbType.UniqueIdentifier, context.GetTargetPrimaryKeyId().Value));
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

		#region List
		protected override void PreListInsideTransaction(BusinessContext context)
		{
			base.PreListInsideTransaction(context);

			// OZ 2008-11-11 Fix Organization view for ibn partners
			ListRequest request = (ListRequest)context.Request;

			Guid orgUid, contactUid;

			// Check  if (OrgUid!=null)   -> View Org with OrgUid only 
			if (PartnerUtil.GetClientInfo((int)DataContext.Current.CurrentUserId, out orgUid, out contactUid))
			{
				List<FilterElement> filters = new List<FilterElement>(request.Filters);

				filters.Add(FilterElement.EqualElement("PrimaryKeyId", orgUid));

				request.Filters = filters.ToArray();
			}
			//
		}
		#endregion

		#region InitializeMappingDocument
		/// <summary>
		/// Initializes the mapping document.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void InitializeMappingDocument(BusinessContext context)
		{
			InitializeMappingDocumentRequest request = ((InitializeMappingDocumentRequest)context.Request);

			MetaClass metaClass = DataContext.Current.GetMetaClass(context.GetTargetMetaClassName());
			List<PrimaryKeyId> primaryKeyIds = new List<PrimaryKeyId>();

			// Create Default Mapping 
			MappingDocument retVal = new MappingDocument();

			DataTable dataTable = request.Data.Tables[request.TableIndex];

			MappingElement mapping = new MappingElement(dataTable.TableName, metaClass.Name);
			retVal.Add(mapping);

			MappingElementBuilder builder = new MappingElementBuilder(retVal);

			//Call creation mapping document by saved patterns 
			DefaultMappingHelper.CreateMapingByPatternComparision(dataTable, metaClass, builder);

			// Write Response
			InitializeMappingDocumentResponse response = new InitializeMappingDocumentResponse();
			response.MappingDocument = retVal;

			context.SetResponse(response);
		}
		#endregion
		#endregion
	}
}
