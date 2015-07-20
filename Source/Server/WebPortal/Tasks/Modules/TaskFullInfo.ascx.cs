namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using System.Globalization;
	using System.Reflection;

	/// <summary>
	///		Summary description for TaskFullInfo.
	/// </summary>
	public partial class TaskFullInfo : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskGeneral", typeof(TaskFullInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTrackingInfo", typeof(TaskFullInfo).Assembly);

		private DateTime TargetStartDate = DateTime.UtcNow;
		private DateTime TargetFinishDate = DateTime.UtcNow;

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
					throw new Exception("Invalid Task ID");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblOverdue.Text = LocRM.GetString("Overdue");
			lblComplTypeTitle.Text = LocRM.GetString("CompletionType") + ":";
			lblParentLabel.Text = LocRM.GetString("ParentTask") + ":";

			BindToolBar();
		}

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
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			tbView.AddText(LocRM.GetString("tTitle"));

			string text = "<img src='../Layouts/Images/icons/task1_edit.gif' border='0' width='16' height='16' align='absmiddle'> " + LocRM.GetString("Edit");
			string link = "../Tasks/TaskEdit.aspx?TaskID=" + TaskID;
			if (Task.CanUpdateConfigurationInfo(TaskID))
				tbView.AddRightLink(text, link);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			bool IsExternal = Security.CurrentUser.IsExternal;
			try
			{
				bool canViewFinances = Task.CanViewFinances(TaskID);

				using (IDataReader rdr = Task.GetTask(TaskID))
				{
					if (rdr.Read())
					{
						lblTitle.Text = (string)rdr["Title"];
						if (!IsExternal && Project.CanRead((int)rdr["ProjectId"]))
							lblBelongsTo.Text = "<a href='../Projects/ProjectView.aspx?ProjectID=" + (int)rdr["ProjectId"] + "'>" + (string)rdr["ProjectTitle"] + "</a>";
						else
							lblBelongsTo.Text = (string)rdr["ProjectTitle"];
						lblPriority.Text = (string)rdr["PriorityName"];

						TargetStartDate = (DateTime)rdr["StartDate"];
						TargetFinishDate = (DateTime)rdr["FinishDate"];
						lblStartDate.Text = TargetStartDate.ToShortDateString() + " " + TargetStartDate.ToShortTimeString();
						lblDueDate.Text = TargetFinishDate.ToShortDateString() + " " + TargetFinishDate.ToShortTimeString();

						if ((DateTime)rdr["FinishDate"] < UserDateTime.UserNow && !(bool)rdr["IsCompleted"])
							lblOverdue.Visible = true;
						else
							lblOverdue.Visible = false;

						lblOverallStatus.Text = (int)rdr["PercentCompleted"] + "% ";// + LocRM.GetString("Completed");
						lblOutlineNumber.Text = (string)rdr["OutlineNumber"];
						bool IsSummary = (bool)rdr["IsSummary"];
						bool IsMilestone = (bool)rdr["IsMilestone"];
						if (IsSummary || IsMilestone)
						{
							lblOutlineNumber.Text += " (";
							if (IsSummary)
								lblOutlineNumber.Text += LocRM.GetString("SummaryTask");
							else if (IsMilestone)
								lblOutlineNumber.Text += LocRM.GetString("Milestone");
							lblOutlineNumber.Text += ")";
							lblComplTypeTitle.Visible = false;
							lblCompletionType.Visible = false;
						}
						lblDescription.Text = CommonHelper.parsetext(rdr["Description"].ToString(), false);
						lblManager.Text = CommonHelper.GetUserStatus((int)rdr["ManagerId"]);
						lblCreated.Text = CommonHelper.GetUserStatus((int)rdr["CreatorId"]) + "&nbsp;&nbsp;" + ((DateTime)rdr["CreationDate"]).ToString("g");

						bool IsCompleted = (bool)rdr["IsCompleted"];
						if (IsCompleted)
						{
							if (rdr["CompletedBy"] != DBNull.Value)
								lblCompleted.Text = GetCompletionType((int)rdr["ReasonId"]) + " " + CommonHelper.GetUserStatus((int)rdr["CompletedBy"]);
							else
								lblCompleted.Text = LocRM.GetString("Yes");

							if (rdr["ActualFinishDate"] != DBNull.Value)
								lblActualFinishDate.Text = ((DateTime)rdr["ActualFinishDate"]).ToString("g");
						}
						else
						{
							lblCompleted.Text = LocRM.GetString("No");
						}

						if (rdr["ActualStartDate"] != DBNull.Value)
							lblActualStartDate.Text = ((DateTime)rdr["ActualStartDate"]).ToString("g");

						lblCompletionType.Text = rdr["CompletionTypeName"].ToString();
						lblActivationType.Text = rdr["ActivationTypeName"].ToString();
						if ((bool)rdr["MustBeConfirmed"])
							lblCompletionType.Text += "<br>" + LocRM.GetString("MustConfirm");
						lblOutlineLevel.Text = rdr["OutlineLevel"].ToString();

						lblTaskTime.Text = Util.CommonHelper.GetHours((int)rdr["TaskTime"]);

						if (canViewFinances)
						{
							SpentTimeLabel.Text = String.Format(CultureInfo.InvariantCulture,
								"{0} / {1}:",
								LocRM3.GetString("spentTime"),
								LocRM3.GetString("approvedTime"));

							lblSpentTime.Text = String.Format(CultureInfo.InvariantCulture,
								"{0} / {1}",
								Util.CommonHelper.GetHours((int)rdr["TotalMinutes"]),
								Util.CommonHelper.GetHours((int)rdr["TotalApproved"]));
						}
					}
					else
						Response.Redirect("../Common/NotExistingID.aspx?TaskID=1", true);
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("../Common/NotExistingId.aspx?AD=1");
			}

			int ParentId = Task.GetParent(TaskID);
			if (ParentId > 0)
			{
				string ParentTitle = Task.GetTaskTitle(ParentId);
				if (Task.CanRead(ParentId))
					lblParentTitle.Text = String.Format("<a href='../Tasks/TaskView.aspx?TaskId={0}'>{1}</a>", ParentId, ParentTitle);
				else
					lblParentTitle.Text = ParentTitle;
			}
			else
			{
				lblParentLabel.Visible = false;
				lblParentTitle.Visible = false;
			}


			// Categories
			string categories = String.Empty;
			using (IDataReader rdr = Task.GetListCategories(TaskID))
			{
				while (rdr.Read())
				{
					if (categories != "") categories += ", ";
					categories += (string)rdr["CategoryName"];
				}
			}
			lblCategory.Text = categories;

			tdPriority.Visible = tdPriority2.Visible = PortalConfig.CommonTaskAllowViewPriorityField;
			tdActivation.Visible = tdActivation2.Visible = PortalConfig.TaskAllowViewActivationTypeField;
			tdCompletion.Visible = tdCompletion2.Visible = PortalConfig.TaskAllowViewCompletionTypeField;
			tdTaskTime.Visible = tdTaskTime2.Visible = PortalConfig.TaskAllowViewTaskTimeField;
			trCategories.Visible = PortalConfig.CommonTaskAllowViewGeneralCategoriesField;
		}
		#endregion

		#region BindReminders
		private void BindReminders()
		{
			Task.TaskSecurity sec = Task.GetSecurity(TaskID);
			bool IsMember = sec.IsManager || sec.IsRealTaskResource;

			if (IsMember && TargetStartDate > DateTime.UtcNow)
			{
				int Lag = -1;
				bool IsActive = false;

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.Task_StartDate, TaskID))
				{
					if (reader.Read())
					{
						Lag = (int)reader["Lag"];
						IsActive = (bool)reader["IsActive"];
					}
				}

				if (IsActive)
				{
					imgReminderStart.ToolTip = String.Format("{0}: {1}", LocRM2.GetString("Start"), CommonHelper.GetIntervalString(Lag));
				}
				else
				{
					imgReminderStart.ToolTip = CommonHelper.GetIntervalString(-1);
					if (imgReminderStart.ToolTip.StartsWith("["))
						imgReminderStart.ToolTip = imgReminderStart.ToolTip.Substring(1);
					if (imgReminderStart.ToolTip.EndsWith("]"))
						imgReminderStart.ToolTip = imgReminderStart.ToolTip.Substring(0, imgReminderStart.ToolTip.Length - 1);
					imgReminderStart.ImageUrl = "~/Layouts/Images/reminder2.gif";
				}
			}
			else
			{
				imgReminderStart.Visible = false;
			}

			if (IsMember && TargetFinishDate > DateTime.UtcNow)
			{
				int Lag = -1;
				bool IsActive = false;

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.Task_FinishDate, TaskID))
				{
					if (reader.Read())
					{
						Lag = (int)reader["Lag"];
						IsActive = (bool)reader["IsActive"];
					}
				}

				if (IsActive)
				{
					imgReminderFinish.ToolTip = String.Format("{0}: {1}", LocRM2.GetString("Finish"), CommonHelper.GetIntervalString(Lag));
				}
				else
				{
					imgReminderFinish.ToolTip = CommonHelper.GetIntervalString(-1);
					if (imgReminderFinish.ToolTip.StartsWith("["))
						imgReminderFinish.ToolTip = imgReminderFinish.ToolTip.Substring(1);
					if (imgReminderFinish.ToolTip.EndsWith("]"))
						imgReminderFinish.ToolTip = imgReminderFinish.ToolTip.Substring(0, imgReminderFinish.ToolTip.Length - 1);
					imgReminderFinish.ImageUrl = "~/Layouts/Images/reminder2.gif";
				}
			}
			else
			{
				imgReminderFinish.Visible = false;
			}
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindData();
			BindReminders();
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
	}
}
