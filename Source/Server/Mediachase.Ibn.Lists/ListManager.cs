using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Lists.Mapping;
using System.Data;
using Mediachase.Ibn.Web.UI.Controls.Util;
using System.Text.RegularExpressions;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Core.Database;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.Ibn.Lists
{
	/// <summary>
	/// Represents list manager and organizes list, publication and list folder common actions.
	/// </summary>
	public static class ListManager
	{
		#region consts
		public const string ListTypeEnumName = "ListType";
		public const string ListStatusEnumName = "ListStatus";
		#endregion

		#region Events

		/// <summary>
		/// Occurs when [list creating].
		/// </summary>
		public static event EventHandler<InfoEventArgs> ListCreating;
		public static event EventHandler<InfoEventArgs> ListCreated;

		public static event EventHandler<InfoEventArgs> ListDeleting;
		public static event EventHandler<InfoEventArgs> ListDeleted;

		public static event EventHandler<InfoEventArgs> ListMoving;
		public static event EventHandler<InfoEventArgs> ListMoved;

		public static event EventHandler<InfoEventArgs> ListSaving;
		public static event EventHandler<InfoEventArgs> ListSaved;

		public static event EventHandler<PublicationEventArgs> PublicationCreating;
		public static event EventHandler<PublicationEventArgs> PublicationCreated;

		public static event EventHandler<PublicationEventArgs> PublicationDeleting;
		public static event EventHandler<PublicationEventArgs> PublicationDeleted;

		#endregion

		#region internal static void CreateInfoEvent(EventHandler<ListEventArgs> eventHandler, ListInfo listInfo)
		internal static void CreateInfoEvent(EventHandler<InfoEventArgs> eventHandler, ListInfo listInfo)
		{
			if (eventHandler != null)
				eventHandler(null, new InfoEventArgs(listInfo));
		}
		#endregion
		#region private static void CreatePublicationEvent(EventHandler<PublicationEventArgs> eventHandler, ListPublication listPublication)
		private static void CreatePublicationEvent(EventHandler<PublicationEventArgs> eventHandler, ListPublication listPublication)
		{
			if (eventHandler != null)
				eventHandler(null, new PublicationEventArgs(listPublication));
		}
		#endregion

		#region Get XXX Root
		/// <summary>
		/// Gets the public root.
		/// </summary>
		/// <returns></returns>
		public static ListFolder GetPublicRoot()
		{
			ListFolder[] nodes = ListFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
				FilterElement.IsNullElement("Owner_PrincipalId"),
				FilterElement.IsNullElement("ProjectId"));

			if (nodes.Length > 0)
				return nodes[0];

			// Create Public Root
			return CreateRootNode("Public", null, null);
		}

		/// <summary>
		/// Gets the project root.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns></returns>
		public static ListFolder GetProjectRoot(int projectId)
		{
			ListFolder[] nodes = ListFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
				FilterElement.IsNullElement("Owner_PrincipalId"),
				FilterElement.EqualElement("ProjectId", projectId));

			if (nodes.Length > 0)
				return nodes[0];

			// Create Project Root
			return CreateRootNode(string.Format(CultureInfo.InvariantCulture, "Project_{0}", projectId), projectId, null);
		}

		/// <summary>
		/// Gets the private root.
		/// </summary>
		/// <param name="ownerId">The owner id.</param>
		/// <returns></returns>
		public static ListFolder GetPrivateRoot(int ownerId)
		{
			ListFolder[] nodes = ListFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
				FilterElement.EqualElement("Owner_PrincipalId", ownerId),
				FilterElement.IsNullElement("ProjectId"));

			if (nodes.Length > 0)
				return nodes[0];

			// Create Private Root
			return CreateRootNode(string.Format(CultureInfo.InvariantCulture, "Private_{0}", ownerId), null, ownerId);
		}

		/// <summary>
		/// Creates the root.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="projectId">The project id.</param>
		/// <param name="ownerId">The owner id.</param>
		/// <returns></returns>
		private static ListFolder CreateRootNode(string title, int? projectId, int? ownerId)
		{
			if (title == null)
				throw new ArgumentNullException("title");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Step 1. Create List Folder
				ListFolder newRoot = new ListFolder();
				newRoot.Title = title;
				newRoot.ProjectId = projectId;
				newRoot.Owner = ownerId;
				newRoot.Save();

				// Step 2. Assign Root
				TreeNode retVal = TreeManager.AppendRootNode(ListInfo.GetAssignedMetaClass(), newRoot);

				tran.Commit();

				return newRoot;
			}
		}
		#endregion

		#region CreateList
		/// <summary>
		/// Creates the list.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static ListInfo CreateList(int folderId, string name)
		{
			ListFolder folder = new ListFolder(folderId);

			return CreateList(folder, name);
		}

		/// <summary>
		/// Creates the list.
		/// </summary>
		/// <param name="folder">The folder.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static ListInfo CreateList(ListFolder folder, string name)
		{
			int? ownerId = folder.Owner is RolePrincipal ? ((RolePrincipal)folder.Owner).PrincipalId : null;
			return CreateList(folder, name, folder.ProjectId, ownerId);
		}

		/// <summary>
		/// Creates the list.
		/// </summary>
		/// <param name="folder">The folder.</param>
		/// <param name="name">The name.</param>
		/// <param name="projectId">The project id.</param>
		/// <param name="ownerId">The owner id.</param>
		/// <returns></returns>
		public static ListInfo CreateList(ListFolder folder, string name, int? projectId, int? ownerId)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (folder == null)
				throw new ArgumentNullException("folder");
			if (!folder.GetTreeService().CurrentNode.IsAttached)
				throw new ArgumentException("The folder is dettached from tree.", "folder");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Step 1. Create New ListInfo
				ListInfo newItem = new ListInfo();
				newItem.Title = name;
				newItem.FolderId = folder.PrimaryKeyId.Value;
				newItem.ProjectId = projectId;
				newItem.Owner = ownerId;

				// Raise List Creating Event
				CreateInfoEvent(ListCreating, newItem);
				// Add additional parameters

				newItem.Save();

				// Step 2. Create New List - MetaClass
				CreateListMetaClass(name, newItem, true);

				int listId = newItem.PrimaryKeyId.Value;

				// Ibn 4.7 Add-on. Project Roles Security
				if (projectId.HasValue)
				{
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Manager, (int)ListInfoBus.ListAccess.Admin);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.ExecutiveManager, (int)ListInfoBus.ListAccess.Admin);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.TeamMembers, (int)ListInfoBus.ListAccess.Write);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Sponsors, (int)ListInfoBus.ListAccess.Write);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Stakeholders, (int)ListInfoBus.ListAccess.Write);
				}

				// OZ: 2009-02-17 Create Default Access
				if (ownerId == null)
				{
					ListInfoBus.CreateListAccess(listId, Mediachase.IBN.Business.Security.CurrentUser.UserID, (int)ListInfoBus.ListAccess.Admin);
					if (!Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.Security.CurrentUser.UserID, InternalSecureGroups.Partner))
						ListInfoBus.CreateListAccess(listId, (int)InternalSecureGroups.Intranet, (int)ListInfoBus.ListAccess.Write);
					else
						ListInfoBus.CreateListAccess(listId, User.GetGroupForPartnerUser(Mediachase.IBN.Business.Security.CurrentUser.UserID), (int)ListInfoBus.ListAccess.Write);
				}
				//

				// Step. Commit Transaction Return new list info
				tran.Commit();
				return newItem;
			}
		}


		/// <summary>
		/// Creates the list meta class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="newItem">The new item.</param>
		/// <param name="bCreateDefaultMetaView">if set to <c>true</c> [b create default meta view].</param>
		/// <returns></returns>
		private static MetaClass CreateListMetaClass(string name, ListInfo newItem, bool bCreateDefaultMetaView)
		{
			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				string metaClassName = ListManager.GetListMetaClassName(newItem);

				MetaClass listMetaClass = DataContext.Current.MetaModel.CreateMetaClass(metaClassName,
					name,
					name,
					"cls_" + metaClassName);

				// Set List Primary Key -> Id
				//listMetaClass.Fields[0].FriendlyName = "Id";

				//AttributeCollection mcTitleAttrs = new AttributeCollection();
				//mcTitleAttrs.Add("MaxLength", 255);
				//listMetaClass.CreateMetaField("Title", "Title", "Text", false, "''", mcTitleAttrs);
				//listMetaClass.TitleFieldName = "Title";

				// Create Default Meta Class View
				if (bCreateDefaultMetaView)
				{
					CreateDefaultViewProfile(listMetaClass, metaClassName, name);
					CreateDefaultForm(listMetaClass);
				}

				// Install Change Tracking Service
				BusinessObjectServiceManager.InstallService(listMetaClass, ChangeDetectionService.ServiceName);

				// Raise List Created Event
				CreateInfoEvent(ListCreated, newItem);

				scope.SaveChanges();

				return listMetaClass;
			}
		}

		private static void CreateDefaultViewProfile(MetaClass listMetaClass, string metaClassName, string friendlyName)
		{
			//List<MetaField> availableField = new List<MetaField>();
			//DataContext.Current.MetaModel.CreateMetaView(listMetaClass, name, friendlyName,
			//    new MetaField[] { 
			//            //listMetaClass.Fields["Title"] 
			//        });

			ListViewProfile profile = new ListViewProfile();

			profile.Id = Guid.NewGuid().ToString();
			profile.Name = "{IbnFramework.ListInfo:lvpGeneralView}";
			profile.IsSystem = true;
			profile.IsPublic = true;

			ListViewProfile.SaveSystemProfile(metaClassName, "EntityList", (int)DataContext.Current.CurrentUserId, profile);
		}

		public static void CreateDefaultForm(MetaClass listMetaClass)
		{
			FormDocument fdBase = FormController.CreateFormDocument(listMetaClass.Name, FormController.BaseFormType);
			fdBase.FormTable.Rows[0].Cells[0].Sections[0].BorderType = (int)BorderType.None;
			fdBase.FormTable.Rows[0].Cells[0].Sections[0].ShowLabel = false;
			fdBase.Save();

			FormDocument fdView = FormController.CreateFormDocument(listMetaClass.Name, FormController.GeneralViewFormType);
			fdView.FormTable.Rows[0].Cells[0].Sections[0].BorderType = (int)BorderType.None;
			fdView.FormTable.Rows[0].Cells[0].Sections[0].ShowLabel = false;
			fdView.Save();
		}

		#endregion

		#region CreateListFromTemplate
		/// <summary>
		/// Creates the list from template.
		/// </summary>
		/// <param name="templateListId">The template list id.</param>
		/// <param name="folderId">The folder id.</param>
		/// <param name="name">The name.</param>
		/// <param name="copyRecords">if set to <c>true</c> [copy records].</param>
		/// <returns></returns>
		public static ListInfo CreateListFromTemplate(int templateListId, int folderId, string name, bool copyRecords)
		{
			ListInfo listInfo = new ListInfo(templateListId);
			ListFolder folder = new ListFolder(folderId);

			return CreateListFromTemplate(listInfo, folder, name, copyRecords);
		}

		/// <summary>
		/// Creates the list from template.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <param name="folder">The folder.</param>
		/// <param name="name">The name.</param>
		/// <param name="copyRecords">if set to <c>true</c> [copy records].</param>
		/// <returns></returns>
		public static ListInfo CreateListFromTemplate(ListInfo template, ListFolder folder, string name, bool copyRecords)
		{
			int? ownerId = folder.Owner is RolePrincipal ? ((RolePrincipal)folder.Owner).PrincipalId : null;
			return CreateListFromTemplate(template, folder, name, folder.ProjectId, ownerId, copyRecords);
		}

		/// <summary>
		/// Creates the list from template.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <param name="folder">The folder.</param>
		/// <param name="name">The name.</param>
		/// <param name="projectId">The project id.</param>
		/// <param name="ownerId">The owner id.</param>
		/// <param name="copyRecords">if set to <c>true</c> [copy records].</param>
		/// <returns></returns>
		public static ListInfo CreateListFromTemplate(ListInfo template, ListFolder folder, string name, int? projectId, int? ownerId, bool copyRecords)
		{
			if (template == null)
				throw new ArgumentNullException("template");
			if (name == null)
				throw new ArgumentNullException("name");
			if (folder == null)
				throw new ArgumentNullException("folder");
			if (!folder.GetTreeService().CurrentNode.IsAttached)
				throw new ArgumentException("The folder is dettached from tree.", "folder");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Step 1. Create New ListInfo
				ListInfo newItem = new ListInfo();
				newItem.Title = name;
				newItem.FolderId = folder.PrimaryKeyId.Value;
				newItem.ProjectId = projectId;
				newItem.Owner = ownerId;

				// Copy Additional Parameters
				newItem.Description = template.Description;
				newItem.ListType = template.ListType;
				newItem.Status = template.Status;

				// Raise List Creating Event
				CreateInfoEvent(ListCreating, newItem);
				// Add additional parameters

				newItem.Save();

				// Step 2. Create New List - MetaClass
				MetaClass destListMetaClass = CreateListMetaClass(name, newItem, false);

				MetaClass srtListMetaClass = GetListMetaClass(template);

				InnerCopy(srtListMetaClass, destListMetaClass, copyRecords);

				int listId = newItem.PrimaryKeyId.Value;

				// Ibn 4.7 Add-on. Project Roles Security
				if (projectId.HasValue)
				{
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Manager, (int)ListInfoBus.ListAccess.Admin);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.ExecutiveManager, (int)ListInfoBus.ListAccess.Admin);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.TeamMembers, (int)ListInfoBus.ListAccess.Write);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Sponsors, (int)ListInfoBus.ListAccess.Write);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Stakeholders, (int)ListInfoBus.ListAccess.Write);
				}

				// OZ: 2009-02-17 Create Default Access
				if (ownerId == null)
				{
					ListInfoBus.CreateListAccess(listId, Mediachase.IBN.Business.Security.CurrentUser.UserID, (int)ListInfoBus.ListAccess.Admin);

					if (!Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.Security.CurrentUser.UserID, InternalSecureGroups.Partner))
						ListInfoBus.CreateListAccess(listId, (int)InternalSecureGroups.Intranet, (int)ListInfoBus.ListAccess.Write);
					else
						ListInfoBus.CreateListAccess(listId, User.GetGroupForPartnerUser(Mediachase.IBN.Business.Security.CurrentUser.UserID), (int)ListInfoBus.ListAccess.Write);
				}
				//

				// Step. Commit Transaction Return new list info
				tran.Commit();

				return newItem;
			}
		}


		#endregion

		#region GetListMetaClassName
		/// <summary>
		/// Gets the name of the list meta class.
		/// </summary>
		/// <param name="listInfoId">The list info id.</param>
		/// <returns></returns>
		public static string GetListMetaClassName(int listInfoId)
		{
			return string.Format(CultureInfo.InvariantCulture, "List_{0}", listInfoId);
		}

		/// <summary>
		/// Gets the name of the list meta class.
		/// </summary>
		/// <param name="info">ListInfo object.</param>
		/// <returns></returns>
		public static string GetListMetaClassName(MetaObject info)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			return string.Format(CultureInfo.InvariantCulture, "List_{0}", info.PrimaryKeyId.Value);
		}
		#endregion

		#region GetListIdByClassName
		/// <summary>
		/// Gets the id of the list id by ClassName.
		/// </summary>
		/// <param name="className">Name of the class.</param>
		/// <returns></returns>
		public static int GetListIdByClassName(string className)
		{
			return int.Parse(className.Substring(5), CultureInfo.InvariantCulture);
		}
		#endregion

		#region GetListInfoByMetaClass
		/// <summary>
		/// Gets the name of the list meta class.
		/// </summary>
		/// <param name="listInfoId">The list info id.</param>
		/// <returns></returns>
		public static ListInfo GetListInfoByMetaClassName(string metaClassName)
		{
			if (metaClassName == null)
				throw new ArgumentNullException("metaClassName");

			if (HistoryManager.MetaClassIsHistory(metaClassName))
				metaClassName = metaClassName.Replace("_History", string.Empty);

			// O.R. [2009-12-02] Fix for numeric class name, ex. cls_1111
			if (!metaClassName.StartsWith("list_", StringComparison.InvariantCultureIgnoreCase))
				return null;

			int listId = -1;
			if (!int.TryParse(metaClassName.Substring(metaClassName.LastIndexOf('_') + 1), out listId))
				return null;
			//			int listId = int.Parse(metaClassName.Substring(metaClassName.LastIndexOf('_')+1));

			return new ListInfo(listId);
		}

		/// <summary>
		/// Gets the name of the list meta class.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <returns></returns>
		public static ListInfo GetListInfoByMetaClass(MetaClass metaClass)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			return GetListInfoByMetaClassName(metaClass.Name);
		}
		#endregion

		#region GetListMetaClass
		/// <summary>
		/// Gets the name of the list meta class.
		/// </summary>
		/// <param name="listInfoId">The list info id.</param>
		/// <returns></returns>
		public static MetaClass GetListMetaClass(int listInfoId)
		{
			return DataContext.Current.GetMetaClass(GetListMetaClassName(listInfoId));
		}

		/// <summary>
		/// Gets the name of the list meta class.
		/// </summary>
		/// <param name="info">ListInfo.</param>
		/// <returns></returns>
		public static MetaClass GetListMetaClass(MetaObject info)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			return DataContext.Current.GetMetaClass(GetListMetaClassName(info));
		}
		#endregion

		#region DeleteList
		/// <summary>
		/// Deletes the list.
		/// </summary>
		/// <param name="listInfoId">The list info id.</param>
		public static void DeleteList(int listInfoId)
		{
			DeleteList(new ListInfo(listInfoId));
		}

		/// <summary>
		/// Deletes the list.
		/// </summary>
		/// <param name="listInfo">The list info.</param>
		public static void DeleteList(ListInfo listInfo)
		{
			if (listInfo == null)
				throw new ArgumentNullException("listInfo");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				int listId = listInfo.PrimaryKeyId.Value;

				string metaClassName = ListManager.GetListMetaClassName(listInfo);

				//DELETE FROM HistoryEntity
				SqlHelper.ExecuteNonQuery(SqlContext.Current, System.Data.CommandType.StoredProcedure,
					"bus_cls_ListObject_Delete",
					SqlHelper.SqlParameter("@ClassName", SqlDbType.NVarChar, 250, metaClassName),
					SqlHelper.SqlParameter("@ObjectId", SqlDbType.VarChar, 36, "0"));

				CreateInfoEvent(ListDeleting, listInfo);

				// Delete Attached McMetaViewPreferences
				//foreach (MetaView metaView in DataContext.Current.MetaModel.GetMetaViews(metaClassName))
				//{
				//    UserMetaViewPreference.DeleteAll(metaView.Name);
				//}

				// Delete Form Documents
				foreach (FormDocument formDocument in FormDocument.GetFormDocuments(metaClassName))
					formDocument.Delete();


				// Delete List MetaClass
				using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
				{
					// Cleaun Up History
					if (IsHistoryActivated(metaClassName))
					{
						DeactivateHistory(metaClassName);
					}

					//string metaClassName = ListManager.GetListMetaClassName(listInfo);

					if (DataContext.Current.MetaModel.MetaClasses.Contains(metaClassName))
					{
						DataContext.Current.MetaModel.DeleteMetaClass(metaClassName);
					}

					scope.SaveChanges();
				}

				listInfo.InnerDelete();

				// Ibn 4.7 add-on.
				ListInfoBus.DeleteListAccessByList(listId);
				Mediachase.IBN.Business.Common.DeleteHistory(ObjectTypes.List, listId);
				//

				CreateInfoEvent(ListDeleted, listInfo);

				tran.Commit();
			}

		}

		#endregion

		#region GetLists
		/// <summary>
		/// Lists the specified folder id.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <returns></returns>
		public static ListInfo[] GetLists(int folderId)
		{
			// Load primary list
			List<ListInfo> retVal = new List<ListInfo>(ListInfo.List(folderId));

			//TODO: load list from publications

			return retVal.ToArray();
		}

		/// <summary>
		/// Lists the specified folder.
		/// </summary>
		/// <param name="folder">ListFolder.</param>
		/// <returns></returns>
		public static ListInfo[] GetLists(MetaObject folder)
		{
			// Load primary list
			List<ListInfo> retVal = new List<ListInfo>(ListInfo.List(folder));

			//TODO: load list from publications

			return retVal.ToArray();
		}

		/// <summary>
		/// Gets the lists by keyword.
		/// </summary>
		/// <param name="keyword">The keyword.</param>
		/// <returns></returns>
		public static ListInfo[] GetLists(string keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			keyword = Regex.Replace(keyword, @"(\[|%|_)", "[$0]", RegexOptions.IgnoreCase);
			//

			List<ListInfo> retVal = new List<ListInfo>();
			FilterElement filter = new FilterElement("Title", FilterElementType.Contains, keyword);
			ListInfo[] lists = ListInfo.List(filter);
			foreach (ListInfo li in lists)
			{
				if (ListInfoBus.CanRead(li.PrimaryKeyId.Value))
					retVal.Add(li);
			}

			return retVal.ToArray();
		}
		#endregion

		#region CreatePublication
		/// <summary>
		/// Creates the publication.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <param name="infoId">The info id.</param>
		/// <returns></returns>
		public static ListPublication CreatePublication(int folderId, int infoId)
		{
			return CreatePublication(new ListFolder(folderId), new ListInfo(infoId));
		}

		/// <summary>
		/// Creates the publication.
		/// </summary>
		/// <param name="folder">ListFolder.</param>
		/// <param name="info">ListInfo.</param>
		/// <returns></returns>
		public static ListPublication CreatePublication(MetaObject folder, MetaObject info)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");
			if (info == null)
				throw new ArgumentNullException("info");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				ListPublication publication = new ListPublication();

				publication.FolderId = folder.PrimaryKeyId.Value;
				publication.ListInfoId = info.PrimaryKeyId.Value;

				CreatePublicationEvent(PublicationCreating, publication);

				publication.Save();

				CreatePublicationEvent(PublicationCreated, publication);

				tran.Commit();

				return publication;
			}
		}

		#endregion

		#region DeletePublication
		/// <summary>
		/// Deletes the publication.
		/// </summary>
		/// <param name="publicationId">The publication id.</param>
		public static void DeletePublication(int publicationId)
		{
			DeletePublication(new ListPublication(publicationId));
		}

		/// <summary>
		/// Deletes the publication.
		/// </summary>
		/// <param name="publication">The publication.</param>
		public static void DeletePublication(ListPublication publication)
		{
			if (publication == null)
				throw new ArgumentNullException("publication");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				CreatePublicationEvent(PublicationDeleting, publication);

				publication.Delete();

				CreatePublicationEvent(PublicationDeleted, publication);

				tran.Commit();
			}
		}
		#endregion

		#region CreateFolder
		/// <summary>
		/// Creates the folder.
		/// </summary>
		/// <param name="parentId">The parent id.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static ListFolder CreateFolder(int parentId, string name)
		{
			return CreateFolder(new ListFolder(parentId), name);
		}

		/// <summary>
		/// Creates the folder.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static ListFolder CreateFolder(ListFolder parent, string name)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("parent");

			// TODO: Check name is unique

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Create Detached Folder
				ListFolder newRoot = new ListFolder();

				newRoot.Title = name;
				newRoot.ProjectId = parent.ProjectId;
				newRoot.Owner = parent.Owner is RolePrincipal ? ((RolePrincipal)parent.Owner).PrincipalId : null;
				newRoot.Save();

				// Append to
				TreeNode node = parent.GetTreeService().AppendChild(newRoot);
				parent.Save();

				tran.Commit();

				return newRoot;
			}
		}
		#endregion

		#region DeleteFolder
		/// <summary>
		/// Deletes the folder.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		public static void DeleteFolder(int folderId)
		{
			DeleteFolder(new ListFolder(folderId), true);
		}


		/// <summary>
		/// Deletes the folder.
		/// </summary>
		/// <param name="folder">The folder.</param>
		public static void DeleteFolder(ListFolder folder)
		{
			DeleteFolder(folder, true);
		}

		/// <summary>
		/// Deletes the folder.
		/// </summary>
		/// <param name="folder">The folder.</param>
		/// <param name="checkSecurity">if set to <c>true</c> [check security].</param>
		private static void DeleteFolder(ListFolder folder, bool checkSecurity)
		{
			if (folder == null)
				throw new ArgumentNullException("folder");

			if (checkSecurity && !CanDeleteFolder(folder))
				throw new AccessDeniedException();

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				TreeService treeService = folder.GetTreeService();

				foreach (TreeNode node in TreeManager.GetAllChildNodes(treeService.CurrentNode))
				{
					RemoveElements(node.ObjectId);
				}

				RemoveElements(folder.PrimaryKeyId.Value);

				folder.Delete();

				tran.Commit();
			}
		}

		/// <summary>
		/// Removes the elements.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		private static void RemoveElements(int folderId)
		{
			// Remove Publications (w/o Security)
			foreach (ListPublication item in ListPublication.List(FilterElement.EqualElement("FolderId", folderId)))
			{
				item.Delete();
			}

			// Remove ListInfos (w/o Security)
			foreach (ListInfo item in ListInfo.List(FilterElement.EqualElement("FolderId", folderId)))
			{
				item.Delete();
			}
		}
		#endregion

		#region Delete XXX Root
		public static void DeleteProjectRoot(int projectId)
		{
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				ListFolder[] nodes = ListFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
					FilterElement.IsNullElement("Owner_PrincipalId"),
					FilterElement.EqualElement("ProjectId", projectId));

				foreach (ListFolder node in nodes)
				{
					DeleteFolder(node, false);
				}

				tran.Commit();
			}
		}

		/// <summary>
		/// Gets the private root.
		/// </summary>
		/// <param name="ownerId">The owner id.</param>
		/// <returns></returns>
		public static void DeletePrivateRoot(int ownerId)
		{
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				ListFolder[] nodes = ListFolder.List(FilterElement.IsNullElement(TreeService.ParentIdFieldName),
					FilterElement.EqualElement("Owner_PrincipalId", ownerId),
					FilterElement.IsNullElement("ProjectId"));

				foreach (ListFolder node in nodes)
				{
					DeleteFolder(node, false);
				}

				tran.Commit();
			}
		}
		#endregion

		#region MoveList
		/// <summary>
		/// Moves the list.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="newFolder">The new folder.</param>
		public static void MoveList(ListInfo info, ListFolder newFolder)
		{
			if (info == null)
				throw new ArgumentNullException("info");
			if (newFolder == null)
				throw new ArgumentNullException("newFolder");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				int oldProjectId = (info.ProjectId.HasValue) ? (int)info.ProjectId.Value : -1;

				// Raise List Moving
				CreateInfoEvent(ListMoving, info);

				int oldOwnerId = -1;
				RolePrincipal oldOwner = info.Owner as RolePrincipal;
				if (oldOwner != null && oldOwner.PrincipalId.HasValue)
					oldOwnerId = (int)oldOwner.PrincipalId.Value;

				info.FolderId = newFolder.PrimaryKeyId.Value;
				info.ProjectId = newFolder.ProjectId;
				info.Owner = newFolder.Owner is RolePrincipal ? ((RolePrincipal)newFolder.Owner).PrincipalId : null;

				info.Save();

				// Ibn 4.7 add-on
				int newProjectId = (info.ProjectId.HasValue) ? (int)info.ProjectId.Value : -1;
				int listId = info.PrimaryKeyId.Value;
				if (oldProjectId == -1 && newProjectId > 0)	// 
				{
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Manager, (int)ListInfoBus.ListAccess.Admin);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.ExecutiveManager, (int)ListInfoBus.ListAccess.Admin);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.TeamMembers, (int)ListInfoBus.ListAccess.Write);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Sponsors, (int)ListInfoBus.ListAccess.Write);
					ListInfoBus.CreateListAccess(listId, (int)ListInfoBus.ListProjectRole.Stakeholders, (int)ListInfoBus.ListAccess.Write);
				}
				else if (oldProjectId > 0 && newProjectId == -1)
				{
					ListInfoBus.DeleteListAccess(listId, (int)ListInfoBus.ListProjectRole.Manager);
					ListInfoBus.DeleteListAccess(listId, (int)ListInfoBus.ListProjectRole.ExecutiveManager);
					ListInfoBus.DeleteListAccess(listId, (int)ListInfoBus.ListProjectRole.TeamMembers);
					ListInfoBus.DeleteListAccess(listId, (int)ListInfoBus.ListProjectRole.Sponsors);
					ListInfoBus.DeleteListAccess(listId, (int)ListInfoBus.ListProjectRole.Stakeholders);
				}
				//

				// If moving from personal to general, grant access for old owner
				if (oldOwnerId > 0 && info.Owner == null)
					ListInfoBus.CreateListAccess(listId, oldOwnerId, (int)ListInfoBus.ListAccess.Admin);

				CreateInfoEvent(ListMoved, info);

				tran.Commit();
			}
		}
		#endregion

		#region Templates Methods
		#region GetTemplates
		public static ListInfo[] GetTemplates()
		{
			return ListInfo.List(FilterElement.EqualElement("IsTemplate", true));
		}
		#endregion

		#region CreateTemplate
		public static ListInfo CreateTemplate(string title)
		{
			if (title == null)
				throw new ArgumentNullException("title");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Step 1. Create New ListInfo
				ListInfo newItem = new ListInfo();
				newItem.Title = title;
				newItem.IsTemplate = true;

				// Raise List Creating Event
				CreateInfoEvent(ListCreating, newItem);
				// Add additional parameters

				newItem.Save();

				// Step 2. Create New List - MetaClass
				CreateListMetaClass(title, newItem, true);

				// Step. Commit Transaction Return new list info
				tran.Commit();

				return newItem;
			}
		}

		/// <summary>
		/// Creates the template.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="srcListId">The SRC list id.</param>
		/// <returns></returns>
		public static ListInfo CreateTemplate(string title, int sourceListId)
		{
			ListInfo srcList = new ListInfo(sourceListId);

			return CreateTemplate(title, srcList, false);
		}

		/// <summary>
		/// Creates the template based on source list without records.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="srcList">The SRC list.</param>
		/// <returns></returns>
		public static ListInfo CreateTemplate(string title, ListInfo sourceListInfo)
		{
			return CreateTemplate(title, sourceListInfo, false);
		}

		/// <summary>
		/// Creates the template.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="srcListId">The SRC list id.</param>
		/// <param name="copyRecords">if set to <c>true</c> [copy records].</param>
		/// <returns></returns>
		public static ListInfo CreateTemplate(string title, int sourceListId, bool copyRecords)
		{
			ListInfo srcList = new ListInfo(sourceListId);

			return CreateTemplate(title, srcList, copyRecords);
		}

		/// <summary>
		/// Creates the template based on source list.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="srcList">The SRC list.</param>
		/// <param name="copyRecords">if set to <c>true</c> [copy records].</param>
		/// <returns></returns>
		public static ListInfo CreateTemplate(string title, ListInfo sourceListInfo, bool copyRecords)
		{
			if (title == null)
				throw new ArgumentNullException("title");
			if (sourceListInfo == null)
				throw new ArgumentNullException("sourceListInfo");

			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				// Step 1. Create New ListInfo
				ListInfo newItem = new ListInfo();
				newItem.Title = title;

				// Copy Additional Parameters
				newItem.Description = sourceListInfo.Description;
				newItem.Status = sourceListInfo.Status;
				newItem.ListType = sourceListInfo.ListType;

				newItem.IsTemplate = true;

				// Raise List Creating Event
				CreateInfoEvent(ListCreating, newItem);
				// Add additional parameters

				newItem.Save();

				// Step 2. Create New List - MetaClass
				MetaClass destListMetaClass = CreateListMetaClass(title, newItem, false);

				// Step 3. Duplicate MetaFields
				MetaClass srtListMetaClass = GetListMetaClass(sourceListInfo);

				InnerCopy(srtListMetaClass, destListMetaClass, copyRecords);

				// Step. Commit Transaction Return new list info
				tran.Commit();

				return newItem;
			}

		}

		private static void InnerCopy(MetaClass srcListMetaClass, MetaClass destListMetaClass, bool copyRecords)
		{
			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				CopyInstalledServices(srcListMetaClass, destListMetaClass);

				CopyMetaFields(srcListMetaClass, destListMetaClass);

				destListMetaClass.TitleFieldName = srcListMetaClass.TitleFieldName;

				//CopyMetaViews(srtListMetaClass, destListMetaClass);

				//CopyListViewProfiles(srcListMetaClass, destListMetaClass);

				scope.SaveChanges();
			}

			// Copy External Objects like MetaView preferences
			//CopyMetaViewPrerences(srtListMetaClass, destListMetaClass);
			CopyListViewProfiles(srcListMetaClass, destListMetaClass);

			// Copy Form Document
			CopyFormDocument(srcListMetaClass, destListMetaClass);

			// Step 4. Duplicate records
			if (copyRecords)
			{
				CopyMetaObjects(srcListMetaClass, destListMetaClass);
			}
		}

		private static void CopyListViewProfiles(MetaClass srcListMetaClass, MetaClass destListMetaClass)
		{
			foreach (mcweb_ListViewProfileRow srcRow in mcweb_ListViewProfileRow.List(FilterElement.EqualElement("MetaClassName", srcListMetaClass.Name)))
			{
				mcweb_ListViewProfileRow destRow = new mcweb_ListViewProfileRow();

				destRow.IsPublic = srcRow.IsPublic;
				destRow.IsSystem = srcRow.IsSystem;
				destRow.MetaClassName = destListMetaClass.Name;
				destRow.PlaceName = srcRow.PlaceName;
				destRow.UserId = srcRow.UserId;
				destRow.ViewName = Guid.NewGuid().ToString();

				string xsListViewProfile = srcRow.XSListViewProfile;

				// Fix Primary Key Columns
				xsListViewProfile = xsListViewProfile.Replace(srcListMetaClass.Name, destListMetaClass.Name);

				destRow.XSListViewProfile = xsListViewProfile;

				destRow.Update();
			}
		}

		/// <summary>
		/// Copies the form document.
		/// </summary>
		/// <param name="srtListMetaClass">The SRT list meta class.</param>
		/// <param name="destListMetaClass">The dest list meta class.</param>
		private static void CopyFormDocument(MetaClass srtListMetaClass, MetaClass destListMetaClass)
		{
			foreach (FormDocument formDocument in FormDocument.GetFormDocuments(srtListMetaClass.Name))
			{
				formDocument.MetaClassName = destListMetaClass.Name;
				formDocument.Id = null;

				formDocument.Save();
			}
		}

		/// <summary>
		/// Copies the mc meta view prerences.
		/// </summary>
		/// <param name="srtListMetaClass">The SRT list meta class.</param>
		/// <param name="destListMetaClass">The dest list meta class.</param>
		[Obsolete]
		private static void CopyMetaViewPrerences(MetaClass srtListMetaClass, MetaClass destListMetaClass)
		{
			foreach (MetaView srcMetaView in DataContext.Current.MetaModel.GetMetaViews(srtListMetaClass))
			{
				string metaViewName = srcMetaView.Name;
				metaViewName = metaViewName.Replace(srtListMetaClass.Name, destListMetaClass.Name);

				MetaView destMetaView = DataContext.Current.GetMetaView(metaViewName);

				CopyMetaViewPrerences(srcMetaView, destMetaView);
			}
		}

		/// <summary>
		/// Copies the meta view prerences.
		/// </summary>
		/// <param name="srcMetaView">The SRC meta view.</param>
		/// <param name="destMetaView">The dest meta view.</param>
		[Obsolete]
		private static void CopyMetaViewPrerences(MetaView srcMetaView, MetaView destMetaView)
		{
			McMetaViewPreference preferences = UserMetaViewPreference.LoadDefault(srcMetaView);

			if (preferences != null)
			{
				preferences.MetaView = destMetaView;
				UserMetaViewPreference.SaveDefault(preferences);
			}
		}

		/// <summary>
		/// Gets the primary key meta field.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <returns></returns>
		private static MetaField GetPrimaryKeyMetaField(MetaClass metaClass)
		{
			foreach (MetaField mf in metaClass.Fields)
			{
				if (mf.DataSource.InPrimaryKey)
					return mf;
			}
			return null;
		}

		/// <summary>
		/// Copies the meta views.
		/// </summary>
		/// <param name="srtListMetaClass">The SRT list meta class.</param>
		/// <param name="destListMetaClass">The dest list meta class.</param>
		[Obsolete]
		private static void CopyMetaViews(MetaClass srtListMetaClass, MetaClass destListMetaClass)
		{
			foreach (MetaView srcMetaView in DataContext.Current.MetaModel.GetMetaViews(srtListMetaClass))
			{
				string metaViewName = srcMetaView.Name;
				metaViewName = metaViewName.Replace(srtListMetaClass.Name, destListMetaClass.Name);

				List<MetaField> availableFields = new List<MetaField>();

				foreach (MetaField mf in srcMetaView.AvailableFields)
				{
					if (mf.DataSource.InPrimaryKey)
					{
						// Add Primary Key
						availableFields.Add(GetPrimaryKeyMetaField(destListMetaClass));
					}
					else if (destListMetaClass.Fields.Contains(mf.Name))
					{
						availableFields.Add(destListMetaClass.Fields[mf.Name]);
					}

					//if (CheckMetaField(mf))
					//{
					//    availableFields.Add(destListMetaClass.Fields[mf.Name]);
					//}
					//else if (mf.DataSource.InPrimaryKey)
					//{
					//    // Add Primary Key
					//    availableFields.Add(GetPrimaryKeyMetaField(destListMetaClass));
					//}
				}

				// Meta View + Available Fields
				MetaView destMetaView = DataContext.Current.MetaModel.CreateMetaView(destListMetaClass,
					metaViewName, srcMetaView.FriendlyName,
					availableFields.ToArray());

				// Namespace
				destMetaView.Namespace = srcMetaView.Namespace;

				// Attributes
				destMetaView.Attributes.AddRange(srcMetaView.Attributes);

				// Filters
				destMetaView.Filters.AddRange(srcMetaView.Filters);

				// Sorting
				destMetaView.Sorting.AddRange(srcMetaView.Sorting);

				// Grouping
				//destMetaView.Grouping.AddRange(srcMetaView.Grouping);

				// PrimaryGroupBy
				destMetaView.PrimaryGroupBy = srcMetaView.PrimaryGroupBy;

				// SecondaryGroupBy
				destMetaView.SecondaryGroupBy = srcMetaView.SecondaryGroupBy;

				// SecondaryGroupBy
				destMetaView.TotalGroupBy = srcMetaView.TotalGroupBy;

				// TODO: Card

			}
		}

		/// <summary>
		/// Copies the installed services.
		/// </summary>
		/// <param name="srtListMetaClass">The SRT list meta class.</param>
		/// <param name="destListMetaClass">The dest list meta class.</param>
		private static void CopyInstalledServices(MetaClass srtListMetaClass, MetaClass destListMetaClass)
		{
			// TODO: Not Implemented yet
		}

		/// <summary>
		/// Copies the records.
		/// </summary>
		/// <param name="srtListMetaClass">The SRT list meta class.</param>
		/// <param name="destListMetaClass">The dest list meta class.</param>
		private static void CopyMetaObjects(MetaClass srtListMetaClass, MetaClass destListMetaClass)
		{
			MetaObject[] srcRecords = MetaObject.List(srtListMetaClass);

			foreach (MetaObject srcRecord in srcRecords)
			{
				MetaObject destRecord = new MetaObject(destListMetaClass);

				foreach (MetaField mf in srtListMetaClass.Fields)
				{
					if (CheckMetaField(mf))
					{
						string metaFieldName = mf.Name;

						if (!destRecord.Properties[metaFieldName].IsReadOnly)
							destRecord.Properties[metaFieldName].Value = srcRecord.Properties[metaFieldName].Value;
					}
				}

				destRecord.Save();
			}
		}

		/// <summary>
		/// Copies the meta fields.
		/// </summary>
		/// <param name="srtListMetaClass">The SRT list meta class.</param>
		/// <param name="destListMetaClass">The dest list meta class.</param>
		private static void CopyMetaFields(MetaClass srtListMetaClass, MetaClass destListMetaClass)
		{
			// Step 1. Copy Reference
			foreach (MetaField mfRef in srtListMetaClass.Fields)
			{
				if (mfRef.IsReference)
					destListMetaClass.CreateMetaField(mfRef);
			}

			// Step 2. Copy Field And Referenced Fields (Skip Title)
			foreach (MetaField mf in srtListMetaClass.Fields)
			{
				if (mf.IsReferencedField && destListMetaClass.Fields.Contains(mf.Name))
					continue;

				if (!mf.IsReference && CheckMetaField(mf))
				{
					destListMetaClass.CreateMetaField(mf);
				}
			}
		}
		#endregion

		#region CheckMetaField
		/// <summary>
		/// Supports the history.
		/// </summary>
		/// <param name="srcMetaField">The SRC meta field.</param>
		/// <returns></returns>
		private static bool CheckMetaField(MetaField srcMetaField)
		{
			if (srcMetaField.DataSource.InPrimaryKey)
				return false;

			if (srcMetaField.Name == "Created" ||
				srcMetaField.Name == "CreatorId" ||
				srcMetaField.Name == "Modified" ||
				srcMetaField.Name == "ModifierId")
				return false;

			// TODO: Remove if card field added
			if (srcMetaField.GetMetaType().McDataType == McDataType.Card)
				return false;

			return true;
		}
		#endregion

		#endregion

		#region Import
		/// <summary>
		/// Imports the specified import params.
		/// </summary>
		/// <param name="importParams">The import params.</param>
		/// <returns></returns>
		public static MappingError[] Import(ListImportParameters importParameters)
		{
			if (importParameters == null)
				throw new ArgumentNullException("importParameters");
			if (importParameters.MappingDocument.Count == 0)
				throw new ArgumentException("MappingDocument is empty, create MappingElement or call CreateDefaultMapping.");


			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				bool isNewList = importParameters.IsNewList;

				List<MappingError> errors = new List<MappingError>();

				int listInfoId = -1;

				if (isNewList)
				{
					// Step 1. Create New List
					ListInfo info = CreateList(importParameters.FolderId, importParameters.ListName);

					if (!string.IsNullOrEmpty(importParameters.Description))
						info.Description = importParameters.Description;

					if (importParameters.ListType.HasValue)
						info.ListType = importParameters.ListType;

					if (importParameters.Status.HasValue)
						info.Status = importParameters.Status.Value;

					info.Save();

					listInfoId = info.PrimaryKeyId.Value;

					// OZ: Save listInfoId into importParameters
					importParameters.CreatedListId = listInfoId;
				}
				else
				{
					listInfoId = importParameters.ListInfoId.Value;
				}

				// Step 2. Apply New Meta Fields
				MetaClass listMetaClass = GetListMetaClass(listInfoId);

				// Create New enumerators
				Dictionary<string, MetaFieldType> newEnumTypes = new Dictionary<string, MetaFieldType>();

				foreach (NewEnumInfo enumInfo in importParameters.NewEnumTypes)
				{
					if (EnumIsUsed(enumInfo.Name, importParameters))
					{
						MetaFieldType newEnumType = MetaEnum.Create(enumInfo.Name, enumInfo.FriendlyName, enumInfo.MultipleValues);

						if (enumInfo.MultipleValues)
							newEnumType.Attributes.Add(McDataTypeAttribute.EnumMultivalue, enumInfo.MultipleValues);

						if (enumInfo.IsPrivate)
							newEnumType.Attributes.Add(McDataTypeAttribute.EnumPrivate, listMetaClass.Name);

						newEnumTypes.Add(newEnumType.Name, newEnumType);
					}
				}

				// Update ClassName to fix new list problem
				MappingElement mapping = importParameters.MappingDocument[0];
				mapping.ClassName = listMetaClass.Name;

				// Create New Meta Fields
				foreach (MetaField newField in importParameters.NewMetaFields)
				{
					//if (newField.Name == "Created" ||
					//    newField.Name == "CreatorId" ||
					//    newField.Name == "Modified" ||
					//    newField.Name == "ModifierId")
					//    continue;

					// TODO: Check that new field is used in mappin rule otherwise ignore
					if (ImportCheckIsMetaFieldUsed(mapping, newField))
					{
						// 2008-03-24 Fill Default Value From Rules
						MappingRule rule = importParameters.GetRuleByMetaField(newField.Name);
						if (rule != null && rule.RuleType == MappingRuleType.DefaultValue)
						{
							newField.DefaultValue = rule.DefaultValue;
						}

						if (importParameters.NewEnums.Contains(newField.Name))
						{
							// Create Private Meta Enumerator
							MetaFieldType newPrivateEnum = MetaEnum.Create(
								listMetaClass.Name + "_" + newField.Name,
								newField.FriendlyName, false);

							newPrivateEnum.Attributes.Add(McDataTypeAttribute.EnumPrivate, listMetaClass.Name);

							// With Auto Fill Enumerators
							AttributeCollection attr = new AttributeCollection();
							attr.Add(McDataTypeAttribute.EnumEditable, true);

							listMetaClass.CreateMetaField(newField.Name,
								newField.FriendlyName,
								newPrivateEnum.Name,
								true,
								string.Empty, attr);
						}
						else
						{
							// Fix New Enum Meta Fields
							if (newField.IsEnum && newEnumTypes.ContainsKey(newField.TypeName))
								newField.Attributes.AddRange(newEnumTypes[newField.TypeName].Attributes);
							//


							// And Create new meta field by metafield template
							listMetaClass.CreateMetaField(newField);
						}
					}
				}

				// Update Available Fields
				if (isNewList)
				{
					//CreateDefaultUserPreferences(listMetaClass);
					CreateDefaultListViewProfile(listMetaClass);
					CreateDefaultForm(listMetaClass);
				}

				// Apply Title Field Name
				if (!string.IsNullOrEmpty(importParameters.TitleFieldName) &&
					listMetaClass.Fields.Contains(importParameters.TitleFieldName))
				{
					listMetaClass.TitleFieldName = importParameters.TitleFieldName;
				}


				// Step 3. Create mapping engine
				MappingEngine engine = new MappingEngine();

				//engine.MappingErrorEvent += new EventHandler(ImportEngine_MappingErrorEvent);
				if (importParameters.AutoFillMetaEnum)
					engine.Error += new MappingErrorEventHandler(engine_MappingErrorEvent);

				engine.ProcessMapping(importParameters.MappingDocument, importParameters.ExternalData);

				if (importParameters.AutoFillMetaEnum)
					engine.Error -= new MappingErrorEventHandler(engine_MappingErrorEvent);

				//engine.MappingErrorEvent += delegate(){}
				//engine.Errors;

				// Step. Commit Transaction
				scope.SaveChanges();

				return engine.Errors.ToArray();
			}

		}

		private static void CreateDefaultListViewProfile(MetaClass listMetaClass)
		{
			ListViewProfile[] profiles = ListViewProfile.GetSystemProfiles(listMetaClass.Name, "EntityList");

			if (profiles.Length > 0)
			{
				profiles[0].FieldSet.Clear();
				profiles[0].ColumnsUI.Clear();

				foreach (MetaField mf in listMetaClass.Fields)
				{
					if (!mf.Attributes.GetValue<bool>(MetaClassAttribute.IsSystem, false)) // Hide Primary Key
					{
						profiles[0].FieldSet.Add(mf.Name);
						profiles[0].ColumnsUI.Add(new ColumnProperties(mf.Name, "150px", string.Empty));
					}
				}

				ListViewProfile.SaveSystemProfile(listMetaClass.Name, "EntityList", 
					(int)DataContext.Current.CurrentUserId, 
					profiles[0]);
			}
		}

		[Obsolete]
		private static void CreateDefaultUserPreferences(MetaClass listMetaClass)
		{
			foreach (MetaView view in DataContext.Current.MetaModel.GetMetaViews(listMetaClass))
			{
				// Create Default UserPreference
				McMetaViewPreference.CreateDefaultPreference(view);
				McMetaViewPreference defaultPreference = UserMetaViewPreference.LoadDefault(view);

				view.AvailableFields.Clear();

				int counter = 0;
				foreach (MetaField mf in listMetaClass.Fields)
				{
					if (!mf.Attributes.GetValue<bool>(MetaClassAttribute.IsSystem, false)) // Hide Primary Key
					{
						view.AvailableFields.Add(mf);

						defaultPreference.SetAttribute<int>(mf.Name, McMetaViewPreference.AttrIndex, counter);
						counter++;
					}
				}

				UserMetaViewPreference.SaveDefault(defaultPreference);
			}
		}

		private static bool EnumIsUsed(string enumTypeName, ListImportParameters importParams)
		{
			foreach (MetaField mf in importParams.NewMetaFields)
			{
				if (mf.TypeName == enumTypeName)
					return true;
			}

			return false;
		}

		static void engine_MappingErrorEvent(MappingEngine sender, MappingEngineErrorEventArgs e)
		{
			if (e.Error.Exception is MetaObjectValidationException)
			{
				MetaObjectValidationException exception = (MetaObjectValidationException)e.Error.Exception;

				// Check Enum Errors
				foreach (IValidator validator in exception.InvalidValidator)
				{
					EnumFieldValidator enumValidator = validator as EnumFieldValidator;
					if (enumValidator != null)
					{

						MetaFieldType enumType = DataContext.Current.GetMetaFieldType(enumValidator.EnumTypeName);
						string friendlyName = e.MetaObject.Properties[enumValidator.FieldName].Value.ToString();

						if (string.IsNullOrEmpty(friendlyName))
						{
							e.MetaObject.Properties[enumValidator.FieldName].Value = null;
						}
						else
						{
							e.MetaObject.Properties[enumValidator.FieldName].Value = friendlyName;
							MetaEnum.AddItem(enumType, friendlyName, 1);
						}

						e.ResolveError = true;
					}
				}
			}
		}

		//static void ImportEngine_MappingErrorEvent(MappingEngine sender, MappingEngineErrorEventArgs e)
		//{
		//}

		/// <summary>
		/// Imports the check is meta field used.
		/// </summary>
		/// <param name="mapping">The mapping.</param>
		/// <param name="newField">The new field.</param>
		/// <returns></returns>
		private static bool ImportCheckIsMetaFieldUsed(MappingElement mapping, MetaField newField)
		{
			// TODO: Check that new field is used in mappin rule otherwise ignore
			return true;
		}
		#endregion

		#region GetErrorLog
		/// <summary>
		/// Appends the tag.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="value">The value.</param>
		private static void AppendTag(StringBuilder output, string tag, string value)
		{
			if (output == null)
				throw new ArgumentNullException("output");
			if (tag == null)
				throw new ArgumentNullException("tag");
			if (value == null)
				throw new ArgumentNullException("value");

			output.Append("<");
			output.Append(tag);
			output.Append(">");
			output.Append(value);
			output.Append("</");
			output.Append(tag);
			output.Append(">");
		}

		/// <summary>
		/// Appends the tag.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="attribute">The attribute.</param>
		/// <param name="value">The value.</param>
		private static void AppendTag(StringBuilder output, string tag, string attribute, string value)
		{
			AppendTag(output, tag, attribute, value, false);
		}

		/// <summary>
		/// Appends the tag.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="attribute">The attribute.</param>
		/// <param name="value">The value.</param>
		/// <param name="xmlConvertion">if set to <c>true</c> [XML convertion].</param>
		private static void AppendTag(StringBuilder output, string tag, string attribute, string value, bool xmlConvertion)
		{
			output.Append("<");
			output.Append(tag);
			output.Append(' ');
			output.Append(attribute);
			output.Append(">");
			if (xmlConvertion)
			{
				using (System.IO.StringWriter stringWriter = new System.IO.StringWriter(output, CultureInfo.InvariantCulture))
				{
					System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stringWriter);
					xmlWriter.WriteString(value);
				}
			}
			else output.Append(value);

			output.Append("</");
			output.Append(tag);
			output.Append(">");
		}


		/// <summary>
		/// Gets the error log.
		/// </summary>
		/// <param name="errors">The errors.</param>
		/// <returns></returns>
		public static string GetErrorLog(params MappingError[] errors)
		{
			if (errors == null)
				throw new ArgumentNullException("errors");

			StringBuilder output = new StringBuilder();

			#region Header
			output.AppendLine("<HTML>");
			output.AppendLine("<HEAD>");
			output.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>");
			output.AppendLine("</HEAD>");
			output.AppendLine("<BODY>");
			output.AppendFormat("Ibn List Import Errors ({0})<BR>", DateTime.Now);
			output.AppendLine("This errors occured when the list imported external data.<HR>");
			#endregion

			if (errors.Length != 0)
			{
				#region Body
				output.AppendLine("<table BORDER=\"1\">");

				#region Header
				output.Append("<tr>");

				AppendTag(output, "TH", "Error Type");
				AppendTag(output, "TH", "Error");
				AppendTag(output, "TH", "Exception");
				AppendTag(output, "TH", "Columns");

				foreach (DataColumn column in errors[0].Row.Table.Columns)
				{
					AppendTag(output, "TH", column.ColumnName);
				}

				output.Append("</tr>");

				#endregion

				foreach (MappingError error in errors)
				{

					#region Row Values
					output.Append("<tr>");

					AppendTag(output, "TD", "valign=top", error.ErrorType.ToString());
					AppendTag(output, "TD", "valign=top", error.ErrorDescr);
					AppendTag(output, "TD", "valign=top", error.Exception != null ? error.Exception.ToString() : "&nbsp;");
					AppendTag(output, "TD", "valign=top", "&nbsp;");

					foreach (DataColumn column in error.Row.Table.Columns)
					{
						object value = error.Row[column];

						if (value == DBNull.Value || value == null)
						{
							AppendTag(output, "TD", "valign=top", "NULL");
						}
						else
						{
							string stringValue = value as string;
							if (stringValue != null && stringValue.Length == 0)
								AppendTag(output, "TD", "valign=top", "&nbsp;");
							else
								AppendTag(output, "TD", "valign=top", value.ToString());
						}
					}

					output.Append("</tr>");

					#endregion

					#region Exception
					output.Append("<tr>");


					output.Append("</tr>");
					#endregion
				}

				output.AppendLine("</table>");
				#endregion
			}

			#region Footer
			output.AppendLine("</BODY>");
			output.AppendLine("</HTML>");
			#endregion

			return output.ToString();
		}
		#endregion

		#region History

		/// <summary>
		/// Activates the history.
		/// </summary>
		/// <param name="listMetaClassName">Name of the list meta class.</param>
		public static void ActivateHistory(string listMetaClassName)
		{
			if (listMetaClassName == null)
				throw new ArgumentNullException("listMetaClassName");

			ActivateHistory(DataContext.Current.GetMetaClass(listMetaClassName));
		}

		/// <summary>
		/// Activates the history.
		/// </summary>
		/// <param name="listMetaClass">The list meta class.</param>
		public static void ActivateHistory(MetaClass listMetaClass)
		{
			if (listMetaClass == null)
				throw new ArgumentNullException("listMetaClass");

			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				// Step 1. Default Service Activation
				BusinessObjectServiceManager.InstallService(listMetaClass, HistoryService.ServiceName);

				string historyMetaClassName = HistoryManager.GetHistoryMetaClassName(listMetaClass.Name);

				// Install ChangeTracking Service
				//BusinessObjectServiceManager.InstallService(historyMetaClassName, ChangeDetectionService.ServiceName);

				// Step 2. Copy Current List_MetaView to List_MetaView
				//foreach (MetaView srcMetaView in DataContext.Current.MetaModel.GetMetaViews(listMetaClass))
				//{
				//    string metaViewName = srcMetaView.Name;
				//    metaViewName = metaViewName.Replace(listMetaClass.Name, historyMetaClassName);

				//    MetaView newMetaView = DataContext.Current.MetaModel.CreateMetaView(metaViewName,
				//        historyMetaClassName, srcMetaView.FriendlyName, srcMetaView.AvailableFieldNames);

				//    FilterElement fe = FilterElement.EqualElement("ObjectId", "{QueryString:ObjectId}");
				//    fe.ValueIsTemplate = true;

				//    newMetaView.Filters.Add(fe);

				//    CopyMetaViewPrerences(srcMetaView, newMetaView);
				//}

				// Step 3. Copy [MC_GeneralViewForm]
				FormDocument srcFormDocument = FormDocument.Load(listMetaClass.Name, FormController.GeneralViewFormType);
				srcFormDocument.MetaClassName = historyMetaClassName;
				srcFormDocument.Name = FormController.GeneralViewHistoryFormType;
				srcFormDocument.MetaUITypeId = FormController.GeneralViewHistoryFormType;
				srcFormDocument.Id = null;
				srcFormDocument.Save();

				scope.SaveChanges();
			}
		}

		/// <summary>
		/// Deactivates the history.
		/// </summary>
		/// <param name="listMetaClassName">Name of the list meta class.</param>
		public static void DeactivateHistory(string listMetaClassName)
		{
			if (listMetaClassName == null)
				throw new ArgumentNullException("listMetaClassName");

			DeactivateHistory(DataContext.Current.GetMetaClass(listMetaClassName));
		}


		/// <summary>
		/// Deactivates the history.
		/// </summary>
		/// <param name="listMetaClass">The list meta class.</param>
		public static void DeactivateHistory(MetaClass listMetaClass)
		{
			if (listMetaClass == null)
				throw new ArgumentNullException("listMetaClass");

			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				string historyMetaClassName = HistoryManager.GetHistoryMetaClassName(listMetaClass.Name);

				// Step 0. Clean Up Data
				// Delete Attached McMetaViewPreferences
				//foreach (MetaView metaView in DataContext.Current.MetaModel.GetMetaViews(historyMetaClassName))
				//{
				//    UserMetaViewPreference.DeleteAll(metaView.Name);
				//}

				// Delete Form Documents
				foreach (FormDocument formDocument in FormDocument.GetFormDocuments(historyMetaClassName))
					formDocument.Delete();

				// Step 1. Service Uninstall
				BusinessObjectServiceManager.UninstallService(listMetaClass, HistoryService.ServiceName);

				// Step 2. Delete List_MetaView
				// Automatically

				scope.SaveChanges();
			}
		}

		/// <summary>
		/// Determines whether [is history activated] [the specified list meta class name].
		/// </summary>
		/// <param name="listMetaClassName">Name of the list meta class.</param>
		/// <returns>
		/// 	<c>true</c> if [is history activated] [the specified list meta class name]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsHistoryActivated(string listMetaClassName)
		{
			if (listMetaClassName == null)
				throw new ArgumentNullException("listMetaClassName");

			return IsHistoryActivated(DataContext.Current.GetMetaClass(listMetaClassName));
		}

		/// <summary>
		/// Determines whether [is history activated] [the specified list meta class].
		/// </summary>
		/// <param name="listMetaClass">The list meta class.</param>
		/// <returns>
		/// 	<c>true</c> if [is history activated] [the specified list meta class]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsHistoryActivated(MetaClass listMetaClass)
		{
			if (listMetaClass == null)
				throw new ArgumentNullException("listMetaClass");

			return BusinessObjectServiceManager.IsServiceInstalled(listMetaClass, HistoryService.ServiceName);
		}
		#endregion

		#region MetaClassIsList
		public static bool MetaClassIsList(string metaClassName)
		{
			if (metaClassName == null)
				throw new ArgumentNullException("metaClassName");

			return MetaClassIsList(DataContext.Current.GetMetaClass(metaClassName));
		}

		public static bool MetaClassIsList(MetaClass metaClass)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");

			string metaClassName = metaClass.Name;

			Match match = Regex.Match(metaClassName, @"List_\d+");

			return match.Success && (match.Index == 0) && (metaClassName.Length == match.Length);
		}
		#endregion

		#region CanReadFolder
		/// <summary>
		/// Determines whether this instance [can read folder] the specified folder id.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can read folder] the specified folder id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanReadFolder(int folderId)
		{
			if (folderId > 0)
				return CanReadFolder(new ListFolder(folderId));
			else
				return true;
		}

		public static bool CanReadFolder(ListFolder folder)
		{
			bool retval = false;

			int projectId = -1;
			bool isPrivate = (folder.FolderType == ListFolderType.Private);
			if (folder.ProjectId.HasValue)
				projectId = folder.ProjectId.Value;

			if (isPrivate)
				retval = (((RolePrincipal)(folder.Owner)).PrincipalId == Mediachase.IBN.Business.Security.CurrentUser.UserID);
			else if (projectId > 0)
				retval = Project.CanRead(projectId);
			else
				retval = true;

			return retval;
		}
		#endregion

		#region CanUpdateFolder
		/// <summary>
		/// Determines whether this instance [can update folder] the specified folder id.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can update folder] the specified folder id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanUpdateFolder(int folderId)
		{
			return CanUpdateFolder(new ListFolder(folderId));
		}

		public static bool CanUpdateFolder(ListFolder folder)
		{
			// The following users can update folder:
			// 1. Creator
			// 2. Admin (Public-folder only)
			// 3. PPM, Exec, Project manager (Project-folder only)

			int userId = Mediachase.IBN.Business.Security.CurrentUser.UserID;
			int projectId = -1;
			bool isPrivate = (folder.FolderType == ListFolderType.Private);
			if (folder.ProjectId.HasValue)
				projectId = folder.ProjectId.Value;
			int creatorId = folder.CreatorId;

			return creatorId == userId
				|| !isPrivate && projectId <= 0 && Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Administrator)
				|| !isPrivate && projectId > 0 && (Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) || Project.GetProjectManager(projectId) == userId);
		}
		#endregion

		#region CanDeleteFolder
		/// <summary>
		/// Determines whether this instance [can delete folder] the specified folder id.
		/// </summary>
		/// <param name="folderId">The folder id.</param>
		/// <returns>
		/// 	<c>true</c> if this instance [can delete folder] the specified folder id; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanDeleteFolder(int folderId)
		{
			return CanDeleteFolder(new ListFolder(folderId));
		}

		public static bool CanDeleteFolder(ListFolder folder)
		{
			bool retval = false;
			if (!folder.HasChildren && CanUpdateFolder(folder))
			{
				ListInfo[] lists = ListManager.GetLists(folder);
				if (lists == null || lists.Length == 0)
					retval = true;
			}

			return retval;
		}
		#endregion

		#region FilterListItems
		/// <summary>
		/// Filters the list items.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="keyword">The keyword.</param>
		/// <returns></returns>
		public static MetaObject[] FilterListItems(MetaObject[] items, string keyword)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			if (string.IsNullOrEmpty(keyword))
				return items;

			List<MetaObject> retVal = new List<MetaObject>();

			foreach (MetaObject srcItem in items)
			{
				bool itemContainsKeyword = false;

				// Object Property Contains keyword
				foreach (MetaObjectProperty property in srcItem.Properties)
				{
					if (property.Value == null)
						continue;

					// Value To String
					string strValue = property.Value.ToString();

					// Compare
					if (strValue.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) != -1)
					{
						itemContainsKeyword = true;
						break;
					}
				}

				if (itemContainsKeyword)
					retVal.Add(srcItem);
			}

			return retVal.ToArray();
		}
		#endregion

		#region CreateFilterByKeyword
		/// <summary>
		/// Creates the filter by keyword.
		/// </summary>
		/// <param name="metaClass">The meta class.</param>
		/// <param name="keyword">The keyword.</param>
		/// <returns></returns>
		public static FilterElement CreateFilterByKeyword(MetaClass metaClass, string keyword)
		{
			if (metaClass == null)
				throw new ArgumentNullException("metaClass");
			if (keyword == null)
				throw new ArgumentNullException("keyword");

			// O.R. [2008-08-21]: Wildcard chars replacing
			keyword = Regex.Replace(keyword, @"(\[|%|_)", "[$0]", RegexOptions.IgnoreCase);
			//

			StringBuilder sbQuery = new StringBuilder(255);

			foreach (MetaField mf in metaClass.Fields)
			{
				if (sbQuery.Length > 0)
					sbQuery.Append(" + N' ' + ");

				sbQuery.AppendFormat("ISNULL(CAST([{0}] AS NVARCHAR(4000)),N'')", mf.Name);

				if (mf.GetMetaType().McDataType == McDataType.File)
				{
					sbQuery.AppendFormat(" + N' ' + ISNULL(CAST([{0}_FileName] AS NVARCHAR(4000)), N'') + ' ' + ISNULL(CAST([{0}_Length] AS NVARCHAR(4000)),N'')", mf.Name);
				}
			}

			return new FilterElement(sbQuery.ToString(), FilterElementType.Contains, keyword);
		}
		#endregion

		#region GetListsCountByProject
		/// <summary>
		/// Gets the lists count by project.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <returns></returns>
		public static int GetListsCountByProject(int projectId)
		{
			return ListInfo.GetTotalCount(FilterElement.EqualElement("ProjectId", projectId));
		}
		#endregion

		#region UpdateList
		/// <summary>
		/// Updates the list.
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="title">The title.</param>
		/// <param name="description">The description.</param>
		/// <param name="listType">Type of the list.</param>
		/// <param name="status">The status.</param>
		/// <returns></returns>
		public static void UpdateList(ListInfo list, string title, string description, int listType, int status)
		{
			using (MetaClassManagerEditScope scope = DataContext.Current.MetaModel.BeginEdit())
			{
				// ListInfo
				bool updateMetaClass = false;
				if (list.Title != title)
					updateMetaClass = true;

				list.Title = title;
				list.Description = description;
				list.Properties["ListType"].Value = listType;
				list.Properties["Status"].Value = status;
				list.Save();

				if (updateMetaClass)
				{
					MetaClass mc = ListManager.GetListMetaClass(list);
					mc.FriendlyName = title;
					mc.PluralName = title;
				}

				scope.SaveChanges();
			}
		}
		#endregion

		#region GetListTitle
		/// <summary>
		/// Gets the list title.
		/// </summary>
		/// <param name="listId">The list id.</param>
		/// <returns></returns>
		public static string GetListTitle(int listId)
		{
			string retval = "";
			ListInfo li = new ListInfo(listId);
			if (li != null)
				retval = li.Title;
			return retval;
		}
		#endregion

		#region internal static void SavingList(ListInfo listInfo)
		internal static void SavingList(ListInfo listInfo)
		{
			CreateInfoEvent(ListSaving, listInfo);
		}
		#endregion
		#region internal static void SavedList(ListInfo listInfo)
		internal static void SavedList(ListInfo listInfo)
		{
			CreateInfoEvent(ListSaved, listInfo);
		}
		#endregion
	}

	public class InfoEventArgs : EventArgs
	{
		private ListInfo _listInfo;

		public ListInfo Info
		{
			get { return _listInfo; }
		}

		public InfoEventArgs(ListInfo listInfo)
		{
			_listInfo = listInfo;
		}
	}

	public class PublicationEventArgs : EventArgs
	{
		private ListPublication _listPublication;

		public ListPublication Publication
		{
			get { return _listPublication; }
		}

		public PublicationEventArgs(ListPublication listPublication)
		{
			_listPublication = listPublication;
		}
	}

}
