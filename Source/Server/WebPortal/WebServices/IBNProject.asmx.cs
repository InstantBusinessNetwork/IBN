using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Services;
using System.Runtime.Serialization;
using System.Web.Services.Protocols;
using System.Xml;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Lists;
using IbnServices = Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Web.UI;
using System.Threading;
using System.Globalization;
using Mediachase.Ibn.Lists.Mapping;
using System.Collections.Generic;
using Mediachase.Ibn.Service;
using System.Configuration;
using Mediachase.IBN.Business.EMail;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn;

namespace Mediachase.UI.Web.WebServices
{
	public enum UserStatus
	{
		Offline = 0,
		Online = 1,
		Invisible = 2,
		Dnd = 3,
		Away = 4,
		Na = 5,
	}

	/// <summary>
	/// Summary description for IBNProject.
	/// </summary>
	[WebService(Namespace = "http://mediachase.com/webservices/")]
	public class IBNProject : System.Web.Services.WebService
	{
		private string DateTimeFormatString = "yyyy-MM-ddTHH:mm:ss";

		public class Authenticator : System.Web.Services.Protocols.SoapHeader
		{
			public string userName;
			public string userPassword;
		}

		public class ErrMsg : System.Web.Services.Protocols.SoapHeader
		{
			public string msg;
		}

		public Authenticator objAuth;
		public ErrMsg errMsg;

		public IBNProject()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code

		//Required by the Web Services Designer 
		private IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion


		#region Authenticate Function
		private void DebugAuthenticate(string UserName, string UserPassword)
		{
			try
			{
				string sUserLight = "userlight";

				// check user's name and password here
				UserLight currentUser = Security.GetUser(UserName, UserPassword);
				if (currentUser == null)
					throw new UserNotAuthenticatedException("Exception in user authentication");

				if (HttpContext.Current.Items.Contains(sUserLight))
					HttpContext.Current.Items.Remove(sUserLight);

				HttpContext.Current.Items.Add(sUserLight, currentUser);
				HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(currentUser.Login), null);
			}
			catch
			{
				throw new UserNotAuthenticatedException("Exception in user authentication");
			}
		}

