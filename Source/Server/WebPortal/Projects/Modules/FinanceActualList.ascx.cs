namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for FinanceActualList.
	/// </summary>
	public partial class FinanceActualList : System.Web.UI.UserControl
	{
		protected Mediachase.UI.Web.Modules.BlockHeader secHeader;

		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(FinanceActualList).Assembly);

		#region IDs
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int TaskId
		{
			get
			{
				try
				{
					return int.Parse(Request["TaskId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int IncidentId
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int DocumentId
		{
			get
			{
				try
				{
					return int.Parse(Request["DocumentId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int EventId
		{
			get
			{
				try
				{
					return int.Parse(Request["EventId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int ToDoId
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int ParentProjectId = -1;
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			DefineParentProjectId();
			if (ToDoId > 0 || EventId > 0)
				dgAccounts.Columns[7].Visible = false;
			ApplyLocalization();
			if (!Page.IsPostBack)
				BindDG();
			BindToolbar();
		}

		#region DefineParentProjectId
		private void DefineParentProjectId()
		{
			if (ProjectId > 0)
				ParentProjectId = ProjectId;
			else if (TaskId > 0)
				ParentProjectId = Task.GetProject(TaskId);
			else if (IncidentId > 0)
				using (IDataReader reader = Incident.GetIncident(IncidentId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (DocumentId > 0)
				using (IDataReader reader = Document.GetDocument(DocumentId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (EventId > 0)
				using (IDataReader reader = CalendarEntry.GetEvent(EventId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (ToDoId > 0)
				using (IDataReader reader = ToDo.GetToDo(ToDoId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			dgAccounts.Columns[2].HeaderText = LocRM.GetString("Description");
			dgAccounts.Columns[3].HeaderText = LocRM.GetString("tAccount");
			dgAccounts.Columns[4].HeaderText = LocRM.GetString("tActualDate");
			dgAccounts.Columns[5].HeaderText = LocRM.GetString("Actual");
			dgAccounts.Columns[6].HeaderText = LocRM.GetString("tModifiedBy");
			dgAccounts.Columns[7].HeaderText = LocRM.GetString("tObjTitle");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (this.Parent.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeaderLight = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
				if (ProjectId > 0 && Project.CanEditFinances(ProjectId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>&nbsp;" + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../projects/AddActualItem.aspx?ProjectId=" + ProjectId.ToString() + "',520,270,false);");
				if (TaskId > 0 && Task.CanViewFinances(TaskId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?TaskID=" + TaskId + "',520,270,false);");
				if (IncidentId > 0 && Incident.CanViewFinances(IncidentId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?IncidentId=" + IncidentId + "',520,270,false);");
				if (DocumentId > 0 && Document.CanViewFinances(DocumentId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?DocumentId=" + DocumentId + "',520,270,false);");
				if (EventId > 0 && CalendarEntry.CanViewFinances(EventId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?EventId=" + EventId + "',520,270,false);");
				if (ToDoId > 0 && Mediachase.IBN.Business.ToDo.CanViewFinances(ToDoId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?ToDoId=" + ToDoId + "',520,270,false);");
			}
			else
				if (this.Parent is IToolbarLight)
				{
					BlockHeaderLightWithMenu secHeaderLight = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent).GetToolBar();
					secHeaderLight.AddText(LocRM.GetString("tActFinances"));


					if (ProjectId > 0 && Project.CanEditFinances(ProjectId))
						secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>&nbsp;" + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../projects/AddActualItem.aspx?ProjectId=" + ProjectId.ToString() + "',520,270,false);");
					if (TaskId > 0 && Task.CanViewFinances(TaskId))
						secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?TaskID=" + TaskId + "',520,270,false);");
					if (IncidentId > 0 && Incident.CanViewFinances(IncidentId))
						secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?IncidentId=" + IncidentId + "',520,270,false);");
					if (DocumentId > 0 && Document.CanViewFinances(DocumentId))
						secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?DocumentId=" + DocumentId + "',520,270,false);");
					if (EventId > 0 && CalendarEntry.CanViewFinances(EventId))
						secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?EventId=" + EventId + "',520,270,false);");
					if (ToDoId > 0 && Mediachase.IBN.Business.ToDo.CanViewFinances(ToDoId))
						secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddActualItem.aspx?ToDoId=" + ToDoId + "',520,270,false);");
				}
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			DataTable dt = new DataTable();
			if (ProjectId > 0)
				dt = Finance.GetListActualFinancesByProject(ProjectId);
			else if (TaskId > 0)
				dt = Finance.GetListActualFinancesByTask(TaskId);
			else if (IncidentId > 0)
				dt = Finance.GetListActualFinancesByIncident(IncidentId);
			else if (DocumentId > 0)
				dt = Finance.GetListActualFinancesByDocument(DocumentId);
			else if (EventId > 0)
				dt = Finance.GetListActualFinancesByEvent(EventId);
			else if (ToDoId > 0)
				dt = Finance.GetListActualFinancesByToDo(ToDoId);

			DataView dv = dt.DefaultView;
			if (pc["FinAct_PageSize"] != null)
				dgAccounts.PageSize = int.Parse(pc["FinAct_PageSize"]);

			if (pc["FinAct_Page"] != null)
				dgAccounts.CurrentPageIndex = int.Parse(pc["FinAct_Page"]);

			int pageindex = dgAccounts.CurrentPageIndex;
			int ppi = dv.Count / dgAccounts.PageSize;
			if (dv.Count % dgAccounts.PageSize == 0)
				ppi = ppi - 1;

			if (pageindex <= ppi)
				dgAccounts.CurrentPageIndex = pageindex;
			else dgAccounts.CurrentPageIndex = 0;

			dgAccounts.DataSource = dv;
			dgAccounts.DataBind();

			foreach (DataGridItem dgi in dgAccounts.Items)
			{
				if (dgi.FindControl("ibDelete") != null)
				{
					ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
					ibDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
				}
			}
			//			if(!Project.CanEditFinances(ProjectId))
			//				dgAccounts.Columns[8].Visible = false;
		}
		#endregion

		#region GetObjectLink
		protected string GetObjectLink(int ObjectTypeId, int ObjectId, string ObjectTitle)
		{
			switch (ObjectTypeId)
			{
				case 4:	//calendar entry
					return "<a href='../Events/EventView.aspx?EventId=" + ObjectId.ToString() + "'>" + ObjectTitle + "</a>";
				case 5:	//task
					return "<a href='../Tasks/TaskView.aspx?TaskId=" + ObjectId.ToString() + "'>" + ObjectTitle + "</a>";
				case 6:	//todo
				case 11:	//timesheet todo
					return "<a href='../ToDo/ToDoView.aspx?ToDoId=" + ObjectId.ToString() + "'>" + ObjectTitle + "</a>";
				case 7:	//incident
					return "<a href='../Incidents/IncidentView.aspx?IncidentId=" + ObjectId.ToString() + "'>" + ObjectTitle + "</a>";
				case 16:	//document
					return "<a href='../Documents/DocumentView.aspx?DocumentId=" + ObjectId.ToString() + "'>" + ObjectTitle + "</a>";
				default:
					return ObjectTitle;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgAccounts.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageChange);
			this.dgAccounts.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChange);
			this.dgAccounts.DeleteCommand += new DataGridCommandEventHandler(dgAccounts_DeleteCommand);
			this.dgAccounts.EditCommand += new DataGridCommandEventHandler(dgAccounts_EditCommand);
			this.dgAccounts.UpdateCommand += new DataGridCommandEventHandler(dgAccounts_UpdateCommand);
			this.dgAccounts.CancelCommand += new DataGridCommandEventHandler(dgAccounts_CancelCommand);
		}
		#endregion

		#region DataGrid_Events
		private void dg_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["FinAct_Page"] = e.NewPageIndex.ToString();
			BindDG();
		}

		private void dg_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["FinAct_PageSize"] = e.NewPageSize.ToString();
			BindDG();
		}

		private void dgAccounts_EditCommand(object source, DataGridCommandEventArgs e)
		{
			dgAccounts.EditItemIndex = e.Item.ItemIndex;
			dgAccounts.DataKeyField = "ActualId";
			BindDG();
		}

		private void dgAccounts_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int ItemID = (int)dgAccounts.DataKeys[e.Item.ItemIndex];
			TextBox tbDescr = (TextBox)e.Item.FindControl("tbDescr");
			TextBox tbValue = (TextBox)e.Item.FindControl("tbValue");
			if (tbDescr != null && tbValue != null)
			{
				if (tbValue.Text == "")
					tbValue.Text = "0";
				Finance.UpdateActualFinancesValueAndDescription(ItemID, decimal.Parse(tbValue.Text), tbDescr.Text);
			}
			dgAccounts.EditItemIndex = -1;
			BindDG();
		}

		private void dgAccounts_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			dgAccounts.EditItemIndex = -1;
			BindDG();
		}

		private void dgAccounts_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ActualId = int.Parse(e.CommandArgument.ToString());
			Finance.DeleteActualFinances(ActualId);
			BindDG();
		}
		#endregion
	}
}
