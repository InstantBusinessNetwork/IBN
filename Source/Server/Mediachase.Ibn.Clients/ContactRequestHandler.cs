using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Core.Business.Mapping;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Resources;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Clients
{
	/// <summary>
	/// Represents Contact Request Handler.
	/// </summary>
	public class ContactRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="OrganizationRequestHandler"/> class.
		/// </summary>
		public ContactRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region CopyEntityObjectToMetaObject
		protected override void CopyEntityObjectToMetaObject(EntityObject target, Mediachase.Ibn.Data.Meta.MetaObject metaObject)
		{
			base.CopyEntityObjectToMetaObject(target, metaObject);

			if(metaObject.GetMetaType().Name == AddressEntity.GetAssignedMetaClassName())
				AddressRequestHandler.UpdateAddressName(metaObject);
		}
		#endregion

		#region CreateEntityObject
		/// <summary>
		/// Creates the entity object.
		/// </summary>
		/// <param name="metaClassName">Name of the meta class.</param>
		/// <param name="primaryKeyId">The primary key id.</param>
		/// <returns></returns>
		protected override EntityObject CreateEntityObject(string metaClassName, PrimaryKeyId? primaryKeyId)
		{
			if (metaClassName == ContactEntity.GetAssignedMetaClassName())
			{
				ContactEntity retVal = new ContactEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region Create
		/// <summary>
		/// Pres the create.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCreate(BusinessContext context)
		{
			base.PreCreate(context);

			#region Fill Contact FullName = LastName FirstName MiddleName
			ContactEntity contact = (ContactEntity)context.Request.Target;

			if (string.IsNullOrEmpty(contact.FullName))
			{
				contact.FullName = contact.Properties.GetValue<string>("LastName", string.Empty) + " " + 
					contact.Properties.GetValue<string>("FirstName", string.Empty) + " " +
					contact.Properties.GetValue<string>("MiddleName", string.Empty);
			}

			// OZ 2008-11-11 Partner can create contacts only if his group has organization
			Guid orgUid, contactUid;
			if (PartnerUtil.GetClientInfo((int)DataContext.Current.CurrentUserId, out orgUid, out contactUid))
			{
				if (orgUid == Guid.Empty)
					throw new AccessDeniedException();
				else
					contact.OrganizationId = new PrimaryKeyId(orgUid);
			}
			//

			#endregion
		} 
		#endregion

		#region Update
		protected override void PreUpdate(BusinessContext context)
		{
			base.PreUpdate(context);

			ContactEntity contact = (ContactEntity)context.Request.Target;

			// OZ 2008-11-11 Partner can update contacts only if his group has organization
			Guid orgUid, contactUid;
			if (PartnerUtil.GetClientInfo((int)DataContext.Current.CurrentUserId, out orgUid, out contactUid))
			{
				if(orgUid == Guid.Empty && contactUid == Guid.Empty)
					throw new AccessDeniedException();

				if (orgUid != Guid.Empty)
					contact.OrganizationId = new PrimaryKeyId(orgUid); 
			}
			//
		}
		#endregion

		#region Delete
		/// <summary>
		/// Pres the delete inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreDeleteInsideTransaction(BusinessContext context)
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

			#region Remove references from IBN 4.7 objects
			SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure,
				"bus_cls_Contact_Delete",
				SqlHelper.SqlParameter("@ContactUid", SqlDbType.UniqueIdentifier, context.GetTargetPrimaryKeyId().Value));
			#endregion
		}
		#endregion

		#region List
		protected override void PreListInsideTransaction(BusinessContext context)
		{
			base.PreListInsideTransaction(context);

			// OZ 2008-11-11 Fix Contact view for ibn partners
			ListRequest request = (ListRequest)context.Request;

			Guid orgUid, contactUid;

			// Check  

			if (PartnerUtil.GetClientInfo((int)DataContext.Current.CurrentUserId, out orgUid, out contactUid))
			{
				List<FilterElement> filters = new List<FilterElement>(request.Filters);

				// if (OrgUid!=null)   -> View all Contacts with OrgUid only 
				if (orgUid != Guid.Empty)
				{
					filters.Add(FilterElement.EqualElement("OrganizationId", orgUid));
				}
				// if (ContactUid!=null)   -> View Contact with ContactUid only 
				else if (contactUid != Guid.Empty)
				{
					filters.Add(FilterElement.EqualElement("PrimaryKeyId", contactUid));
				}
				// Nothing show
				else
				{
					filters.Add(FilterElement.IsNullElement("PrimaryKeyId"));
				}

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
