namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI.WebControls;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ToDoGeneralResources.
	/// </summary>
	public partial class ToDoGeneralResources : System.Web.UI.UserControl
	{
		protected Mediachase.UI.Web.Modules.BlockHeader secHeaderExternal;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", typeof(ToDoGeneralResources).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ToDoGeneralResources).Assembly);

		private DataTable dt;
		private int compltype = (int)CompletionType.Any;
		bool bind = false;
		int incidentId = -1;
		int taskId = -1;
		int documentId = -1;


		#region users
		private string users
		{
			set
			{
				ViewState["users"] = value;
			}
			get
			{
				string retval = string.Empty;
				if (ViewState["users"] != null)
					retval = (string)ViewState["users"];
				return retval;
			}
		}
		#endregion

		#region ToDoID
		protected int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoID"]);
				}
				catch
				{
					throw new Exception("ToDoID is Reqired");
				}
			}
		} 
		#endregion

		#region HideIfEmpty
		private bool hideifempty = false;
		public bool HideIfEmpty
		{
			set
			{
				hideifempty = value;
			}
			get
			{
				return hideifempty;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.Visible = true;

			using (IDataReader rdr = Mediachase.IBN.Business.ToDo.GetToDo(ToDoID))
			{
				if (rdr.Read())
				{
					compltype = (int)rdr["CompletionTypeId"];
					if (rdr["IncidentId"] != DBNull.Value)
						incidentId = (int)rdr["IncidentId"];
					if (rdr["TaskId"] != DBNull.Value)
						taskId = (int)rdr["TaskId"];
					if (rdr["DocumentId"] != DBNull.Value)
						documentId = (int)rdr["DocumentId"];
				}
			}

			if (!IsPostBack)
				BinddgMembers();
		}

		#region BinddgMembers
		private void BinddgMembers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("UserName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("OverallStatus");

			dgMembers.Columns[3].Visible = (compltype == (int)CompletionType.All && ToDo.CanUpdate(ToDoID));

			dt = Mediachase.IBN.Business.ToDo.GetListResourcesDataTable(ToDoID);
			dgMembers.DataSource = dt;
			dgMembers.DataBind();

			ArrayList userList = new ArrayList();
			foreach (DataRow row in dt.Rows)
			{
				int principalId = (int)row["UserId"];
				if (Mediachase.IBN.Business.User.IsGroup(principalId))
				{
					using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(principalId, false))
					{
						while (reader.Read())
						{
							if (!userList.Contains((int)reader["UserId"]))
							{
								userList.Add((int)reader["UserId"]);
							}
						}
					}
				}
				else
				{
					if (!userList.Contains(principalId))
					{
						userList.Add(principalId);
					}
				}
			}

			users = String.Empty;
			foreach (int userId in userList)
			{
				if (!String.IsNullOrEmpty(users))
					users += ",";

				users += userId.ToString();
			}

			bind = true;
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("ToDoResources"));

			if ((incidentId > 0 && ToDo.CanUpdateIncidentTodo(ToDoID, incidentId))
				|| (documentId > 0 && ToDo.CanUpdateDocumentTodo(ToDoID, documentId))
				|| (taskId > 0 && ToDo.CanUpdate(ToDoID, taskId))
				|| ToDo.CanUpdate(ToDoID))
			{
				#region Resource Utilization
				if (dgMembers.Items.Count > 0 && !Security.CurrentUser.IsExternal)
				{

					string text = String.Format(CultureInfo.InvariantCulture,
						"<img alt='' src='{0}'/> {1}",
						Page.ResolveUrl("~/Layouts/Images/ResUtil.png"),
						LocRM2.GetString("Utilization"));
					string link = String.Format(CultureInfo.InvariantCulture,
						"javascript:OpenPopUpNoScrollWindow('{0}?users={1}&amp;ObjectId={2}&amp;ObjectTypeId={3}',750,300)",
						Page.ResolveUrl("~/Common/ResourceUtilGraphForObject.aspx"),
						users,
						ToDoID,
						(int)ObjectTypes.ToDo);
					secHeader.AddRightLink(text, link);
				}
				#endregion

				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_ToDoResEdit");
				string cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
				cmd = cmd.Replace("\"", "&quot;");

				if (secHeaderExternal != null && dgMembers.Items.Count == 0)
				{
					secHeaderExternal.AddLinkAt(0, "<img alt='' src='../Layouts/Images/icons/newgroup.gif' title='" + LocRM.GetString("Add") + "'/>", "javascript:" + cmd);
					secHeaderExternal.AddSeparatorAt(1);
				}
				else
				{
					secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"), "javascript:" + cmd);
				}
			}

			if (hideifempty && dgMembers.Items.Count == 0)
				this.Visible = false;
			else 
				this.Visible = true;
		} 
		#endregion

		#region GetLink
		protected string GetLink(int PID, bool IsGroup)
		{
			if (IsGroup)
				return CommonHelper.GetGroupLink(PID);
			else
				return CommonHelper.GetUserStatus(PID);
		} 
		#endregion

		#region GetStatus
		protected string GetStatus(object _mbc, object _rp, object _ic, object _pc)
		{
			bool showpercent = false;
			if (compltype == (int)CompletionType.All)
				showpercent = true;

			bool mbc = (bool)_mbc;

			bool rp = false;
			if (_rp != DBNull.Value)
				rp = (bool)_rp;

			bool ic = false;
			if (_ic != DBNull.Value)
				ic = (bool)_ic;

			int pc = 0;
			if (_pc != DBNull.Value)
				pc = (int)_pc;

			if (!mbc)
			{
				if (showpercent)
					return pc.ToString() + " %";
				else return "";
			}
			else
				if (rp)
					return
						LocRM.GetString("Waiting");
				else
					if (ic)
						if (showpercent)
							return pc.ToString() + " %";
						else
							return LocRM.GetString("Accepted");

					else return LocRM.GetString("Denied");
		} 
		#endregion

		#region CheckEdit
		protected bool CheckEdit(object mustBeConfirmed, object responsePending, object isConfirmed)
		{
			bool retval = false;
			if (mustBeConfirmed != DBNull.Value && responsePending != DBNull.Value && isConfirmed != DBNull.Value)
			{
				retval = true;
				if ((bool)mustBeConfirmed)
					retval = !(bool)responsePending && (bool)isConfirmed;
			}
			retval = retval && (compltype == (int)CompletionType.All);
			return retval;
		}
		#endregion

		protected void btnRefresh_Click(object sender, System.EventArgs e)
		{
		}

		#region dgMembers_ItemDataBound
		protected void dgMembers_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (dgMembers.EditItemIndex >= 0 && dgMembers.EditItemIndex == e.Item.ItemIndex)
			{
				DropDownList ddl = (DropDownList)e.Item.FindControl("PercentList");
				if (ddl != null)
				{
					int currentPercent = (int)((System.Data.DataRowView)(e.Item.DataItem)).Row["PercentCompleted"];
					for (int i = 0; i <= 100; i++)
					{
						ListItem li = new ListItem(String.Concat(i, " %"), i.ToString());
						if (currentPercent == i)
							li.Selected = true;
						ddl.Items.Add(li);
					}
				}
			}
		}
		#endregion

		#region dgMembers_EditCommand
		protected void dgMembers_EditCommand(object source, DataGridCommandEventArgs e)
		{
			dgMembers.EditItemIndex = e.Item.ItemIndex;
			BinddgMembers();
		}
		#endregion

		#region dgMembers_CancelCommand
		protected void dgMembers_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			dgMembers.EditItemIndex = -1;
			BinddgMembers();
		}
		#endregion

		#region dgMembers_UpdateCommand
		protected void dgMembers_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int userId = int.Parse(e.CommandArgument.ToString());
			DropDownList ddl = (DropDownList)e.Item.FindControl("PercentList");
			if (ddl != null)
			{
				int percent = int.Parse(ddl.SelectedValue);
				ToDo.UpdateResourcePercent(ToDoID, userId, percent);
			}

			dgMembers.EditItemIndex = -1;
			BinddgMembers();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (!bind)
				BinddgMembers();

			BindToolbar();
		}
		#endregion
	}
}
