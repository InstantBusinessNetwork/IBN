using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.Ibn.Service;
using Mediachase.IBN.Business.Import;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus.Import;
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.IbnNext.TimeTracking;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for WebServicesHelper.
	/// </summary>
	/// 
	public enum UserStatus
	{
		Offline = 0,
		Online = 1,
		Invisible = 2,
		Dnd = 3,
		Away = 4,
		Na = 5,
	}

	public class WebServicesHelper
	{
		public static int GetLanguageIdByLocale(string Locale)
		{
			int retVal = DBCommon.GetDefaultLanguage();

			using (IDataReader langReader = DBCommon.GetListLanguages())
			{
				while (langReader.Read())
				{
					string currLocale = (string)langReader["Locale"];
					int lanId = (int)langReader["LanguageId"];
					if (currLocale == Locale)
					{
						retVal = lanId;
						break;
					}
				}
				langReader.Close();
			}

			return retVal;
		}

		#region GetListPriorities
		/// <summary>
		/// Reader returns fields:
		///		PriorityId, PriorityName 
		/// </summary>
		public static IDataReader GetListPriorities(int LanguageId)
		{
			return DBCommon.GetListPriorities(LanguageId);
		}
		#endregion

		#region GetListTaskCompletionTypes
		/// <summary>
		/// Reader returns fields:
		///  CompletionTypeId, CompletionTypeName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTaskCompletionTypes(int LanguageId)
		{
			return DBTask.GetListCompletionTypes(LanguageId);
		}
		#endregion

		#region GetListToDoCompletionTypes
		/// <summary>
		/// Reader returns fields:
		///  CompletionTypeId, CompletionTypeName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDoCompletionTypes(int LanguageId)
		{
			return DBToDo.GetListCompletionTypes(LanguageId);
		}
		#endregion

		#region GetListProjectStatus
		/// <summary>
		/// 
		/// </summary>
		/// <param name="LanguageId"></param>
		/// <returns></returns>
		public static IDataReader GetListProjectStatus(int LanguageId)
		{
			return DBProject.GetListProjectStatus(LanguageId);
		}
		#endregion

		#region InsertXMLMethiods

		private static void InsertXMLItem(XmlNode xmlParentNode, string TagName, string TagValue)
		{
			XmlNode xmlNode = xmlParentNode.OwnerDocument.CreateElement(TagName);
			xmlNode.InnerText = TagValue;
			xmlParentNode.AppendChild(xmlNode);
		}

		private static void InsertDictionaryItem(XmlNode xmlParentNode, string Value, string Data)
		{
			XmlNode xmlDictionaryItemNode = xmlParentNode.OwnerDocument.CreateElement("Item");
			XmlNode xmlValueNode = xmlParentNode.OwnerDocument.CreateElement("Value");
			XmlNode xmlDataNode = xmlParentNode.OwnerDocument.CreateElement("Data");

			xmlValueNode.InnerText = Value;
			xmlDataNode.InnerText = Data;

			xmlDictionaryItemNode.AppendChild(xmlValueNode);
			xmlDictionaryItemNode.AppendChild(xmlDataNode);

			xmlParentNode.AppendChild(xmlDictionaryItemNode);
		}

		private static void InsertDictionaryItemList(XmlNode ownerNode, string GroupName, IDataReader reader, string ValueId, string DataId)
		{
			XmlNode xmlNewRootNode = ownerNode.OwnerDocument.CreateElement(GroupName);
			try
			{
				while (reader.Read())
				{
					InsertDictionaryItem(xmlNewRootNode, reader[ValueId].ToString(), reader[DataId].ToString());
				}
			}
			finally
			{
				reader.Close();
			}

			ownerNode.AppendChild(xmlNewRootNode);
		}

		private static void InsertDictionaryItemList(XmlNode ownerNode, string GroupName, EntityObject[] items, string TitleFieldName)
		{
			XmlNode xmlNewRootNode = ownerNode.OwnerDocument.CreateElement(GroupName);

			foreach(EntityObject item in items)
			{
				InsertDictionaryItem(xmlNewRootNode, item.PrimaryKeyId.Value.ToString(), item[TitleFieldName].ToString());
			}

			ownerNode.AppendChild(xmlNewRootNode);
		}

		private static void InsertOrganizationDictionaryItemList(XmlNode ownerNode, string GroupName)
		{
			XmlNode xmlNewRootNode = ownerNode.OwnerDocument.CreateElement(GroupName);

			EntityObject[] list = BusinessManager.List(OrganizationEntity.GetAssignedMetaClassName(), null,
				new Mediachase.Ibn.Data.SortingElement[] { Mediachase.Ibn.Data.SortingElement.Ascending("Name") });

			foreach (OrganizationEntity org in list)
			{
				InsertDictionaryItem(xmlNewRootNode, org.Name, org.PrimaryKeyId.ToString());
			}
			ownerNode.AppendChild(xmlNewRootNode);
		}

		private static void InsertContactDictionaryItemList(XmlNode ownerNode, string GroupName)
		{
			XmlNode xmlNewRootNode = ownerNode.OwnerDocument.CreateElement(GroupName);

			EntityObject[] list = BusinessManager.List(ContactEntity.GetAssignedMetaClassName(), null,
				new Mediachase.Ibn.Data.SortingElement[] { Mediachase.Ibn.Data.SortingElement.Ascending("FullName") });

			foreach (ContactEntity contact in list)
			{
				InsertDictionaryItem(xmlNewRootNode, contact.FullName, contact.PrimaryKeyId.ToString());
			}
			ownerNode.AppendChild(xmlNewRootNode);
		}

		private static void InsertTimeTrackingTemplatesDictionaryItemList(XmlNode ownerNode, string GroupName)
		{
			XmlNode xmlNewRootNode = ownerNode.OwnerDocument.CreateElement(GroupName);

			TimeTrackingBlockType[] list = TimeTrackingBlockType.List(Mediachase.Ibn.Data.FilterElement.EqualElement("IsProject", "1")); ;

			foreach (TimeTrackingBlockType item in list)
			{
				InsertDictionaryItem(xmlNewRootNode, item.Title, item.PrimaryKeyId.ToString());
			}
			ownerNode.AppendChild(xmlNewRootNode);
		}

		private static void InsertUserInformation(XmlNode xmlParentNode, int UserId)
		{
			//			<User>
			//				<UserId>
			//				<FirstName>
			//				<LastName>
			//				<Status> // Only if IM Present
			//			<User>
			using (IDataReader reader = Mediachase.IBN.Business.User.GetUserInfo(UserId))
			{
				if (reader.Read())
				{
					XmlNode xmlUserNode = xmlParentNode.OwnerDocument.CreateElement("User");

					XmlNode xmlUserIdNode = xmlParentNode.OwnerDocument.CreateElement("UserId");
					XmlNode xmlFirstNameNode = xmlParentNode.OwnerDocument.CreateElement("FirstName");
					XmlNode xmlLastNameNode = xmlParentNode.OwnerDocument.CreateElement("LastName");

					XmlNode xmlIsCurrentUserNode = xmlParentNode.OwnerDocument.CreateElement("IsCurrentUser");
					xmlIsCurrentUserNode.InnerText = true.ToString();

					xmlUserIdNode.InnerText = UserId.ToString();
					xmlFirstNameNode.InnerText = (string)reader["FirstName"];
					xmlLastNameNode.InnerText = (string)reader["LastName"];

					xmlUserNode.AppendChild(xmlUserIdNode);
					xmlUserNode.AppendChild(xmlFirstNameNode);
					xmlUserNode.AppendChild(xmlLastNameNode);

					if (UserId == Security.CurrentUser.UserID)
					{
						xmlUserNode.AppendChild(xmlIsCurrentUserNode);
					}

					XmlNode xmlUserStatus = xmlParentNode.OwnerDocument.CreateElement("Status");

					bool IsExternal = (bool)reader["IsExternal"];
					bool IsPending = (bool)reader["IsPending"];

					if (!IsExternal && !IsPending)
					{
						int _status = Mediachase.IBN.Business.User.GetUserStatus(UserId);

						xmlUserStatus.InnerText = ((UserStatus)_status).ToString();
						xmlUserNode.AppendChild(xmlUserStatus);
					}
					else if (IsExternal)
					{
						xmlUserStatus.InnerText = "external";
						xmlUserNode.AppendChild(xmlUserStatus);
					}
					else if (IsPending)
					{
						xmlUserStatus.InnerText = "pending";
						xmlUserNode.AppendChild(xmlUserStatus);
					}

					xmlParentNode.AppendChild(xmlUserNode);
				}
			}
		}
		#endregion

		#region UpdateCategoryList
		public static void UpdateCategoryList(int ObjectType, string UpdateList)
		{
			//////////////////////////////////////////////////////////////////////////
			/* 
					<Categories>
						<Category State="Added">
							<CategoryId/>	<!-- Not used -->
							<CategoryName/>
						</Category>
						<Category State="Modified">
							<CategoryId/>	
							<CategoryName/>
						</Category>
						<Category State="Deleted">
							<CategoryId/>
							<CategoryName/>
						</Category>
					</Categories>

				 */

			XmlDocument xmlCategoryList = new XmlDocument();
			xmlCategoryList.LoadXml(UpdateList);

			bool bWasCatchExceptions = false;
			using (DbTransaction tran = DbTransaction.Begin())
			{
				XmlNodeList AddedItems = xmlCategoryList.SelectNodes("Categories/Category[@State=\"Added\"]");
				XmlNodeList DeletedItems = xmlCategoryList.SelectNodes("Categories/Category[@State=\"Deleted\"]");
				XmlNodeList ModifiedItems = xmlCategoryList.SelectNodes("Categories/Category[@State=\"Modified\"]");

				switch ((ObjectTypes)ObjectType)
				{
					case ObjectTypes.Project:
						foreach (XmlNode modifiedItem in ModifiedItems)
						{
							try
							{
								int CategoryId = int.Parse(modifiedItem.SelectSingleNode("CategoryId").InnerText);
								string CategoryName = modifiedItem.SelectSingleNode("CategoryName").InnerText;
								Dictionaries.UpdateItem(CategoryId, CategoryName, DictionaryTypes.ProjectCategories);
							}
							catch (Exception)
							{
								bWasCatchExceptions = true;
							}
						}

						foreach (XmlNode addedItem in AddedItems)
						{
							try
							{
								string CategoryName = addedItem.SelectSingleNode("CategoryName").InnerText;
								Dictionaries.AddItem(CategoryName, DictionaryTypes.ProjectCategories);
							}
							catch (Exception)
							{
								bWasCatchExceptions = true;
							}
						}

						foreach (XmlNode deletedItem in DeletedItems)
						{
							try
							{
								int CategoryId = int.Parse(deletedItem.SelectSingleNode("CategoryId").InnerText);
								Dictionaries.DeleteItem(CategoryId, DictionaryTypes.ProjectCategories);
							}
							catch (Exception)
							{
								bWasCatchExceptions = true;
							}
						}
						break;
					case ObjectTypes.Issue:
						foreach (XmlNode modifiedItem in ModifiedItems)
						{
							try
							{
								int CategoryId = int.Parse(modifiedItem.SelectSingleNode("CategoryId").InnerText);
								string CategoryName = modifiedItem.SelectSingleNode("CategoryName").InnerText;
								Dictionaries.UpdateItem(CategoryId, CategoryName, DictionaryTypes.IncidentCategories);
							}
							catch (Exception)
							{
								bWasCatchExceptions = true;
							}
						}

						foreach (XmlNode addedItem in AddedItems)
						{
							try
							{
								string CategoryName = addedItem.SelectSingleNode("CategoryName").InnerText;
								Dictionaries.AddItem(CategoryName, DictionaryTypes.IncidentCategories);
							}
							catch (Exception)
							{
								bWasCatchExceptions = true;
							}
						}

						foreach (XmlNode deletedItem in DeletedItems)
						{
							try
							{
								int CategoryId = int.Parse(deletedItem.SelectSingleNode("CategoryId").InnerText);
								Dictionaries.DeleteItem(CategoryId, DictionaryTypes.IncidentCategories);
							}
							catch (Exception)
							{
								bWasCatchExceptions = true;
							}
						}
						break;
					default:
						throw new NotSupportedException();
				}

				tran.Commit();
			}

			if (bWasCatchExceptions)
				throw new Exception("During the update, Some changes weren't accepted.");
		}
		#endregion

		#region UpdateDictionaries
		public static void UpdateDictionaries(string Dictionary)
		{
			XmlDocument xmlDictionaries = new XmlDocument();
			xmlDictionaries.LoadXml(Dictionary);
			//			//////////////////////////////////////////////////////////////////////////
			//			/* 
			//				 <Dictionaries>
			//					<ProjectClient>
			//						<Item	State="Added">
			//							<Value>Cool Value</Value>
			//							<Data>12/<Data>
			//						</Item>
			//						<Item	State="Modified">
			//							<Value/>
			//							<Data/>
			//						</Item>
			//						<Item	State="Deleted">
			//							<Value/>
			//							<Data/>
			//						</Item>
			//					</ProjectClient>
			//				 </Dictionaries>	
			//				 */
			//
			bool bWasCatchExceptions = false;
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				//////////////////////////////////////////////////////////////////////////
				// ProjectClient [5/26/2004]
				XmlNodeList AddedItems = xmlDictionaries.SelectNodes("Dictionaries/ProjectClient/Item[@State=\"Added\"]");
				XmlNodeList DeletedItems = xmlDictionaries.SelectNodes("Dictionaries/ProjectClient/Item[@State=\"Deleted\"]");
				XmlNodeList ModifiedItems = xmlDictionaries.SelectNodes("Dictionaries/ProjectClient/Item[@State=\"Modified\"]");


				#region Update Project Client
				foreach (XmlNode modifiedItem in ModifiedItems)
				{
					//try
					//{
						//string clientId = modifiedItem.SelectSingleNode("Data").InnerText;
						//string clientName = modifiedItem.SelectSingleNode("Value").InnerText;

						//PrimaryKeyId pk = PrimaryKeyId.Parse(clientId);

						//try
						//{
						//    ContactEntity contact = (ContactEntity)BusinessManager.Load(ContactEntity.GetAssignedMetaClassName(), pk);
						//    contact.FullName = clientName;
						//    BusinessManager.Update(contact);
						//}
						//catch
						//{
						//    try
						//    {
						//        OrganizationEntity organization = (OrganizationEntity)BusinessManager.Load(OrganizationEntity.GetAssignedMetaClassName(), pk);
						//        organization.Name = clientName;
						//        BusinessManager.Update(organization);
						//    }
						//    catch 
						//    {
						//        bWasCatchExceptions = true;
						//    }
						//}
					//}
					//catch (Exception)
					//{
					//    bWasCatchExceptions = true;
					//}
				}
				#endregion

				#region Add Project Client
				foreach (XmlNode addedItem in AddedItems)
				{
					//try
					//{
					//    string ClientName = addedItem.SelectSingleNode("Value").InnerText;
					//    int ClientId = int.Parse(addedItem.SelectSingleNode("Data").InnerText);
					//    if (ClientId < 0)
					//    {
					//        // TODO: переделать на Mediachase.Ibn.Clients
					//        //Organization.AddOrganization(ClientName);
					//    }
					//    else
					//    {
					//        // TODO: переделать на Mediachase.Ibn.Clients
					//        //VCard.VCard card = VCard.VCard.Create();
					//        //card.FullName = ClientName;
					//        //VCard.VCard.Update(card);
					//    }
					//}
					//catch (Exception)
					//{
					//    bWasCatchExceptions = true;
					//}
				} 
				#endregion

				#region Delete Project Client
				foreach (XmlNode deletedItem in DeletedItems)
				{
					//try
					//{
					//    int ClientId = int.Parse(deletedItem.SelectSingleNode("Data").InnerText);
					//    if (ClientId < 0)
					//    {
					//        // TODO: переделать на Mediachase.Ibn.Clients
					//        //Organization.DeleteOrganization(Math.Abs(ClientId));
					//    }
					//    else
					//    {
					//        // TODO: переделать на Mediachase.Ibn.Clients
					//        //VCard.VCard.Delete(ClientId);
					//    }
					//}
					//catch (Exception)
					//{
					//    bWasCatchExceptions = true;
					//}
				} 
				#endregion
				//////////////////////////////////////////////////////////////////////////

				tran.Commit();
			}

			if (bWasCatchExceptions)
				throw new Exception("During the update, Some changes weren't accepted.");

		}
		#endregion

		#region LoadDictionaries
		public static string LoadDictionaries(string Locale, int ObjectType)
		{
			/************************************************************************/
			/* 
				  <Dictionaries>
					-- Common
					-- Current user
					<CurrentUser>
					</CurrentUser>
					<PortalConfig>
					</PortalConfig>
					<Priority>
						<Item>
							<Value/>
							<Data/>
						</Item>	
					</Priority>
					
					-- For Incident
					<IncidentType>
						...
					</IncidentType>
					<Severity>
						...
					</Severity>
					
					<IncidentOrganization>
						...
					</IncidentOrganization>

					<IncidentContact>
						...
					</IncidentContact>

					<IncidentBox>
					    ...
					</IncidentBox>
								
					-- For Event
					<EventType>
						...
					</EventType>
					
					<Organizers>
					</Organizers>
					
					<EventOrganization>
						...
					</EventOrganization>

					<EventContact>
						...
					</EventContact>
			
					-- For ToDo
					<CompletionType>
						...
					</CompletionType>
					
					<ToDoOrganization>
						...
					</ToDoOrganization>

					<ToDoContact>
						...
					</ToDoContact>
			
					-- For Task
					<CompletionType>
						...
					</CompletionType>
					
					-- For Project
					<ProjectType>
						...
					</ProjectType>

					<ProjectOrganization>
						...
					</ProjectOrganization>

					<ProjectContact>
						...
					</ProjectContact>
					
					<ProjectManager>
						<User>
							<UserId>
							<FirstName>
							<LastName>
						</User>
						...
					</ProjectManager>

					<ExecuteveManager>
						<User>
							<UserId>
							<FirstName>
							<LastName>
						</User>
						...
					</ExecuteveManager>
					
					<ProjectStatus>
						...
					</ProjectStatus>

					<ProjectCalendar>
						...
					</ProjectCalendar>

					<ProjectTemplate>
						...
					</ProjectTemplate>

					<ProjectPriority>
						...
					</ProjectPriority>

					<ProjectPortfolio>
						...
					</ProjectPortfolio>
					
					<ProjectPhase>
						...
					</ProjectPhase>
					-- For List
					<ListStatus>
						...
					</ListStatus>
					<ListType>
						...
					</ListType>
					<ListTemplate>
						...
					</ListTemplate>
				  </Dictionaries>
				  
				/************************************************************************/
			int LanguageId = WebServicesHelper.GetLanguageIdByLocale(Locale);

			XmlDocument xmlDictionariesList = new XmlDocument();
			xmlDictionariesList.LoadXml("<Dictionaries></Dictionaries>");

			XmlNode xmlDictionariesNode = xmlDictionariesList.SelectSingleNode("Dictionaries");

			// Step 1. Build Common Dictionaryes [2/9/2004]
			// Step 1.0 Current User information
			XmlNode xmlCurrentUserNode = xmlDictionariesList.CreateElement("CurrentUser");

			InsertXMLItem(xmlCurrentUserNode, "UserId", Security.CurrentUser.UserID.ToString());
			InsertXMLItem(xmlCurrentUserNode, "FirstName", Security.CurrentUser.FirstName);
			InsertXMLItem(xmlCurrentUserNode, "LastName", Security.CurrentUser.LastName);
			InsertXMLItem(xmlCurrentUserNode, "Email", Security.CurrentUser.Email);
			InsertXMLItem(xmlCurrentUserNode, "IsAdmin", Security.IsUserInGroup(InternalSecureGroups.Administrator).ToString());
			InsertXMLItem(xmlCurrentUserNode, "IsPM", Security.IsUserInGroup(InternalSecureGroups.ProjectManager).ToString());
			InsertXMLItem(xmlCurrentUserNode, "IsPPM", Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager).ToString());
			InsertXMLItem(xmlCurrentUserNode, "IsHDM", Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager).ToString());
			InsertXMLItem(xmlCurrentUserNode, "TimeOffset", User.GetCurrentBias(Security.CurrentUser.TimeZoneId).ToString());

			xmlDictionariesNode.AppendChild(xmlCurrentUserNode);

			// OZ: Portal Config 2008-07-18
			XmlNode xmlPortalConfigNode = xmlDictionariesList.CreateElement("PortalConfig");
			InsertXMLItem(xmlPortalConfigNode, "WorkTimeStart", PortalConfig.WorkTimeStart);
			InsertXMLItem(xmlPortalConfigNode, "WorkTimeFinish", PortalConfig.WorkTimeFinish);
			xmlDictionariesNode.AppendChild(xmlPortalConfigNode);


			// Step 1.1. Priority [2/9/2004]
			XmlNode xmlPriorityNode = xmlDictionariesList.CreateElement("Priority");
			using (IDataReader PriorityReader = WebServicesHelper.GetListPriorities(LanguageId))
			{
				while (PriorityReader.Read())
				{
					InsertDictionaryItem(xmlPriorityNode, PriorityReader["PriorityName"].ToString(), PriorityReader["PriorityId"].ToString());
				}
				PriorityReader.Close();
			}
			xmlDictionariesNode.AppendChild(xmlPriorityNode);

			// Step 2. Build Specific Dictionaries [2/9/2004]
			switch ((ObjectTypes)ObjectType)
			{
				case ObjectTypes.Issue:
					//  [2/9/2004]
					XmlNode xmlIncidentTypeNode = xmlDictionariesList.CreateElement("IncidentType");
					using (IDataReader incidentTypesReader = Incident.GetListIncidentTypes())
					{
						while (incidentTypesReader.Read())
						{
							InsertDictionaryItem(xmlIncidentTypeNode, incidentTypesReader["TypeName"].ToString(), incidentTypesReader["TypeId"].ToString());
						}
						incidentTypesReader.Close();
					}
					xmlDictionariesNode.AppendChild(xmlIncidentTypeNode);
					//  [2/9/2004]
					XmlNode xmlSeverityTypeNode = xmlDictionariesList.CreateElement("Severity");
					using (IDataReader severityTypesReader = Incident.GetListIncidentSeverity())
					{
						while (severityTypesReader.Read())
						{
							InsertDictionaryItem(xmlSeverityTypeNode, severityTypesReader["SeverityName"].ToString(), severityTypesReader["SeverityId"].ToString());
						}
						severityTypesReader.Close();
					}
					xmlDictionariesNode.AppendChild(xmlSeverityTypeNode);

					// IncidentOrganization [11/21/2006]
					InsertOrganizationDictionaryItemList(xmlDictionariesNode, "IncidentOrganization");

					// IncidentContacts [11/21/2006]
					InsertContactDictionaryItemList(xmlDictionariesNode, "IncidentContact");

					// IncidentBox [12/12/2006]
					XmlNode xmlIncidentBoxNode = xmlDictionariesList.CreateElement("IncidentBox");
					foreach (EMail.IncidentBox box in EMail.IncidentBox.List())
					{
						InsertDictionaryItem(xmlIncidentBoxNode, box.Name, box.IncidentBoxId.ToString());
					}
					xmlDictionariesNode.AppendChild(xmlIncidentBoxNode);

					break;
				case ObjectTypes.CalendarEntry:
					//  [2/9/2004]
					XmlNode xmlEventTypeNode = xmlDictionariesList.CreateElement("EventType");
					using (IDataReader eventTypesReader = CalendarEntry.GetListEventTypes())
					{
						while (eventTypesReader.Read())
						{
							InsertDictionaryItem(xmlEventTypeNode, eventTypesReader["TypeName"].ToString(), eventTypesReader["TypeId"].ToString());
						}
						eventTypesReader.Close();
					}
					xmlDictionariesNode.AppendChild(xmlEventTypeNode);

					// Organizers [5/6/2004]
					XmlNode xmlEventOrganizersNode = xmlDictionariesList.CreateElement("Organizers");
					using (IDataReader eventOrganizersReader = CalendarView.GetListPeopleForCalendar())
					{
						InsertUserInformation(xmlEventOrganizersNode, (int)Security.CurrentUser.UserID);

						while (eventOrganizersReader.Read())
						{
							if ((int)eventOrganizersReader["Level"] == 1)
								InsertUserInformation(xmlEventOrganizersNode, (int)eventOrganizersReader["UserId"]);
							//InsertDictionaryItem(xmlEventTypeNode,eventTypesReader["TypeName"].ToString(),eventTypesReader["TypeId"].ToString());
						}
						eventOrganizersReader.Close();
					}
					xmlDictionariesNode.AppendChild(xmlEventOrganizersNode);

					// EventOrganization [11/21/2006]
					InsertOrganizationDictionaryItemList(xmlDictionariesNode, "EventOrganization");

					// EventContacts [11/21/2006]
					InsertContactDictionaryItemList(xmlDictionariesNode, "EventContact");

					break;
				case ObjectTypes.Task:
					//  [2/9/2004]
					XmlNode xmlTaskCompTypeNode = xmlDictionariesList.CreateElement("CompletionType");
					using (IDataReader taskCompTypesReader = WebServicesHelper.GetListTaskCompletionTypes(LanguageId))
					{
						while (taskCompTypesReader.Read())
						{
							InsertDictionaryItem(xmlTaskCompTypeNode, taskCompTypesReader["CompletionTypeName"].ToString(), taskCompTypesReader["CompletionTypeId"].ToString());
						}
						taskCompTypesReader.Close();
					}
					xmlDictionariesNode.AppendChild(xmlTaskCompTypeNode);
					break;
				case ObjectTypes.ToDo:
					//  [2/9/2004]
					InsertDictionaryItemList(xmlDictionariesNode, "CompletionType", WebServicesHelper.GetListToDoCompletionTypes(LanguageId), "CompletionTypeName", "CompletionTypeId");

					// ToDoOrganization [11/21/2006]
					InsertOrganizationDictionaryItemList(xmlDictionariesNode, "ToDoOrganization");

					// ToDoContacts [11/21/2006]
					InsertContactDictionaryItemList(xmlDictionariesNode, "ToDoContact");

					break;
				case ObjectTypes.Project:
					if (!Project.CanCreate())
						throw new AccessDeniedException();
					// ProjectType [3/30/2004]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectType", Project.GetListProjectTypes(), "TypeName", "TypeId");

					// ProjectClient [3/30/2004]
					//InsertDictionaryItemList(xmlDictionariesNode,"ProjectClient", Project.GetListClients(),"ClientName", "ClientId");

					// ProjectOrganization [11/21/2006]
					InsertOrganizationDictionaryItemList(xmlDictionariesNode, "ProjectOrganization");

					// ProjectContacts [11/21/2006]
					InsertContactDictionaryItemList(xmlDictionariesNode, "ProjectContact");

					// ProjectStatus [3/30/2004]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectStatus", WebServicesHelper.GetListProjectStatus(LanguageId), "StatusName", "StatusId");

					// ProjectCalendar [3/30/2004]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectCalendar", Project.GetListCalendars(0), "CalendarName", "CalendarId");

					// ProjectCurrency [3/30/2004]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectCurrency", Project.GetListCurrency(), "CurrencySymbol", "CurrencyId");

					// ProjectManager [3/30/2004]
					XmlNode xmlManagersNode = xmlDictionariesList.CreateElement("ProjectManager");

					if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
					{
						using (IDataReader iManagers = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager))
						{
							while (iManagers.Read())
							{
								InsertUserInformation(xmlManagersNode, (int)iManagers["UserId"]);
							}
							iManagers.Close();
						}
					}
					else
					{
						InsertUserInformation(xmlManagersNode, Security.CurrentUser.UserID);
					}
					xmlDictionariesNode.AppendChild(xmlManagersNode);

					// ExecutiveManager [3/30/2004]
					XmlNode xmlExManagersNode = xmlDictionariesList.CreateElement("ExecutiveManager");
					using (IDataReader iExManagers = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager))
					{
						while (iExManagers.Read())
						{
							InsertUserInformation(xmlExManagersNode, (int)iExManagers["UserId"]);
						}
						iExManagers.Close();
					}
					xmlDictionariesNode.AppendChild(xmlExManagersNode);

					// Templates [3/17/2005]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectTemplate", ProjectTemplate.GetListProjectTemplate(), "TemplateName", "TemplateId");

					// Priority [3/17/2005]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectPriority", Project.GetListPriorities(), "PriorityName", "PriorityId");

					// Portfolio [3/17/2005]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectPortfolio", ProjectGroup.GetProjectGroups(), "Title", "ProjectGroupId");

					// Phases [3/24/2005]
					InsertDictionaryItemList(xmlDictionariesNode, "ProjectPhase", Project.GetListProjectPhases(), "PhaseName", "PhaseId");

					// TimeTrackingTemplates [2008-10-29]
					InsertTimeTrackingTemplatesDictionaryItemList(xmlDictionariesNode, "TimeTrackingTemplates");

					break;
				case ObjectTypes.List:
					/*
					// Statuses [04/11/2005]
					InsertDictionaryItemList(xmlDictionariesNode, "ListStatus", List.GetListStatus(), "StatusName", "StatusId");

					// Types [04/11/2005]
					InsertDictionaryItemList(xmlDictionariesNode, "ListType", List.GetListType(), "TypeName", "TypeId");

					// Templates [04/11/2005]	// TODO: move to business and use InsertDictionaryItemList instead
					XmlNode xmlListTemplateRootNode = xmlDictionariesNode.OwnerDocument.CreateElement("ListTemplate");
					Mediachase.MetaDataPlus.Configurator.MetaClassCollection mcc = Mediachase.MetaDataPlus.Configurator.MetaClass.GetList(Mediachase.IBN.Business.List.ListTemplateRoot, true);
					foreach (Mediachase.MetaDataPlus.Configurator.MetaClass mc in mcc)
					{
						XmlNode xmlDictionaryItemNode = xmlListTemplateRootNode.OwnerDocument.CreateElement("Item");
						XmlNode xmlValueNode = xmlListTemplateRootNode.OwnerDocument.CreateElement("Value");
						XmlNode xmlDataNode = xmlListTemplateRootNode.OwnerDocument.CreateElement("Data");

						xmlValueNode.InnerText = mc.FriendlyName;
						xmlDataNode.InnerText = mc.Id.ToString();

						xmlDictionaryItemNode.AppendChild(xmlValueNode);
						xmlDictionaryItemNode.AppendChild(xmlDataNode);

						xmlListTemplateRootNode.AppendChild(xmlDictionaryItemNode);
					}
					xmlDictionariesNode.AppendChild(xmlListTemplateRootNode);
					 */

					break;
			}

			return xmlDictionariesList.InnerXml;
		}

		#endregion

		#region GetCategoryList
		public static string GetCategoryList()
		{
			//////////////////////////////////////////////////////////////////////////
			/*
					<Categories>
						<Category>
							<CategoryId/>
							<CategoryName/>
						</Category>
					</Categories>
				 */

			XmlDocument xmlCategoryList = new XmlDocument();
			xmlCategoryList.LoadXml("<Categories></Categories>");

			XmlNode xmlCategoriesNode = xmlCategoryList.SelectSingleNode("Categories");

			// Get User List for Group [1/27/2004]
			using (IDataReader categoryReader = Task.GetListCategoriesAll())
			{
				while (categoryReader.Read())
				{
					XmlNode xmlCategoryNode = xmlCategoryList.CreateElement("Category");
					//////////////////////////////////////////////////////////////////////////
					XmlElement CategoryIdNode = xmlCategoryList.CreateElement("CategoryId");
					XmlElement CategoryNameNode = xmlCategoryList.CreateElement("CategoryName");

					CategoryIdNode.InnerText = categoryReader["CategoryId"].ToString();
					CategoryNameNode.InnerText = categoryReader["CategoryName"].ToString();
					//////////////////////////////////////////////////////////////////////////
					xmlCategoryNode.AppendChild(CategoryIdNode);
					xmlCategoryNode.AppendChild(CategoryNameNode);

					xmlCategoriesNode.AppendChild(xmlCategoryNode);
				}
			}

			//////////////////////////////////////////////////////////////////////////
			return xmlCategoryList.InnerXml;
		}
		#endregion

		#region GetCategoryReader
		static IDataReader GetCategoryReader(ObjectTypes type)
		{
			switch (type)
			{
				case ObjectTypes.Project:
					return Project.GetListProjectCategories();
				case ObjectTypes.Issue:
					return Incident.GetListIncidentCategories();
				default:
					return Task.GetListCategoriesAll();
			}
		}
		#endregion

		#region GetCategoryList2
		public static string GetCategoryList2(int ObjectType)
		{
			//////////////////////////////////////////////////////////////////////////
			/*
					<Categories>
						<Category>
							<CategoryId/>
							<CategoryName/>
						</Category>
					</Categories>
				 */

			XmlDocument xmlCategoryList = new XmlDocument();
			xmlCategoryList.LoadXml("<Categories></Categories>");

			XmlNode xmlCategoriesNode = xmlCategoryList.SelectSingleNode("Categories");

			// Skip List
			if(ObjectTypes.List == (ObjectTypes)ObjectType)
				return xmlCategoryList.InnerXml;

			// Get User List for Group [1/27/2004]
			using (IDataReader categoryReader = GetCategoryReader((ObjectTypes)ObjectType))
			{
				while (categoryReader.Read())
				{
					XmlNode xmlCategoryNode = xmlCategoryList.CreateElement("Category");
					//////////////////////////////////////////////////////////////////////////
					XmlElement CategoryIdNode = xmlCategoryList.CreateElement("CategoryId");
					XmlElement CategoryNameNode = xmlCategoryList.CreateElement("CategoryName");

					CategoryIdNode.InnerText = categoryReader["CategoryId"].ToString();
					CategoryNameNode.InnerText = categoryReader["CategoryName"].ToString();
					//////////////////////////////////////////////////////////////////////////
					xmlCategoryNode.AppendChild(CategoryIdNode);
					xmlCategoryNode.AppendChild(CategoryNameNode);

					xmlCategoriesNode.AppendChild(xmlCategoryNode);
				}
			}

			//////////////////////////////////////////////////////////////////////////

			return xmlCategoryList.InnerXml;

		}
		#endregion

		#region GetRoleList
		public static string GetRoleList(int TemplateId)
		{
			XmlDocument xmlRoleList = new XmlDocument();
			xmlRoleList.LoadXml("<Roles></Roles>");

			XmlNode xmlRolesNode = xmlRoleList.SelectSingleNode("Roles");

			DataTable dt = Task.MakeProjectAssignments(TemplateId);
			foreach (DataRow row in dt.Rows)
			{
				XmlNode xmlRoleNode = xmlRoleList.CreateElement("Role");

				XmlElement RoleIdNode = xmlRoleList.CreateElement("RoleId");
				XmlElement RoleNameNode = xmlRoleList.CreateElement("RoleName");

				RoleIdNode.InnerText = row["RoleId"].ToString();
				RoleNameNode.InnerText = row["RoleName"].ToString();

				xmlRoleNode.AppendChild(RoleIdNode);
				xmlRoleNode.AppendChild(RoleNameNode);

				xmlRolesNode.AppendChild(xmlRoleNode);
			}
			return xmlRoleList.InnerXml;
		}
		#endregion

		#region CheckFileName
		public static bool CheckFileName(int ObjectTypeId, int ObjectId, string FileName)
		{
			// !! code duplication
			string ContainerName = "FileLibrary";
			string ContainerKey = String.Empty;
			int folderId = 0;

			switch ((ObjectTypes)ObjectTypeId)
			{
				case ObjectTypes.Project:
					ContainerKey = "ProjectId_" + ObjectId.ToString();
					break;
				case ObjectTypes.Issue:
					ContainerKey = "IncidentId_" + ObjectId.ToString();
					break;
				case ObjectTypes.Task:
					ContainerKey = "TaskId_" + ObjectId.ToString();
					break;
				case ObjectTypes.CalendarEntry:
					ContainerKey = "EventId_" + ObjectId.ToString();
					break;
				case ObjectTypes.Folder:
					ContainerKey = "Workspace";
					if (ObjectId != 0)
					{
						folderId = ObjectId;
						ContainerKey = Mediachase.IBN.Business.ControlSystem.DirectoryInfo.GetContainerKey(folderId);
					}
					break;
				case ObjectTypes.Document:
					ContainerKey = "DocumentId_" + ObjectId.ToString();
					break;
				case ObjectTypes.ToDo:
					ContainerKey = "ToDoId_" + ObjectId.ToString();
					break;
			}
			if (ContainerKey != String.Empty)
			{
				Mediachase.IBN.Business.ControlSystem.BaseIbnContainer bic = Mediachase.IBN.Business.ControlSystem.BaseIbnContainer.Create(ContainerName, ContainerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

				if (folderId == 0)
					folderId = fs.Root.Id;

				return fs.FileExist(FileName, folderId);
			}
			else return false;
		}
		#endregion
	}
}