		private void Authenticate()
		{
			try
			{
				string sUserLight = "userlight";

				// check user's name and password here
				UserLight currentUser = Security.GetUser(objAuth.userName, objAuth.userPassword);
				if (currentUser == null)
					throw new UserNotAuthenticatedException("Exception in user authentication");

				if (HttpContext.Current.Items.Contains(sUserLight))
					HttpContext.Current.Items.Remove(sUserLight);

				HttpContext.Current.Items.Add(sUserLight, currentUser);
				HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(currentUser.Login), null);

				DataContext.Current.CurrentUserId = Security.CurrentUser.UserID;
				DataContext.Current.CurrentUserTimeZone = Security.CurrentUser.CurrentTimeZone;
			}
			catch
			{
				throw new UserNotAuthenticatedException("Exception in user authentication");
			}
		}
		#endregion

		#region		IBNPubWiz Functions

		private void RecursiveCreateXML(Mediachase.IBN.Business.ControlSystem.FileStorage fs, XmlNode currParent, int FolderID, bool bIncludeAsset, int DeepLevel, int CurDeepLevel, int AssetId)
		{
			Mediachase.IBN.Business.ControlSystem.DirectoryInfo[] infos = fs.GetDirectories(FolderID);
			foreach (Mediachase.IBN.Business.ControlSystem.DirectoryInfo info in infos)
			{
				XmlNode folderItem = InsertFolderInformation(fs, currParent, info, info.Name, AssetId);

				if (folderItem != null)
				{
					if (DeepLevel <= 0 || DeepLevel > (CurDeepLevel + 1))
					{
						XmlNode childFolderItemNode = folderItem.SelectSingleNode("ChildList");

						RecursiveCreateXML(fs, childFolderItemNode, info.Id, bIncludeAsset, DeepLevel, CurDeepLevel + 1, AssetId);
					}
				}
			}
			if (bIncludeAsset)
			{
				Mediachase.IBN.Business.ControlSystem.FileInfo[] files = fs.GetFiles(FolderID);
				foreach (Mediachase.IBN.Business.ControlSystem.FileInfo file in files)
				{
					InsertAssetInformation(currParent, FolderID, file);
				}
			}
		}
		/*
		private void RecursiveCreateXML(Mediachase.IBN.Business.ControlSystem.FileStorage fs, XmlNode currParent, Mediachase.IBN.Business.ControlSystem.DirectoryInfo directory, bool bIncludeAsset, int DeepLevel, int CurDeepLevel)
		{
			XmlNode folderItem = InsertFolderInformation(fs, currParent, directory);

			Mediachase.IBN.Business.ControlSystem.DirectoryInfo[] infos = fs.GetDirectories(directory);
			foreach(Mediachase.IBN.Business.ControlSystem.DirectoryInfo info in infos)
			{
				if(DeepLevel<=0 || DeepLevel>(CurDeepLevel+1))
				{
					XmlNode childFolderItemNode = folderItem.SelectSingleNode("ChildList");
		
					RecursiveCreateXML(fs, childFolderItemNode,info, bIncludeAsset,DeepLevel, CurDeepLevel + 1);
				}
			}
			if (bIncludeAsset)
			{
				Mediachase.IBN.Business.ControlSystem.FileInfo[] files = fs.GetFiles(directory);
				foreach(Mediachase.IBN.Business.ControlSystem.FileInfo file in files)
				{
					InsertAssetInformation(currParent,directory.Id,file);
				}
			}
		}
		*/
		private XmlNode InsertFolderInformation(Mediachase.IBN.Business.ControlSystem.FileStorage fs, XmlNode parentFolderNode, Mediachase.IBN.Business.ControlSystem.DirectoryInfo info, string Title, int AssetId)
		{
			if (fs.CanUserRead(info.Id))
			{
				XmlNode folderNode = parentFolderNode.OwnerDocument.CreateElement("Folder");

				XmlNode folderIDNode = parentFolderNode.OwnerDocument.CreateElement("Id");
				folderIDNode.InnerText = info.Id.ToString();

				XmlNode folderNameNode = parentFolderNode.OwnerDocument.CreateElement("Name");
				folderNameNode.InnerText = Title;

				XmlNode canUserWriteNode = parentFolderNode.OwnerDocument.CreateElement("CanUserWrite");
				//canUserWriteNode.InnerText = Folder.CanWrite(FolderID).ToString();
				canUserWriteNode.InnerText = fs.CanUserWrite(info.Id).ToString();

				XmlNode assetNode = parentFolderNode.OwnerDocument.CreateElement("AssetId");
				assetNode.InnerText = AssetId.ToString();

				XmlNode childNode = parentFolderNode.OwnerDocument.CreateElement("ChildList");

				folderNode.AppendChild(folderIDNode);
				folderNode.AppendChild(folderNameNode);
				folderNode.AppendChild(canUserWriteNode);
				folderNode.AppendChild(assetNode);
				folderNode.AppendChild(childNode);

				parentFolderNode.AppendChild(folderNode);

				return folderNode;
			}
			else return null;
		}

		private void InsertAssetInformation(XmlNode parentFolderNode, int FolderID, Mediachase.IBN.Business.ControlSystem.FileInfo file)
		{
			XmlNode assetNode = parentFolderNode.OwnerDocument.CreateElement("Asset");

			XmlNode assetGeneralIDNode = parentFolderNode.OwnerDocument.CreateElement("Id");
			assetGeneralIDNode.InnerText = file.Id.ToString();

			XmlNode assetIDNode = parentFolderNode.OwnerDocument.CreateElement("VersionId");
			assetIDNode.InnerText = file.Id.ToString();

			XmlNode assetTitleNode = parentFolderNode.OwnerDocument.CreateElement("Title");
			assetTitleNode.InnerText = Path.GetFileNameWithoutExtension(file.Name);

			XmlNode assetDescriptionNode = parentFolderNode.OwnerDocument.CreateElement("Description");
			assetDescriptionNode.InnerText = file.Name;

			XmlNode assetCreateNode = parentFolderNode.OwnerDocument.CreateElement("CreateDate");
			assetCreateNode.InnerText = file.Created.ToString();

			XmlNode assetCreatedByNode = parentFolderNode.OwnerDocument.CreateElement("CreatedBy");
			assetCreatedByNode.InnerText = Mediachase.IBN.Business.User.GetUserName(file.CreatorId);

			XmlNode assetIsInternalNode = parentFolderNode.OwnerDocument.CreateElement("Internal");
			assetIsInternalNode.InnerText = true.ToString();	// TODO: ???

			XmlNode assetUrlNode = parentFolderNode.OwnerDocument.CreateElement("Url");
			assetUrlNode.InnerText = "";						// TODO: ???

			XmlNode assetFileNameNode = parentFolderNode.OwnerDocument.CreateElement("FileName");
			assetFileNameNode.InnerText = file.Name;

			XmlNode assetContentTypeNode = parentFolderNode.OwnerDocument.CreateElement("ContentType");
			assetContentTypeNode.InnerText = file.FileBinaryContentType;

			XmlNode assetSizeNode = parentFolderNode.OwnerDocument.CreateElement("Size");
			assetSizeNode.InnerText = file.Length.ToString();

			assetNode.AppendChild(assetGeneralIDNode);
			assetNode.AppendChild(assetIDNode);
			assetNode.AppendChild(assetTitleNode);
			assetNode.AppendChild(assetDescriptionNode);
			assetNode.AppendChild(assetCreateNode);
			assetNode.AppendChild(assetCreatedByNode);
			assetNode.AppendChild(assetIsInternalNode);
			assetNode.AppendChild(assetUrlNode);
			assetNode.AppendChild(assetFileNameNode);
			assetNode.AppendChild(assetContentTypeNode);
			assetNode.AppendChild(assetSizeNode);

			parentFolderNode.AppendChild(assetNode);
		}

		#endregion

		[WebMethod]
		public string GetServerVersion()
		{
			return IbnConst.VersionMajorDotMinor;
		}

		[WebMethod]
		public int GetServerVersionInt()
		{
			return 470;
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetIBNProjectList()
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				/*
				 	
				 <Projects>
					<Project>
						<!-- IBN Compatibility 3.0 -->
						<ObjectID/>
						<Title/>
						<DateCreated/>
						<Creator/>
						<StartDate/>
						<EndDate/>
						<ManagerCanonName/>
						<Description/>
						<IsActive/>
					</Project>
					...
				<Projects>
				 */

				XmlDocument xmlProjectList = new XmlDocument();

				xmlProjectList.LoadXml("<Projects></Projects>");

				XmlNode xmlProjectsNode = xmlProjectList.SelectSingleNode("Projects");

				using (IDataReader projectList = Project.GetListProjectsForPMAndPPM())
				{
					while (projectList.Read())
					{
						// Display only active project list
						if (!(bool)projectList["IsActive"])
							continue;

						XmlElement projectNode = xmlProjectList.CreateElement("Project");

						//////////////////////////////////////////////////////////////////////////
						XmlElement ObjectIDNode = xmlProjectList.CreateElement("ObjectID");
						XmlElement TitleNode = xmlProjectList.CreateElement("Title");
						XmlElement DateCreatedNode = xmlProjectList.CreateElement("DateCreated");
						XmlElement CreatorNode = xmlProjectList.CreateElement("Creator");
						XmlElement StartDateNode = xmlProjectList.CreateElement("StartDate");
						XmlElement EndDateNode = xmlProjectList.CreateElement("EndDate");
						XmlElement ManagerCanonNameNode = xmlProjectList.CreateElement("ManagerCanonName");
						XmlElement DescriptionNode = xmlProjectList.CreateElement("Description");
						XmlElement IsMSProjectNode = xmlProjectList.CreateElement("IsMSProject");
						XmlElement IsActiveNode = xmlProjectList.CreateElement("IsActive");

						//////////////////////////////////////////////////////////////////////////
						ObjectIDNode.InnerText = projectList["ProjectId"].ToString();
						TitleNode.InnerText = projectList["Title"].ToString();
						DateCreatedNode.InnerText = XmlConvert.ToString((DateTime)projectList["CreationDate"], DateTimeFormatString);

						UserLight creator = UserLight.Load((int)projectList["CreatorId"]);
						CreatorNode.InnerText = string.Format("{0} {1}", creator.FirstName, creator.LastName);

						StartDateNode.InnerText = XmlConvert.ToString((DateTime)projectList["TargetStartDate"], DateTimeFormatString);
						EndDateNode.InnerText = XmlConvert.ToString((DateTime)projectList["TargetFinishDate"], DateTimeFormatString);

						UserLight manager = UserLight.Load((int)projectList["ManagerId"]);
						ManagerCanonNameNode.InnerText = string.Format("{0} {1}", manager.FirstName, manager.LastName);

						DescriptionNode.InnerText = projectList["Description"].ToString();

						IsMSProjectNode.InnerText = projectList["IsMSProject"].ToString();
						IsActiveNode.InnerText = projectList["IsActive"].ToString();

						//////////////////////////////////////////////////////////////////////////
						projectNode.AppendChild(ObjectIDNode);
						projectNode.AppendChild(TitleNode);
						projectNode.AppendChild(DateCreatedNode);
						projectNode.AppendChild(CreatorNode);
						projectNode.AppendChild(DateCreatedNode);
						projectNode.AppendChild(StartDateNode);
						projectNode.AppendChild(EndDateNode);
						projectNode.AppendChild(ManagerCanonNameNode);
						projectNode.AppendChild(DescriptionNode);
						projectNode.AppendChild(IsMSProjectNode);
						projectNode.AppendChild(IsActiveNode);

						//////////////////////////////////////////////////////////////////////////
						xmlProjectsNode.AppendChild(projectNode);
					}
				}

				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return xmlProjectList.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}


		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string ExportIBNProject(int ProjectID)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				string ProjectXML;

				if (Project.GetIsMSProject(ProjectID))
				{
					//////////////////////////////////////////////////////////////////////////
					//  [4/14/2008]
					ProjectXML = Task.TaskExport2(ProjectID).InnerXml;
					//////////////////////////////////////////////////////////////////////////
				}
				else
				{
					//////////////////////////////////////////////////////////////////////////
					//  [1/19/2004]
					ProjectXML = IBNPrj2XML.IBN2XML(ProjectID).InnerXml;
					//////////////////////////////////////////////////////////////////////////
				}

				errMsg.msg = "OK";
				return ProjectXML;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		/// <summary>
		/// Imports the ms project.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <param name="MsProjectImportXml">The ms project import XML.</param>
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void ImportMsProject(int ProjectId, string MsProjectImportXml)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				bool bSyncMode = false;
				if (ProjectId < 0)
				{
					ProjectId *= -1;
					bSyncMode = true;
				}

				if (Project.GetIsMSProject(ProjectId) || bSyncMode)
				{
					/////////////////////////////////////////////////////////////////////////
					// OlegO [4/14/2008]
					System.IO.MemoryStream memStream = new System.IO.MemoryStream();
					System.IO.StreamWriter writer = new System.IO.StreamWriter(memStream);

					writer.Write(MsProjectImportXml);
					memStream.Seek(0, System.IO.SeekOrigin.Begin);

					int dataFileId = Project.UploadImportXML(ProjectId, "ImportIBNProject.xml", memStream);

					DataTable taskAssignments = Task.TaskImportAssignments(ProjectId, dataFileId);

					Task.TasksImport2(taskAssignments, ProjectId, dataFileId, CompletionType.All, true);
					//////////////////////////////////////////////////////////////////////////
				}
				else
				{
					//////////////////////////////////////////////////////////////////////////
					// OlegO [4/29/2004]
					System.IO.MemoryStream memStream = new System.IO.MemoryStream();
					System.IO.StreamWriter writer = new System.IO.StreamWriter(memStream);

					writer.Write(MsProjectImportXml);
					memStream.Seek(0, System.IO.SeekOrigin.Begin);

					int XMLVersionID = Project.UploadImportXML(ProjectId, "ImportIBNProject.xml", memStream);

					DataTable taskAssignments = Task.TaskImportAssignments(ProjectId, XMLVersionID);

					Task.TasksImport(taskAssignments, ProjectId, XMLVersionID);
					//////////////////////////////////////////////////////////////////////////
				}

				errMsg.msg = "OK";
				return;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string FileLibraryDir(int FolderID, bool bIncludeAsset, int DeepLevel)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml("<DirList><Projects/></DirList>");
				XmlNode dirlistNode = xmlDoc.SelectSingleNode("DirList");
				XmlNode dirlistNodePrj = xmlDoc.SelectSingleNode("DirList/Projects");

				string ContainerName = "FileLibrary";
				string ContainerKey = "Workspace";

				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

				Mediachase.IBN.Business.ControlSystem.DirectoryInfo dir = fs.Root;

				if (FolderID != 0)
					dir = fs.GetDirectory(FolderID);

				// Recursive Create XML
				RecursiveCreateXML(fs, dirlistNode, dir.Id, bIncludeAsset, DeepLevel, 0, -1);

				if (FolderID == 0)
				{
					DataTable dt = Project.GetListActiveProjectsByUserDataTable();
					foreach (DataRow dr in dt.Select(string.Empty, "Title ASC"))
					{
						ContainerKey = Mediachase.IBN.Business.UserRoleHelper.CreateProjectContainerKey((int)dr["ProjectId"]);
						bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
						fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

						dir = fs.Root;

						XmlNode folderItem = InsertFolderInformation(fs, dirlistNodePrj, dir, dr["Title"].ToString(), (int)dr["ProjectId"]);
						if (folderItem != null)
						{
							XmlNode childFolderItem = folderItem.SelectSingleNode("ChildList");

							RecursiveCreateXML(fs, childFolderItem, dir.Id, bIncludeAsset, DeepLevel, 0, (int)dr["ProjectId"]);
						}
					}
				}

				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return xmlDoc.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}


		private void InsertUserInformation(XmlNode xmlParentNode, int UserId)
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

						xmlUserStatus.InnerText = ((Mediachase.UI.Web.WebServices.UserStatus)_status).ToString();
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

		#region GetProjectsReader
		static IDataReader GetProjectsReader(int ObjectType)
		{
			// OZ: 2008-07-18
			return Project.GetListProjects();

			//return Project.GetListProjectsByKeyword(string.Empty);

			/*switch (ObjectType)
			{
				case (int)ObjectTypes.Issue:
					return Incident.GetListProjects();
				case (int)ObjectTypes.Task:
					return Project.GetListProjects();
				case (int)ObjectTypes.ToDo:
					return Mediachase.IBN.Business.ToDo.GetListProjects();
				case (int)ObjectTypes.CalendarEntry:
					return CalendarEntry.GetListProjects();
				default:
					return Project.GetListProjects();
			}*/
		}
		#endregion

		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetIBNProjectList2(int ObjectType)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				/*
						 <Projects>
							<Managers>
								<User>
									<UserId>
									<FirstName>
									<LastName>
								<User>
							<Managers>
							<Project>
								<!-- IBN Compatibility 3.0 -->
								<ObjectID/>
								<Title/>
								<Manager>
									<User>
										<UserId>
										<FirstName>
										<LastName>
									<User>
								<Manager>
							</Project>
							...
						<Projects>
						 */
				XmlDocument xmlProjectList = new XmlDocument();
				xmlProjectList.LoadXml("<Projects><Managers></Managers></Projects>");
				XmlNode xmlProjectsNode = xmlProjectList.SelectSingleNode("Projects");

				using (IDataReader projectListReader = GetProjectsReader(ObjectType))
				{
					// Step 1. Create Project List [1/30/2004]
					while (projectListReader.Read())
					{
						// Reaad Only Active Project
						int statusId = (int)projectListReader["StatusId"];
						if (statusId != 1 && statusId != 5 && statusId != 4)
							continue;

						XmlElement projectNode = xmlProjectList.CreateElement("Project");

						//////////////////////////////////////////////////////////////////////////
						XmlElement ObjectIDNode = xmlProjectList.CreateElement("ObjectID");
						XmlElement TitleNode = xmlProjectList.CreateElement("Title");
						XmlElement ManagerNode = xmlProjectList.CreateElement("Manager");
						//					XmlElement DateCreatedNode = xmlProjectList.CreateElement("DateCreated");
						//					XmlElement CreatorNode = xmlProjectList.CreateElement("Creator");
						//					XmlElement StartDateNode = xmlProjectList.CreateElement("StartDate");
						//					XmlElement EndDateNode = xmlProjectList.CreateElement("EndDateID");
						//					XmlElement ManagerCanonNameNode = xmlProjectList.CreateElement("ManagerCanonName");
						//					XmlElement DescriptionNode = xmlProjectList.CreateElement("Description");

						//////////////////////////////////////////////////////////////////////////
						int ProjectId = (int)projectListReader["ProjectId"];
						ObjectIDNode.InnerText = ProjectId.ToString();
						TitleNode.InnerText = projectListReader["Title"].ToString();

						InsertUserInformation(ManagerNode, Project.GetProjectManager(ProjectId));
						//					DateCreatedNode.InnerText = projectListReader["CreationDate"].ToString();
						//
						//					UserLight	creator = new UserLight((int)projectListReader["CreatorId"]);
						//					CreatorNode.InnerText = string.Format("{0} {1}",creator.FirstName,creator.LastName);
						//
						//					StartDateNode.InnerText = projectListReader["StartDate"].ToString();
						//					EndDateNode.InnerText = projectListReader["FinishDate"].ToString();
						//
						//					UserLight	manager = new UserLight((int)projectListReader["ManagerId"]);
						//					ManagerCanonNameNode.InnerText = string.Format("{0} {1}",manager.FirstName,manager.LastName);
						//
						//					DescriptionNode.InnerText = projectListReader["Description"].ToString();

						//////////////////////////////////////////////////////////////////////////
						projectNode.AppendChild(ObjectIDNode);
						projectNode.AppendChild(TitleNode);
						projectNode.AppendChild(ManagerNode);
						//					projectNode.AppendChild(DateCreatedNode);
						//					projectNode.AppendChild(CreatorNode);
						//					projectNode.AppendChild(DateCreatedNode);
						//					projectNode.AppendChild(StartDateNode);
						//					projectNode.AppendChild(EndDateNode);
						//					projectNode.AppendChild(ManagerCanonNameNode);
						//					projectNode.AppendChild(DescriptionNode);

						//////////////////////////////////////////////////////////////////////////
						xmlProjectsNode.AppendChild(projectNode);
					}
				}

				// Step 2. Build Manager List [1/30/2004]
				XmlNode ManagersList = xmlProjectList.SelectSingleNode("Projects/Managers");
				if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				{
					using (IDataReader managerReader = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager))
					{
						while (managerReader.Read())
						{
							InsertUserInformation(ManagersList, (int)managerReader["UserId"]);
						}
					}
				}
				else
				{
					InsertUserInformation(ManagersList, Security.CurrentUser.UserID);
				}
				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return xmlProjectList.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}

		}

		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetGroupList()
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				/*
					 <Groups>
						<Group>
							<GroupId>
							<GroupName>
							<Level>
						</Group>
						...
					<Groups>
				*/

				XmlDocument xmlGroupList = new XmlDocument();
				xmlGroupList.LoadXml("<Groups></Groups>");

				XmlNode xmlGroupsNode = xmlGroupList.SelectSingleNode("Groups");

				// Get All Available Group [1/27/2004]
				using (IDataReader groupReader = SecureGroup.GetListGroupsAsTree())
				{
					while (groupReader.Read())
					{
						XmlElement groupNode = xmlGroupList.CreateElement("Group");

						//////////////////////////////////////////////////////////////////////////
						XmlElement GroupIDNode = xmlGroupList.CreateElement("GroupId");
						XmlElement GroupNameNode = xmlGroupList.CreateElement("GroupName");
						XmlElement LevelNode = xmlGroupList.CreateElement("Level");

						GroupIDNode.InnerText = groupReader["GroupId"].ToString();
						GroupNameNode.InnerText = Mediachase.UI.Web.Util.CommonHelper.GetResFileString(groupReader["GroupName"].ToString());
						LevelNode.InnerText = groupReader["Level"].ToString();

						//////////////////////////////////////////////////////////////////////////
						groupNode.AppendChild(GroupIDNode);
						groupNode.AppendChild(GroupNameNode);
						groupNode.AppendChild(LevelNode);

						xmlGroupsNode.AppendChild(groupNode);
					}
				}

				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return xmlGroupList.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetResourceListByGroup(int GroupId)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				/*
				 	
					 <Resources>
						<User>
							<UserId>
							<FirstName>
							<LastName>
						</User>
						...
					<Resources>
					 */

				XmlDocument xmlResourceList = new XmlDocument();
				xmlResourceList.LoadXml("<Resources></Resources>");

				XmlNode xmlResourcesNode = xmlResourceList.SelectSingleNode("Resources");

				// Get User List for Group [1/27/2004]
				using (IDataReader teamReader = SecureGroup.GetListActiveUsersInGroup(GroupId))
				{
					while (teamReader.Read())
					{
						InsertUserInformation(xmlResourcesNode, (int)teamReader["UserId"]);
					}
				}

				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return xmlResourceList.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetResourceListBySubstring(string searchstr)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				/*
				 	
					 <Resources>
						<User>
							<UserId>
							<FirstName>
							<LastName>
						</User>
						...
					<Resources>
					 */

				XmlDocument xmlResourceList = new XmlDocument();
				xmlResourceList.LoadXml("<Resources></Resources>");

				XmlNode xmlResourcesNode = xmlResourceList.SelectSingleNode("Resources");

				// Get User List for Group [1/27/2004]
				using (IDataReader teamReader = Mediachase.IBN.Business.User.GetListUsersBySubstring(searchstr))
				{
					while (teamReader.Read())
					{
						InsertUserInformation(xmlResourcesNode, (int)teamReader["UserId"]);
					}
				}

				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return xmlResourceList.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetResourceListByProject(int ProjectId)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				/*
				 	
					 <Resources>
						<User>
							<UserId>
							<FirstName>
							<LastName>
						</User>
						...
					<Resources>
					 */

				XmlDocument xmlResourceList = new XmlDocument();
				xmlResourceList.LoadXml("<Resources></Resources>");

				XmlNode xmlResourcesNode = xmlResourceList.SelectSingleNode("Resources");

				// Get User List for Group [1/27/2004]
				using (IDataReader teamReader = Project.GetListTeamMemberNames(ProjectId))
				{
					while (teamReader.Read())
					{
						InsertUserInformation(xmlResourcesNode, (int)teamReader["UserId"]);
					}
				}

				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return xmlResourceList.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetResourceListByIBNUsers(int[] IBNUserId)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//////////////////////////////////////////////////////////////////////////
				/*
					<Resources>
						<User>
							<UserId>
							<FirstName>
							<LastName>
							<Status>
						</User>
					<Resources>
					 */

				XmlDocument xmlResourceList = new XmlDocument();
				xmlResourceList.LoadXml("<Resources></Resources>");

				XmlNode xmlResourcesNode = xmlResourceList.SelectSingleNode("Resources");

				foreach (int OriginalId in IBNUserId)
				{
					// Get User List for Group [1/27/2004]
					using (IDataReader teamReader = Mediachase.IBN.Business.User.GetUserInfoByOriginalId(OriginalId))
					{
						if (teamReader.Read())
						{
							//////////////////////////////////////////////////////////////////////////
							InsertUserInformation(xmlResourcesNode, (int)teamReader["UserId"]);
						}
					}

					//////////////////////////////////////////////////////////////////////////
				}

				errMsg.msg = "OK";
				return xmlResourceList.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}


		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetCategoryList()
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				errMsg.msg = "OK";
				return WebServicesHelper.GetCategoryList();
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		// IBN 3.6 Function's [1/27/2004]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetCategoryList2(int ObjectType)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();


				errMsg.msg = "OK";
				return WebServicesHelper.GetCategoryList2(ObjectType);
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		// IBN 3.6 Function's [3/21/2005]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetRoleList(int TemplateId)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				errMsg.msg = "OK";
				return WebServicesHelper.GetRoleList(TemplateId);
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateIncident(string Title, string Description, int ProjectId, int TypeId, int Priority, int SeverityId, int[] IncidentCategory)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				Mediachase.IBN.Business.Common.GetDefaultClient(PortalConfig.IncidentDefaultValueClientField, out contact_id, out org_id);
				ObjectId = Incident.Create(Title, Description, ProjectId, TypeId, Priority, SeverityId, 
					int.Parse(PortalConfig.IncidentDefaultValueTaskTimeField), 0, 0, 0, Security.CurrentUser.UserID,
					Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.IncidentDefaultValueGeneralCategoriesField), 
					new ArrayList(IncidentCategory), null, null,
					false, DateTime.UtcNow, -1, contact_id, org_id);

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateIncident45(string Title, string Description, int ProjectId, int TypeId,
			int Priority, int SeverityId, int[] IncidentCategory, int IncidentBoxId, 
			int VCardId, int OrgId)
		{
			return CreateIncident47(Title, Description, ProjectId, TypeId,
				Priority, SeverityId, IncidentCategory, IncidentBoxId, string.Empty, string.Empty, string.Empty);
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateIncident47(string Title, string Description, int ProjectId, int TypeId, 
			int Priority, int SeverityId, int[] IncidentCategory, int IncidentBoxId,
			string ContactUid, string OrgUid,
			string SenderEmail)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				if (IncidentBoxId <= 0)
				{
					// Calculate Incident box
					Mediachase.IBN.Business.EMail.IncidentInfo info = new Mediachase.IBN.Business.EMail.IncidentInfo();
					info.Description = Description;
					info.GeneralCategories = new ArrayList();
					info.IncidentCategories = new ArrayList(IncidentCategory);
					info.PriorityId = Priority;
					info.SeverityId = SeverityId;
					info.Title = Title;
					info.TypeId = TypeId;
					info.MailSenderEmail = SenderEmail??string.Empty;
					info.ContactUid = PrimaryKeyId.Parse(ContactUid);
					info.OrgUid = PrimaryKeyId.Parse(OrgUid);
					info.CreatorId = Security.CurrentUser.UserID;

					IncidentBoxId = IncidentBoxRule.Evaluate(info).IncidentBoxId;
				}

				ObjectId = Incident.Create(Title, 
					Description, ProjectId, TypeId, Priority, SeverityId, 
					int.Parse(PortalConfig.IncidentDefaultValueTaskTimeField), 
					0, 0, 0, Security.CurrentUser.UserID,
					Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.IncidentDefaultValueGeneralCategoriesField), 
					new ArrayList(IncidentCategory), null, null, 
					false, DateTime.UtcNow, IncidentBoxId, PrimaryKeyId.Parse(ContactUid), PrimaryKeyId.Parse(OrgUid));

				// OZ 2008-08-19: Append email sender
				if(!string.IsNullOrEmpty(SenderEmail))
					EMailIssueExternalRecipient.Create(ObjectId, SenderEmail);

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateTask(string Title, string Description, int ProjectId, DateTime StartDate, DateTime EndDate, bool IsMilestone, int Priority, int CompletionTypeId, bool MustBeConfirmedByManager, int[] Resources)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList alCategories = Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.TaskDefaultValueGeneralCategoriesField);
				ArrayList alResources = new ArrayList(Resources);

				StartDate = StartDate.ToUniversalTime();
				EndDate = EndDate.ToUniversalTime();

				ObjectId = Task.CreateUseUniversalTime(ProjectId, Title, Description, StartDate, EndDate, 
					Priority, IsMilestone, int.Parse(PortalConfig.TaskDefaultValueActivationTypeField), 
					CompletionTypeId, MustBeConfirmedByManager, alCategories, alResources);

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateEvent(string Title, string Location, string Description, int ProjectId, 
			int ManagerId, DateTime StartDate, DateTime EndDate, int Priority, int TypeId, 
			int ReminderInterval, int[] Resources)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList alCategories = Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.CEntryDefaultValueGeneralCategoriesField);
				ArrayList alResources = new ArrayList(Resources);

				StartDate = StartDate.ToUniversalTime();
				EndDate = EndDate.ToUniversalTime();
				
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				Mediachase.IBN.Business.Common.GetDefaultClient(PortalConfig.CEntryDefaultValueClientField, out contact_id, out org_id);
				
				ObjectId = CalendarEntry.CreateUseUniversalTime(Title, Description, Location, ProjectId, ManagerId, Priority, TypeId, StartDate, EndDate, alCategories, null, null, alResources, contact_id, org_id);

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateEvent45(string Title, string Location, string Description, int ProjectId, int ManagerId, DateTime StartDate, DateTime EndDate, int Priority, int TypeId, int ReminderInterval, int VCardId, int OrgId, int[] Resources)
		{
			return CreateEvent47(Title, Location, Description, ProjectId, ManagerId, StartDate, EndDate, Priority, TypeId, ReminderInterval, 
				string.Empty, string.Empty, Resources);
		}


		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateEvent47(string Title, string Location, string Description, int ProjectId, int ManagerId,
			DateTime StartDate, DateTime EndDate, int Priority, int TypeId, int ReminderInterval,
			string ContactUid, string OrgUid, int[] Resources)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList alCategories = Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.CEntryDefaultValueGeneralCategoriesField);
				ArrayList alResources = new ArrayList(Resources);

				StartDate = StartDate.ToUniversalTime();
				EndDate = EndDate.ToUniversalTime();

				ObjectId = CalendarEntry.CreateUseUniversalTime(Title, Description, Location, ProjectId, ManagerId, Priority, TypeId, StartDate, EndDate, alCategories, null, null, alResources,
					PrimaryKeyId.Parse(ContactUid), PrimaryKeyId.Parse(OrgUid));

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateToDo(string Title, string Description, int ProjectId, int ManagerId, DateTime StartDate, DateTime EndDate, int DateFlag, int Priority, int CompletionTypeId, bool MustBeConfirmedByManager, int[] Resources)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList alResources = new ArrayList(Resources);

				StartDate = StartDate.ToUniversalTime();
				EndDate = EndDate.ToUniversalTime();

				ObjectId = Mediachase.IBN.Business.ToDo.CreateUseUniversalTime(ProjectId, ManagerId, Title, Description,
					(DateFlag & 1) == 0 ? DateTime.MinValue : StartDate, (DateFlag & 2) == 0 ? DateTime.MinValue : EndDate, Priority,
					(int)ActivationTypes.AutoWithCheck,
					CompletionTypeId, MustBeConfirmedByManager, -1, new ArrayList(), null, null, alResources,
					PrimaryKeyId.Empty, PrimaryKeyId.Empty);

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}


		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateToDo45(string Title, string Description, int ProjectId, int ManagerId, DateTime StartDate, DateTime EndDate, int DateFlag, int Priority, int CompletionTypeId, bool MustBeConfirmedByManager, int VCardId, int OrgId, int[] Resources)
		{
			return CreateToDo47(Title, Description, ProjectId, ManagerId, StartDate, EndDate, DateFlag, Priority, CompletionTypeId, MustBeConfirmedByManager, 
				string.Empty, string.Empty, Resources);
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateToDo47(string Title, string Description, int ProjectId, int ManagerId, 
			DateTime StartDate, DateTime EndDate, int DateFlag, int Priority, int CompletionTypeId, 
			bool MustBeConfirmedByManager, string ContactUid, string OrgUid, int[] Resources)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList alResources = new ArrayList(Resources);

				StartDate = StartDate.ToUniversalTime();
				EndDate = EndDate.ToUniversalTime();

				ObjectId = Mediachase.IBN.Business.ToDo.CreateUseUniversalTime(ProjectId, ManagerId, Title, Description,
					(DateFlag & 1) == 0 ? DateTime.MinValue : StartDate, (DateFlag & 2) == 0 ? DateTime.MinValue : EndDate, Priority,
					(int)ActivationTypes.AutoWithCheck,
					CompletionTypeId, MustBeConfirmedByManager, -1, new ArrayList(), null, null, alResources,
					PrimaryKeyId.Parse(ContactUid), PrimaryKeyId.Parse(OrgUid));

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateProject(string title,
			string description, string goals, string scope, string deliverable,
			int manager_id, int exmanager_id, DateTime start_date, DateTime finish_date,
			int status_id, int type_id, int calendar_id, int client_id, int currency_id,
			int[] ProjectCategory, int[] Resources)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList categories = new ArrayList();
				ArrayList project_categories = new ArrayList(ProjectCategory);
				ArrayList alResources = new ArrayList(Resources);
				ArrayList alPort = new ArrayList();

				start_date = start_date.ToUniversalTime();
				finish_date = finish_date.ToUniversalTime();

				int phase_id = 1;
				using (IDataReader reader = Project.GetListProjectPhases())
				{
					if (reader.Read())
					{
						phase_id = (int)reader["PhaseId"];
					}
				}
				ObjectId = Project.CreateUseUniversalTime(title, description, goals, scope, deliverable,
					manager_id, exmanager_id, start_date, finish_date, DateTime.MinValue, DateTime.MinValue,
					status_id, type_id, calendar_id, PrimaryKeyId.Empty, PrimaryKeyId.Empty, currency_id, phase_id, phase_id, 0, 1, 1, categories, project_categories, alPort, alResources, true);


				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateProject40(
			string title,	// TODO: rename to CreateProject45 and leave original CreateProject40 unchanged
			string description,
			string goals,
			string scope,
			string deliverable,
			int manager_id,
			int exmanager_id,
			DateTime start_date, DateTime finish_date,
			int status_id,
			int type_id,
			int calendar_id,
			int client_id,
			int currency_id, int priority_id, int phase_id,
			int[] ProjectCategory, int[] Resources, int[] ProjectPortfolio)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList categories = new ArrayList();
				ArrayList project_categories = new ArrayList(ProjectCategory);
				ArrayList alResources = new ArrayList(Resources);
				ArrayList alPort = new ArrayList(ProjectPortfolio);

				start_date = start_date.ToUniversalTime();
				finish_date = finish_date.ToUniversalTime();

				ObjectId = Project.CreateUseUniversalTime(title, description, goals, scope, deliverable,
					manager_id, exmanager_id, start_date, finish_date, DateTime.MinValue, DateTime.MinValue,
					status_id, type_id, calendar_id, PrimaryKeyId.Empty, PrimaryKeyId.Empty, currency_id, priority_id, phase_id, phase_id,
					0, 1, 1, categories, project_categories, alPort, alResources, true);

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateProject45(
			string title,	// TODO: rename to CreateProject45 and leave original CreateProject40 unchanged
			string description,
			string goals,
			string scope,
			string deliverable,
			int manager_id,
			int exmanager_id,
			DateTime start_date, DateTime finish_date,
			int status_id,
			int type_id,
			int calendar_id,
			int vcard_id, int org_id, 
			int currency_id, int priority_id, int phase_id,
			int[] ProjectCategory, int[] Resources, int[] ProjectPortfolio)
		{
			int TimeTrackingTemplateId = 0;

			// Find First Item
			TimeTrackingBlockType[] list = TimeTrackingBlockType.List(Mediachase.Ibn.Data.FilterElement.EqualElement("IsProject", "1")); ;

			if(list.Length>0)
				TimeTrackingTemplateId = list[0].PrimaryKeyId.Value;

			return CreateProject47(title,	
			description,
			goals,
			scope,
			deliverable,
			manager_id,
			exmanager_id,
			start_date, finish_date,
			status_id,
			type_id,
			calendar_id,
			string.Empty, string.Empty,
			currency_id, priority_id, phase_id,
			ProjectCategory, Resources, ProjectPortfolio, TimeTrackingTemplateId);
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateProject47(
			string title,	
			string description,
			string goals,
			string scope,
			string deliverable,
			int manager_id,
			int exmanager_id,
			DateTime start_date, DateTime finish_date,
			int status_id,
			int type_id,
			int calendar_id,
			string ñontactUid, string orgUid,
			int currency_id, int priority_id, int phase_id,
			int[] ProjectCategory, int[] Resources, int[] ProjectPortfolio, int TimeTrackingTemplateId)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList categories = new ArrayList();
				ArrayList project_categories = new ArrayList(ProjectCategory);
				ArrayList alResources = new ArrayList(Resources);
				ArrayList alPort = new ArrayList(ProjectPortfolio);

				start_date = start_date.ToUniversalTime();
				finish_date = finish_date.ToUniversalTime();

				ObjectId = Project.CreateUseUniversalTime(title, description, goals, scope, deliverable,
					manager_id, exmanager_id, start_date, finish_date, DateTime.MinValue, DateTime.MinValue,
					status_id, type_id, calendar_id, PrimaryKeyId.Parse(ñontactUid), PrimaryKeyId.Parse(orgUid), currency_id, priority_id, phase_id, phase_id,
					0, 1, TimeTrackingTemplateId, categories, project_categories, alPort, alResources, true);

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void UpdateDictionaries(string Dictionary)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				WebServicesHelper.UpdateDictionaries(Dictionary);

				errMsg.msg = "OK";
				return;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void UpdateCategoryList(int ObjectType, string UpdateList)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				WebServicesHelper.UpdateCategoryList(ObjectType, UpdateList);

				errMsg.msg = "OK";
				return;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return;
			}
		}


		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string LoadDictionaries(string Locale, int ObjectType)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				errMsg.msg = "OK";

				if (ObjectTypes.List == (ObjectTypes)ObjectType)
				{
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Locale);
					Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(Locale);

					string defaultXml = WebServicesHelper.LoadDictionaries(Locale, ObjectType);

					XmlDocument xmlDictionariesList = new XmlDocument();
					xmlDictionariesList.LoadXml(defaultXml);

					XmlNode xmlDictionariesNode = xmlDictionariesList.SelectSingleNode("Dictionaries");

					// ListStatus
					InsertMetaEnumItems(xmlDictionariesNode, "ListStatus", DataContext.Current.GetMetaFieldType("ListStatus").EnumItems);

					// ListType
					InsertMetaEnumItems(xmlDictionariesNode, "ListType", DataContext.Current.GetMetaFieldType("ListType").EnumItems);

					// ListTemplate
					XmlNode xmlNewListTemplateNode = xmlDictionariesNode.OwnerDocument.CreateElement("ListTemplate");
					foreach (ListInfo info in ListManager.GetTemplates())
					{
						InsertDictionaryItem(xmlNewListTemplateNode, info.Title, info.PrimaryKeyId.Value.ToString());
					}
					xmlDictionariesNode.AppendChild(xmlNewListTemplateNode);

					return xmlDictionariesList.InnerXml;
				}
				else
					return WebServicesHelper.LoadDictionaries(Locale, ObjectType);
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		private static void InsertMetaEnumItems(XmlNode ownerNode, string groupName, MetaEnumItem[] items)
		{
			XmlNode xmlNewRootNode = ownerNode.OwnerDocument.CreateElement(groupName);

			foreach (MetaEnumItem item in items)
			{
				string name = CHelper.GetResFileString(item.Name);

				InsertDictionaryItem(xmlNewRootNode, name, item.Handle.ToString());
			}

			ownerNode.AppendChild(xmlNewRootNode);
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



		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string ImportMsProjectStep1(int ProjectId, string MsProjectImportXml)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

//				bool bSyncMode = false;
				if (ProjectId < 0)
				{
					ProjectId *= -1;
//					bSyncMode = true;
				}

				//////////////////////////////////////////////////////////////////////////
				System.IO.MemoryStream memStream = new System.IO.MemoryStream();
				System.IO.StreamWriter writer = new System.IO.StreamWriter(memStream);

				writer.Write(MsProjectImportXml);
				writer.Flush();
				memStream.Seek(0, System.IO.SeekOrigin.Begin);

				if (!Util.CommonHelper.ValidateXMLWithMsProjectSchema(new XmlTextReader(memStream)))
				{
					throw new Exception("The XML File has an invalid format.");
				}
				memStream.Seek(0, System.IO.SeekOrigin.Begin);

				//  [1/19/2004]
				// Step 1. We add a XML to the IBN Project [4/27/2004]
				int XMLVersionID = Project.UploadImportXML(ProjectId, "ImportIBNProjectStep1.xml", memStream);

				// Step 2. Read Project Datatable [4/27/2004]
				// !!Attention
				DataTable taskAssignments = Task.TaskImportAssignments(ProjectId, XMLVersionID);

				// Step 3. Create a output XML [4/27/2004]
				XmlDocument outputXml = new XmlDocument();

				outputXml.LoadXml("<ImportConfig><XMLVersionID/><Resources/><ImportAssignments/></ImportConfig>");
				/*
				 <ImportConfig> 
					<XMLVersionID>12</XMLVersionID>
					<Resources>
						<User>
							<UserId>
							<FirstName>
							<LastName>
						</User>
						...
					<Resources>
					<ImportAssignments>
						<Assignment>
							<ResourceId/>
							<ResourceName/>
							<UserId/>
						</Assignment>
					</ImportAssignments>
					...
				 </ImportConfig>
				 */

				//<XMLVersionID>12</XMLVersionID>
				outputXml.SelectSingleNode("ImportConfig/XMLVersionID").InnerText = XMLVersionID.ToString();

				//<Resources>
				XmlNode xmlResourcesNode = outputXml.SelectSingleNode("ImportConfig/Resources");

				// Get User List for Group [1/27/2004]
				using (IDataReader teamReader = Project.GetListTeamMemberNames(ProjectId))
				{
					while (teamReader.Read())
					{
						InsertUserInformation(xmlResourcesNode, (int)teamReader["UserId"]);
					}
				}

				//<ImportAssignments>
				XmlNode xmlImportAssignmentsNode = outputXml.SelectSingleNode("ImportConfig/ImportAssignments");

				foreach (DataRow dr in taskAssignments.Rows)
				{
					int ResourceId = (int)dr["ResourceId"];
					string ResourceName = (string)dr["ResourceName"];
					int PrincipalId = (int)dr["PrincipalId"];

					XmlNode xmlAssignmentNode = outputXml.CreateElement("Assignment");

					XmlNode xmlResourceIdNode = outputXml.CreateElement("ResourceId");
					XmlNode xmlResourceNameNode = outputXml.CreateElement("ResourceName");
					XmlNode xmlPrincipalIdNode = outputXml.CreateElement("UserId");

					xmlResourceIdNode.InnerText = ResourceId.ToString();
					xmlResourceNameNode.InnerText = ResourceName.ToString();
					xmlPrincipalIdNode.InnerText = PrincipalId.ToString();

					xmlAssignmentNode.AppendChild(xmlResourceIdNode);
					xmlAssignmentNode.AppendChild(xmlResourceNameNode);
					xmlAssignmentNode.AppendChild(xmlPrincipalIdNode);

					xmlImportAssignmentsNode.AppendChild(xmlAssignmentNode);
				}

				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return outputXml.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void ImportMsProjectStep2(int ProjectId, string ImportResourceMapXml)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				/*
				 <ImportConfig> 
					<XMLVersionID>12</XMLVersionID>
					<ImportAssignments>
						<Assignment>
							<ResourceId/>
							<UserId/>
						</Assignment>
					</ImportAssignments>
				 </ImportConfig>
				 */

				//////////////////////////////////////////////////////////////////////////
				//  [4/29/2004]
				XmlDocument xmlImportConfigDocument = new XmlDocument();
				xmlImportConfigDocument.LoadXml(ImportResourceMapXml);

				XmlNodeList xmlImportAssignmentsList = xmlImportConfigDocument.SelectNodes("ImportConfig/ImportAssignments/Assignment");

				DataTable table = new DataTable();

				DataColumn Column = table.Columns.Add("ResourceId", typeof(int));
				table.PrimaryKey = new DataColumn[] { Column };
				table.Columns.Add("ResourceName", typeof(string));
				table.Columns.Add("PrincipalId", typeof(int));

				foreach (XmlNode xmlAssignmentNode in xmlImportAssignmentsList)
				{
					DataRow row = table.NewRow();

					row["ResourceId"] = Int32.Parse(xmlAssignmentNode.SelectSingleNode("ResourceId").InnerText);
					row["ResourceName"] = "";
					row["PrincipalId"] = Int32.Parse(xmlAssignmentNode.SelectSingleNode("UserId").InnerText);

					table.Rows.Add(row);
				}

				bool bSyncMode = false;
				if (ProjectId < 0)
				{
					ProjectId *= -1;
					bSyncMode = true;
				}

				if (Project.GetIsMSProject(ProjectId) || bSyncMode)
					Task.TasksImport2(table, ProjectId, Int32.Parse(xmlImportConfigDocument.SelectSingleNode("ImportConfig/XMLVersionID").InnerText), CompletionType.All, true);
				else
					Task.TasksImport(table, ProjectId, Int32.Parse(xmlImportConfigDocument.SelectSingleNode("ImportConfig/XMLVersionID").InnerText));
				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void ImportMsProjectStep2SyncMode(int ProjectId, string ImportResourceMapXml)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				/*
				 <ImportConfig> 
					<XMLVersionID>12</XMLVersionID>
					<ImportAssignments>
						<Assignment>
							<ResourceId/>
							<UserId/>
						</Assignment>
					</ImportAssignments>
				 </ImportConfig>
				 */

				//////////////////////////////////////////////////////////////////////////
				//  [4/29/2004]
				XmlDocument xmlImportConfigDocument = new XmlDocument();
				xmlImportConfigDocument.LoadXml(ImportResourceMapXml);

				XmlNodeList xmlImportAssignmentsList = xmlImportConfigDocument.SelectNodes("ImportConfig/ImportAssignments/Assignment");

				DataTable table = new DataTable();

				DataColumn Column = table.Columns.Add("ResourceId", typeof(int));
				table.PrimaryKey = new DataColumn[] { Column };
				table.Columns.Add("ResourceName", typeof(string));
				table.Columns.Add("PrincipalId", typeof(int));

				foreach (XmlNode xmlAssignmentNode in xmlImportAssignmentsList)
				{
					DataRow row = table.NewRow();

					row["ResourceId"] = Int32.Parse(xmlAssignmentNode.SelectSingleNode("ResourceId").InnerText);
					row["ResourceName"] = "";
					row["PrincipalId"] = Int32.Parse(xmlAssignmentNode.SelectSingleNode("UserId").InnerText);

					table.Rows.Add(row);
				}

				Task.TasksImport2(table, ProjectId, Int32.Parse(xmlImportConfigDocument.SelectSingleNode("ImportConfig/XMLVersionID").InnerText), CompletionType.All, true);
				//////////////////////////////////////////////////////////////////////////

				errMsg.msg = "OK";
				return;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return;
			}
		}


		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetCalendarEvents(DateTime dateFrom, DateTime dateTo, bool LoadEvent, bool LoadTask, bool LoadToDo)
		{
			string ret = string.Empty;
			try
			{
				Authenticate();
				//DebugAuthenticate("admin","ibn");

				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml("<ibnCalendar><fromDate/><toDate/><events/><tasks/><todos/></ibnCalendar>");

				xmlDocument.SelectSingleNode("ibnCalendar/fromDate").InnerText = dateFrom.ToString("s");
				xmlDocument.SelectSingleNode("ibnCalendar/toDate").InnerText = dateTo.ToString("s");

				if (LoadTask || LoadToDo || LoadEvent)
					CalendarView.GetListCalendarEntriesXML(xmlDocument, dateFrom, dateTo, LoadToDo, LoadTask, LoadEvent);

				ret = xmlDocument.InnerXml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex);
				errMsg.msg = ex.Message;
			}
			return ret;
		}

		// CreateListFolder Methods [4/13/2005]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public int CreateListFolder(int ParentFolderId, string Name)
		{
			int retVal = -1;
			try
			{
				Authenticate();

				using (TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					//retVal = List.AddFolder(Name, ParentFolderId);
					ListFolder folder = new ListFolder(ParentFolderId);

					ListFolder newFolder = new ListFolder();
					newFolder.Title = Name;
					newFolder.Save();

					folder.GetTreeService().AppendChild(newFolder);
					folder.Save();

					tran.Commit();

					retVal = newFolder.PrimaryKeyId.Value;
				}

			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
			}
			return retVal;
		}

		// CreateFolder Methods [2/7/2005]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public int CreateFolder(int ParentFolderId, string Name)
		{
			int retVal = -1;
			try
			{
				Authenticate();

				string ContainerName = "FileLibrary";
				//string ContainerKey = "Workspace";
				string ContainerKey = Mediachase.IBN.Business.ControlSystem.DirectoryInfo.GetContainerKey(ParentFolderId);

				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

				if (!fs.DirectoryExist(Name, ParentFolderId) && fs.CanUserWrite(ParentFolderId))
				{
					retVal = fs.CreateDirectory(ParentFolderId, Name).Id;
				}
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
			}
			return retVal;
		}

		// DeleteFolder Methods [2/7/2005]
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void DeleteFolder(int FolderId)
		{
			try
			{
				Authenticate();

				string ContainerName = "FileLibrary";
				string ContainerKey = "Workspace";

				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

				if (!fs.CanUserWrite(FolderId))
				{
					fs.DeleteFolder(FolderId);
				}
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void updateProjectFromTemplate(int ProjectId, int TemplateId, string[] assignments)
		{
			/*
				assignments are pairs of principle id(user id) and role id separeted by ":"
			*/
			if (errMsg == null)
				errMsg = new ErrMsg();
			errMsg.DidUnderstand = true;

			try
			{
				Authenticate();

				DataTable table = new DataTable();
				table.Columns.Add("RoleId", typeof(int));
				table.Columns.Add("PrincipalId", typeof(int));

				foreach (string assignment in assignments)
				{
					string[] ids = assignment.Split(new char[] { ':' });
					int principle_id = int.Parse(ids[0]);
					int role_id = int.Parse(ids[1]);

					table.Rows.Add(new object[] { role_id, principle_id });
				}
				Task.MakeProjectFromTemplate(TemplateId, ProjectId, table);

				errMsg.msg = "OK";
				return;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
			}
		}


		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetListFolders(int FolderID, int DeepLevel)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				string xml = GetListFoldersXml(FolderID, DeepLevel);

				errMsg.msg = "OK";
				return xml;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		#region GetListFolders
		private static string GetListFoldersXml(int FolderID, int DeepLevel)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<Folders/>");
			XmlNode dirlistNode = xmlDoc.SelectSingleNode("Folders");

			CreateFolderListXml(dirlistNode, FolderID, DeepLevel);

			return xmlDoc.InnerXml;
		}


		private static void CreateFolderListXml(XmlNode currParent, int folderID, int deepLevel)
		{
			if (folderID == 0)
			{
				// Private
				ListFolder privateRootFolder = ListManager.GetPrivateRoot(Security.CurrentUser.UserID);
				InsertFolderInfo(currParent, privateRootFolder.PrimaryKeyId.Value, "@@2", privateRootFolder.HasChildren);

				// Public
				ListFolder publicRootFolder = ListManager.GetPublicRoot();
				InsertFolderInfo(currParent, publicRootFolder.PrimaryKeyId.Value, "@@1", publicRootFolder.HasChildren);

				// Projects
				InsertFolderInfo(currParent, -1, "@@3", true);
			}
			else if (folderID == -1)
			{
				// Enum Projects
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
					{
						ListFolder projectRootFolder = ListManager.GetProjectRoot((int)reader["ProjectId"]);

						InsertFolderInfo(currParent, projectRootFolder.PrimaryKeyId.Value, (string)reader["Title"], true);
					}
				}
			}
			else
			{
				ListFolder folder = new ListFolder(folderID);

				foreach (IbnServices.TreeNode childNode in folder.GetTreeService().GetChildNodes())
				{
					InsertFolderInfo(currParent, childNode.ObjectId, childNode.Title, childNode.HasChildren);
				}
			}

			if (deepLevel == -1 || deepLevel > 0)
			{
				XmlNodeList childFolderList = currParent.SelectNodes("Folder");

				foreach (XmlNode folderItem in childFolderList)
				{
					XmlNode childFolderItemNode = folderItem.SelectSingleNode("ChildList");
					if (childFolderItemNode != null)
					{
						int ChildFolderID = Int32.Parse(folderItem.SelectSingleNode("Id").InnerXml);

						CreateFolderListXml(childFolderItemNode, ChildFolderID, deepLevel == -1 ? -1 : deepLevel - 1);
					}
				}
			}
		}

		private static void InsertFolderInfo(XmlNode parentFolderNode, int FolderID, string FolderName, bool HaveSubFolders)
		{
			XmlNode folderNode = parentFolderNode.OwnerDocument.CreateElement("Folder");

			XmlNode folderIDNode = parentFolderNode.OwnerDocument.CreateElement("Id");
			folderIDNode.InnerText = FolderID.ToString();

			XmlNode folderNameNode = parentFolderNode.OwnerDocument.CreateElement("Name");
			folderNameNode.InnerText = FolderName;

			XmlNode canUserWriteNode = parentFolderNode.OwnerDocument.CreateElement("CanUserWrite");
			canUserWriteNode.InnerText = FolderID > 0 ? "True" : "False";		// TODO: so far always can write

			folderNode.AppendChild(folderIDNode);
			folderNode.AppendChild(folderNameNode);
			folderNode.AppendChild(canUserWriteNode);

			if (HaveSubFolders)
			{
				XmlNode childNode = parentFolderNode.OwnerDocument.CreateElement("ChildList");

				folderNode.AppendChild(childNode);
			}
			parentFolderNode.AppendChild(folderNode);
		}
		#endregion

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateList(int TemplateId, string Title, string Description, int FolderID, int TypeId, int StatusId,
			int[] ListCategory)
		{
			try
			{
				int ObjectId = -1;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				//ArrayList alCategories = new ArrayList(ListCategory);
				//ObjectId = Mediachase.IBN.Business.List.AddList(TemplateId, Title, Description, FolderID, TypeId, StatusId, alCategories);

				using(TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					if(TemplateId<=0)
					{
						ListInfo info = ListManager.CreateList(FolderID, Title);
						info.Description = Description;
						info.ListType = TypeId;
						info.Status = StatusId;
						info.Save();
						ObjectId = info.PrimaryKeyId.Value;
					}
					else
					{
						ListInfo info = ListManager.CreateListFromTemplate(TemplateId, FolderID, Title, true);
						info.Description = Description;
						info.ListType = TypeId;
						info.Status = StatusId;
						info.Save();
						ObjectId = info.PrimaryKeyId.Value;
					}
					tran.Commit();
				}

				errMsg.msg = "OK";
				return ObjectId;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return -1;
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return -1;
			}
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public string GetListFieldTemplate(int TemplateId)
		{
			try
			{
				string ret = string.Empty;
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ret = string.Empty;

				errMsg.msg = "OK";

				return ret;
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
				return "";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
				return "";
			}
		}

		#region GetListDefaultFields

		private static string GetListDefaultFields(int templateId)
		{
			XmlDocument xmlDoc = new XmlDocument();

			if (templateId > 0)
			{
				xmlDoc.LoadXml("<List><Fields></Fields><DictionaryFields></DictionaryFields></List>");
			}
			else
			{
				xmlDoc.LoadXml("<List><Fields><Field><Name>CreationDate</Name><DataType>4</DataType><IsSystem>-1</IsSystem></Field>" +
							 "<Field><Name>LastSavedDate</Name><DataType>4</DataType><IsSystem>-1</IsSystem></Field>" +
							 "<Field><Name>CreatorId</Name><DataType>8</DataType><IsSystem>-1</IsSystem></Field>" +
							 "<Field><Name>LastEditorId</Name><DataType>8</DataType><IsSystem>-1</IsSystem></Field>" +
					   "</Fields><DictionaryFields></DictionaryFields></List>");
			}

			/*XmlNode dictionaryNode = xmlDoc.SelectSingleNode("List/DictionaryFields");
			MetaFieldCollection mfc = MetaField.GetList(MetaDataPlus.MetaNamespace.UserRoot, Mediachase.IBN.Business.List.ListTemplateFieldRoot + templateId.ToString());
			foreach (MetaField field in mfc)
			{
				if (field.IsUser)
				{
					insertField(dictionaryNode,
						field.Name,
						field.FriendlyName,
						field.Description,
						(int)field.DataType,
						field.AllowNulls,
						field.SaveHistory,
						field.AllowSearch,
						null);
				}
			}*/

			return xmlDoc.InnerXml;
		}
		/*private static void insertField(XmlNode fieldsNode, string name, string friendlyName, string description, int dataType, bool allowNull, bool saveHistory, bool allowSearch, MetaDictionary dictionary)
		{
			XmlNode fieldNode = fieldsNode.OwnerDocument.CreateElement("Field");

			XmlNode nameNode = fieldsNode.OwnerDocument.CreateElement("Name");
			nameNode.InnerText = name;

			XmlNode friendlyNameNode = fieldNode.OwnerDocument.CreateElement("FriendlyName");
			friendlyNameNode.InnerText = friendlyName;

			XmlNode descriptionNode = fieldNode.OwnerDocument.CreateElement("Description");
			descriptionNode.InnerText = description;

			XmlNode dataTypeNode = fieldsNode.OwnerDocument.CreateElement("DataType");
			dataTypeNode.InnerText = dataType.ToString();

			XmlNode SystemNode = fieldsNode.OwnerDocument.CreateElement("IsSystem");
			SystemNode.InnerText = "0";

			XmlNode AllowNullNode = fieldsNode.OwnerDocument.CreateElement("AllowNull");
			AllowNullNode.InnerText = allowNull ? "-1" : "0";

			XmlNode SaveHistoryNode = fieldsNode.OwnerDocument.CreateElement("SaveHistory");
			SaveHistoryNode.InnerText = saveHistory ? "-1" : "0";

			XmlNode AllowSearchNode = fieldsNode.OwnerDocument.CreateElement("AllowSearch");
			AllowSearchNode.InnerText = allowSearch ? "-1" : "0";

			fieldNode.AppendChild(nameNode);
			fieldNode.AppendChild(friendlyNameNode);
			fieldNode.AppendChild(descriptionNode);
			fieldNode.AppendChild(dataTypeNode);
			fieldNode.AppendChild(SystemNode);
			fieldNode.AppendChild(AllowNullNode);
			fieldNode.AppendChild(SaveHistoryNode);
			fieldNode.AppendChild(AllowSearchNode);

			if (dictionary != null)
			{
				foreach (MetaDictionaryItem item in dictionary)
				{
					XmlNode ItemNode = fieldsNode.OwnerDocument.CreateElement("LookupValue");
					ItemNode.InnerText = item.Value;

					fieldNode.AppendChild(ItemNode);
				}
			}

			fieldsNode.AppendChild(fieldNode);
		}*/
		#endregion

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public void CreateListFields(int ListID, string xmlFields)
		{
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				throw new NotImplementedException();
				//WebServicesHelper.CreateListFields(ListID, xmlFields);

				//errMsg.msg = "OK";
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				errMsg.msg = ex.Message;
			}
		}
		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public long CreateListFromExcel(string excelPath, string SheetName, string title, string description, int folderId, int typeId, int statusId, int[] listCategory, string[] filter, bool ignoreErrors)
		{
			int listId = -1;

			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				ArrayList alCategories = new ArrayList(listCategory);
				ArrayList alFilter = new ArrayList(filter);

				using (TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					//listId = WebServicesHelper.CreateAndFillList(excelPath, SheetName, title, description, folderId, typeId, statusId, alCategories, alFilter, ignoreErrors);
					//DataSet externalData = null;

					IMCOleDBHelper helper = (IMCOleDBHelper)Activator.GetObject(typeof(IMCOleDBHelper), ConfigurationManager.AppSettings["McOleDbServiceString"]);
					DataSet externalData = helper.ConvertExcelToDataSet(excelPath);

					ListImportParameters param = new ListImportParameters(folderId, title, externalData, externalData.Tables.IndexOf(SheetName));
					param.Description = description;

					MappingError[] errors = ListManager.Import(param);

					// OZ 2009-01-27 Update retval value
					if(param.CreatedListId.HasValue)
						listId = param.CreatedListId.Value;

					tran.Commit();
				}

				errMsg.msg = "OK";
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				//errMsg.msg = ex.Message;
				errMsg.msg = ex.ToString();
			}
			return listId;
		}

		[WebMethod]
		[System.Web.Services.Protocols.SoapHeader("objAuth", Direction = SoapHeaderDirection.In)]
		[System.Web.Services.Protocols.SoapHeader("errMsg", Direction = SoapHeaderDirection.Out)]
		public int CheckFileName(int ObjectTypeId, int ObjectId, string FileName)
		{
			bool exists = false;
			try
			{
				if (errMsg == null)
					errMsg = new ErrMsg();
				errMsg.DidUnderstand = true;
				Authenticate();

				exists = WebServicesHelper.CheckFileName(ObjectTypeId, ObjectId, FileName);

				errMsg.msg = "OK";
			}
			catch (UserNotAuthenticatedException)
			{
				errMsg.msg = "Your login or password is invalid.";
			}
			catch (Exception ex)
			{
				//errMsg.msg = ex.Message;
				errMsg.msg = ex.ToString();
			}
			return exists ? -1 : 0;
		}
	}

	[Serializable]
	public class IBNProjectException : Exception, ISerializable
	{
		public IBNProjectException()
			: base()
		{
		}

		public IBNProjectException(String msg)
			: base(msg)
		{
		}

		public IBNProjectException(String msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}

	public class UserNotAuthenticatedException : IBNProjectException
	{
		public UserNotAuthenticatedException()
			: base()
		{
		}

		public UserNotAuthenticatedException(String msg)
			: base(msg)
		{
		}

		public UserNotAuthenticatedException(String msg, Exception innerException)
			: base(msg, innerException)
		{
		}
	}

}
