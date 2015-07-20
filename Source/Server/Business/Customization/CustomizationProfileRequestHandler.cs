using System;
using System.Collections.Generic;
using System.Text;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.WidgetEngine;
using System.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.Ibn.Business.Customization
{
	/// <summary>
	/// Represents CustomizationProfile request handler.
	/// </summary>
	public class CustomizationProfileRequestHandler : BusinessObjectRequestHandler
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="CustomizationProfileRequestHandler"/> class.
		/// </summary>
		public CustomizationProfileRequestHandler()
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
			if (metaClassName == CustomizationProfileEntity.ClassName)
			{
				CustomizationProfileEntity retVal = new CustomizationProfileEntity();
				retVal.PrimaryKeyId = primaryKeyId;
				return retVal;
			}

			return base.CreateEntityObject(metaClassName, primaryKeyId);
		}
		#endregion

		#region PreUpdateInsideTransaction
		/// <summary>
		/// Preupdates inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreUpdateInsideTransaction(BusinessContext context)
		{
			base.PreUpdateInsideTransaction(context);

			// Check that the value of WorkspacePersonalization property changed from True to False
			CustomizationProfileEntity entity = (CustomizationProfileEntity)BusinessManager.Load(CustomizationProfileEntity.ClassName, context.GetTargetPrimaryKeyId().Value);
			if (entity != null 
				&& (bool)entity.WorkspacePersonalization
				&& !(bool)context.Request.Target.Properties[CustomizationProfileEntity.FieldWorkspacePersonalization].Value)
			{
				int profileId = (int)context.GetTargetPrimaryKeyId().Value;

				List<string> users = new List<string>();

				if (profileId > 0)
				{
					// Get users by profile
					FilterElementCollection fec = new FilterElementCollection();
					fec.Add(FilterElement.EqualElement(CustomizationProfileUserEntity.FieldProfileId, profileId));

					foreach (CustomizationProfileUserEntity user in BusinessManager.List(CustomizationProfileUserEntity.ClassName, fec.ToArray()))
						users.Add(user.PrimaryKeyId.Value.ToString());
				}
				else // default profile
				{
					// 1. Get list all users 
					using (IDataReader reader = Mediachase.IBN.Business.User.GetListAll())
					{
						while (reader.Read())
							users.Add(reader["UserId"].ToString());
					}

					// 2. Exclude users with non-default profile
					EntityObject[] entityList = BusinessManager.List(CustomizationProfileUserEntity.ClassName, (new FilterElementCollection()).ToArray());
					foreach (CustomizationProfileUserEntity puEntity in entityList)
					{
						users.Remove(puEntity.PrincipalId.ToString());
					}
				}

				// Remove CustomPages for all users in Profile
				FilterElementCollection filters = new FilterElementCollection();

				OrBlockFilterElement orBlock = new OrBlockFilterElement();
				foreach (string userId in users)
					orBlock.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldUserId, userId));
				filters.Add(orBlock);

				foreach (EntityObject page in BusinessManager.List(CustomPageEntity.ClassName, filters.ToArray()))
					BusinessManager.Delete(page);
			}
		}
 		#endregion

		#region PreDeleteInsideTransaction
		/// <summary>
		/// Pres the delete inside transaction.
		/// </summary>
		/// <param name="context">The context.</param>
		protected override void PreDeleteInsideTransaction(BusinessContext context)
		{
			base.PreDeleteInsideTransaction(context);

			CustomizationProfileEntity entity = (CustomizationProfileEntity)BusinessManager.Load(CustomizationProfileEntity.ClassName, context.GetTargetPrimaryKeyId().Value);

			int profileId = (int)entity.PrimaryKeyId.Value;

			// Reset users to default profile
			if (profileId != -1)
			{
				EntityObject[] list = BusinessManager.List(CustomizationProfileUserEntity.ClassName,
					new FilterElementCollection(FilterElement.EqualElement(CustomizationProfileUserEntity.FieldProfileId, profileId)).ToArray());
				foreach (CustomizationProfileUserEntity userProfile in list)
				{
					string userId = userProfile.PrincipalId.ToString();
					BusinessManager.Delete(userProfile);
					DataCache.RemoveByUser(userId);
				}
			}

			// Delete profile info from cls_CustomizationItem 
			EntityObject[] customizationItems = BusinessManager.List(CustomizationItemEntity.ClassName,
				new FilterElementCollection(FilterElement.EqualElement(CustomizationItemEntity.FieldProfileId, profileId)).ToArray());
			foreach (EntityObject obj in customizationItems)
				BusinessManager.Delete(obj);

			// Delete profile info from cls_CustomPage 
			EntityObject[] customPageItems = BusinessManager.List(CustomPageEntity.ClassName,
				new FilterElementCollection(FilterElement.EqualElement(CustomPageEntity.FieldProfileId, profileId)).ToArray());
			foreach (EntityObject obj in customizationItems)
				BusinessManager.Delete(obj);

			// Delete history
			SqlHelper.ExecuteNonQuery(SqlContext.Current,
				System.Data.CommandType.StoredProcedure,
				"HistoryEntityDelete",
				SqlHelper.SqlParameter("@ClassName", SqlDbType.NVarChar, 250, CustomizationProfileEntity.ClassName),
				SqlHelper.SqlParameter("@ObjectId", SqlDbType.VarChar, 36, profileId.ToString()));
		} 
		#endregion
		#endregion
	}
}
