using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Business.Customization
{
	/// <summary>
	/// Represents CustomizationProfileUser request handler.
	/// </summary>
	public class CustomizationProfileUserRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CustomizationProfileUserRequestHandler"/> class.
		/// </summary>
		public CustomizationProfileUserRequestHandler()
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
			if (metaClassName == CustomizationProfileUserEntity.ClassName)
			{
				CustomizationProfileUserEntity retVal = new CustomizationProfileUserEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region PreCreateInsideTransaction
		protected override void PreCreateInsideTransaction(BusinessContext context)
		{
			base.PreCreateInsideTransaction(context);

			CustomizationProfileUserEntity newEntity = (CustomizationProfileUserEntity)context.Request.Target;

			// OZ: нужна уникальность по PrincipalId.
			foreach (EntityObject item in BusinessManager.List(CustomizationProfileUserEntity.ClassName,
				new FilterElement[] { FilterElement.EqualElement(CustomizationProfileUserEntity.FieldPrincipalId, newEntity.PrincipalId) }))
			{
				BusinessManager.Delete(item);
			}
		}
		#endregion

		#region PreDeleteInsideTransaction
		/// <summary>
		/// Predelete inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			CustomizationProfileUserEntity entity = (CustomizationProfileUserEntity)BusinessManager.Load(CustomizationProfileUserEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			if (entity != null)
			{
				DataCache.RemoveByUser(entity.PrincipalId.ToString());
			}
		}
		#endregion

		#region PostCreateInsideTransaction
		/// <summary>
		/// Postcreate inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PostCreateInsideTransaction(BusinessContext context)
		{
			base.PostCreateInsideTransaction(context);

			int userId = (int)(PrimaryKeyId)context.Request.Target.Properties[CustomizationProfileUserEntity.FieldPrincipalId].Value;
			// New profile defines new Left Menu so we should empty cache.
			DataCache.RemoveByUser(userId.ToString());

			// Check WorkspacePersonalization flag in profile and clear user settings for dashboards if the value is False.
			CustomizationProfileUserEntity profileUserEntity = (CustomizationProfileUserEntity)context.Request.Target;
			PrimaryKeyId profileId = profileUserEntity.ProfileId;
			CustomizationProfileEntity profileEntity = (CustomizationProfileEntity)BusinessManager.Load(CustomizationProfileEntity.ClassName, profileId);
			if (!profileEntity.WorkspacePersonalization)
			{
				FilterElementCollection filters = new FilterElementCollection();
				filters.Add(FilterElement.EqualElement(CustomPageEntity.FieldUserId, userId));

				foreach (EntityObject page in BusinessManager.List(CustomPageEntity.ClassName, filters.ToArray()))
					BusinessManager.Delete(page);
			}
		}
		#endregion

		#region Create
		/// <summary>
		/// Creates the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void Create(BusinessContext context)
		{
			CustomizationProfileUserEntity newEntity = (CustomizationProfileUserEntity)context.Request.Target;
			if (newEntity.ProfileId > 0)
			{
				base.Create(context);
			}
			else  // for default profile we don't need to create a new record
			{
				context.SetResponse(new CreateResponse(PrimaryKeyId.Empty));
			}
		}
		#endregion
		#endregion
	}
}
