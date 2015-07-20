using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Mediachase.Ibn.Business.Customization;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business.WidgetEngine
{
	public sealed class CustomPageManager
	{
		#region GetCustomPage
		/// <summary>
		/// Gets the custom page.
		/// </summary>
		/// <param name="uid">The page uid.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="userId">The user id.</param>
		/// <returns></returns>
		public static CustomPageEntity GetCustomPage(Guid uid, int? profileId, int? userId)
		{
			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(FilterElement.EqualElement(CustomPageEntity.FieldUid, uid));

			if (userId.HasValue)	// User layer
			{
				if (!profileId.HasValue)
					profileId = ProfileManager.GetProfileIdByUser(userId.Value);

				// ProfileId is null AND UserId is null
				AndBlockFilterElement andBlock1 = new AndBlockFilterElement();
				andBlock1.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
				andBlock1.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));

				// ProfileId = value AND UserId is null
				AndBlockFilterElement andBlock2 = new AndBlockFilterElement();
				andBlock2.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldProfileId, profileId.Value));
				andBlock2.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));

				// ProfileId is null AND UserId = value
				AndBlockFilterElement andBlock3 = new AndBlockFilterElement();
				andBlock3.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
				andBlock3.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldUserId, userId.Value));

				OrBlockFilterElement orBlock = new OrBlockFilterElement();
				orBlock.ChildElements.Add(andBlock1);
				orBlock.ChildElements.Add(andBlock2);
				orBlock.ChildElements.Add(andBlock3);

				filters.Add(orBlock);
			}
			else if (profileId.HasValue)	// Profile layer
			{
				filters.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));

				OrBlockFilterElement orBlock = new OrBlockFilterElement();
				orBlock.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldProfileId, profileId.Value));
				orBlock.ChildElements.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
				filters.Add(orBlock);
			}
			else // Site layer
			{
				filters.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
				filters.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));
			}

			EntityObject[] pages = BusinessManager.List(CustomPageEntity.ClassName, filters.ToArray());

			CustomPageEntity retval = null;
			if (pages.Length > 0)
				retval = (CustomPageEntity)pages[0];

			return retval;
		}
		#endregion

		#region UpdateCustomPage
		/// <summary>
		/// Updates the custom page.
		/// </summary>
		/// <param name="uid">The page uid.</param>
		/// <param name="jsonData">The json data.</param>
		/// <param name="templateUid">The template uid.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="userId">The user id.</param>
		public static void UpdateCustomPage(Guid uid, string jsonData, Guid templateUid, int? profileId, int? userId)
		{
			CustomPageEntity page = GetCustomPage(uid, profileId, userId);

			if (page == null)
				throw new ArgumentException("The page is not found", "uid");

			CustomPageEntity updatedPage = page;
			bool isNew = false;

			if (userId.HasValue)
			{
				if (!(page.UserId.HasValue && page.UserId.Value == userId.Value)) // create new user page
				{
					updatedPage = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
					updatedPage.UserId = userId.Value;
					isNew = true;
				}
			}
			else if (profileId.HasValue)
			{
				if (!(page.ProfileId.HasValue && page.ProfileId.Value == profileId.Value))	// create new profile page
				{
					updatedPage = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
					updatedPage.ProfileId = profileId.Value;
					isNew = true;
				}
			}
			else
			{
				if (page.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName)) // create new site page
				{
					updatedPage = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
					isNew = true;
				}
			}

			updatedPage.TemplateId = templateUid;
			updatedPage.JsonData = jsonData;

			if (isNew)
			{
				updatedPage.Uid = page.Uid;
				updatedPage.Title = page.Title;
				updatedPage.Description = page.Description;
				updatedPage.Icon = page.Icon;
				updatedPage.PropertyJsonData = page.PropertyJsonData;

				BusinessManager.Create(updatedPage);
			}
			else
			{
				BusinessManager.Update(updatedPage);
			}
		}
		#endregion

		#region UpdateCustomPageProperty
		/// <summary>
		/// Updates the custom page property.
		/// </summary>
		/// <param name="uid">The uid.</param>
		/// <param name="propertyJsonData">The property json data.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="userId">The user id.</param>
		public static void UpdateCustomPageProperty(Guid uid, string propertyJsonData, int? profileId, int? userId)
		{
			CustomPageEntity page = GetCustomPage(uid, profileId, userId);

			if (page == null)
				throw new ArgumentException("The page is not found", "uid");

			CustomPageEntity updatedPage = page;
			bool isNew = false;

			if (userId.HasValue)
			{
				if (!(page.UserId.HasValue && page.UserId.Value == userId.Value)) // create new user page
				{
					updatedPage = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
					updatedPage.UserId = userId.Value;
					isNew = true;
				}
			}
			else if (profileId.HasValue)
			{
				if (!(page.ProfileId.HasValue && page.ProfileId.Value == profileId.Value))	// create new profile page
				{
					updatedPage = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
					updatedPage.ProfileId = profileId.Value;
					isNew = true;
				}
			}
			else
			{
				if (page.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName)) // create new site page
				{
					updatedPage = BusinessManager.InitializeEntity<CustomPageEntity>(CustomPageEntity.ClassName);
					isNew = true;
				}
			}

			updatedPage.PropertyJsonData = propertyJsonData;

			if (isNew)
			{
				updatedPage.Uid = page.Uid;
				updatedPage.Title = page.Title;
				updatedPage.Description = page.Description;
				updatedPage.Icon = page.Icon;
				updatedPage.TemplateId = page.TemplateId;
				updatedPage.JsonData = page.JsonData;

				BusinessManager.Create(updatedPage);
			}
			else
			{
				BusinessManager.Update(updatedPage);
			}
		}
		#endregion

		#region ResetCustomPage
		/// <summary>
		/// Resets the custom page.
		/// </summary>
		/// <param name="uid">The page uid.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="userId">The user id.</param>
		public static void ResetCustomPage(Guid uid, int? profileId, int? userId)
		{
			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(FilterElement.EqualElement(CustomPageEntity.FieldUid, uid));

			if (userId.HasValue)	// User layer
			{
				filters.Add(FilterElement.EqualElement(CustomPageEntity.FieldUserId, userId.Value));
			}
			else if (profileId.HasValue)	// Profile layer
			{
				filters.Add(FilterElement.EqualElement(CustomPageEntity.FieldProfileId, profileId.Value));
			}
			else // Site layer
			{
				filters.Add(FilterElement.IsNullElement(CustomPageEntity.FieldProfileId));
				filters.Add(FilterElement.IsNullElement(CustomPageEntity.FieldUserId));
			}

			foreach (CustomPageEntity page in BusinessManager.List(CustomPageEntity.ClassName, filters.ToArray()))
			{
				if (!page.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName))
					BusinessManager.Delete(page);
			}
		} 
		#endregion

		#region ResetUserSettingsByProfile
		/// <summary>
		/// Resets the user settings by profile.
		/// </summary>
		/// <param name="uid">The page uid.</param>
		/// <param name="profileId">The profile id.</param>
		public static void ResetUserSettingsByProfile(Guid uid, int profileId)
		{
			List<string> users = new List<string>();

			if (profileId > 0)
			{
				FilterElementCollection fec = new FilterElementCollection();
				fec.Add(FilterElement.EqualElement(CustomizationProfileUserEntity.FieldProfileId, profileId));

				foreach (CustomizationProfileUserEntity entity in BusinessManager.List(CustomizationProfileUserEntity.ClassName, fec.ToArray()))
					users.Add(entity.PrincipalId.ToString());
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

			// O.R. [2010-10-05]. Don't process profile without users
			if (users.Count > 0)
			{
				// Remove CustomPages for all users in Profile
				FilterElementCollection filters = new FilterElementCollection();
				filters.Add(FilterElement.EqualElement(CustomPageEntity.FieldUid, uid));

				OrBlockFilterElement orBlock = new OrBlockFilterElement();
				foreach (string userId in users)
					orBlock.ChildElements.Add(FilterElement.EqualElement(CustomPageEntity.FieldUserId, userId));
				filters.Add(orBlock);

				// Skip CustomPageNormalizationPlugin
				ListRequest request = new ListRequest(CustomPageEntity.ClassName, filters.ToArray());
				request.Parameters.Add("CustomPageNormalizationPlugin", false);
				ListResponse response = (ListResponse)BusinessManager.Execute(request);

				foreach (EntityObject page in response.EntityObjects)
					BusinessManager.Delete(page);
			}
		} 
		#endregion

		#region DeleteCustomPage
		/// <summary>
		/// Deletes the custom page.
		/// </summary>
		/// <param name="uid">The uid.</param>
		public static void DeleteCustomPage(Guid uid)
		{
			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(FilterElement.EqualElement(CustomPageEntity.FieldUid, uid));

			ListRequest request = new ListRequest(CustomPageEntity.ClassName, filters.ToArray());
			request.Parameters.Add("CustomPageNormalizationPlugin", false);
			ListResponse response = (ListResponse)BusinessManager.Execute(request);

			foreach (CustomPageEntity page in response.EntityObjects)
			{
				if (!page.Properties.Contains(LocalDiskEntityObjectPlugin.IsLoadDiskEntityPropertyName))
					BusinessManager.Delete(page);
			}
		}
		#endregion
	}
}
