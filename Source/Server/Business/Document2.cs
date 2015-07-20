using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	public class Document2
	{
		// Private
		private const ObjectTypes OBJECT_TYPE = ObjectTypes.Document;
		#region VerifyCanUpdate()
		private static void VerifyCanUpdate(int objectId)
		{
			if(!Document.CanUpdate(objectId))
				throw new AccessDeniedException();
		}
		#endregion
		#region UpdateListResources()
		private static void UpdateListResources(int objectId, DataTable items, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(objectId);

			int managerId = -1;

			using(IDataReader reader = DBDocument.GetDocument(objectId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId) )
			{
				reader.Read();
				managerId = (int)reader["ManagerId"];
			}

			ArrayList oldItems = new ArrayList();
			using(IDataReader reader = DBDocument.GetListResources(objectId, Security.CurrentUser.TimeZoneId))
			{
				Common.LoadPrincipals(reader, oldItems);
			}

			ArrayList add = new ArrayList();
			ArrayList del = new ArrayList();
			foreach(DataRow row in items.Rows)
			{
				int id = (int)row["PrincipalId"];
				if(oldItems.Contains(id))
					oldItems.Remove(id);
				else
					add.Add(id);
			}

			del.AddRange(oldItems);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int id in del)
				{
					DBCommon.DeleteGate((int)OBJECT_TYPE, objectId, id);
					DBDocument.DeleteResource(objectId, id);

					// OZ: User Role Addon
					UserRoleHelper.DeleteDocumentResourceRole(objectId, id);
					if(id!=managerId)
						UserRoleHelper.DeleteDocumentManagerRole(objectId, id);
					//
					SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_ResourceList_AssignmentDeleted, objectId, id);
				}

				foreach(DataRow row in items.Rows)
				{
					int id = (int)row["PrincipalId"];
					bool mustBeConfirmed = (bool)row["MustBeConfirmed"];
					bool canManage = (bool)row["CanManage"];
					bool updated = true;

					if (add.Contains(id))
					{
						DbDocument2.ResourceAdd(objectId, id, mustBeConfirmed, canManage);
						if (User.IsExternal(id))
							DBCommon.AddGate((int)OBJECT_TYPE, objectId, id);
					}
					else
						updated = (0 < DbDocument2.ResourceUpdate(objectId, id, mustBeConfirmed, canManage));
					
					// OZ: User Role Addon
					if(canManage)
					{
						if(id!=managerId)
							UserRoleHelper.AddDocumentManagerRole(objectId, id);
					}
					else
						UserRoleHelper.AddDocumentResourceRole(objectId, id);
					// end

					if(updated)
					{
						if(mustBeConfirmed)
							SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_ResourceList_RequestAdded, objectId, id);
						else
							SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_ResourceList_AssignmentAdded, objectId, id);
					}
				}

				tran.Commit();
			}
		}
		#endregion

		// Public:
		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int documentId, string title, string description)
		{
			UpdateGeneralInfo(documentId, title, description, true);
		}

		internal static void UpdateGeneralInfo(int documentId, string title, string description, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(documentId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbDocument2.UpdateGeneralInfo(documentId, title, description);
				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_GeneralInfo, documentId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdatePriority()
		public static void UpdatePriority(int documentId, int priorityId)
		{
			UpdatePriority(documentId, priorityId, true);
		}

		internal static void UpdatePriority(int documentId, int priorityId, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(documentId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbDocument2.UpdatePriority(documentId, priorityId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_Priority, documentId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateStatus()
		public static void UpdateStatus(int documentId, int statusId)
		{
			UpdateStatus(documentId, statusId, true);
		}

		internal static void UpdateStatus(int documentId, int statusId, bool checkAccess)
		{
			if(checkAccess && !Document.CanUpdateStatus(documentId))
				throw new AccessDeniedException();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if(0 < DbDocument2.UpdateStatus(documentId, statusId))
					SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_Status, documentId);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateResources()
		public static void UpdateResources(int documentId, DataTable resources)
		{
			UpdateListResources(documentId, resources, true);
		}
		#endregion

		#region UpdateTimeLine()
		public static void UpdateTimeLine(int documentId, int taskTime)
		{
			UpdateTimeLine(documentId, taskTime, true);
		}

		internal static void UpdateTimeLine(int documentId, int taskTime, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(documentId);

			int projectId = Document.GetProject(documentId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				// O.R TODO: SystemEvent [2/28/2006]
				if (DbDocument2.UpdateTimeline(documentId, taskTime) > 0)
				{
					// O.R: Recalculating project TaskTime
					if (projectId > 0)
						TimeTracking.RecalculateProjectTaskTime(projectId);
				}

				tran.Commit();
			}
		}
		#endregion

		#region UpdateManager()
		public static void UpdateManager(int documentId, int managerId)
		{
			UpdateManager(documentId, managerId, true);
		}

		internal static void UpdateManager(int documentId, int managerId, bool checkAccess)
		{
			if (checkAccess)
				VerifyCanUpdate(documentId);

			int oldManagerId = DBDocument.GetManager(documentId);

			if (oldManagerId != managerId)
			{
				using (DbTransaction tran = DbTransaction.Begin())
				{
					DbDocument2.UpdateManager(documentId, managerId);

					// TODO: implement Document_Updated_Manager
					// SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_Manager, documentId);

					// OZ: User Role Addon
					if (managerId != oldManagerId)
					{
						UserRoleHelper.DeleteDocumentManagerRole(documentId, oldManagerId);
						UserRoleHelper.AddDocumentManagerRole(documentId, managerId);
					}
					// end OZ

					tran.Commit();
				}
			}
		}
		#endregion

		#region AcceptResource
		public static void AcceptResource(int documentId)
		{
			int userId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbDocument2.ResourceReply(documentId, userId, true);
				
				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_ResourceList_RequestAccepted, documentId, userId);

				tran.Commit();
			}
		}
		#endregion
		#region DeclineResource
		public static void DeclineResource(int documentId)
		{
			int userId = Security.CurrentUser.UserID;

			using(DbTransaction tran = DbTransaction.Begin())
			{
				DbDocument2.ResourceReply(documentId, userId, false);

				SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_ResourceList_RequestDenied, documentId, userId);

				tran.Commit();
			}
		}
		#endregion

		// Categories
		#region UpdateCategories()
		public static void UpdateCategories(ListAction action, int documentId, ArrayList categories)
		{
			UpdateCategories(action, documentId, categories, true);
		}

		internal static void UpdateCategories(ListAction action, int documentId, ArrayList categories, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(documentId);

			ArrayList oldCategories = new ArrayList();
			using(IDataReader reader = Document.GetListCategories(documentId))
			{
				Common.LoadCategories(reader, oldCategories);
			}
			Common.UpdateList(action, oldCategories, categories, OBJECT_TYPE, documentId, SystemEventTypes.Document_Updated_GeneralCategories, new UpdateListDelegate(Common.ListUpdate), null);
		}
		#endregion

		#region AddGeneralCategories()
		public static void AddGeneralCategories(int documentId, ArrayList categories)
		{
			UpdateCategories(ListAction.Add, documentId, categories);
		}
		#endregion
		#region RemoveGeneralCategories()
		public static void RemoveGeneralCategories(int documentId, ArrayList categories)
		{
			UpdateCategories(ListAction.Remove, documentId, categories);
		}
		#endregion
		#region SetGeneralCategories()
		public static void SetGeneralCategories(int documentId, ArrayList categories)
		{
			UpdateCategories(ListAction.Set, documentId, categories);
		}
		#endregion

		// Batch
		#region Update()
		public static void Update(int documentId, string title, string description,
			int priorityId, int managerId, int taskTime, ArrayList categories,
			PrimaryKeyId ContactUid, PrimaryKeyId OrgUid)
		{
			VerifyCanUpdate(documentId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				UpdateGeneralInfo(documentId, title, description, false);
				UpdatePriority(documentId, priorityId, false);
				UpdateManager(documentId, managerId, false);
				UpdateCategories(ListAction.Set, documentId, categories, false);
				UpdateTimeLine(documentId, taskTime, false);
				UpdateClient(documentId, ContactUid, OrgUid, false);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateClient()
		public static void UpdateClient(int DocumentId, PrimaryKeyId ContactUid, PrimaryKeyId OrgUid)
		{
			UpdateClient(DocumentId, ContactUid, OrgUid, true);
		}

		internal static void UpdateClient(int DocumentId, PrimaryKeyId ContactUid, PrimaryKeyId OrgUid, bool checkAccess)
		{
			if(checkAccess)
				VerifyCanUpdate(DocumentId);

			using(DbTransaction tran = DbTransaction.Begin())
			{
				if (0 < DbDocument2.UpdateClient(DocumentId, ContactUid == PrimaryKeyId.Empty ? null : (object)ContactUid, OrgUid == PrimaryKeyId.Empty ? null : (object)OrgUid))
				{
					// TODO:
					//SystemEvents.AddSystemEvents(SystemEventTypes.Document_Updated_Client, projectId);
				}

				tran.Commit();
			}
		}
		#endregion

	}
}
