using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.IBN.DefaultUserReports;
using System.Globalization;

namespace Mediachase.UI.Web.UserReports.Modules
{
	public partial class TaskDates : System.Web.UI.UserControl, Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strTaskDates", typeof(TaskDates).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
				BindData();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ApplyButton.Text = LocRM.GetString("Show");

			MainGrid.Columns[0].HeaderText = LocRM.GetString("Project");
			MainGrid.Columns[1].HeaderText = LocRM.GetString("Task");
			MainGrid.Columns[2].HeaderText = LocRM.GetString("User");
			MainGrid.Columns[3].HeaderText = LocRM.GetString("Changed");
			MainGrid.Columns[4].HeaderText = LocRM.GetString("OldDate");
			MainGrid.Columns[5].HeaderText = LocRM.GetString("NewDate");

			HeaderControl.Title = LocRM.GetString("ReportName");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			// Period
			PeriodType.Items.Add(new ListItem(LocRM.GetString("LastWeek"), "1"));
			PeriodType.Items.Add(new ListItem(LocRM.GetString("LastMonth"), "2"));
			PeriodType.Items.Add(new ListItem(LocRM.GetString("Custom"), "3"));
			PeriodFrom.SelectedDate = UserDateTime.Today.AddDays(-7);
			PeriodTo.SelectedDate = UserDateTime.Now;

			// Projects
			ProjectList.Items.Add(new ListItem(LocRM.GetString("Any"), "0"));
			using (IDataReader reader = Mediachase.IBN.Business.Project.GetListActiveProjectsByManager())
			{
				while (reader.Read())
				{
					ProjectList.Items.Add(new ListItem(reader["Title"].ToString(), reader["ProjectId"].ToString()));
				}
			}
		}
		#endregion

		#region ApplyButton_Click
		protected void ApplyButton_Click(object sender, EventArgs e)
		{
			// Filter info (for print)
			string filter = String.Format(
				CultureInfo.InvariantCulture,
				"&nbsp;&nbsp;{0}: ",
				LocRM.GetString("Period"));

			if (PeriodType.Value == "1")
				filter += LocRM.GetString("LastWeek");
			else if (PeriodType.Value == "2")
				filter += LocRM.GetString("LastMonth");
			else
				filter += String.Format(
					CultureInfo.InvariantCulture,
					"{0} - {1}",
					PeriodFrom.SelectedDate.ToShortDateString(), PeriodTo.SelectedDate.ToShortDateString());

			filter += String.Format(
				CultureInfo.InvariantCulture,
				"<br/>&nbsp;&nbsp;{0}: {1}",
				LocRM.GetString("Project"),
				ProjectList.SelectedItem.Text);
			HeaderControl.Filter = filter;

			// Bind Grid
			BindGrid();
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DateTime fromDate;
			DateTime toDate;

			if (PeriodType.Value == "1")	// LastWeek
			{
				fromDate = UserDateTime.Today.AddDays(-7);
				toDate = UserDateTime.Today.AddDays(1);
			}
			else if (PeriodType.Value == "2")	// LastMonth
			{
				fromDate = UserDateTime.Today.AddMonths(-1);
				toDate = UserDateTime.Today.AddDays(1);
			}
			else
			{
				fromDate = PeriodFrom.SelectedDate;
				toDate = PeriodTo.SelectedDate.AddDays(1);
			}

			DataTable dt = new DataTable();
			dt.Load(Mediachase.IBN.Business.Task.GetTaskDates(int.Parse(ProjectList.SelectedValue), fromDate, toDate));

			// remove duplicated project names
			int oldProject = -1;
			int oldTask = -1;
			foreach (DataRow dr in dt.Rows)
			{
				int curProject = (int)dr["ProjectId"];
				int curTask = (int)dr["TaskId"];
				if (curProject == oldProject)
					dr["ProjectName"] = "";
				if (curTask == oldTask)
					dr["TaskName"] = "";
				oldProject = curProject;
				oldTask = curTask;
			}

			MainGrid.DataSource = dt.DefaultView;
			MainGrid.DataBind();
		}
		#endregion

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (PeriodType.Value != "3")
				DateTable.Style.Add(HtmlTextWriterStyle.Display, "none");
			else
				DateTable.Style.Add(HtmlTextWriterStyle.Display, "block");
		}

		#region IUserReportInfo Members
		public string ShowName
		{
			get 
			{
				return LocRM.GetString("ReportName");
			}
		}

		public string Description
		{
			get
			{
				return string.Empty;
			}
		}
		#endregion
	}
}