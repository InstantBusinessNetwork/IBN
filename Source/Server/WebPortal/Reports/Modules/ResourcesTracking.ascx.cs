namespace Mediachase.UI.Web.Reports.Modules
{
	using System;
	using System.Data;
	using System.Text;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ResourcesTracking.
	/// </summary>
	public partial  class ResourcesTracking : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strResourcesUtilization", typeof(ResourcesTracking).Assembly);
		

		protected System.Web.UI.HtmlControls.HtmlInputHidden hdnUserID;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divCurrentGrid;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divComplGrid;


		protected Mediachase.UI.Web.Modules.Separator1 Sep2;

		protected System.Web.UI.WebControls.DataGrid grdCurPrjActs;
		protected System.Web.UI.WebControls.DataGrid grdComplActs;

		private int ProjectID=0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();

			if(!Page.IsPostBack)
			{
				BindList();
				ProjectID = int.Parse(ddProject.SelectedItem.Value);
				BindData();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			Printheader1.BtnPrintVisible=false;
			Printheader1.Title = LocRM.GetString("tResTracking");
			Sep0.Title = LocRM.GetString("tProjInf");
			Sep1.Title = LocRM.GetString("tQuickCounts");
			lblProjTitle.Text = LocRM.GetString("tPrjTitle")+": ";
			btnApply.Text = LocRM.GetString("tApply");
		}
		#endregion

		#region BindList
		private void BindList()
		{
			using (IDataReader reader = Project.GetListProjects())
			{
				while(reader.Read())
					ddProject.Items.Add(new ListItem(reader["Title"].ToString(),reader["ProjectId"].ToString()));
			}

			try
			{
				ProjectID = int.Parse(Request["ProjectId"].ToString());
			}
			catch
			{
			}
			ListItem liItem = ddProject.Items.FindByValue(ProjectID.ToString());
			if(liItem!=null)
				liItem.Selected = true;			
		}
		#endregion

		#region BindData
		private void BindData()
		{
			using (IDataReader reader = Report.GetProjectStatisticReport(ProjectID))
			{
				if(reader.Read())
				{
					lblTotalTasks.Text = reader["Tasks"].ToString();
					lblTotalCalEntries.Text = reader["Events"].ToString();
					lblTotalIssues.Text = reader["Incidents"].ToString();
					lblTotalDiscussions.Text = reader["Discussions"].ToString();
					lblTotalToDos.Text = reader["ToDo"].ToString();
//					lblTotalFiles.Text = reader["Files"].ToString();
					lblTotalResources.Text = reader["Resources"].ToString();
				}
			}
			
			using (IDataReader reader = Project.GetProject(ProjectID))
			{
				if(reader.Read())
				{
					Printheader1.Filter = LocRM.GetString("tPrjTitle") +" : "+ reader["Title"].ToString();
					lblStartDate.Text = ((DateTime)reader["StartDate"]).ToShortDateString();
					lblTargetEndDate.Text = ((DateTime)reader["TargetFinishDate"]).ToShortDateString();
					if(reader["ActualFinishDate"]!=DBNull.Value)
						lblActualEndDate.Text =  ((DateTime)reader["ActualFinishDate"]).ToShortDateString();
					else
						lblActualEndDate.Text = LocRM.GetString("tNA");
					lblProjectStatus.Text = reader["StatusName"].ToString();
					lblProjectType.Text = reader["TypeName"].ToString();
					lblCustomer.Text = reader["ClientName"].ToString();
				}
			}

			StringBuilder sbSponsors=new StringBuilder();
			using (IDataReader reader = Project.GetListSponsors(ProjectID))
			{
				while(reader.Read())
				{
					bool IsGroup = (bool)reader["IsGroup"];
					int iSponsorId = (int)reader["PrincipalId"];
					if(IsGroup)
					{
						using (IDataReader principal = SecureGroup.GetGroup(iSponsorId))
						{
							if(principal.Read())
								sbSponsors.Append(CommonHelper.GetResFileString(principal["GroupName"].ToString()) + "<br>");
						}
					}
					else
					{
						using (IDataReader principal = User.GetUserInfo(iSponsorId, false))
						{
							if(principal.Read())
								sbSponsors.Append(principal["LastName"] + " " + principal["FirstName"] + "<br>");
						}
					}
				}
			}

			if (sbSponsors.Length == 0) 
				sbSponsors.Append(LocRM.GetString("tEmpty"));

			StringBuilder sbStake=new StringBuilder();
			using (IDataReader reader = Project.GetListStakeholders(ProjectID))
			{
				while(reader.Read())
				{
					bool IsGroup = (bool)reader["IsGroup"];
					int iStakeId = (int)reader["PrincipalId"];
					if(IsGroup)
					{
						using (IDataReader principal = SecureGroup.GetGroup(iStakeId))
						{
							if(principal.Read())
								sbStake.Append(CommonHelper.GetResFileString(principal["GroupName"].ToString()) + "<br>");
						}
					}
					else
					{
						using (IDataReader principal = User.GetUserInfo(iStakeId, false))
						{
							if(principal.Read())
								sbStake.Append(principal["LastName"] + " " + principal["FirstName"] + "<br>");
						}
					}
				}
			}
			if (sbStake.Length == 0) sbStake.Append(LocRM.GetString("tEmpty"));

			lblSponsors.Text = sbSponsors.ToString();
			lblStakeholders.Text = sbStake.ToString();
			
			DataTable dt = new DataTable();
			dt.Columns.Add("ResourceID", typeof(int));
			dt.Columns.Add("ResourceInf", typeof(string));
			dt.Columns.Add("IssuesCreated", typeof(int));
			dt.Columns.Add("IssuesModified", typeof(int));
			dt.Columns.Add("IssuesClosed", typeof(int));
//			dt.Columns.Add("FilesPublished", typeof(int));
			dt.Columns.Add("CalendarEntries", typeof(int));
			dt.Columns.Add("DiscussionsAdded", typeof(int));

			DataRow row;
			using (IDataReader reader = Project.GetListTeamMembers(ProjectID))
			{
				while (reader.Read())
				{
					row = dt.NewRow();
					int iResId = (int)reader["UserId"];
					decimal iRate = (decimal)reader["Rate"];
					row["ResourceID"] = iResId;
					using (IDataReader Member = Report.GetProjectStatisticByUserReport(ProjectID, iResId))
					{
						if(Member.Read())
						{
							row["ResourceInf"] = "<b>"+CommonHelper.GetUserStatusPureName(iResId)+"</b>,&nbsp;"+
								LocRM.GetString("tRate")+":&nbsp;<b>"+iRate.ToString("f")+"</b>";
							row["IssuesCreated"] = (int)Member["IncidentsCreated"];
							row["IssuesModified"] = (int)Member["IncidentsModified"];
							row["IssuesClosed"] = (int)Member["IncidentsClosed"];
//							row["FilesPublished"] = (int)Member["Files"];
							row["CalendarEntries"] = (int)Member["Events"];
							row["DiscussionsAdded"] = (int)Member["Discussions"];
						}
					}
					dt.Rows.Add(row);
				}
			}

			DataView dv = dt.DefaultView;			
			ResourcesRep.DataSource=dv;
			ResourcesRep.DataBind();
			
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
			this.ResourcesRep.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.GridBound);
		}
		#endregion

		#region ItemGridBound
		private void GridBound(object sender, RepeaterItemEventArgs e)
		{
			HtmlInputHidden InputId = (HtmlInputHidden)e.Item.FindControl("hdnUserID");
			if(InputId==null)
				return;

			int id = int.Parse(InputId.Value);
			DataGrid GridCurrent = (DataGrid)e.Item.FindControl("grdCurPrjActs");
			if(GridCurrent==null)
				return;

			GridCurrent.Columns[0].HeaderText = LocRM.GetString("tPriority");
			GridCurrent.Columns[1].HeaderText = LocRM.GetString("tType");
			GridCurrent.Columns[2].HeaderText = LocRM.GetString("tTitle");
			GridCurrent.Columns[3].HeaderText = LocRM.GetString("tCreateDate");
			GridCurrent.Columns[4].HeaderText = LocRM.GetString("tDueDate");
			
			DataTable dt = ToDo.GetListActiveToDoAndTasks(ProjectID, id);
			if(dt.Rows.Count!=0)
			{
				DataView dv = dt.DefaultView;
				dv.Sort="PriorityId DESC";
				GridCurrent.DataSource = dv;
				GridCurrent.DataBind();
			}
			else
			{
				GridCurrent.Visible=false;
				HtmlGenericControl divCur = (HtmlGenericControl)e.Item.FindControl("divCurrentGrid");
				if(divCur!=null)
					divCur.Visible=false;
			}			
			
			DataGrid GridCompl = (DataGrid)e.Item.FindControl("grdComplActs");
			if(GridCompl==null)
				return;

			GridCompl.Columns[0].HeaderText = LocRM.GetString("tPriority");
			GridCompl.Columns[1].HeaderText = LocRM.GetString("tType");
			GridCompl.Columns[2].HeaderText = LocRM.GetString("tTitle");
			GridCompl.Columns[3].HeaderText = LocRM.GetString("tCreateDate");
			GridCompl.Columns[4].HeaderText = LocRM.GetString("tComplDate");
			
			dt = ToDo.GetListCompletedToDoAndTasks(ProjectID, id);
			if(dt.Rows.Count!=0)
			{
				DataView dv = dt.DefaultView;
				dv.Sort = "PriorityId DESC";
				GridCompl.DataSource = dv;
				GridCompl.DataBind();
			}
			else
			{
				GridCompl.Visible=false;
				HtmlGenericControl divCompl = (HtmlGenericControl)e.Item.FindControl("divComplGrid");
				if(divCompl!=null)
					divCompl.Visible=false;
			}
		}
		#endregion
		
		protected string GetType(int IsToDo)
		{
			if(IsToDo==1)
				return LocRM.GetString("tToDo");
			else
				return LocRM.GetString("tTask");
		}

		protected void btnAplly_Click(object sender, System.EventArgs e)
		{
			ProjectID = int.Parse(ddProject.SelectedItem.Value);
			BindData();
		}

//===========================================================================
// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable Printheader1.
//===========================================================================
    public Mediachase.UI.Web.Modules.ReportHeader Printheader1 {
        get { return Migrated_Printheader1; }
        //set { Migrated_Printheader1 = value; }
    }
	}
}
