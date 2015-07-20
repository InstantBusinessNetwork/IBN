namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for TaskGeneralResources.
	/// </summary>
	public partial  class TaskGeneralResources : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskGeneral", typeof(TaskGeneralResources).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(TaskGeneralResources).Assembly);

		private DataTable dt;
		private int compltype = (int)CompletionType.Any;
		bool bind = false;

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

		#region TaskID
		protected int TaskID
		{
			get 
			{
				try
				{
					return int.Parse(Request["TaskID"]);
				}
				catch
				{
					throw new Exception("TaskID is Reqired");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			CheckVisibility();
			if (!IsPostBack)
				BinddgMembers();
		}

		#region CheckVisibility
		private void CheckVisibility()
		{
			///  TaskId, TaskNum, ProjectId, ProjectTitle, ManagerId, CreatorId, CompletedBy, Title, 
			///  Description,	CreationDate, StartDate, FinishDate, 	Duration, 
			///  ActualFinishDate, PriorityId, PriorityName, PercentCompleted, OutlineNumber, 
			///  OutlineLevel, 	IsSummary, IsMilestone, ConstraintTypeId, ConstraintTypeName, 
			///  ConstraintDate, CompletionTypeId, CompletionTypeName, IsCompleted, 
			///  MustBeConfirmed, ReasonId
			bool IsSummary = false;
			bool IsMileStone = false;
			bool IsExternal = Security.CurrentUser.IsExternal;
			using (IDataReader reader = Task.GetTask(TaskID))
			{
				if (reader.Read())
				{
					IsSummary = (bool)reader["IsSummary"];
					IsMileStone = (bool)reader["IsMileStone"];
					compltype = (int)reader["CompletionTypeId"];
				}
			}

			if (IsSummary || IsMileStone || IsExternal)
				this.Visible = false;
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("UserName");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("CanManage");
			dgMembers.Columns[3].HeaderText = LocRM.GetString("OverallStatus");

			dgMembers.Columns[4].Visible = (compltype == (int)CompletionType.All && Task.CanUpdate(TaskID));

			dt = Task.GetListResourcesDataTable(TaskID);
			dgMembers.DataSource = dt;
			dgMembers.DataBind();

			users = String.Empty;
			foreach (DataRow row in dt.Rows)
			{
				if (!String.IsNullOrEmpty(users))
					users += ",";
				users += row["UserId"].ToString();
			}

			bind = true;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("TaskResources"));
			if (Task.CanModifyResources(TaskID))
			{
				// resource utilization
				if (dgMembers.Items.Count > 0)
				{
					string text = String.Format(CultureInfo.InvariantCulture,
						"<img alt='' src='{0}'/> {1}",
						Page.ResolveUrl("~/Layouts/Images/ResUtil.png"),
						LocRM2.GetString("Utilization"));
					string link = String.Format(CultureInfo.InvariantCulture,
						"javascript:OpenPopUpNoScrollWindow('{0}?users={1}&amp;ObjectId={2}&amp;ObjectTypeId={3}',750,300)",
						Page.ResolveUrl("~/Common/ResourceUtilGraphForObject.aspx"),
						users,
						TaskID,
						(int)ObjectTypes.Task);
					secHeader.AddRightLink(text, link);
				}

				// Modify
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_TaskResEdit");
				string cmd = cm.AddCommand("Task", "", "TaskView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/editgroup.gif'/> " + LocRM.GetString("Modify"), "javascript:" + cmd);
			}
		}
		#endregion

		#region GetLink
		protected string GetLink(int PID,bool IsGroup)
		{
			if (IsGroup)
				return CommonHelper.GetGroupLink(PID);
			else
				return CommonHelper.GetUserStatus(PID);
		}
		#endregion

		#region GetStatus
		protected string GetStatus(object _mbc, object _rp,object _ic,object _pc)
		{
			bool showpercent = false;
			if (compltype == (int)Mediachase.IBN.Business.CompletionType.All)
				showpercent = true;

			bool mbc = (bool)_mbc;

			bool rp = false;
			if (_rp!=DBNull.Value)
				rp = (bool)_rp;

			bool ic = false;
			if (_ic!=DBNull.Value)
				ic = (bool)_ic;

			int pc = 0;
			if (_pc!=DBNull.Value)
				pc = (int)_pc;

			if (!mbc)
			{
				if (showpercent) 
					return pc.ToString()+" %";
				else return "";
			}
			else
				if (rp) 
				return 
					LocRM.GetString("Waiting");
			else
				if (ic) 
				if (showpercent) 
					return pc.ToString()+" %"; 
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

		#region GetManageType
		protected string GetManageType(bool CanManage)
		{
			if(CanManage)
				return "<img src='../Layouts/Images/accept.gif' border='0' width='16' height='16' align='absmiddle'>";
			else
				return "";
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

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
				Task.UpdateResourcePercent(TaskID, userId, percent);
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
