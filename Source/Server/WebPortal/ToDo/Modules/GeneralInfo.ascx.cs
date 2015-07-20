namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Modules;
	using System.Globalization;
	using System.Reflection;

	/// <summary>
	///		Summary description for GeneralInfo.
	/// </summary>
	public partial class GeneralInfo : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", typeof(GeneralInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTrackingInfo", typeof(GeneralInfo).Assembly);

		private DateTime TargetStartDate = DateTime.UtcNow;
		private DateTime TargetFinishDate = DateTime.UtcNow;

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
					throw new Exception("Invalid ToDo ID");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
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

		#region BindToolbar()
		private void BindToolbar()
		{
			tbView.AddText(LocRM.GetString("tTitle"));
			if (Mediachase.IBN.Business.ToDo.CanUpdate(ToDoID))
				tbView.AddRightLink("<img id='imgNewTS' src='../Layouts/Images/icons/task_edit.gif'/> " + LocRM.GetString("Edit"),"../ToDo/ToDoEdit.aspx?ToDoID=" + ToDoID);
		}
		#endregion

		#region BindToDoData
		private void BindToDoData()
		{
			bool IsExternal = Security.CurrentUser.IsExternal;
			lblOverdue.Text = LocRM.GetString("Overdue");
			bool canViewFinances = ToDo.CanViewFinances(ToDoID);
			using(IDataReader rdr = ToDo.GetToDo(ToDoID))
			{
				///  ToDoId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
				///  Title, Description, CreationDate, StartDate, FinishDate, 
				///  ActualFinishDate, PriorityId, PriorityName, StatusId, StatusName, 
				///  PercentCompleted, IsActual, CompletionTypeId, CompletionTypeName
				if (rdr.Read())
				{
					lblTitle.Text = (string)rdr["Title"];

					lblPriority.Text = (string)rdr["PriorityName"];
					if(rdr["StartDate"] != DBNull.Value)
					{
						TargetStartDate = (DateTime)rdr["StartDate"];
						lblStartDate.Text = TargetStartDate.ToShortDateString() + " " + TargetStartDate.ToShortTimeString();
					}
					if(rdr["FinishDate"] != DBNull.Value)
					{
						TargetFinishDate = (DateTime)rdr["FinishDate"];
						lblDueDate.Text = TargetFinishDate.ToShortDateString() + " " + TargetFinishDate.ToShortTimeString();
					}
					lblOverallStatus.Text = (int)rdr["PercentCompleted"] +"% ";// + LocRM.GetString("Completed");

					if (rdr["IncidentId"]!=DBNull.Value)
					{
						lblParentTitle.Text = LocRM.GetString("BelongsToIncident");
						if (!IsExternal)
							lblParent.Text = "<a href='../Incidents/IncidentView.aspx?IncidentID="+ (int)rdr["IncidentId"] + "'>" + (string)rdr["IncidentTitle"]+"</a>";
						else
							lblParent.Text = (string)rdr["IncidentTitle"];
					}
					else if(rdr["ProjectId"] != DBNull.Value)
					{
						if(Configuration.ProjectManagementEnabled)
						{
							lblParentTitle.Text = LocRM.GetString("BelongsToProject");
					
							if (!IsExternal && Project.CanRead((int)rdr["ProjectId"]))
								lblParent.Text = "<a href='../Projects/ProjectView.aspx?ProjectID="+ (int)rdr["ProjectId"] + "'>" + (string)rdr["ProjectTitle"]+"</a>";
							else
								lblParent.Text =(string)rdr["ProjectTitle"];
						}
						else
						{
							tdParentTitle.Visible = false;
							tdParentName.Visible = false;
						}
					}
					else
					{
						tdParentTitle.Visible = false;
						tdParentName.Visible = false;
					}
				
				
					lblDescription.Text = CommonHelper.parsetext(rdr["Description"].ToString(),false);
					lblManager.Text = CommonHelper.GetUserStatus((int)rdr["ManagerId"]);
					lblCreated.Text = CommonHelper.GetUserStatus((int)rdr["CreatorId"]) + "&nbsp;&nbsp;"  + ((DateTime)rdr["CreationDate"]).ToShortDateString() + " " + ((DateTime)rdr["CreationDate"]).ToShortTimeString();
					if (rdr["FinishDate"]!=DBNull.Value && (DateTime)rdr["FinishDate"]<UserDateTime.UserNow && !(bool)rdr["IsCompleted"])
						lblOverdue.Visible = true;
					else
						lblOverdue.Visible = false;

					//lblCompleted.Text = rdr["StatusName"].ToString();
					lblCompletionType.Text = (string)rdr["CompletionTypeName"];
					if ((bool)rdr["MustBeConfirmed"])
						lblCompletionType.Text+= "<br>" + LocRM.GetString("MustConfirm");

					bool IsCompleted = (bool)rdr["IsCompleted"];
					if (IsCompleted)
					{ 
						if (rdr["CompletedBy"]!=DBNull.Value)
							lblCompleted.Text = GetCompletionType((int)rdr["ReasonId"])+ " " + CommonHelper.GetUserStatus((int)rdr["CompletedBy"]);
						else
							lblCompleted.Text = LocRM.GetString("Yes");

						if (rdr["ActualFinishDate"] != DBNull.Value)
							lblActualFinishDate.Text = ((DateTime)rdr["ActualFinishDate"]).ToString("g");
						trActualFinishDate.Visible = true;
					}
					else
					{
						lblCompleted.Text = LocRM.GetString("No");
						trActualFinishDate.Visible = false;
					}

					string categories = "";
					using(IDataReader rdr1 = ToDo.GetListCategories(ToDoID))
					{
						while (rdr1.Read())
						{
							if (categories!="") categories+=", ";
							categories+=(string)rdr1["CategoryName"];
						}
					}
					lblCategory.Text = categories;

					lblClient.Text = Util.CommonHelper.GetClientLink(this.Page, rdr["OrgUid"], rdr["ContactUid"], rdr["ClientName"]);

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
			}

			tdPriority.Visible = tdPriority2.Visible = PortalConfig.CommonToDoAllowViewPriorityField;
			tdCompletion.Visible = tdCompletion2.Visible = PortalConfig.ToDoAllowViewCompletionTypeField;
			tdCategories.Visible = tdCategories2.Visible = PortalConfig.CommonToDoAllowViewGeneralCategoriesField;
			tdClient.Visible = tdClient2.Visible = PortalConfig.CommonToDoAllowViewClientField;
			trCategoriesClient.Visible = tdCategories.Visible || tdClient.Visible;
			tdTaskTime.Visible = tdTaskTime2.Visible = PortalConfig.CommonToDoAllowViewTaskTimeField;
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

		#region BindReminders
		private void BindReminders()
		{
			Mediachase.IBN.Business.ToDo.ToDoSecurity sec = Mediachase.IBN.Business.ToDo.GetSecurity(ToDoID);
			bool IsMember = sec.IsManager || sec.IsResource || sec.IsCreator;

			if (IsMember && TargetStartDate > DateTime.UtcNow)
			{
				int Lag = -1;
				bool IsActive = false;

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.Todo_StartDate, ToDoID))
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

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.Todo_FinishDate, ToDoID))
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
			BindToDoData();
			BindReminders();
		}
		#endregion
	}
}
