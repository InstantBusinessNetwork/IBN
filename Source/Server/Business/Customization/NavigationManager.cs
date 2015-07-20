using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using System.Xml.XPath;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.XmlTools;

namespace Mediachase.Ibn.Business.Customization
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class NavigationManager
	{
		#region CONST
		public const string ItemArgumentText = "text";
		public const string ItemArgumentOrder = "order";
		public const string ItemArgumentIconUrl = "iconurl";
		public const string ItemArgumentCommand = "command";
		public const string ItemArgumentCommandType = "commandtype";
		public const string ItemArgumentTarget = "target";
		public const string ItemArgumentUrl = "url";
		public const string ItemArgumentEnableHandler = "enablehandler";
		public const string ItemArgumentParams = "params";

		public const string CustomizationLayerGlobal = "global";
		public const string CustomizationLayerProfile = "profile";
		public const string CustomizationLayerUser = "user"; 
		#endregion

		private NavigationManager()
		{
		}

		// ------------------- Public methods -------------------
		#region AddNavigationItem
		/// <summary>
		/// Adds the navigation Item.
		/// </summary>
		/// <param name="parentFullId">The parent full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="url">The URL.</param>
		public static void AddNavigationItem(string parentFullId, int order, string text, string url)
		{
			AddNavigationItem(parentFullId, order, text, url, null, null);
		}

		/// <summary>
		/// Adds the navigation item.
		/// </summary>
		/// <param name="parentFullId">The parent full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="url">The URL.</param>
		/// <param name="profileId">The profile id.</param>
		public static void AddNavigationItem(string parentFullId, int order, string text, string url, PrimaryKeyId? profileId)
		{
			AddNavigationItem(parentFullId, order, text, url, profileId, null);
		}
		
		/// <summary>
		/// Adds the navigation item.
		/// </summary>
		/// <param name="parentFullId">The parent full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="url">The URL.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void AddNavigationItem(string parentFullId, int order, string text, string url, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			using (TransactionScope transaction = DataContext.Current.BeginTransaction())
			{
				// Create item
				CustomizationItemEntity item = CreateCustomizationItemByParent(parentFullId, CustomizationStructureType.NavigationMenu, ItemCommandType.Add, profileId, principalId);

				CreateCustomizationItemArgument(item.PrimaryKeyId.Value, ItemArgumentText, text);
				CreateCustomizationItemArgument(item.PrimaryKeyId.Value, ItemArgumentOrder, order.ToString());
				CreateCustomizationItemArgument(item.PrimaryKeyId.Value, ItemArgumentCommand, Guid.NewGuid().ToString("D"));
				if (!string.IsNullOrEmpty(url))
					CreateCustomizationItemArgument(item.PrimaryKeyId.Value, ItemArgumentUrl, url);

				transaction.Commit();
			}

			ClearCache(profileId, principalId);
		}
		#endregion

		#region AddNavigationItemForList
		public static void AddNavigationItemForList(string parentFullId, int order, string text, string url, int listId)
		{
			using (TransactionScope transaction = DataContext.Current.BeginTransaction())
			{
				// Create item
				CustomizationItemEntity item = CreateCustomizationItemByParent(parentFullId, CustomizationStructureType.NavigationMenu, ItemCommandType.Add, null, null);
				PrimaryKeyId itemId = item.PrimaryKeyId.Value;

				CreateCustomizationItemArgument(itemId, ItemArgumentText, text);
				CreateCustomizationItemArgument(itemId, ItemArgumentOrder, order.ToString());
				CreateCustomizationItemArgument(itemId, ItemArgumentCommand, Guid.NewGuid().ToString("D"));
				CreateCustomizationItemArgument(itemId, ItemArgumentEnableHandler, "Mediachase.Ibn.Web.UI.ListApp.CommandHandlers.CanReadList, Mediachase.UI.Web");
				CreateCustomizationItemArgument(itemId, ItemArgumentParams, listId.ToString());
				if (!string.IsNullOrEmpty(url))
					CreateCustomizationItemArgument(itemId, ItemArgumentUrl, url);

				transaction.Commit();
			}

			ClearCache(null, null);
		} 
		#endregion

		#region UpdateNavigationItem
		/// <summary>
		/// Updates the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="url">The URL.</param>
		public static void UpdateNavigationItem(string fullId, int order, string text, string url)
		{
			UpdateNavigationItem(fullId, order, text, url, null, null);
		}

		/// <summary>
		/// Updates the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="url">The URL.</param>
		/// <param name="profileId">The profile id.</param>
		public static void UpdateNavigationItem(string fullId, int order, string text, string url, PrimaryKeyId? profileId)
		{
			UpdateNavigationItem(fullId, order, text, url, profileId, null);
		}

		/// <summary>
		/// Updates the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="url">The URL.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void UpdateNavigationItem(string fullId, int order, string text, string url, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity item = GetCustomizationItem(fullId, CustomizationStructureType.NavigationMenu, ItemCommandType.Add, profileId, principalId);
			if (item != null)
			{
				PrimaryKeyId itemId = item.PrimaryKeyId.Value;
				Dictionary<string, CustomizationItemArgumentEntity> arguments = GetCustomizationItemArguments(itemId);

				using (TransactionScope transaction = DataContext.Current.BeginTransaction())
				{
					// Text
					if (arguments.ContainsKey(ItemArgumentText.ToLowerInvariant()))
						UpdateCustomizationItemArgument(arguments[ItemArgumentText.ToLowerInvariant()], text);
					else
						CreateCustomizationItemArgument(itemId, ItemArgumentText, text);

					// Order
					if (arguments.ContainsKey(ItemArgumentOrder.ToLowerInvariant()))
						UpdateCustomizationItemArgument(arguments[ItemArgumentOrder.ToLowerInvariant()], order.ToString());
					else
						CreateCustomizationItemArgument(itemId, ItemArgumentOrder, order.ToString());

					// Url
					if (arguments.ContainsKey(ItemArgumentUrl.ToLowerInvariant()))
						UpdateCustomizationItemArgument(arguments[ItemArgumentUrl.ToLowerInvariant()], url);
					else
						CreateCustomizationItemArgument(itemId, ItemArgumentUrl, url);

					transaction.Commit();
				}
			}

			ClearCache(profileId, principalId);
 		}
		#endregion

		#region DeleteNavigationItem
		/// <summary>
		/// Deletes the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		public static void DeleteNavigationItem(string fullId)
		{
			DeleteNavigationItem(fullId, null, null);
		}

		/// <summary>
		/// Deletes the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="profileId">The profile id.</param>
		public static void DeleteNavigationItem(string fullId, PrimaryKeyId? profileId)
		{
			DeleteNavigationItem(fullId, profileId, null);
		}

		/// <summary>
		/// Deletes the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void DeleteNavigationItem(string fullId, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(new FilterElement(CustomizationItemEntity.FieldXmlFullId, FilterElementType.Like, fullId + "%"));
			filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldStructureType, (int)CustomizationStructureType.NavigationMenu));
			filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldCommandType, (int)ItemCommandType.Add));

			// delete user level only 
			if (principalId.HasValue)
			{
				filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldPrincipalId, principalId.Value));
			}
			// delete profile and user level
			else if (profileId.HasValue)
			{
				OrBlockFilterElement block = new OrBlockFilterElement();
				block.ChildElements.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldProfileId, profileId.Value));
				block.ChildElements.Add(FilterElement.IsNotNullElement(CustomizationItemEntity.FieldPrincipalId));
				filters.Add(block);
			}
			// else delete all - we don't use filter in that case

			using (TransactionScope transaction = DataContext.Current.BeginTransaction())
			{
				foreach (EntityObject item in BusinessManager.List(CustomizationItemEntity.ClassName, filters.ToArray()))
				{
					BusinessManager.Delete(item);
				}

				transaction.Commit();
			}

			ClearCache(profileId, principalId);
		}
		#endregion

		#region ModifyNavigationItem
		/// <summary>
		/// Modifies the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		public static void ModifyNavigationItem(string fullId, int order, string text)
		{
			ModifyNavigationItem(fullId, order, text, null, null);
		}

		/// <summary>
		/// Modifies the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="profileId">The profile id.</param>
		public static void ModifyNavigationItem(string fullId, int order, string text, PrimaryKeyId? profileId)
		{
			ModifyNavigationItem(fullId, order, text, profileId, null);
		}

		/// <summary>
		/// Modifies the navigation item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="order">The order.</param>
		/// <param name="text">The text.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void ModifyNavigationItem(string fullId, int order, string text, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity item = GetCustomizationItem(fullId, CustomizationStructureType.NavigationMenu, ItemCommandType.Update, profileId, principalId);
			if (item == null) // add
			{
				using (TransactionScope transaction = DataContext.Current.BeginTransaction())
				{
					// Create item
					item = CreateCustomizationItem(fullId, CustomizationStructureType.NavigationMenu, ItemCommandType.Update, profileId, principalId);

					// Create arguments
					CreateCustomizationItemArgument(item.PrimaryKeyId.Value, ItemArgumentText, text);
					CreateCustomizationItemArgument(item.PrimaryKeyId.Value, ItemArgumentOrder, order.ToString());

					transaction.Commit();
				}
			}
			else // update
			{
				PrimaryKeyId itemId = item.PrimaryKeyId.Value;
				Dictionary<string, CustomizationItemArgumentEntity> arguments = GetCustomizationItemArguments(itemId);

				using (TransactionScope transaction = DataContext.Current.BeginTransaction())
				{
					// Text
					if (arguments.ContainsKey(ItemArgumentText.ToLowerInvariant()))
						UpdateCustomizationItemArgument(arguments[ItemArgumentText.ToLowerInvariant()], text);
					else
						CreateCustomizationItemArgument(itemId, ItemArgumentText, text);

					// Order
					if (arguments.ContainsKey(ItemArgumentOrder.ToLowerInvariant()))
						UpdateCustomizationItemArgument(arguments[ItemArgumentOrder.ToLowerInvariant()], order.ToString());
					else
						CreateCustomizationItemArgument(itemId, ItemArgumentOrder, order.ToString());

					transaction.Commit();
				}
			}

			ClearCache(profileId, principalId);
		}
		#endregion

		#region UndoModifyNavigationItem
		/// <summary>
		/// Undoes the modify navigation item.
		/// </summary>
		/// <param name="xmlFullId">The XML full id.</param>
		public static void UndoModifyNavigationItem(string xmlFullId)
		{
			UndoModifyNavigationItem(xmlFullId, null, null);
		}

		/// <summary>
		/// Undoes the modify navigation item.
		/// </summary>
		/// <param name="xmlFullId">The XML full id.</param>
		/// <param name="profileId">The profile id.</param>
		public static void UndoModifyNavigationItem(string xmlFullId, PrimaryKeyId? profileId)
		{
			UndoModifyNavigationItem(xmlFullId, profileId, null);
		}

		/// <summary>
		/// Undoes the modify navigation item.
		/// </summary>
		/// <param name="xmlFullId">The XML full id.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void UndoModifyNavigationItem(string xmlFullId, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity item = GetCustomizationItem(xmlFullId, CustomizationStructureType.NavigationMenu, ItemCommandType.Update, profileId, principalId);
			if (item != null)
			{
				BusinessManager.Delete(item);

				ClearCache(profileId, principalId);
			}
		}
 		#endregion

		#region HideCustomizationItem
		/// <summary>
		/// Hides the customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		public static void HideCustomizationItem(string fullId, CustomizationStructureType structureType)
		{
			HideCustomizationItem(fullId, structureType, null, null);
		}

		/// <summary>
		/// Hides the customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="profileId">The profile id.</param>
		public static void HideCustomizationItem(string fullId, CustomizationStructureType structureType, PrimaryKeyId? profileId)
		{
			HideCustomizationItem(fullId, structureType, profileId, null);
		}

		/// <summary>
		/// Hides the customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void HideCustomizationItem(string fullId, CustomizationStructureType structureType, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity item = GetCustomizationItem(fullId, structureType, ItemCommandType.Remove, profileId, principalId);
			if (item == null)
			{
				CreateCustomizationItem(fullId, structureType, ItemCommandType.Remove, profileId, principalId);

				ClearCache(profileId, principalId);
			}
		}
		#endregion

		#region ShowCustomizationItem
		/// <summary>
		/// Shows the hidden customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		public static void ShowCustomizationItem(string fullId, CustomizationStructureType structureType)
		{
			ShowCustomizationItem(fullId, structureType, null, null);
		}

		/// <summary>
		/// Shows the hidden customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="profileId">The profile id.</param>
		public static void ShowCustomizationItem(string fullId, CustomizationStructureType structureType, PrimaryKeyId? profileId)
		{
			ShowCustomizationItem(fullId, structureType, profileId, null);
		}

		/// <summary>
		/// Shows the hidden customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		public static void ShowCustomizationItem(string fullId, CustomizationStructureType structureType, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity item = GetCustomizationItem(fullId, structureType, ItemCommandType.Remove, profileId, principalId);
			if (item != null)
			{
				BusinessManager.Delete(item);

				ClearCache(profileId, principalId);
			}
		}
		#endregion

		#region GetCustomizationItemArguments
		/// <summary>
		/// Gets the customization item arguments.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <returns></returns>
		public static Dictionary<string, CustomizationItemArgumentEntity> GetCustomizationItemArguments(string fullId, CustomizationStructureType structureType, ItemCommandType commandType)
		{
			return GetCustomizationItemArguments(fullId, structureType, commandType, null, null);
		}

		/// <summary>
		/// Gets the customization item arguments.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="profileId">The profile id.</param>
		/// <returns></returns>
		public static Dictionary<string, CustomizationItemArgumentEntity> GetCustomizationItemArguments(string fullId, CustomizationStructureType structureType, ItemCommandType commandType, PrimaryKeyId? profileId)
		{
			return GetCustomizationItemArguments(fullId, structureType, commandType, profileId, null);
		}

		/// <summary>
		/// Gets the customization item arguments.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		public static Dictionary<string, CustomizationItemArgumentEntity> GetCustomizationItemArguments(string fullId, CustomizationStructureType structureType, ItemCommandType commandType, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity item = GetCustomizationItem(fullId, structureType, commandType, profileId, principalId);

			if (item != null && item.PrimaryKeyId.HasValue)
			{
				return GetCustomizationItemArguments(item.PrimaryKeyId.Value);
			}

			return null;
		}

		/// <summary>
		/// Gets the customization item arguments.
		/// </summary>
		/// <param name="itemId">The item id.</param>
		/// <returns></returns>
		public static Dictionary<string, CustomizationItemArgumentEntity> GetCustomizationItemArguments(PrimaryKeyId itemId)
		{
			Dictionary<string, CustomizationItemArgumentEntity> arguments = new Dictionary<string, CustomizationItemArgumentEntity>();

			FilterElement[] argumentFilters = { FilterElement.EqualElement(CustomizationItemArgumentEntity.FieldItemId, itemId) };

			foreach (CustomizationItemArgumentEntity arg in BusinessManager.List(CustomizationItemArgumentEntity.ClassName, argumentFilters))
			{
				arguments.Add(arg.Name.ToLowerInvariant(), arg);
			}

			return arguments;
		}
		#endregion

		// ------------------- Private methods -------------------
		#region CreateCustomizationItemByParent(string parentFullId, CustomizationStructureType structureType, ItemCommandType commandType, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		/// <summary>
		/// Creates the navigation item by parent.
		/// </summary>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="parentFullId">The parent full id.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		private static CustomizationItemEntity CreateCustomizationItemByParent(string parentFullId, CustomizationStructureType structureType, ItemCommandType commandType, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			string fullId = string.Concat(parentFullId, "/", Guid.NewGuid().ToString("D"));
			return CreateCustomizationItem(fullId, structureType, commandType, profileId, principalId);
		}
		#endregion
		#region CreateCustomizationItem(CustomizationStructureType structureType, string parentFullId, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		/// <summary>
		/// Creates the navigation item.
		/// </summary>
		/// <param name="structureType">Type of the structure.</param>
		/// <param name="fullId">The full id.</param>
		/// <param name="commandType">Type of the command.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		private static CustomizationItemEntity CreateCustomizationItem(string fullId, CustomizationStructureType structureType, ItemCommandType commandType, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity item = BusinessManager.InitializeEntity<CustomizationItemEntity>(CustomizationItemEntity.ClassName);

			if (structureType != CustomizationStructureType.Undefined)
			{
				item.StructureType = (int)structureType;
			}
			item.XmlFullId = fullId;
			if (profileId.HasValue)
				item.Properties["ProfileId"].Value = profileId;
			if (principalId.HasValue)
				item.Properties["PrincipalId"].Value = principalId;

			item.CommandType = (int)commandType;

			item.PrimaryKeyId = BusinessManager.Create(item);

			return item;
		}
		#endregion

		#region GetCustomizationItem
		/// <summary>
		/// Gets the customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <returns></returns>
		private static CustomizationItemEntity GetCustomizationItem(string fullId, CustomizationStructureType structureType, ItemCommandType commandType)
		{
			return GetCustomizationItem(fullId, structureType, commandType, null, null);
		}

		/// <summary>
		/// Gets the customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="profileId">The profile id.</param>
		/// <returns></returns>
		private static CustomizationItemEntity GetCustomizationItem(string fullId, CustomizationStructureType structureType, ItemCommandType commandType, PrimaryKeyId? profileId)
		{
			return GetCustomizationItem(fullId, structureType, commandType, profileId, null);
		}

		/// <summary>
		/// Gets the customization item.
		/// </summary>
		/// <param name="fullId">The full id.</param>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		/// <returns></returns>
		private static CustomizationItemEntity GetCustomizationItem(string fullId, CustomizationStructureType structureType, ItemCommandType commandType, PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			CustomizationItemEntity result = null;

			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldXmlFullId, fullId));
			filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldStructureType, (int)structureType));
			filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldCommandType, (int)commandType));

			// Filter by profile
			if (profileId.HasValue)
				filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldProfileId, profileId.Value));
			else
				filters.Add(FilterElement.IsNullElement(CustomizationItemEntity.FieldProfileId));

			// Filter by principal
			if (principalId.HasValue)
				filters.Add(FilterElement.EqualElement(CustomizationItemEntity.FieldPrincipalId, principalId.Value));
			else
				filters.Add(FilterElement.IsNullElement(CustomizationItemEntity.FieldPrincipalId));

			EntityObject[] items = BusinessManager.List(CustomizationItemEntity.ClassName, filters.ToArray());

			if (items != null && items.Length > 0)
			{
				result = items[0] as CustomizationItemEntity;
			}

			return result;
		}
		#endregion

		#region CreateCustomizationItemArgument(PrimaryKeyId itemId, string name, string value)
		/// <summary>
		/// Creates the navigation item argument.
		/// </summary>
		/// <param name="itemId">The item id.</param>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private static CustomizationItemArgumentEntity CreateCustomizationItemArgument(PrimaryKeyId itemId, string name, string value)
		{
			CustomizationItemArgumentEntity argument = BusinessManager.InitializeEntity<CustomizationItemArgumentEntity>(CustomizationItemArgumentEntity.ClassName);

			argument.ItemId = itemId;
			argument.Name = name;
			argument.Value = value;

			argument.PrimaryKeyId = BusinessManager.Create(argument);

			return argument;
		}
		#endregion
		#region UpdateCustomizationItemArgument(CustomizationItemArgumentEntity argument, string value)
		/// <summary>
		/// Updates the customization item argument.
		/// </summary>
		/// <param name="argument">The argument.</param>
		/// <param name="value">The value.</param>
		private static void UpdateCustomizationItemArgument(CustomizationItemArgumentEntity argument, string value)
		{
			argument.Value = value;
			BusinessManager.Update(argument);
		}
		#endregion

		#region ClearCache
		/// <summary>
		/// Clears the cache.
		/// </summary>
		/// <param name="profileId">The profile id.</param>
		/// <param name="principalId">The principal id.</param>
		private static void ClearCache(PrimaryKeyId? profileId, PrimaryKeyId? principalId)
		{
			// user
			if (principalId.HasValue)
			{
				DataCache.RemoveByUser(principalId.Value.ToString());
			}
			// profile
			else if (profileId.HasValue)
			{
				ProfileManager.ClearCacheForProfileUsers(profileId.Value);
			}
			XmlCommand.ClearCache();
			XmlBuilder.ClearCache();
		} 
		#endregion
	}
}
