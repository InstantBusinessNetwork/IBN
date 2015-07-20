namespace Mediachase.UI.Web.Reports.Modules
{
	using System;
	using System.Data;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Text;

	/// <summary>
	///		Summary description for OverallProjectSnapshot.
	/// </summary>
	public partial class OverallProjectSnapshot : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strProjectSnapshot", typeof(OverallProjectSnapshot).Assembly);
		String cSymbol = String.Empty;
		
		private int ProjectID = 0;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			filter.Visible = false;
			if (!IsPostBack)
			{
				BindList();
			}
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindData();
			DataBind();
		}

		private void BindList()
		{
			using (IDataReader reader = Project.GetListProjects())
			{
				while (reader.Read())
				{
					ddProject.Items.Add(new ListItem(reader["Title"].ToString(), reader["ProjectId"].ToString()));
				}
			}

			string sProjectId = Request["ProjectId"];
			ProjectID = 0;
			if (sProjectId != null)
			{
				try
				{
					ProjectID = int.Parse(sProjectId);
				}
				catch { }
			}

			ListItem liItem = ddProject.Items.FindByValue(ProjectID.ToString());
			if (liItem != null)
				liItem.Selected = true;

			if (ddProject.SelectedItem != null)
				ProjectID = int.Parse(ddProject.SelectedItem.Value);

			btnApply.Text = LocRM.GetString("tApply");
			lblProjTitle.Text = LocRM.GetString("tProject") + ": ";
		}

		#region BindData
		private void BindData()
		{
			BindTopBlock();
			BindIssuesStatistics();
			BindProjectsStatistics();
			BindPastDueActivities();
			BindOpenIssues();
			BindFinStatistics();
		}

		private void BindIssuesStatistics()
		{
			/*Gistogram gIssues = new Gistogram(Server.MapPath(CommonHelper.ChartPath),System.Drawing.Imaging.ImageFormat.Png);
			gIssues.Title	= LocRM.GetString("oIssuesGraphTitle");
			gIssues.Width	= 320;
			gIssues.Height	= 100;
			gIssues.Sort	= false;*/

			int inew = 0;
			int itotal = 0;
			int iactive = 0;
			int iclosed = 0;
			int iopen = 0;
			int ietc = 0;

			using (IDataReader rdr = Incident.GetIncidentStatistic(ProjectID))
			{
				if (rdr.Read())
				{
					inew = (int)rdr["NewIncidentCount"];
					iactive = (int)rdr["ActiveIncidentCount"];
					itotal = (int)rdr["IncidentCount"];
					iopen = inew + iactive;
					ietc = itotal - iopen;
					iclosed = ietc;
				}
			}

			/*if(itotal>0) 
			{
				string[] titles = new string[] {LocRM.GetString("oNew"),LocRM.GetString("oOpen"),LocRM.GetString("oActive"),LocRM.GetString("oEtc")};
				double[] values = new double[] {Convert.ToDouble(inew),Convert.ToDouble(iopen),Convert.ToDouble(iactive),Convert.ToDouble(ietc)};
				gIssues.CollectDataPoints(titles, values);
			}
			else
				gIssues.CollectDataPoints(new string[0],new double[0]);*/

			imgIssuesGraph.ImageUrl = "~/Modules/ChartImage.aspx?Type=Gist&OverPrjSnap_Issues=1&ProjectId=" + ProjectID;//CommonHelper.ChartPath + gIssues.Draw();

			lblIssuesActive.Text = iactive.ToString();
			lblIssuesClosed.Text = iclosed.ToString();
			lblIssuesOpen.Text = iopen.ToString();
			lblIssuesTotal.Text = itotal.ToString();
			if (itotal > 0)
				lblIssuesPClosed.Text = ((int)(iclosed * 100 / itotal)).ToString() + "%";
		}

		private void BindTopBlock()
		{
			using (IDataReader reader = Project.GetProject(ProjectID))
			{
				if (reader.Read())
				{
					Printheader1.Filter = "&nbsp;&nbsp;&nbsp;&nbsp;" + LocRM.GetString("tProject") + " : " + reader["Title"].ToString();
					lblStartDate.Text = ((DateTime)reader["TargetStartDate"]).ToShortDateString();
					lblTargetEndDate.Text = ((DateTime)reader["TargetFinishDate"]).ToShortDateString();
					if (reader["ActualFinishDate"] != DBNull.Value)
						lblActualEndDate.Text = ((DateTime)reader["ActualFinishDate"]).ToShortDateString();
					else
						lblActualEndDate.Text = LocRM.GetString("tNA");
					lblProjectStatus.Text = reader["StatusName"].ToString();
					lblProjectType.Text = reader["TypeName"].ToString();
					lblCustomer.Text = reader["ClientName"].ToString();
				}
			}

			StringBuilder sbSponsors = new StringBuilder();
			using (IDataReader reader = Project.GetListSponsors(ProjectID))
			{
				while (reader.Read())
				{
					bool IsGroup = (bool)reader["IsGroup"];
					int iSponsorId = (int)reader["PrincipalId"];
					if (IsGroup)
					{
						using (IDataReader principal = SecureGroup.GetGroup(iSponsorId))
						{
							if (principal.Read())
								sbSponsors.Append(CommonHelper.GetResFileString(principal["GroupName"].ToString()) + "<br>");
						}
					}
					else
					{
						using (IDataReader principal = User.GetUserInfo(iSponsorId, false))
						{
							if (principal.Read())
								sbSponsors.Append(principal["LastName"] + " " + principal["FirstName"] + "<br>");
						}
					}
				}
			}
			if (sbSponsors.Length == 0)
				sbSponsors.Append(LocRM.GetString("tEmpty"));

			StringBuilder sbStake = new StringBuilder();
			using (IDataReader reader = Project.GetListStakeholders(ProjectID))
			{

				while (reader.Read())
				{
					bool IsGroup = (bool)reader["IsGroup"];
					int iStakeId = (int)reader["PrincipalId"];
					if (IsGroup)
					{
						using (IDataReader principal = SecureGroup.GetGroup(iStakeId))
						{
							if (principal.Read())
								sbStake.Append(CommonHelper.GetResFileString(principal["GroupName"].ToString()) + "<br>");
						}
					}
					else
					{
						using (IDataReader principal = User.GetUserInfo(iStakeId, false))
						{
							if (principal.Read())
								sbStake.Append(principal["LastName"] + " " + principal["FirstName"] + "<br>");
						}
					}
				}
			}
			if (sbStake.Length == 0) sbStake.Append(LocRM.GetString("tEmpty"));

			lblSponsors.Text = sbSponsors.ToString();
			lblStakeholders.Text = sbStake.ToString();

			Printheader1.BtnPrintVisible = false;
			Printheader1.Title = LocRM.GetString("oProjectSnapshotTitle");
		}

		private void BindOpenIssues()
		{
			dgOpenIssues.Columns[0].HeaderText = LocRM.GetString("oTitle");
			dgOpenIssues.Columns[1].HeaderText = LocRM.GetString("oType");
			dgOpenIssues.Columns[2].HeaderText = LocRM.GetString("oManager");
			dgOpenIssues.DataSource = Incident.GetListOpenIncidentsByProject(ProjectID);
		}

		private void BindPastDueActivities()
		{
			dgPastDue.Columns[0].HeaderText = LocRM.GetString("oTitle");
			dgPastDue.Columns[1].HeaderText = LocRM.GetString("oDescription");
			dgPastDue.Columns[2].HeaderText = LocRM.GetString("oResources");
			dgPastDue.Columns[3].HeaderText = LocRM.GetString("oDueDate");
			dgPastDue.DataSource = ToDo.GetListToDoAndTaskPastDueByProject(ProjectID);
		}

		private void BindProjectsStatistics()
		{
			/*Gistogram gTaskTodos = new Gistogram(Server.MapPath(CommonHelper.ChartPath),System.Drawing.Imaging.ImageFormat.Png);
			gTaskTodos.Title	= LocRM.GetString("oTaskGraphTitle");
			gTaskTodos.Width	= 320;
			gTaskTodos.Height	= 100;
			gTaskTodos.Sort		= false;*/

			int itotal = 0;
			int icompleted = 0;
			int iactive = 0;
			int ipastdue = 0;

			using (IDataReader rdr = Report.GetToDoAndTaskTrackingReport(ProjectID))
			{
				if (rdr.Read())
				{
					itotal = (int)rdr["Total"];
					icompleted = (int)rdr["Completed"];
					iactive = (int)rdr["Active"];
					ipastdue = (int)rdr["PastDue"];
				}
			}

			using (IDataReader rdr = Report.GetProjectStatisticReport(ProjectID))
			{
				if (rdr.Read())
				{
					lblOtherDiscussions.Text = rdr["Discussions"].ToString();
					//					lblOtherFiles.Text		 = rdr["Files"].ToString();
				}
			}

			/*if(itotal>0) 
			{
				string[] titles = new string[] {LocRM.GetString("oTotal"),LocRM.GetString("oCompleted"),LocRM.GetString("oActive"),LocRM.GetString("oPastDue")};
				double[] values = new double[] {Convert.ToDouble(itotal),Convert.ToDouble(icompleted),Convert.ToDouble(iactive),Convert.ToDouble(ipastdue)};
				gTaskTodos.CollectDataPoints(titles, values);
			}
			else
				gTaskTodos.CollectDataPoints(new string[0],new double[0]);*/

			imgTaskTodosGraph.ImageUrl = "~/Modules/ChartImage.aspx?Type=Gist&OverPrjSnap_TaskToDo=1&ProjectId=" + ProjectID;//CommonHelper.ChartPath + gTaskTodos.Draw();

			lblTaskTotal.Text = itotal.ToString();
			lblTaskCompleted.Text = icompleted.ToString();
			lblTaskActive.Text = iactive.ToString();
			lblTaskPastdue.Text = ipastdue.ToString();
			if (itotal > 0)
				lblTaskPCompleted.Text = ((int)(icompleted * 100 / itotal)).ToString() + "%";

			cSymbol = Project.GetCurrencySymbol(ProjectID);
			dgResources.Columns[0].HeaderText = LocRM.GetString("oName");
			dgResources.Columns[1].HeaderText = LocRM.GetString("oCode");
			dgResources.Columns[2].HeaderText = LocRM.GetString("oRate");
			dgResources.Columns[3].HeaderText = LocRM.GetString("oTotalApproved");

			DataTable dt = Report.GetProjectTeamBreakdownReportDataTable(ProjectID);
			dgResources.DataSource = dt.DefaultView;

			/*using(IDataReader rdr = Report.GetProjectLaborCostsReport(ProjectID))
			{
				if (rdr.Read())
				{
					lblLCTotalHours.Text = rdr["Hours"].ToString();
					lblLCResourceCount.Text = rdr["Resources"].ToString();
					lblLCAvgRate.Text =  ShowValue(rdr["AvgRate"]);
					lblLCAvg.Text = ShowValue(rdr["AvgHour"]);
					lblLCActual.Text = ShowValue(rdr["Actual"]);
					//if (rdr["Actual"]!=DBNull.Value)
						//actual  = (int)rdr["Actual"];
				}
			}*/
		}

		public void BindFinStatistics()
		{
			/*dgItems.Columns[0].HeaderText = LocRM.GetString("oCategory");
			dgItems.Columns[1].HeaderText = LocRM.GetString("oDescription");
			dgItems.Columns[2].HeaderText = LocRM.GetString("oBudget");
			dgItems.Columns[3].HeaderText = LocRM.GetString("oEstimate");
			dgItems.Columns[4].HeaderText = LocRM.GetString("oActual");
			DataTable dt = Project.GetListFixedCostItemsDataTable(ProjectID);
			dgItems.DataSource = dt;*/

			/*dgLaborCI.Columns[0].HeaderText = LocRM.GetString("oCategory");
			dgLaborCI.Columns[1].HeaderText = LocRM.GetString("oDescription");
			dgLaborCI.Columns[2].HeaderText = LocRM.GetString("oBudget");
			dgLaborCI.Columns[3].HeaderText = LocRM.GetString("oEstimate");
			dgLaborCI.Columns[4].HeaderText = LocRM.GetString("oActual");
			dt = Project.GetLaborCostItemsDataTable(ProjectID);
			dgLaborCI.DataSource = dt;

			dgLaborCI.ShowFooter = true;
			dgLaborCI.FooterStyle.CssClass = "text";
			dgLaborCI.FooterStyle.BackColor = System.Drawing.Color.Gainsboro;
			dgLaborCI.FooterStyle.HorizontalAlign = HorizontalAlign.Right;

			Decimal totalBudget		= 0;
			Decimal totalEstimated	= 0;

			cSymbol = Project.GetCurrencySymbol(ProjectID);
			foreach (DataRow dr in dt.Rows)
			{
				totalBudget		+= (Decimal)dr["Budget"];
				totalEstimated	+= (Decimal)dr["Estimate"];
			}
			dgLaborCI.Columns[4].FooterText = lblLCActual.Text;

			dgLaborCI.Columns[2].FooterText = ShowValue(totalBudget);
			dgLaborCI.Columns[3].FooterText = ShowValue(totalEstimated);*/

			/*dgExpenses.Columns[0].HeaderText = LocRM.GetString("oCategory");
			dgExpenses.Columns[1].HeaderText = LocRM.GetString("oBudget");
			dgExpenses.Columns[2].HeaderText = LocRM.GetString("oEstimate");
			dgExpenses.Columns[3].HeaderText = LocRM.GetString("oActual");
			dt = Project.GetListExpensesDataTable(ProjectID);
			dgExpenses.DataSource = dt;*/

			//lblBudgetTitle.Text = LocRM.GetString("oBudget");
			//lblActualCostTitle.Text = LocRM.GetString("oActual");
			//lblEstimatedCostTitle.Text = LocRM.GetString("oEstimate");

			//lblBudget.Text = "";
			//lblEstimatedCost.Text = "";
			//lblActualCost.Text = "";

			// ToDo: вместо fixedBudget который брался ис budget использовать что-то другое
			/*
						using (IDataReader reader = Project.GetFinancesTotals(ProjectID))
						{
							if(reader.Read())
							{	
								if(reader["Budget"] != DBNull.Value)
									if(reader["Budget"].ToString() != "0")
									{
										lblDeltaBudgetO.Text = ((Decimal)(fixedBudget - (Decimal)reader["Budget"])).ToString("N")+cSymbol;
										lblBudget.Text = ((Decimal)reader["Budget"]).ToString("N")+cSymbol;
									}
								if((reader["Estimate"] != DBNull.Value) && (reader["Estimate"].ToString() != "0"))
								{
									lblDeltaBudgetEstim.Text = ((Decimal)(fixedBudget - (Decimal)reader["Estimate"])).ToString("N")+cSymbol;
									lblEstimatedCost.Text = ((Decimal)reader["Estimate"]).ToString("N")+cSymbol;
								}
								if((reader["Actual"] != DBNull.Value) && (reader["Actual"].ToString() != "0"))
								{
									actual += (Decimal)reader["Actual"];
								}
								if (actual>0)
									lblActualCost.Text = (actual).ToString("N")+cSymbol;

								lblDeltaBudget.Text = ((Decimal)(fixedBudget - actual)).ToString("N")+cSymbol;
							}
						}
			*/
		}
		#endregion

		public string ShowValue(object obj)
		{
			Decimal dv = 0;
			try
			{
				dv = Convert.ToDecimal(obj);
			}
			catch (InvalidCastException)
			{
			}

			string str = dv.ToString("N") + " " + cSymbol;
			if (str != "0")
				return str;
			else return "-";
		}

		protected string GetResourcesList(int ID, int IsToDo)
		{
			using (IDataReader rdr = (IsToDo == 1) ? ToDo.GetListResources(ID) : Task.GetListResources(ID))
			{
				StringBuilder sb = new StringBuilder();
				while (rdr.Read())
				{
					sb.Append(CommonHelper.GetUserStatusPureName((int)rdr["UserId"]));
					sb.Append("<br>");
				}
				return sb.ToString();
			}
		}

		protected void btnAplly_Click(object sender, System.EventArgs e)
		{
			ProjectID = int.Parse(ddProject.SelectedItem.Value);
		}

		//===========================================================================
		// This public property was added by conversion wizard to allow the access of a protected, autogenerated member variable Printheader1.
		//===========================================================================
		public Mediachase.UI.Web.Modules.ReportHeader Printheader1
		{
			get { return Migrated_Printheader1; }
			//set { Migrated_Printheader1 = value; }
		}
	}
}
