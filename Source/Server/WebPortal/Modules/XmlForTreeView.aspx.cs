using System;
using System.ComponentModel;
using System.Data;
using System.Resources;
using System.Drawing;
using System.Web;
using System.Text;
using System.Collections;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using ComponentArt.Web.UI;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.IBN.Business.Pop3;
using Mediachase.IBN.Business.SpreadSheet;
using RssToolkit;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Data.Meta;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Modules
{
	/// <summary>
	/// Summary description for XmlForTreeView.
	/// </summary>
	public partial class XmlForTreeView : System.Web.UI.Page
	{
		protected System.Resources.ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Modules.Resources.strTemplate", typeof(XmlForTreeView).Assembly);
		protected ComponentArt.Web.UI.TreeView TreeView1 = new ComponentArt.Web.UI.TreeView();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			if (Request["FolderID"] != null)
				BindLibrary(int.Parse(Request["FolderID"].ToString()));
			else if (Request["GroupID"] != null)
				BindSecurityDirectory(int.Parse(Request["GroupID"].ToString()));
			else if (Request["IMGroupID"] != null)
				BindContactDirectory();
			else if (Request["ListFolderId1"] != null)
				BindListFolders1(int.Parse(Request["ListFolderId1"].ToString()));
			else if (Request["PrjStatus"] != null)
				BindProjectStatus();
			else if (Request["PrjType"] != null)
				BindProjectTypes();
			else if (Request["PrjCat"] != null)
				BindProjectCategories();
			else if (Request["PrjGenCat"] != null)
				BindGeneralCategoriesForProject();
			else if (Request["PrjGrp"] != null)
				BindPortfolios();
			else if (Request["PrjTemp"] != null)
				BindProjectTemplates();
			else if (Request["IssEBox"] != null)
				BindIssuesEBoxes();
			else if (Request["IssState"] != null)
				BindIssuesState();
			else if (Request["IssGenCat"] != null)
				BindGeneralCategoriesForIssue();
			else if (Request["IssCat"] != null)
				BindIssuesCategories();
			else if (Request["IssType"] != null)
				BindIssuesTypes();
			else if (Request["IssSev"] != null)
				BindIssuesSeverities();
			else if (Request["IssBox"] != null)
				BindIssueBoxes();
			else if (Request["ShCals"] != null)
				BindSharedCalendars();
			else if (Request["PrjPhs"] != null)
				BindProjectPhases();
			else if (Request["DocStatus"] != null)
				BindDocStatus();
			else if (Request["DocGenCat"] != null)
				BindGeneralCategoriesForDocument();
			else if (Request["MoveFrame"] != null)
			{
				MoveFrame();
				return;
			}
			else if (Request["AddClip"] != null)
			{
				AddClip();
				return;
			}
			else if (Request["ClearClip"] != null)
			{
				ClearClip();
				return;
			}
			else if (Request["ProjectFinance"] != null)
			{
				BindProjectFinance();
				return;
			}
			else if (Request["ProjectFinanceCompare"] != null)
			{
				BindProjectFinanceCompare();
				return;
			}
			else if (Request["RssPath"] != null)
			{
				BindRssNews();
				return;
			}

			else if (Request["TopRight"] != null)
			{
				TopRightChange();
				return;
			}

			Response.ContentType = "text/xml";
			Response.Write(TreeView1.GetXml());
		}

		#region TopRightChange
		private void TopRightChange()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			string ss = "";
			if (Request["TopRight"].ToString() == "1")
				if (pc["HideTopFrame"] != null && pc["HideTopFrame"].ToString() == "1")
				{
					pc["HideTopFrame"] = "0";
					ss = "0";
				}
				else
				{
					pc["HideTopFrame"] = "1";
					ss = "1";
				}

			Response.ContentType = "text/plain";
			Response.Write(ss);
		}
		#endregion

		#region BindRssNews
		private void BindRssNews()
		{
			string ss = "";
			string RssPath = (Request["RssPath"] == null) ? "" : Request["RssPath"].ToString();
			int RssCount = (Request["RssCount"] == null) ? 0 : int.Parse(Request["RssCount"].ToString());
			if (RssPath != "")
			{
				RssDataSource rss = new RssDataSource();
				GenericRssChannel channel = new GenericRssChannel();

				try
				{
					rss.Url = RssPath;
					rss.DataBind();
					channel = rss.Channel;
				}
				catch
				{
					Response.ContentType = "text/xml";
					Response.Write("<div style='text-align:center;padding:10px;color:red;' class='text'>" + LocRM.GetString("tRSSProblems") + "</div>");
					return;
				}
				//XMLUtilities
				// Create root element
				XmlDocument doc = new XmlDocument();
				XmlNode root = doc.AppendChild(doc.CreateElement("rssData"));
				XmlNode node = doc.CreateElement("htmlData");

				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("<div>");
				sb.AppendFormat("<div style='padding-top: 3px;padding-bottom:3px;color: #444;font-size: 10pt;'><b>{0}</b></div>", channel.Attributes["Title"]);
				for (int i = 0; i < channel.Items.Count && i < RssCount; i++)
				{
					sb.AppendFormat("<div style='padding-top: 3px;' class='text'><a href='{1}' target='_blank'>{0}</a></div>", channel.Items[i].Attributes["Title"], channel.Items[i].Attributes["Link"]);
					sb.AppendFormat("<div style='color: gray;' class='text'>{0}</div>", Convert.ToDateTime(channel.Items[i].Attributes["pubDate"]).ToString());
					//sb.AppendFormat("<div style='color: black; font-family: tahoma; font-size: 12px;'>{0}</div>", channel.Items[i].Attributes["description"]);
				}
				sb.Append("</div>");
				XmlNode cdata = doc.CreateCDataSection(sb.ToString());
				//node.Name = "htmlData";
				node.AppendChild(cdata);
				root.AppendChild(node);
				//doc.DocumentElement.AppendChild(doc.CreateCDataSection(sb.ToString()));
				ss += doc.InnerText;
			}
			Response.ContentType = "text/xml";
			Response.Write(ss);
		}
		#endregion

		#region Work With Frame
		private void MoveFrame()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			string ss = "";
			if (Request["MoveFrame"].ToString() == "left")
				if (pc["HideLeftFrame"] != null && pc["HideLeftFrame"].ToString() == "1")
				{
					pc["HideLeftFrame"] = "0";
					ss = "0";
				}
				else
				{
					pc["HideLeftFrame"] = "1";
					ss = "1";
				}
			else if (Request["MoveFrame"].ToString() == "r")
			{
				pc["HideLeftFrame"] = "0";
				ss = "0";
			}
			else if (Request["MoveFrame"].ToString() == "t")
			{
				if (pc["HidePartTopFrame"] != null && pc["HidePartTopFrame"].ToString() == "1")
				{
					pc["HidePartTopFrame"] = "0";
					ss = "0";
				}
				else
				{
					pc["HidePartTopFrame"] = "1";
					ss = "1";
				}
			}
			else
			{
				try
				{
					int lWidth = int.Parse(Request["MoveFrame"]);
					if (lWidth > 10)
					{
						pc["LeftFrameWidth"] = lWidth.ToString();
					}
				}
				catch { }
			}
			Response.ContentType = "text/plain";
			Response.Write(ss);
		}
		#endregion

		#region Work With Clipboard
		#region AddClip
		private void AddClip()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			int iCount = 10;
			if (pc["ClipboardItemsCount"] != null)
				iCount = int.Parse(pc["ClipboardItemsCount"].ToString());
			string ss = "";
			if (Request["AddClip"].ToString() == "Prj")
			{
				try
				{
					int ProjectId = int.Parse(Request["ProjectId"].ToString());
					string sNewPrjClip = "";
					if (pc["ClipboardPrjs"] != null)
						sNewPrjClip = pc["ClipboardPrjs"].ToString();
					sNewPrjClip = WorkWithClipboard(iCount, ProjectId.ToString() + "|" + sNewPrjClip);
					pc["ClipboardPrjs"] = sNewPrjClip;
					ss = String.Format(LocRM.GetString("tProjectWasAdded"), ProjectId.ToString());
				}
				catch
				{
					ss = LocRM.GetString("tProjectWasNotAdded");
				}
			}
			else if (Request["AddClip"].ToString() == "Files")
			{
				try
				{
					ArrayList alFiles = new ArrayList();
					string sFiles = Request["Files"];
					while (sFiles.Length > 0)
					{
						alFiles.Add(sFiles.Substring(0, sFiles.IndexOf(",")));
						sFiles = sFiles.Remove(0, sFiles.IndexOf(",") + 1);
					}
					foreach (string sFileId in alFiles)
					{
						int FileId = int.Parse(sFileId);
						string sNewFileClip = "";
						if (pc["ClipboardFiles"] != null)
							sNewFileClip = pc["ClipboardFiles"].ToString();
						sNewFileClip = WorkWithClipboard(iCount, sFileId + "|" + sNewFileClip);
						pc["ClipboardFiles"] = sNewFileClip;
					}

					ss = String.Format(LocRM.GetString("tFilesAdded"), alFiles.Count.ToString());
				}
				catch
				{
					ss = LocRM.GetString("tFilesNotAdded");
				}
			}
			else if (Request["AddClip"].ToString() == "Issue")
			{
				try
				{
					int IncidentId = int.Parse(Request["IncidentId"].ToString());
					string sNewIssueClip = "";
					if (pc["ClipboardIssues"] != null)
						sNewIssueClip = pc["ClipboardIssues"].ToString();
					sNewIssueClip = WorkWithClipboard(iCount, IncidentId.ToString() + "|" + sNewIssueClip);
					pc["ClipboardIssues"] = sNewIssueClip;
					ss = String.Format(LocRM.GetString("tIssueWasAdded"), IncidentId.ToString());
				}
				catch
				{
					ss = LocRM.GetString("tIssueWasNotAdded");
				}
			}
			Response.ContentType = "text/plain";
			Response.Write(ss);
		}
		#endregion

		#region WorkWithClipboard
		private string WorkWithClipboard(int iCount, string sClip)
		{
			int pCount = 0;
			string sCheck = sClip;
			int[] iArray = new int[iCount + 1];
			ArrayList aList = new ArrayList();
			while (sCheck.Length > 0)
			{
				if (sCheck.IndexOf("|") >= 0)
				{
					int iObj = int.Parse(sCheck.Substring(0, sCheck.IndexOf("|")));
					if (!aList.Contains(iObj))
					{
						aList.Add(iObj);
						iArray[pCount++] = iObj;
					}
					sCheck = sCheck.Substring(sCheck.IndexOf("|") + 1);
				}
				if (pCount >= iCount)
					break;
			}
			sClip = "";
			for (int i = 0; i < pCount; i++)
				sClip += iArray[i].ToString() + "|";
			return sClip;
		}
		#endregion

		#region ClearClip
		private void ClearClip()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			string ss = "";
			if (Request["ClearClip"].ToString() == "Prj")
			{
				try
				{
					pc["ClipboardPrjs"] = null;
					ss = LocRM.GetString("tCBWasCleared");
				}
				catch
				{
					ss = LocRM.GetString("tCBWasNotCleared");
				}
			}
			else if (Request["ClearClip"].ToString() == "Ast")
			{
				try
				{
					pc["ClipboardAsts"] = null;
					ss = LocRM.GetString("tCBWasCleared");
				}
				catch
				{
					ss = LocRM.GetString("tCBWasNotCleared");
				}
			}
			else if (Request["ClearClip"].ToString() == "Issue")
			{
				try
				{
					pc["ClipboardIssues"] = null;
					ss = LocRM.GetString("tCBWasCleared");
				}
				catch
				{
					ss = LocRM.GetString("tCBWasNotCleared");
				}
			}
			Response.ContentType = "text/plain";
			Response.Write(ss);
		}
		#endregion
		#endregion

		#region BindLibrary
		private void BindLibrary(int parentId)
		{
			using (IDataReader reader = Project.GetListProjects())
			{
				while (reader.Read())
				{
					TreeViewNode rootNode = new TreeViewNode();
					rootNode.Text = reader["Title"].ToString();
					rootNode.NavigateUrl = "../FileStorage/default.aspx?ProjectId=" + reader["ProjectId"].ToString();

					TreeViewNode node;
					//					node = new TreeViewNode();
					//					node.Text = LocRM.GetString("tSimpleIssues");
					//					node.NavigateUrl = "../FileStorage/default.aspx?ProjectId="+reader["ProjectId"].ToString()+"&TypeId=7";
					//					rootNode.Nodes.Add(node);

					node = new TreeViewNode();
					node.Text = LocRM.GetString("tSimpleTasks");
					node.NavigateUrl = "../FileStorage/default.aspx?ProjectId=" + reader["ProjectId"].ToString() + "&TypeId=5";
					rootNode.Nodes.Add(node);

					node = new TreeViewNode();
					node.Text = LocRM.GetString("tTabDocuments");
					node.NavigateUrl = "../FileStorage/default.aspx?ProjectId=" + reader["ProjectId"].ToString() + "&TypeId=16";
					rootNode.Nodes.Add(node);

					node = new TreeViewNode();
					node.Text = LocRM.GetString("tSimpleEvents");
					node.NavigateUrl = "../FileStorage/default.aspx?ProjectId=" + reader["ProjectId"].ToString() + "&TypeId=4";
					rootNode.Nodes.Add(node);

					node = new TreeViewNode();
					node.Text = LocRM.GetString("tSimpleToDos");
					node.NavigateUrl = "../FileStorage/default.aspx?ProjectId=" + reader["ProjectId"].ToString() + "&TypeId=6";
					rootNode.Nodes.Add(node);

					TreeView1.Nodes.Add(rootNode);
				}
			}
		}
		#endregion

		#region BindSecurityDirectory
		private void BindSecurityDirectory(int parentId)
		{
			if (Security.IsUserInGroup(InternalSecureGroups.Partner) && (parentId == 1 || parentId == 6))
			{
				int iCurGroupId = -1;
				ArrayList VisiblePartnerGroups = new ArrayList();
				ArrayList VisibleUnpartnerGroups = new ArrayList();
				using (IDataReader reader = Mediachase.IBN.Business.User.GetListSecureGroup(Security.CurrentUser.UserID))
				{
					if (reader.Read())
					{
						iCurGroupId = (int)reader["GroupId"];
					}
				}
				VisiblePartnerGroups.Clear();
				VisibleUnpartnerGroups.Clear();
				VisiblePartnerGroups.Add(iCurGroupId);
				using (IDataReader reader = SecureGroup.GetListGroupsByPartner(iCurGroupId))
				{
					while (reader.Read())
					{
						int iGroupId = (int)reader["GroupId"];
						if (SecureGroup.IsPartner(iGroupId))
							VisiblePartnerGroups.Add(iGroupId);
						else
							VisibleUnpartnerGroups.Add(iGroupId);
					}
				}
				ClearUnpartnerGroups(VisibleUnpartnerGroups);
				if (VisibleUnpartnerGroups.Contains(1))
				{
					BindOrdinaryTree(parentId);
					return;
				}

				ArrayList children = new ArrayList();
				if (parentId == 1)
				{
					children.AddRange(VisibleUnpartnerGroups);
					children.Add(6);
				}
				else if (parentId == 6)
				{
					children.AddRange(VisiblePartnerGroups);
				}
				children.Sort();
				foreach (int iGroupId in children)
				{
					TreeViewNode node = new TreeViewNode();
					node.ImageHeight = 16;
					node.ImageWidth = 16;
					node.ImageUrl = "../Layouts/Images/icons/regular.gif";
					node.ExpandedImageUrl = "../Layouts/Images/icons/regular.gif";
					if (iGroupId < 9 && iGroupId != 6)
					{
						node.ImageUrl = "../Layouts/Images/icons/Admins.gif";
						node.ExpandedImageUrl = "../Layouts/Images/icons/Admins.gif";
					}
					if (SecureGroup.IsPartner(iGroupId) || iGroupId == 6)
					{
						node.ImageUrl = "../Layouts/Images/icons/Partners.gif";
						node.ExpandedImageUrl = "../Layouts/Images/icons/Partners.gif";
					}
					if (iGroupId == 6)
					{
						node.ContentCallbackUrl = "../Modules/XmlForTreeView.aspx?GroupID=6";
					}
					else
						using (IDataReader rdr = SecureGroup.GetListChildGroups(iGroupId))
						{
							if (rdr.Read())
								node.ContentCallbackUrl = "../Modules/XmlForTreeView.aspx?GroupID=" + iGroupId.ToString();
						}
					using (IDataReader rdr = SecureGroup.GetGroup(iGroupId))
					{
						if (rdr.Read())
							node.Text = CommonHelper.GetResFileString(rdr["GroupName"].ToString());
					}
					node.ID = "sgroup" + iGroupId.ToString();
					node.Value = iGroupId.ToString();
					node.NavigateUrl = "../Directory/Directory.aspx?Tab=0&SGroupID=" + iGroupId.ToString();
					TreeView1.Nodes.Add(node);
				}
			}
			else
				BindOrdinaryTree(parentId);
		}

		private void BindOrdinaryTree(int parentId)
		{
			using (IDataReader reader = SecureGroup.GetListChildGroups(parentId))
			{
				while (reader.Read())
				{
					int iGroupId = (int)reader["GroupId"];
					TreeViewNode node = new TreeViewNode();
					node.ImageHeight = 16;
					node.ImageWidth = 16;
					node.ImageUrl = "../Layouts/Images/icons/regular.gif";
					node.ExpandedImageUrl = "../Layouts/Images/icons/regular.gif";
					if (iGroupId < 9 && iGroupId != 6)
					{
						node.ImageUrl = "../Layouts/Images/icons/Admins.gif";
						node.ExpandedImageUrl = "../Layouts/Images/icons/Admins.gif";
					}
					if (SecureGroup.IsPartner(iGroupId) || iGroupId == 6)
					{
						node.ImageUrl = "../Layouts/Images/icons/Partners.gif";
						node.ExpandedImageUrl = "../Layouts/Images/icons/Partners.gif";
					}
					using (IDataReader rdr = SecureGroup.GetListChildGroups(iGroupId))
					{
						if (rdr.Read())
							node.ContentCallbackUrl = "../Modules/XmlForTreeView.aspx?GroupID=" + iGroupId.ToString();
					}
					node.Text = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					node.ID = "sgroup" + iGroupId.ToString();
					node.Value = iGroupId.ToString();
					node.NavigateUrl = "../Directory/Directory.aspx?Tab=0&SGroupID=" + iGroupId.ToString();
					TreeView1.Nodes.Add(node);
				}
			}
			if (parentId == 1)
				TreeView1.Nodes.Sort("Text", true);
		}
		#endregion

		#region BindContactDirectory
		private void BindContactDirectory()
		{
			int userIMGroupId = 0;
			DataTable dt = null;
			TreeViewNode node;

			if (!IMGroup.CanCreate())
			{
				using (IDataReader rdr = Mediachase.IBN.Business.User.GetUserInfo(Security.CurrentUser.UserID))
				{
					rdr.Read();
					userIMGroupId = (int)rdr["IMGroupId"];
				}

				dt = IMGroup.GetListIMGroupsYouCanSee(userIMGroupId);

				string imGroupName = IMGroup.GetIMGroupName(userIMGroupId, null);
				if (imGroupName != null)
				{
					DataRow dr = dt.NewRow();

					dr["IMGroupId"] = userIMGroupId;
					dr["IMGroupName"] = imGroupName;

					dt.Rows.InsertAt(dr, 0);
				}
			}
			else
			{
				dt = IMGroup.GetListIMGroup();
			}

			foreach (DataRow dr in dt.Rows)
			{
				int imGroupId = (int)dr["IMGroupId"];
				node = new TreeViewNode();
				node.ImageHeight = 16;
				node.ImageWidth = 16;
				node.ImageUrl = "../Layouts/Images/icons/ibngroup.gif";
				node.ExpandedImageUrl = "../Layouts/Images/icons/ibngroup.gif";
				node.Text = dr["IMGroupName"].ToString();
				node.NavigateUrl = "../Directory/Directory.aspx?Tab=1&IMGroupID=" + imGroupId.ToString();
				TreeView1.Nodes.Add(node);
			}
		}
		#endregion

		#region BindListFolders1
		private void BindListFolders1(int parentId)
		{
			TreeViewNode node;
			if (parentId == 0)
			{
				ListFolder privFolder = ListManager.GetPrivateRoot(Mediachase.Ibn.Data.Services.Security.CurrentUserId);
				node = new TreeViewNode();
				node.ImageHeight = 16;
				node.ImageWidth = 16;
				node.NavigateUrl = "~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + privFolder.PrimaryKeyId.Value.ToString();
				node.Text = LocRM.GetString("tPrivLists");
				node.ID = "listfolder" + privFolder.PrimaryKeyId.Value.ToString();
				node.Value = privFolder.PrimaryKeyId.Value.ToString();
				if (privFolder.HasChildren)
					node.ContentCallbackUrl = ResolveUrl("~/Modules/XmlForTreeView.aspx") + "?ListFolderId1=" + privFolder.PrimaryKeyId.Value.ToString();
				TreeView1.Nodes.Add(node);

				ListFolder pubFolder = ListManager.GetPublicRoot();
				node = new TreeViewNode();
				node.ImageHeight = 16;
				node.ImageWidth = 16;
				node.NavigateUrl = "~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + pubFolder.PrimaryKeyId.Value.ToString();
				node.Text = LocRM.GetString("tPubLists");
				node.ID = "listfolder" + pubFolder.PrimaryKeyId.Value.ToString();
				node.Value = pubFolder.PrimaryKeyId.Value.ToString();
				if (pubFolder.HasChildren)
					node.ContentCallbackUrl = ResolveUrl("~/Modules/XmlForTreeView.aspx") + "?ListFolderId1=" + pubFolder.PrimaryKeyId.Value.ToString();
				TreeView1.Nodes.Add(node);

				if (Configuration.ProjectManagementEnabled)
				{
					node = new TreeViewNode();
					node.ImageHeight = 16;
					node.ImageWidth = 16;
					node.NavigateUrl = "~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=-1";
					node.Text = LocRM.GetString("tPrjLists");
					node.ID = "listfolder_1";
					node.Value = "-1";
					TreeView1.Nodes.Add(node);
				}
			}
			else
			{
				ListFolder fParent = new ListFolder(parentId);
				if (fParent != null)
				{
					Mediachase.Ibn.Data.Services.TreeService ts = fParent.GetService<Mediachase.Ibn.Data.Services.TreeService>();
					foreach (Mediachase.Ibn.Data.Services.TreeNode tN in ts.GetChildNodes())
					{
						MetaObject moFolder = tN.InnerObject;
						ListFolder folder = new ListFolder(moFolder.PrimaryKeyId.Value);
						int iFolderId = folder.PrimaryKeyId.Value;
						node = new TreeViewNode();
						node.Text = folder.Title;

						bool IsPrivate = (folder.FolderType == ListFolderType.Private);
						if (folder.HasChildren)
						{
							if (Request["MoveList"] == null)
								node.ContentCallbackUrl = ResolveUrl("~/Modules/XmlForTreeView.aspx") + "?ListFolderId1=" + iFolderId.ToString();
							else
							{
								string sMoveList = Request["MoveList"];
								node.ContentCallbackUrl = ResolveUrl("~/Modules/XmlForTreeView.aspx") + "?MoveList=" + sMoveList + "&ListFolderId1=" + iFolderId.ToString();
							}
						}
						node.ID = "listfolder" + iFolderId.ToString();
						if (Request["MoveList"] != null && IsPrivate)
							node.ID += "private";
						node.Value = iFolderId.ToString();
						if (Request["MoveList"] == null)
							node.NavigateUrl = "~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + iFolderId.ToString();
						TreeView1.Nodes.Add(node);
					}
				}
			}
		}
		#endregion

		#region BindProjectStatus
		private void BindProjectStatus()
		{
			using (IDataReader reader = Project.GetListProjectStatus())
			{
				while (reader.Read())
				{
					int StatusId = (int)reader["StatusId"];
					TreeViewNode node = new TreeViewNode();
					string img = "project_active.gif";
					Project.ProjectStatus ps = (Project.ProjectStatus)StatusId;
					if (ps == Project.ProjectStatus.Pending || ps == Project.ProjectStatus.OnHold)
						img = "project_pending.gif";
					if (ps == Project.ProjectStatus.AtRisk)
						img = "project_atrisk.gif";
					if (ps == Project.ProjectStatus.Completed || ps == Project.ProjectStatus.Cancelled)
						img = "project_completed.gif";
					node.ImageUrl = "../layouts/images/icons/" + img;
					node.ImageWidth = 16;
					node.ImageHeight = 16;
					node.Text = reader["StatusName"].ToString();
					node.NavigateUrl = "../Apps/ProjectManagement/Pages/ProjectList.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindProjectTypes
		private void BindProjectTypes()
		{
			using (IDataReader reader = Project.GetListProjectTypes())
			{
				while (reader.Read())
				{
					int TypeId = (int)reader["TypeId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["TypeName"].ToString();
					node.Value = "PrjTyp";
					node.NavigateUrl = "../Apps/ProjectManagement/Pages/ProjectList.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindPortfolios
		private void BindPortfolios()
		{
			using (IDataReader reader = ProjectGroup.GetProjectGroups())
			{
				while (reader.Read())
				{
					int ProjectGroupId = (int)reader["ProjectGroupId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["Title"].ToString();
					node.Value = ProjectGroupId.ToString() + "PrjGrp";
					node.NavigateUrl = "../Apps/ProjectManagement/Pages/ProjectList.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindProjectCategories
		private void BindProjectCategories()
		{
			using (IDataReader reader = Project.GetListProjectCategories())
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["CategoryName"].ToString();
					node.Value = "PrjCat";
					node.NavigateUrl = "../Apps/ProjectManagement/Pages/ProjectList.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindGeneralCategoriesForProject
		private void BindGeneralCategoriesForProject()
		{
			using (IDataReader reader = Project.GetListCategoriesAll())
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["CategoryName"].ToString();
					node.Value = "GenCat";
					node.NavigateUrl = "../Apps/ProjectManagement/Pages/ProjectList.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindProjectTemplates
		private void BindProjectTemplates()
		{
			using (IDataReader reader = ProjectTemplate.GetListProjectTemplate())
			{
				while (reader.Read())
				{
					int TemplateId = (int)reader["TemplateId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["TemplateName"].ToString();
					node.Value = TemplateId.ToString() + "NewPrj";
					node.NavigateUrl = "../Projects/ProjectEdit.aspx?TemplateId=" + TemplateId.ToString();
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindIssuesEBoxes
		private void BindIssuesEBoxes()
		{
			TreeViewNode node = new TreeViewNode();
			node.Text = LocRM.GetString("tIntIss");
			node.NavigateUrl = "../Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
			TreeView1.Nodes.Add(node);
			if (Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
			{
				foreach (Mediachase.IBN.Business.Pop3.Pop3Box box in Mediachase.IBN.Business.Pop3.Pop3Manager.Current.GetPop3BoxList())
				{
					if (box.Handlers.Contains("IssueRequest.Pop3MessageHandler"))
					{
						int MailboxId = box.Id;
						node = new TreeViewNode();
						node.Text = box.Name;
						node.NavigateUrl = "../Incidents/default.aspx?BTab=MailIncidents&MailBoxId=" + MailboxId.ToString();
						TreeView1.Nodes.Add(node);
					}
				}
			}
			/*	using (IDataReader reader = Mailbox.GetSimple())
				{
					while (reader.Read())
					{
						if ((int)reader["Type"]>0)
							continue;
						int MailboxId = (int)reader["MailboxId"];
						node = new TreeViewNode();
						node.Text = reader["Title"].ToString();
						node.NavigateUrl = "../Incidents/default.aspx?BTab=MailIncidents&MailBoxId="+MailboxId.ToString();
						TreeView1.Nodes.Add(node);
					}
				}*/
		}
		#endregion

		#region BindIssuesState
		private void BindIssuesState()
		{
			using (IDataReader reader = Mediachase.IBN.Business.Incident.GetListIncidentStates())
			{
				while (reader.Read())
				{
					int StateId = (int)reader["StateId"];
					if ((int)reader["StateId"] == (int)ObjectStates.Overdue)
						continue;
					TreeViewNode node = new TreeViewNode();
					string img = "incident_closed.gif";
					switch (StateId)
					{
						case (int)ObjectStates.Upcoming:
							img = "incident_new.gif";
							break;
						case (int)ObjectStates.Active:
						case (int)ObjectStates.ReOpen:
							img = "incident_active.gif";
							break;
						default:
							break;
					}
					node.ImageUrl = "../layouts/images/icons/" + img;
					node.ImageWidth = 16;
					node.ImageHeight = 16;
					node.Text = reader["StateName"].ToString();
					node.NavigateUrl = "../Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindGeneralCategoriesForIssue
		private void BindGeneralCategoriesForIssue()
		{
			using (IDataReader reader = Incident.GetListCategoriesAll())
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["CategoryName"].ToString();
					node.Value = "GenCat";
					node.NavigateUrl = "../Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindIssuesCategories
		private void BindIssuesCategories()
		{
			using (IDataReader reader = Incident.GetListIncidentCategories())
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["CategoryName"].ToString();
					node.Value = "IssCat";
					node.NavigateUrl = "../Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindIssuesTypes
		private void BindIssuesTypes()
		{
			using (IDataReader reader = Incident.GetListIncidentTypes())
			{
				while (reader.Read())
				{
					int TypeId = (int)reader["TypeId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["TypeName"].ToString();
					node.Value = "IssTyp";
					node.NavigateUrl = "../Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindIssuesSeverities
		private void BindIssuesSeverities()
		{
			using (IDataReader reader = Incident.GetListIncidentSeverity())
			{
				while (reader.Read())
				{
					int SeverityId = (int)reader["SeverityId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["SeverityName"].ToString();
					node.Value = "IssSev";
					node.NavigateUrl = "../Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindIssueBoxes
		private void BindIssueBoxes()
		{
			foreach (IncidentBox ib in IncidentBox.List())
			{
				TreeViewNode node = new TreeViewNode();
				node.Text = ib.Name;
				node.NavigateUrl = "../Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
				TreeView1.Nodes.Add(node);
			}
		}
		#endregion

		#region BindSharedCalendars
		private void BindSharedCalendars()
		{
			using (IDataReader reader = Mediachase.IBN.Business.CalendarView.GetListPeopleForCalendar())
			{
				while (reader.Read())
				{
					int iUserId = (int)reader["UserId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["LastName"].ToString() + " " + reader["FirstName"].ToString();
					node.NavigateUrl = "../Calendar/default.aspx?Tab=SharedCalendars&PersonId=" + iUserId.ToString();
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region ClearUnpartnerGroups
		private void ClearUnpartnerGroups(ArrayList VisibleUnpartnerGroups)
		{
			ArrayList alDelete = new ArrayList();
			for (int i = 0; i < VisibleUnpartnerGroups.Count - 1; i++)
			{
				for (int j = i + 1; j < VisibleUnpartnerGroups.Count; j++)
				{
					int a1 = (int)VisibleUnpartnerGroups[i];
					int a2 = (int)VisibleUnpartnerGroups[j];
					if (IsChild(a1, a2) && !alDelete.Contains(a2))
						alDelete.Add(a2);
				}
			}
			foreach (int k in alDelete)
			{
				VisibleUnpartnerGroups.Remove(k);
			}
		}

		private bool IsChild(int ParentId, int ChildId)
		{
			bool retVal = false;
			int CurParent = ChildId;
			while (CurParent > 1)
			{
				CurParent = SecureGroup.GetParentGroup(CurParent);
				if (CurParent == ParentId)
				{
					retVal = true;
					break;
				}
			}

			return retVal;
		}
		#endregion

		#region BindProjectPhases
		private void BindProjectPhases()
		{
			using (IDataReader reader = Project.GetListProjectPhases())
			{
				while (reader.Read())
				{
					int PhaseId = (int)reader["PhaseId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["PhaseName"].ToString();
					node.Value = "PrjPhs";
					node.NavigateUrl = "../Apps/ProjectManagement/Pages/ProjectList.aspx";
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindDocStatus
		private void BindDocStatus()
		{
			using (IDataReader reader = Document.GetListDocumentStatus())
			{
				while (reader.Read())
				{
					int StatusId = (int)reader["StatusId"];
					TreeViewNode node = new TreeViewNode();
					/*string img = "incident_closed.gif";
					switch(StatusId) 
					{
						case 1:
							img = "incident_new.gif"; 
							break;
						case 2:
							img = "incident_active.gif";
							break;
						default:
							break;
					}
					node.ImageUrl = "../layouts/images/icons/"+img;
					node.ImageWidth=16;
					node.ImageHeight=16;*/
					node.Text = reader["StatusName"].ToString();
					node.Value = "DocStat";
					node.NavigateUrl = "../Documents/default.aspx?DocStatus=" + StatusId.ToString();
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindGeneralCategoriesForDocument
		private void BindGeneralCategoriesForDocument()
		{
			using (IDataReader reader = Document.GetListCategoriesAll())
			{
				while (reader.Read())
				{
					int CategoryId = (int)reader["CategoryId"];
					TreeViewNode node = new TreeViewNode();
					node.Text = reader["CategoryName"].ToString();
					node.Value = "GenCat";
					node.NavigateUrl = "../Documents/default.aspx?GenCat=" + CategoryId.ToString();
					TreeView1.Nodes.Add(node);
				}
			}
		}
		#endregion

		#region BindProjectFinance
		private void BindProjectFinance()
		{

			SpreadSheetView view = ProjectSpreadSheet.LoadView(int.Parse(Request["ProjectId"]), int.Parse(Request["BasePlanSlotId1"]), int.Parse(Request["FromYear"]), int.Parse(Request["ToYear"]));
			SpreadSheetView view2 = ProjectSpreadSheet.LoadView(int.Parse(Request["ProjectId"]), int.Parse(Request["BasePlanSlotId2"]), int.Parse(Request["FromYear"]), int.Parse(Request["ToYear"]));

			//Change data on-fly
			if (Request["action"] != null)
			{
				//Drag and Drop
				if (Request["action"] == "moverow")
				{
					view.ReOrderBlockRow(Request["blockId"], Request["rowIdOld"], Request["rowIdNew"]);
				}
				//update excisting cell
				if (Request["action"] == "update")
				{
					view.SetValue(((Column)view.Columns[int.Parse(Request["cellnumber"])]).Id, Request["rowid"], Request["Value"]);
				}//add new row
				else if (Request["action"] == "newrow")
				{
					view.AddBlockRow(Request["RowId"], Request["RowId"] + "-" + Request["newrowid"]);
				}//delete row
				else if (Request["action"] == "deleterow")
				{
					view.DeleteBlockRow(Request["RowId"]);
				}

				//update Database
				ProjectSpreadSheet.SaveView(int.Parse(Request["ProjectId"]), int.Parse(Request["BasePlanSlotId1"]), view);
			}

			Response.ContentType = "text/xml";
			XmlDocument doc;
			if (Request["action"] != null)
			{
				if (Request["action"] == "moverow")
					return;
				//generate xml for update 
				if (Request["compare"] == null)
					doc = ProjectSpreadSheet.CreateDocChanges(view);
				else
					doc = ProjectSpreadSheet.CreateDocChangesCompare(view, view2);
			}
			else if (Request["compare"] != null)
			{
				//doc = TestSpreadSheetDocumentFactory.CreateViewDoc2(view);
				doc = ProjectSpreadSheet.CreateViewCompareDoc(view, view2);
			}
			else
			{
				//genereate xml for new grid with one index
				doc = ProjectSpreadSheet.CreateViewDoc(view);
			}

			Response.Write(doc.InnerXml);
		}
		#endregion

		#region BindProjectFinanceCompare
		private ArrayList LoadProjectList()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			ArrayList projectList = new ArrayList();

			//UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

			string strProjectListType = pc["Report_ProjectListType"];

			if (strProjectListType == null)
				strProjectListType = "All";

			if (strProjectListType == "Custom")
			{
				string strProjectList = pc["Report_ProjectListData"];

				if (strProjectList != null)
				{
					foreach (string strItem in strProjectList.Split(';'))
					{
						string strPrjId = strItem.Trim();

						if (strPrjId != string.Empty)
						{
							projectList.Add(int.Parse(strPrjId));
						}
					}
				}
			}
			else if (strProjectListType == "All")
			{
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
					{
						projectList.Add((int)reader["ProjectId"]);
					}
				}
			}
			else if (strProjectListType == "Portfolio")
			{
				string strPortfolioId = pc["Report_ProjectListData"];

				DataTable dt = Project.GetListProjectGroupedByPortfolio(int.Parse(strPortfolioId), 0, 0);

				foreach (DataRow row in dt.Rows)
				{
					int prid = int.Parse(row["ProjectId"].ToString());
					if (prid > 0)
						projectList.Add(prid);
				}
			}

			return projectList;
		}

		private void BindProjectFinanceCompare()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			ArrayList PrIds = LoadProjectList();

			SpreadSheetDocumentType type = SpreadSheetDocumentType.WeekYear;

			if (pc["ProjectsByBS_FinanceType"] != null)
			{
				switch (pc["ProjectsByBS_FinanceType"])
				{
					case "1":
						type = SpreadSheetDocumentType.WeekYear;
						break;
					case "2":
						type = SpreadSheetDocumentType.MonthQuarterYear;
						break;
					case "3":
						type = SpreadSheetDocumentType.QuarterYear;
						break;
					case "4":
						type = SpreadSheetDocumentType.Year;
						break;
					case "5":
						type = SpreadSheetDocumentType.Total;
						break;
				}
			}

			int ProjectsByBS_BasePlan1 = 0;
			if (pc["ProjectsByBS_BasePlan1"] != null)
				ProjectsByBS_BasePlan1 = int.Parse(pc["ProjectsByBS_BasePlan1"]);

			int ProjectsByBS_BasePlan2 = -2;
			if (pc["ProjectsByBS_BasePlan2"] != null)
				ProjectsByBS_BasePlan2 = int.Parse(pc["ProjectsByBS_BasePlan2"]);

			int ProjectsByBS_FromYear = DateTime.Now.Year;
			if (pc["ProjectsByBS_FromYear"] != null)
				ProjectsByBS_FromYear = int.Parse(pc["ProjectsByBS_FromYear"]);

			int ProjectsByBS_ToYear = ProjectsByBS_FromYear;
			if (pc["ProjectsByBS_ToYear"] != null)
				ProjectsByBS_ToYear = int.Parse(pc["ProjectsByBS_ToYear"]);

			bool Reverse = false;
			if (pc["ProjectsByBS_Reverse"] != null)
				Reverse = bool.Parse(pc["ProjectsByBS_Reverse"]);

			SpreadSheetView view = null;
			if (!Reverse)
				view = ProjectSpreadSheet.CompareProjects(PrIds, type, ProjectsByBS_BasePlan1,
				ProjectsByBS_FromYear, ProjectsByBS_ToYear);
			else
				view = ProjectSpreadSheet.CompareProjectsReverse(PrIds, type, ProjectsByBS_BasePlan1,
					ProjectsByBS_FromYear, ProjectsByBS_ToYear);
			Response.ContentType = "text/xml";
			XmlDocument doc;
			if (Request["compare"] == null || ProjectsByBS_BasePlan2 == -2)
			{
				doc = ProjectSpreadSheet.CreateViewDocForAnalysis(view);
			}
			else
			{
				SpreadSheetView view2 = null;
				if (!Reverse)
					view2 = ProjectSpreadSheet.CompareProjects(PrIds, type,
					ProjectsByBS_BasePlan2,
					ProjectsByBS_FromYear, ProjectsByBS_ToYear);
				else
					view2 = ProjectSpreadSheet.CompareProjectsReverse(PrIds, type,
						ProjectsByBS_BasePlan2,
						ProjectsByBS_FromYear, ProjectsByBS_ToYear);

				doc = ProjectSpreadSheet.CreateViewCompareDocForAnalysis(view, view2);
			}

			Response.Write(doc.InnerXml);
		}
		#endregion
	}
}
