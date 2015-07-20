using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for Security.
	/// </summary>
	public partial class Security : System.Web.UI.Page
	{
		#region HTML Vars
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddEditPermission", typeof(Security).Assembly);

		private int _folderId = -1;
		private string _containerKey = "";
		private string _containerName = "";
		#region Request Variables
		protected int FolderId
		{
			get
			{
				if (Request["FolderId"] != null)
					return int.Parse(Request["FolderId"]);
				else
					return _folderId;
			}
			set
			{
				_folderId = value;
			}
		}

		protected string ContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return _containerKey;
			}
			set
			{
				_containerKey = value;
			}
		}

		protected string ContainerName
		{
			get
			{
				if (Request["ContainerName"] != null)
					return Request["ContainerName"];
				else
					return _containerName;
			}
			set
			{
				_containerName = value;
			}
		}
		#endregion

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.FileStorage fs;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			ApplyAttributes();
			BindToolbar();
			if (!Page.IsPostBack)
			{
				GetACLData();
				BindGroups();
				BindRights();
				BinddgMembers();
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (ViewState["isChecked"].ToString() == "1")
				cbInherit.Checked = true;
			else cbInherit.Checked = false;
			if (FolderId == fs.Root.Id)
				tblInhereted.Style.Add("display", "none");
		}

		#region GetACLData
		private void GetACLData()
		{
			AccessControlList ACLr = AccessControlList.GetACL(FolderId);
			ViewState["ACL"] = ACLr;

			if (ACLr.IsInherited)
				ViewState["isChecked"] = "1";
			else
				ViewState["isChecked"] = "0";
		}
		#endregion

		#region ApplyAttributes
		private void ApplyAttributes()
		{
			if (Request["PrimaryKeyId"] != null)
			{
				string[] elem = Request["PrimaryKeyId"].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				FolderId = int.Parse(elem[1]);
				ContainerName = elem[2];
				ContainerKey = elem[3];
			}

			bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

			btnSave.Attributes.Add("onclick", "DisableButtons(this);DisableCheck();");
			btnAdd.InnerText = LocRM.GetString("tAdd");
			dgMembers.Columns[1].HeaderText = LocRM.GetString("tName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("Rights");
			dgMembers.Columns[3].HeaderText = LocRM.GetString("tAllow");
			//dgMembers.Columns[4].HeaderText = LocRM.GetString("tDeny");

			lgdPModal.InnerText = LocRM.GetString("Security");
			lgdLostRights.InnerText = LocRM.GetString("tAttention");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			DirectoryInfo di = fs.GetDirectory(FolderId);
			string sName = di.Name;
			if (fs.Root.Id == FolderId)
				sName = LocRM.GetString("tRoot");
			if (sName.Length > 33)
				sName = sName.Substring(0, 30) + "...";
			secHeader.Title = LocRM.GetString("tEditSec") + " \"" + sName + "\"";
			secHeader.AddLink("<img alt='' src='../Layouts/Images/saveitem.gif'/> " + LocRM.GetString("Save"), "javascript:FuncSave();");
			secHeader.AddSeparator();
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("Cancel"), "javascript:window.close();");
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(LocRM.GetString("tRoles"), "0"));
			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(true, false, false, false, false, true, true, false, false, false, false))
			{
				while (reader.Read())
				{
					ddGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindUsers(int.Parse(li.Value));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int GroupID)
		{
			ddUsers.Items.Clear();
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			ArrayList alList = new ArrayList();
			foreach (AccessControlEntry ACEr in ACLr)
			{
				if (ACEr.OwnerKey == Guid.Empty)
					alList.Add((ACEr.IsRoleAce) ? ACEr.Role : ACEr.PrincipalId.ToString());
			}
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(string)));					// 0
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));				// 1
			DataRow dr;
			if (GroupID == 0)
			{
				#region Roles

				#region General Roles
				if (!alList.Contains(UserRoleHelper.AdminRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.AdminRoleName;
					dr[1] = LocRM.GetString("tAdmins");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.PowerProjectManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.PowerProjectManagerRoleName;
					dr[1] = LocRM.GetString("tPPManager");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.HelpDeskManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.HelpDeskManagerRoleName;
					dr[1] = LocRM.GetString("tHDM");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.GlobalProjectManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.GlobalProjectManagerRoleName;
					dr[1] = LocRM.GetString("tGlobalPManager");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.GlobalExecutiveManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.GlobalExecutiveManagerRoleName;
					dr[1] = LocRM.GetString("tGlobalEManager");
					dt.Rows.Add(dr);
				}

				if (!alList.Contains(UserRoleHelper.TimeManagerRoleName))
				{
					dr = dt.NewRow();
					dr[0] = UserRoleHelper.TimeManagerRoleName;
					dr[1] = LocRM.GetString("tTM");
					dt.Rows.Add(dr);
				}
				#endregion

				#region General Project Roles
				if (!ContainerKey.StartsWith("Workspace"))
				{
					if (!alList.Contains(UserRoleHelper.ProjectManagerRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.ProjectManagerRoleName;
						dr[1] = LocRM.GetString("tPManager");
						dt.Rows.Add(dr);
					}

					if (!alList.Contains(UserRoleHelper.ExecutiveManagerRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.ExecutiveManagerRoleName;
						dr[1] = LocRM.GetString("tExMan");
						dt.Rows.Add(dr);
					}

					if (!alList.Contains(UserRoleHelper.ProjectTeamRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.ProjectTeamRoleName;
						dr[1] = LocRM.GetString("tTeam");
						dt.Rows.Add(dr);
					}

					if (!alList.Contains(UserRoleHelper.ProjectSponsorRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.ProjectSponsorRoleName;
						dr[1] = LocRM.GetString("tSponsors");
						dt.Rows.Add(dr);
					}

					if (!alList.Contains(UserRoleHelper.ProjectStakeHolderRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.ProjectStakeHolderRoleName;
						dr[1] = LocRM.GetString("tStake");
						dt.Rows.Add(dr);
					}
				}
				#endregion

				#region Incident Roles
				if (ContainerKey.StartsWith("IncidentId_"))
				{
					if (!alList.Contains(UserRoleHelper.IssueCreatorRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.IssueCreatorRoleName;
						dr[1] = LocRM.GetString("tICreator");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.IssueManagerRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.IssueManagerRoleName;
						dr[1] = LocRM.GetString("tIManager");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.IssueResourceRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.IssueResourceRoleName;
						dr[1] = LocRM.GetString("tIResource");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.IssueTodoResource))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.IssueTodoResource;
						dr[1] = LocRM.GetString("tITodoResource");
						dt.Rows.Add(dr);
					}
				}
				#endregion

				#region Task Roles
				if (ContainerKey.StartsWith("TaskId_"))
				{
					if (!alList.Contains(UserRoleHelper.TaskCreatorRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.TaskCreatorRoleName;
						dr[1] = LocRM.GetString("tTkCreator");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.TaskManagerRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.TaskManagerRoleName;
						dr[1] = LocRM.GetString("tTkManager");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.TaskResourceRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.TaskResourceRoleName;
						dr[1] = LocRM.GetString("tTkResource");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.TaskTodoResource))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.TaskTodoResource;
						dr[1] = LocRM.GetString("tTkTodoResource");
						dt.Rows.Add(dr);
					}
				}
				#endregion

				#region Document Roles
				if (ContainerKey.StartsWith("DocumentId_"))
				{
					if (!alList.Contains(UserRoleHelper.DocumentCreatorRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.DocumentCreatorRoleName;
						dr[1] = LocRM.GetString("tDocCreator");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.DocumentManagerRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.DocumentManagerRoleName;
						dr[1] = LocRM.GetString("tDocManager");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.DocumentResourceRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.DocumentResourceRoleName;
						dr[1] = LocRM.GetString("tDocResource");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.DocumentTodoResource))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.DocumentTodoResource;
						dr[1] = LocRM.GetString("tDocTodoResource");
						dt.Rows.Add(dr);
					}
				}
				#endregion

				#region Event Roles
				if (ContainerKey.StartsWith("EventId_"))
				{
					if (!alList.Contains(UserRoleHelper.EventCreatorRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.EventCreatorRoleName;
						dr[1] = LocRM.GetString("tEvCreator");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.EventManagerRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.EventManagerRoleName;
						dr[1] = LocRM.GetString("tEvManager");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.EventResourceRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.EventResourceRoleName;
						dr[1] = LocRM.GetString("tEvResources");
						dt.Rows.Add(dr);
					}
				}
				#endregion

				#region ToDo Roles
				if (ContainerKey.StartsWith("ToDoId_"))
				{
					if (!alList.Contains(UserRoleHelper.TodoCreatorRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.TodoCreatorRoleName;
						dr[1] = LocRM.GetString("tTdCreator");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.TodoManagerRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.TodoManagerRoleName;
						dr[1] = LocRM.GetString("tTdManager");
						dt.Rows.Add(dr);
					}
					if (!alList.Contains(UserRoleHelper.TodoResourceRoleName))
					{
						dr = dt.NewRow();
						dr[0] = UserRoleHelper.TodoResourceRoleName;
						dr[1] = LocRM.GetString("tTdResources");
						dt.Rows.Add(dr);
					}
				}
				#endregion

				#endregion
			}
			else
				using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(GroupID))
				{
					while (reader.Read())
					{
						int iUserId = (int)reader["UserId"];
						if (alList.Contains(iUserId.ToString()))
							continue;
						bool IsExternal = (bool)reader["IsExternal"];
						byte Activity = (byte)reader["Activity"];
						if (!(IsExternal || Activity == (byte)Mediachase.IBN.Business.User.UserActivity.Pending))
						{
							dr = dt.NewRow();
							dr[0] = iUserId.ToString();
							dr[1] = (string)reader["LastName"] + " " + (string)reader["FirstName"];
							dt.Rows.Add(dr);
						}
					}
				}
			DataView dv = new DataView(dt);

			ddUsers.DataSource = dv;
			ddUsers.DataTextField = "UserName";
			ddUsers.DataValueField = "UserId";
			ddUsers.DataBind();
			btnAdd.Disabled = false;
			if (GroupID > 0)
				ddUsers.Items.Insert(0, new ListItem(LocRM.GetString("tAny"), "0"));
			else if (ddUsers.Items.Count == 0)
			{
				ddUsers.Items.Insert(0, new ListItem(LocRM.GetString("tEmpty"), "0"));
				btnAdd.Disabled = true;
			}
		}
		#endregion

		#region BindRights
		private void BindRights()
		{
			ddRights.Items.Clear();
			ddRights.Items.Add(new ListItem(LocRM.GetString("tRead"), "1"));
			ddRights.Items.Add(new ListItem(LocRM.GetString("tWrite"), "2"));
			ddRights.Items.Add(new ListItem(LocRM.GetString("tAdmin"), "3"));

			rbList.Items.Add(new ListItem(LocRM.GetString("tAllow"), "0"));
			rbList.Items.Add(new ListItem(LocRM.GetString("tDeny"), "1"));
			rbList.Items[0].Selected = true;
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			DataTable dt = ACLToDateTable(ACLr);
			DataView dv = dt.DefaultView;
			dv.Sort = "IsInherited DESC, Weight";
			dgMembers.DataSource = dv;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "if(confirm('" + LocRM.GetString("Warning") + "')){DisableButtons(this);DisableCheck();return true;}else return false;");
				ib.ToolTip = LocRM.GetString("Delete");
			}
		}
		#endregion

		#region ACLToDateTable
		private DataTable ACLToDateTable(AccessControlList ACLr)
		{
			DataTable dt = new DataTable();
			DataRow dr;

			dt.Columns.Add(new DataColumn("ID", typeof(int)));
			dt.Columns.Add(new DataColumn("Weight", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Rights", typeof(string)));
			dt.Columns.Add(new DataColumn("Allow", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsInherited", typeof(bool)));
			dt.Columns.Add(new DataColumn("HasOwnerKey", typeof(bool)));
			foreach (AccessControlEntry ACEr in ACLr)
			{
				if (ACEr.IsInternal)
					continue;
				dr = dt.NewRow();
				dr["ID"] = ACEr.Id;
				dr["Weight"] = (ACEr.IsRoleAce) ? 0 : ((Mediachase.IBN.Business.User.IsGroup(ACEr.PrincipalId)) ? 1 : 2);
				dr["Name"] = (ACEr.IsRoleAce) ? ACEr.Role : ACEr.PrincipalId.ToString();
				dr["Allow"] = ACEr.Allow;
				switch (ACEr.Action)
				{
					case "Admin":
						dr["Rights"] = LocRM.GetString("tAdmin");
						break;
					case "Read":
						dr["Rights"] = LocRM.GetString("tRead");
						break;
					case "Write":
						dr["Rights"] = LocRM.GetString("tWrite");
						break;
					default:
						break;
				}
				dr["IsInherited"] = ACEr.IsIherited;
				if (ACEr.OwnerKey == Guid.Empty)
					dr["HasOwnerKey"] = false;
				else
					dr["HasOwnerKey"] = true;
				dt.Rows.Add(dr);
			}
			return dt;
		}
		#endregion

		#region DataGrid Protected Strings
		protected string GetLink(int Weight, string Name)
		{
			if (Weight == 0)
			{
				switch (Name)
				{
					#region General Roles
					case UserRoleHelper.AdminRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tAdmins"));
					case UserRoleHelper.PowerProjectManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tPPManager"));

					case UserRoleHelper.HelpDeskManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tHDM"));

					case UserRoleHelper.GlobalProjectManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tGlobalPManager"));

					case UserRoleHelper.GlobalExecutiveManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tGlobalEManager"));

					case UserRoleHelper.TimeManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/admins.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTM"));
					#endregion

					#region Project Roles
					case UserRoleHelper.ProjectManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/manager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tPManager"));
					case UserRoleHelper.ExecutiveManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/exmanager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tExMan"));
					case UserRoleHelper.ProjectTeamRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTeam"));
					case UserRoleHelper.ProjectSponsorRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/sponsors.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tSponsors"));
					case UserRoleHelper.ProjectStakeHolderRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/stakeholders.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tStake"));
					#endregion

					#region Issue Roles
					case UserRoleHelper.IssueCreatorRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/exmanager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tICreator"));
					case UserRoleHelper.IssueManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/manager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tIManager"));
					case UserRoleHelper.IssueResourceRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tIResource"));
					case UserRoleHelper.IssueTodoResource:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tITodoResource"));
					#endregion

					#region Task Roles
					case UserRoleHelper.TaskCreatorRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/exmanager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTkCreator"));
					case UserRoleHelper.TaskManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/manager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTkManager"));
					case UserRoleHelper.TaskResourceRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTkResource"));
					case UserRoleHelper.TaskTodoResource:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTkTodoResource"));
					#endregion

					#region Document Roles
					case UserRoleHelper.DocumentCreatorRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/exmanager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tDocCreator"));
					case UserRoleHelper.DocumentManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/manager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tDocManager"));
					case UserRoleHelper.DocumentResourceRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tDocResource"));
					case UserRoleHelper.DocumentTodoResource:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tDocTodoResource"));
					#endregion

					#region Event Roles
					case UserRoleHelper.EventCreatorRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/exmanager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tEvCreator"));
					case UserRoleHelper.EventManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/manager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tEvManager"));
					case UserRoleHelper.EventResourceRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tEvResources"));
					#endregion

					#region ToDo Roles
					case UserRoleHelper.TodoCreatorRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/exmanager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTdCreator"));
					case UserRoleHelper.TodoManagerRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/manager.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTdManager"));
					case UserRoleHelper.TodoResourceRoleName:
						return String.Format("<img alt='' src='../layouts/images/icons/newgroup.GIF' border=0 align='absmiddle'>&nbsp;{0}", LocRM.GetString("tTdResources"));
					#endregion

					default:
						return Name;
				}
			}
			else if (Weight == 1)
				return Util.CommonHelper.GetGroupLinkUL(int.Parse(Name));
			else
				return Util.CommonHelper.GetUserStatusUL(int.Parse(Name));
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
			this.ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			this.btnSave.Click += new EventHandler(btnSave_Click);
			this.dgMembers.DeleteCommand += new DataGridCommandEventHandler(dgMembers_DeleteCommand);
			this.btnRestoreInherited.Click += new EventHandler(btnRestoreInherited_Click);
			this.btnSetInherited.Click += new EventHandler(btnSetInherited_Click);
			this.btnClearInherited.Click += new EventHandler(btnClearInherited_Click);
			this.btnSaveLostChanges.Click += new EventHandler(btnSaveLostChanges_Click);
		}
		#endregion

		#region ddGroups - ItemChange
		private void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindUsers(int.Parse(li.Value));
		}
		#endregion

		#region AddElement
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			AccessControlEntry ACEr;
			string sRight = "";
			switch (ddRights.SelectedValue)
			{
				case "1":
					sRight = "Read";
					break;
				case "2":
					sRight = "Write";
					break;
				case "3":
					sRight = "Admin";
					break;
				default:
					break;
			}

			if (ddGroups.SelectedValue == "0")
				ACEr = new AccessControlEntry(ddUsers.SelectedValue, sRight, (rbList.SelectedValue == "0"));
			else
			{
				int iPrincipalId = (ddUsers.SelectedValue == "0") ? int.Parse(ddGroups.SelectedValue) : int.Parse(ddUsers.SelectedValue);
				ACEr = new AccessControlEntry(iPrincipalId, sRight, (rbList.SelectedValue == "0"));
			}
			ACLr.Add(ACEr);

			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion

		#region DeleteRight
		private void dgMembers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			int Id = int.Parse(e.Item.Cells[0].Text);

			ACLr.Remove(Id);
			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
		}
		#endregion

		#region Save Events
		private void btnSave_Click(object sender, EventArgs e)
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			try
			{
				AccessControlList.SetACL(fs, ACLr);

				CommandParameters cp = new CommandParameters("FL_Security");

				if (Request["PrimaryKeyId"] != null)
				{
					cp.CommandName = "FL_Storage_SecureItem";
					Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
				}
				else
				{
					if (Request["New"] != null)
						Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
					else
						CHelper.CloseItAndRefresh(Response);
				}
			}
			catch (AllUserAccessWillBeDeniedException)
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"openMenuLostrights();", true);
			}
			catch (AdminAccessWillBeDeniedException)
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
							"openMenuLostrights();", true);
			}
		}

		private void btnSaveLostChanges_Click(object sender, EventArgs e)
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			AccessControlList.SetACL(fs, ACLr, false);

			CommandParameters cp = new CommandParameters("FL_Security");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());

			CHelper.CloseItAndRefresh(Response);
		}
		#endregion

		#region InheritedEvents
		private void btnRestoreInherited_Click(object sender, EventArgs e)
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			ACLr.TurnOnIsInherited();
			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
			ViewState["isChecked"] = "1";
		}

		private void btnSetInherited_Click(object sender, EventArgs e)
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			ACLr.TurnOffIsInherited(true);
			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
			ViewState["isChecked"] = "0";
		}

		private void btnClearInherited_Click(object sender, EventArgs e)
		{
			AccessControlList ACLr = (AccessControlList)ViewState["ACL"];
			ACLr.TurnOffIsInherited(false);
			ViewState["ACL"] = ACLr;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers(int.Parse(_li.Value));
			BinddgMembers();
			ViewState["isChecked"] = "0";
		}
		#endregion
	}
}
