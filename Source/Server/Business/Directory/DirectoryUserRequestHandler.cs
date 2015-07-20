using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Business.Directory
{
	/// <summary>
	/// Represents DirectoryUser request handler.
	/// </summary>
	public class DirectoryUserRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryUserRequestHandler"/> class.
		/// </summary>
		public DirectoryUserRequestHandler()
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
			if (metaClassName == DirectoryUserEntity.ClassName)
			{
				DirectoryUserEntity retVal = new DirectoryUserEntity();
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
			DirectoryUserEntity user = (DirectoryUserEntity)context.Request.Target;

			if (string.IsNullOrEmpty(user.FullName))
			{
				user.FullName = user.Properties.GetValue<string>(DirectoryUserEntity.FieldLastName, string.Empty) + " " +
					user.Properties.GetValue<string>(DirectoryUserEntity.FieldFirstName, string.Empty) + " " +
					user.Properties.GetValue<string>(DirectoryUserEntity.FieldMiddleName, string.Empty);
			}
			#endregion
		}


		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);
		}

		protected override void PostCreateInsideTransaction(BusinessContext context)
		{
			base.PostCreateInsideTransaction(context);

			// Add User To Principal
			DirectoryManager.CreatePrincipal(DirectoryPrincipalType.User,
				((CreateResponse)context.Response).PrimaryKeyId,
				((DirectoryUserEntity)context.Request.Target).FullName);
		}
		#endregion

		#region Delete
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			// Remove User From Principal
			DirectoryManager.DeletePrincipal(context.GetTargetPrimaryKeyId().Value);
		}
		#endregion

		#region Update
		protected override void PostUpdateInsideTransaction(BusinessContext context)
		{
			base.PostUpdateInsideTransaction(context);

			// Update Principal
			if (context.Request.Target.Properties.Contains(DirectoryUserEntity.FieldFullName))
			{
				DirectoryManager.UpdatePrincipal(context.GetTargetPrimaryKeyId().Value,
					(string)context.Request.Target[DirectoryUserEntity.FieldFullName]);
			}
		}
		#endregion


		#endregion
	}
}
