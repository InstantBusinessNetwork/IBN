namespace Mediachase.UI.Web.Events.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using System.Resources;

	using ComponentArt.Web.UI;
	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for EventView1.
	/// </summary>
	public partial class EventView1 : System.Web.UI.UserControl, ITopTabs
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventGeneral", typeof(EventView1).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(EventView1).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strEventView", typeof(EventView1).Assembly);
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Events.Resources.strRecEditor", typeof(EventView1).Assembly);
		protected ResourceManager LocRM5 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(EventView1).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region Tab
		private string Tab
		{
			get
			{
				return Request["Tab"];
			}
		}
		#endregion

		#region EventId
		private int EventId
		{
			get
			{
				try
				{
					return int.Parse(Request["EventId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				return Util.CommonHelper.GetRequestInteger(Request, "SharedID", -1);
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			BindTabs();
			if(!IsPostBack)
				ViewState["CurrentTab"] = pc["EventView_CurrentTab"];

			if (!IsPostBack)
			{
				CalendarEntry.AddHistory(EventId, CalendarEntry.GetEventTitle(EventId));
			}
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
			this.btnAddToFavorites.Click += new EventHandler(btnAddToFavorites_Click);
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			string sharedreq = "";
			if (SharedID > 0)
				sharedreq = "&SharedId=" + SharedID;

			secHeader.Title = LocRM.GetString("tTitle");

			bool canUpdate = CalendarEntry.CanUpdate(EventId);
			bool canDelete = CalendarEntry.CanDelete(EventId);
			bool isExternal = Security.CurrentUser.IsExternal;

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			#region Edit: Event
			if (canUpdate && !isExternal)
			{
				ComponentArt.Web.UI.MenuItem editItem = new ComponentArt.Web.UI.MenuItem();
				editItem.Text = LocRM.GetString("Edit");
				editItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
				editItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
				editItem.Look.RightIconWidth = Unit.Pixel(15);
				editItem.Look.RightIconHeight = Unit.Pixel(10);

				#region Edit Event
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/event_edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../Events/EventEdit.aspx?EventId=" + EventId + sharedreq;
				subItem.Text = LocRM.GetString("EditEvent");
				editItem.Items.Add(subItem);
				#endregion

				#region Reccurence
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/recurrence.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../Events/RecEditor.aspx?EventId=" + EventId + sharedreq;
				subItem.Text = LocRM4.GetString("Recurrence");
				editItem.Items.Add(subItem);
				#endregion
				/*
				#region Edit General Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditGeneralInfo.aspx?IncidentId=" + IncidentId + "', 500, 400);";
				subItem.Text = LocRM2.GetString("EditGeneralInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Categories
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditCategories.aspx?IncidentId=" + IncidentId + "', 300, 350);";
				subItem.Text = LocRM2.GetString("EditCategories");
				editItem.Items.Add(subItem);
				#endregion
*/
				topMenuItem.Items.Add(editItem);
			}
			#endregion

			#region Modyfy Resources
			if (canUpdate && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/editgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_EventParticipants");
				string cmd = cm.AddCommand("Event", "", "EventView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				//subItem.ClientSideCommand = "javascript:ShowWizard('ParticipantsEditor.aspx?EventId=" + EventId + "', 650, 350);";
				subItem.Text = LocRM.GetString("EventParticipants");
				topMenuItem.Items.Add(subItem);

				if (!IsPostBack && Request["Assign"] == "1")
				{
					ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
					 "function Assign(){" + cmd + "} setTimeout('Assign()', 400);", true);
					//Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>ShowWizard('ParticipantsEditor.aspx?EventId=" + EventId + "', 650, 350);</script>");
				}
			}
			#endregion

			#region Delete
			if (canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/event_delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:DeleteEvent()";
				subItem.Text = LocRM.GetString("Delete");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Seperator ---
			if (canUpdate || canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Add Comments
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/comments.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			string commentaddlink = "../Common/CommentAdd.aspx?";
			if (Security.CurrentUser.IsExternal)
				commentaddlink = "../External/ExternalCommentAdd.aspx?";
			subItem.ClientSideCommand = String.Format("javascript:OpenWindow('{0}EventId={1}',{2},false);",
			  commentaddlink, EventId, (Security.CurrentUser.IsExternal) ? "800,600" : "520,270");
			subItem.Text = LocRM2.GetString("CreateComment");
			topMenuItem.Items.Add(subItem);
			#endregion

			#region Timesheet
			// OR [2007-08-23]: We should use IbnNext TimeTracking
			/*
			if(Configuration.ProjectManagementEnabled)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/timesheet.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalTimeTracking.aspx" : "../TimeTracking/TimeTrackingWeek.aspx";
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?EventId={1}', {2});",
					sPath, EventId, (Security.CurrentUser.IsExternal) ? "800,600" : "450, 200");
				subItem.Text = LocRM2.GetString("AddTimeSheet");
				topMenuItem.Items.Add(subItem);
			}
			 */
			#endregion

			#region UpdateHistory
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/SystemEventsByObject.aspx?ObjectId={0}&ObjectTypeId={1}', 750, 466);", EventId, (int)ObjectTypes.CalendarEntry);
				subItem.Text = LocRM5.GetString("UpdateHistory");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Latest Visitors
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/LatestVisitors.aspx?ObjectId={0}&ObjectTypeId={1}', 450, 266);", EventId, (int)ObjectTypes.CalendarEntry);
				subItem.Text = LocRM5.GetString("LatestVisitors");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region System Notifications
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("../Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", EventId, ((int)ObjectTypes.CalendarEntry).ToString());
				subItem.Text = LocRM2.GetString("SystemNotifications");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region System Reminder
			if (!isExternal)
			{
				CalendarEntry.EventSecurity sec = CalendarEntry.GetSecurity(EventId);
				if (sec.IsManager || sec.IsResource)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/reminder.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId={0}&ObjectId={1}', 420, 150)", (int)ObjectTypes.CalendarEntry, EventId);
					subItem.Text = LocRM2.GetString("EditReminder");
					topMenuItem.Items.Add(subItem);
				}
			}
			#endregion

			#region Favorites
			if (!CalendarEntry.CheckFavorites(EventId) && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Favorites.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(btnAddToFavorites, "");
				subItem.Text = LocRM.GetString("AddToFavorites");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			secHeader.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{
			if (Tab != null && (Tab == "General" || Tab == "FileLibrary" || Tab == "Finance" || Tab == "Discussions" || Tab == "Customization"))
				pc["EventView_CurrentTab"] = Tab;
			else if (ViewState["CurrentTab"] != null)
				pc["EventView_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["EventView_CurrentTab"] == null)
				pc["EventView_CurrentTab"] = "General";

			ctrlTopTab.AddTab(LocRM3.GetString("tabGeneral"), "General");
			ctrlTopTab.AddTab(LocRM3.GetString("tabMetaData"), "Customization");
			bool IsExternal = Security.CurrentUser.IsExternal;
			
			ctrlTopTab.AddTab(LocRM3.GetString("tabLibrary"), "FileLibrary");

			if (CalendarEntry.CanViewFinances(EventId) && !IsExternal)
				ctrlTopTab.AddTab(LocRM3.GetString("tabFinance"), "Finance");
			else
			{
				if (pc["EventView_CurrentTab"] == "Finance")
					pc["EventView_CurrentTab"] = "General";
			}
			ctrlTopTab.AddTab(LocRM3.GetString("tabDiscussions"), "Discussions");

			ctrlTopTab.SelectItem(pc["EventView_CurrentTab"]);

			string controlName = "EventGeneral.ascx";
			switch (pc["EventView_CurrentTab"])
			{
				case "General":
					controlName = "EventGeneral.ascx";
					break;
				case "FileLibrary":
					controlName = "FileLibrary.ascx";
					break;
				case "Finance":
					controlName = "Finance.ascx";
					break;
				case "Discussions":
					controlName = "Discussions.ascx";
					break;
				case "Customization":
					controlName = "MetaDataView.ascx";
					break;
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}
		#endregion

		#region btnAddToFavorites_Click
		private void btnAddToFavorites_Click(object sender, EventArgs e)
		{
			string sharedreq = "";
			if (SharedID > 0)
				sharedreq = "&SharedId=" + SharedID;

			CalendarEntry.AddFavorites(EventId);
			Util.CommonHelper.ReloadTopFrame("Favorites.ascx", "../Events/EventView.aspx?EventId=" + EventId + sharedreq, Response);
		}
		#endregion

		#region lbDeleteEventAll_Click
		protected void lbDeleteEventAll_Click(object sender, System.EventArgs e)
		{
			int ProjectId = CalendarEntry.GetProject(EventId);
			string link;
			if (ProjectId > 0)
				link = String.Format("../Projects/ProjectView.aspx?ProjectId={0}", ProjectId);
			else
				link = "../Workspace/default.aspx?Btab=Workspace";

			CalendarEntry.Delete(EventId);

			Response.Redirect(link, true);
		}
		#endregion

		#region ITopTabs Members
		public Mediachase.UI.Web.Modules.TopTabs GetTopTabs()
		{
			return ctrlTopTab;
		}
		#endregion
	}
}
