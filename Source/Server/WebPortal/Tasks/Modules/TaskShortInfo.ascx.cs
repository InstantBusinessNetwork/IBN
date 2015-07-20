namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Resources;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Globalization;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for TaskGeneral.
	/// </summary>
	public partial class TaskShortInfo : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskGeneral", typeof(TaskShortInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", typeof(TaskShortInfo).Assembly);

		#region _taskId
		private int _taskId
		{
			get
			{
				try
				{
					return int.Parse(Request["TaskID"]);
				}
				catch
				{
					throw new Exception("Invalid Task ID");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region BindData
		private void BindData()
		{
			///  TaskId, TaskNum, ProjectId, ProjectTitle, ManagerId, CreatorId, CompletedBy, Title, 
			///  Description,	CreationDate, StartDate, FinishDate, 	Duration, 
			///  ActualFinishDate, PriorityId, PriorityName, PercentCompleted, OutlineNumber, 
			///  OutlineLevel, 	IsSummary, IsMilestone, ConstraintTypeId, ConstraintTypeName, 
			///  ConstraintDate, CompletionTypeId, CompletionTypeName, IsCompleted, 
			///  MustBeConfirmed, ReasonId, ProjectCode
			try
			{
				using (IDataReader rdr = Task.GetTask(_taskId))
				{
					if (rdr.Read())
					{
						lblTimeline.Text = String.Format("{0} {1} - {2} {3}",
							((DateTime)rdr["StartDate"]).ToShortDateString(), ((DateTime)rdr["StartDate"]).ToShortTimeString(),
							((DateTime)rdr["FinishDate"]).ToShortDateString(), ((DateTime)rdr["FinishDate"]).ToShortTimeString());

						string projectPostfix = CHelper.GetProjectNumPostfix((int)rdr["ProjectId"], (string)rdr["ProjectCode"]);
						if (Project.CanRead((int)rdr["ProjectId"]))
							lblTitle.Text = String.Format(CultureInfo.InvariantCulture,
								"<a href='../Projects/ProjectView.aspx?ProjectId={0}' title='{1}'>{2}{3}</a> \\ ", 
								rdr["ProjectId"].ToString(), 
								LocRM2.GetString("Project"),
								rdr["ProjectTitle"].ToString(),
								projectPostfix);
						else
							lblTitle.Text = String.Format(CultureInfo.InvariantCulture,
								"<span title='{0}'>{1}{2}</span> \\ ",
								LocRM2.GetString("Project"), 
								rdr["ProjectTitle"].ToString(),
								projectPostfix);

						lblTitle.Text += String.Format("{0} (#{1})", rdr["Title"].ToString(), _taskId);

						if ((bool)rdr["IsSummary"] || (bool)rdr["IsMileStone"])
						{
							lblSummaryMilestone.Visible = true;
							if ((bool)rdr["IsSummary"])
								lblSummaryMilestone.Text = "(" + LocRM.GetString("SummaryTask") + ")";
							if ((bool)rdr["IsMileStone"])
								lblSummaryMilestone.Text = "(" + LocRM.GetString("Milestone") + ")";
						}

						lblState.ForeColor = Util.CommonHelper.GetStateColor((int)rdr["StateId"]);
						lblState.Text = rdr["StateName"].ToString();
						if (((int)rdr["StateId"] == (int)ObjectStates.Active || (int)rdr["StateId"] == (int)ObjectStates.Overdue)
							&& !(bool)rdr["IsMileStone"])
							lblState.Text += String.Format(" ({0} %)", rdr["PercentCompleted"].ToString());

						lblPriority.Text = rdr["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
						lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)rdr["PriorityId"]);
						lblPriority.Visible = PortalConfig.CommonTaskAllowViewPriorityField;

						if (rdr["Description"] != DBNull.Value)
						{
							string txt = CommonHelper.parsetext(rdr["Description"].ToString(), false);
							if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
								txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
							lblDescription.Text = txt;
						}
					}
					else
						Response.Redirect("../Common/NotExistingId.aspx?TaskId=" + _taskId);
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("../Common/NotExistingId.aspx?AD=1");
			}
		}
		#endregion

		#region GetCompletionType
		private string GetCompletionType(int type)
		{
			CompletionReason rsn = (CompletionReason)type;
			switch (rsn)
			{
				case CompletionReason.SuspendedManually:
				case CompletionReason.SuspendedAutomatically:
					return LocRM.GetString("Suspended");
				case CompletionReason.CompletedManually:
					return LocRM.GetString("CompletedByManager");
				case CompletionReason.CompletedAutomatically:
					return LocRM.GetString("CompletedByResource");
				case CompletionReason.NotCompleted:
					return LocRM.GetString("NotCompleted");
			}
			return "";
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindData();
		}
		#endregion
	}
}
