namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	using System.Reflection;

	/// <summary>
	///		Summary description for EventFullInfo.
	/// </summary>
	public partial class EventFullInfo : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventGeneral", typeof(EventFullInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		private DateTime TargetStartDate = DateTime.UtcNow;
		private DateTime TargetFinishDate = DateTime.UtcNow;
		private bool HasRecurrence = false;

		#region EventID
		protected int EventID
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "EventID", -1);
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "SharedID", -1);
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

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("tTitle"));

			string sharedreq = "";
			if (SharedID > 0)
				sharedreq = "&SharedId=" + SharedID;

			if (CalendarEntry.CanUpdate(EventID))
				secHeader.AddRightLink("<img src='../Layouts/Images/icons/event_edit.gif' border='0' width='16' height='16' align='absmiddle'> " + LocRM.GetString("Edit"), "../Events/EventEdit.aspx?EventID=" + EventID + sharedreq);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			try
			{
				using (IDataReader rdr = CalendarEntry.GetEvent(EventID))
				{
					///  EventId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
					///  Title, Description, Location, CreationDate, StartDate, 
					///  FinishDate, PriorityId, PriorityName, TypeId, TypeName, ReminderInterval, HasRecurrence
					if (rdr.Read())
					{
						lblTitle.Text = (string)rdr["Title"];
						if (Configuration.ProjectManagementEnabled)
						{
							if (rdr["ProjectId"] != DBNull.Value)
							{
								if (Project.CanRead((int)rdr["ProjectId"]))
									lblProject.Text = "<a href='../Projects/ProjectView.aspx?ProjectID=" + (int)rdr["ProjectId"] + "'>" + (string)rdr["ProjectTitle"] + "</a>";
								else
									lblProject.Text = (string)rdr["ProjectTitle"];
							}
							else
								lblProject.Text = LocRM.GetString("tNotSet");
						}
						else
						{
							tdPrjLabel.Visible = false;
							tdPrjName.Visible = false;
						}

						lblLocation.Text = (string)rdr["Location"];
						lblPriority.Text = (string)rdr["PriorityName"];
						lblType.Text = (string)rdr["TypeName"];

						lblManager.Text = CommonHelper.GetUserStatus((int)rdr["ManagerId"]);
						lblCreated.Text = CommonHelper.GetUserStatus((int)rdr["CreatorId"]) + "&nbsp;&nbsp;" + ((DateTime)rdr["CreationDate"]).ToShortDateString() + " " + ((DateTime)rdr["CreationDate"]).ToShortTimeString();

						TargetStartDate = (DateTime)rdr["StartDate"];
						TargetFinishDate = (DateTime)rdr["FinishDate"];
						lblStartDate.Text = TargetStartDate.ToShortDateString() + " " + TargetStartDate.ToShortTimeString();
						lblEndDate.Text = TargetFinishDate.ToShortDateString() + " " + TargetFinishDate.ToShortTimeString();
						lblDescription.Text = CommonHelper.parsetext((string)rdr["Description"], false);

						HasRecurrence = ((int)rdr["HasRecurrence"] == 1);

						lblClient.Text = Util.CommonHelper.GetClientLink(this.Page, rdr["OrgUid"], rdr["ContactUid"], rdr["ClientName"]);

					}
					else
						Response.Redirect("../Common/NotExistingID.aspx?EventID=1");
				}
			}
			catch (AccessDeniedException)
			{
				Response.Redirect("~/Common/NotExistingID.aspx?AD=1");
			}

			using (IDataReader rdr = CalendarEntry.GetListCategories(EventID))
			{
				string categories = "";
				while (rdr.Read())
				{
					if (categories != "") categories += ", ";
					categories += (string)rdr["CategoryName"];
				}

				lblCategory.Text = categories;
			}

			tdPriority.Visible = tdPriority2.Visible = PortalConfig.CommonCEntryAllowViewPriorityField;
			tdCategories.Visible = tdCategories2.Visible = PortalConfig.CommonCEntryAllowViewGeneralCategoriesField;
			tdClient.Visible = tdClient2.Visible = PortalConfig.CommonCEntryAllowViewClientField;
			trCategoriesClient.Visible = tdCategories.Visible || tdClient.Visible;
		}
		#endregion

		#region BindReminders
		private void BindReminders()
		{
			CalendarEntry.EventSecurity sec = CalendarEntry.GetSecurity(EventID);
			bool IsMember = sec.IsManager || sec.IsResource;

			if (IsMember && (TargetStartDate > DateTime.UtcNow || TargetFinishDate > DateTime.UtcNow && HasRecurrence))
			{
				int Lag = -1;
				bool IsActive = false;

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.CalendarEntry_StartDate, EventID))
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

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.CalendarEntry_FinishDate, EventID))
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
	}
}
