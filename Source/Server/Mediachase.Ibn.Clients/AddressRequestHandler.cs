using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Clients
{
	/// <summary>
	/// Represents Address Request Handler.
	/// </summary>
	public class AddressRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="AddressRequestHandler"/> class.
		/// </summary>
		public AddressRequestHandler()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods

		#region UpdateAddressName
		internal static void UpdateAddressName(MetaObject address)
		{
			if (address == null)
				return;

			if (address.GetMetaType().Name != AddressEntity.GetAssignedMetaClassName())
				throw new ArgumentOutOfRangeException("The address object has wrong meta object type. Should be 'Address'.");

			address["Name"] = string.Join(";",
				new string[] {
					address.Properties["Line2"].GetValue<string>(string.Empty),
					address.Properties["Line1"].GetValue<string>(string.Empty),
					address.Properties["City"].GetValue<string>(string.Empty),
					address.Properties["Region"].GetValue<string>(string.Empty), 
					address.Properties["PostalCode"].GetValue<string>(string.Empty), 
					address.Properties["Country"].GetValue<string>(string.Empty)
				});
		}
		#endregion

		#region CopyEntityObjectToMetaObject
		protected override void CopyEntityObjectToMetaObject(EntityObject target, Mediachase.Ibn.Data.Meta.MetaObject metaObject)
		{
			base.CopyEntityObjectToMetaObject(target, metaObject);

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
			if (metaClassName == AddressEntity.GetAssignedMetaClassName())
			{
				AddressEntity retVal = new AddressEntity();
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

			AddressEntity address = (AddressEntity)context.Request.Target;

			// TODO: Not Update IsDefaultContactElement And IsDefaultOrganizationElement
			address.IsDefaultContactElement = false;
			address.IsDefaultOrganizationElement = false;
		}
		#endregion

		#region Delete
		protected override void PreDelete(BusinessContext context)
		{
			base.PreDelete(context);

			AddressEntity address = (AddressEntity)BusinessManager.Load(AddressEntity.GetAssignedMetaClassName(),
				context.GetTargetPrimaryKeyId().Value);

			if (!context.Request.Parameters.Contains(AddressRequestParameters.Delete_SkipDefaultAddressCheck) &&
				(address.IsDefaultOrganizationElement || address.IsDefaultContactElement))
				throw new DeleteDefaultAddressException();
		}
		#endregion

		#region SetDefaultAddress
		/// <summary>
		/// Pres the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PreSetDefaultAddress(BusinessContext context)
		{
		}

		/// <summary>
		/// Pres the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PreSetDefaultAddressInsideTransaction(BusinessContext context)
		{
		}

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void SetDefaultAddress(BusinessContext context)
		{
			// Load Address
			MetaObject newDefaultAddress = MetaObjectActivator.CreateInstance(AddressEntity.GetAssignedMetaClassName(),
				context.GetTargetPrimaryKeyId().Value);


			if (newDefaultAddress["ContactId"] != null)
			{
				// Find default addresses
				MetaObject[] defaultAddressList = MetaObject.List(DataContext.Current.GetMetaClass(AddressEntity.GetAssignedMetaClassName()),
					FilterElement.EqualElement("ContactId", newDefaultAddress["ContactId"]),
					FilterElement.EqualElement("IsDefaultContactElement", true));

				// Reset default addresses
				foreach (MetaObject defaultAddress in defaultAddressList)
				{
					defaultAddress["IsDefaultContactElement"] = false;
					defaultAddress.Save();
				}

				// Set default address
				newDefaultAddress["IsDefaultContactElement"] = true;
				newDefaultAddress.Save();
			}
			else if (newDefaultAddress["OrganizationId"] != null)
			{
				// Find default addresses
				MetaObject[] defaultAddressList = MetaObject.List(DataContext.Current.GetMetaClass(AddressEntity.GetAssignedMetaClassName()),
					FilterElement.EqualElement("OrganizationId", newDefaultAddress["OrganizationId"]),
					FilterElement.EqualElement("IsDefaultOrganizationElement", true));

				// Reset default addresses
				foreach (MetaObject defaultAddress in defaultAddressList)
				{
					defaultAddress["IsDefaultOrganizationElement"] = false;
					defaultAddress.Save();
				}

				// Set default address
				newDefaultAddress["IsDefaultOrganizationElement"] = true;
				newDefaultAddress.Save();
			}

			context.SetResponse(new Response());
		}

		/// <summary>
		/// Posts the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PostSetDefaultAddressInsideTransaction(BusinessContext context)
		{
		}

		/// <summary>
		/// Posts the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected virtual void PostSetDefaultAddress(BusinessContext context)
		{
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

			switch (context.GetMethod())
			{
				case AddressRequestMethod.SetDefaultAddress:
					PreSetDefaultAddress(context);
					break;
			}

		}

		/// <summary>
		/// Pres the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PreCustomMethodInsideTransaction(context);

			switch (context.GetMethod())
			{
				case AddressRequestMethod.SetDefaultAddress:
					PreSetDefaultAddressInsideTransaction(context);
					break;
			}
		}

		/// <summary>
		/// Customs the method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void CustomMethod(BusinessContext context)
		{
			base.CustomMethod(context);

			switch (context.GetMethod())
			{
				case AddressRequestMethod.SetDefaultAddress:
					SetDefaultAddress(context);
					break;
			}
		}

		/// <summary>
		/// Posts the custom method inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethodInsideTransaction(BusinessContext context)
		{
			base.PostCustomMethodInsideTransaction(context);

			switch (context.GetMethod())
			{
				case AddressRequestMethod.SetDefaultAddress:
					PostSetDefaultAddressInsideTransaction(context);
					break;
			}

		}

		/// <summary>
		/// Posts the custom method.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCustomMethod(BusinessContext context)
		{
			base.PostCustomMethod(context);

			switch (context.GetMethod())
			{
				case AddressRequestMethod.SetDefaultAddress:
					PostSetDefaultAddress(context);
					break;
			}

		}
		#endregion

		#endregion
	}
}
