namespace Mediachase.UI.Web.Incidents.Modules
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
	using System.IO;

	/// <summary>
	///		Summary description for IncidentView1.
	/// </summary>
	public partial class IncidentView1 : System.Web.UI.UserControl, ITopTabs
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(IncidentView1).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(IncidentView1).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strPageTitles", typeof(IncidentView1).Assembly);
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

		#region IncidentId
		private int _incidentId = -1;
		private int IncidentId
		{
			get
			{
				if (_incidentId > 0)
					return _incidentId;
				if (Request["IncidentId"] != null && Request["IncidentId"] != "")
				{
					_incidentId = int.Parse(Request["IncidentId"]);
					try
					{
						using (IDataReader reader = Incident.GetIncident(_incidentId))
						{
							if (!reader.Read())
								_incidentId = -1;
						}
					}
					catch (AccessDeniedException)
					{
						Response.Redirect("~/Common/NotExistingID.aspx?AD=1");
					}
					catch
					{
						Response.Redirect("~/Common/NotExistingID.aspx?IncidentId=1");
					}
				}
				return _incidentId;
			}
		}
		#endregion

		#region SharedID
		private int SharedID
		{
			get
			{
				int retval = -1;
				if (Request["SharedID"] != null)
					retval = int.Parse(Request["SharedID"]);
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.HelpDeskEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if (IncidentId < 0)
				Response.Redirect("../Common/NotExistingID.aspx?IncidentID=1");

			string path = PortalConfig.IssueShortInfoControlDefaultValue;
			if (File.Exists(Server.MapPath(PortalConfig.IssueShortInfoControl)))
				path = PortalConfig.IssueShortInfoControl;
			System.Web.UI.Control control = LoadControl(path);
			InfoPlaceHolder.Controls.Add(control);

			if (SharedID > 0)
			{
				apShared.Visible = true;
				lblEntryOwner.Text = Util.CommonHelper.GetUserStatus(SharedID);
			}
			else
				apShared.Visible = false;

			BindToolbar();
			BindTabs();
			if (!IsPostBack)
			{
				ViewState["CurrentTab"] = pc["IncidentView_CurrentTab"];
				try
				{
					Incident.AddHistory(IncidentId, Incident.GetTitle(IncidentId));
				}
				catch
				{
					Response.Redirect("../Common/NotExistingID.aspx?IncidentId=1");
				}
			}

			if (!Security.CurrentUser.IsExternal)
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_RelatedIssAdd");
				cm.AddCommand("Incident", "", "IncidentView", "MC_HDM_Redirect");
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
			this.btnAddRelatedIss.Click += new EventHandler(btnAddRelatedIss_Click);
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tbView");

			bool canUpdate = Incident.CanUpdate(IncidentId);
			bool canAddToDoBox = Incident.CanAddToDo(IncidentId);
			bool canAddResBox = Incident.CanModifyResources(IncidentId);
			bool canViewFinances = Incident.CanViewFinances(IncidentId);
			bool canDelete = Incident.CanDelete(IncidentId);
			bool isExternal = Security.CurrentUser.IsExternal;

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			CommandManager cm = null;
			if(!Security.CurrentUser.IsExternal)
				cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters();
			string cmd = String.Empty;

			#region Create ToDo
			if (canAddToDoBox && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/task_create.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/ToDo/ToDoEdit.aspx?IncidentId=" + IncidentId;
				subItem.Text = LocRM.GetString("tbAdd");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Edit: Incident, GeneralInfo, ResolutionInfo, Status
			if (canUpdate)
			{
				ComponentArt.Web.UI.MenuItem editItem = new ComponentArt.Web.UI.MenuItem();
				editItem.Text = LocRM.GetString("tbViewEdit");
				editItem.Look.RightIconUrl = "../Layouts/Images/arrow_right.gif";
				editItem.Look.HoverRightIconUrl = "../Layouts/Images/arrow_right_hover.gif";
				editItem.Look.RightIconWidth = Unit.Pixel(15);
				editItem.Look.RightIconHeight = Unit.Pixel(10);

				#region Edit Incident
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/incident_edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "../Incidents/IncidentEdit.aspx?IncidentId=" + IncidentId + "&Back=incident";
				subItem.Text = LocRM.GetString("EditIssue");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit General Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditGeneralInfo.aspx?IncidentId=" + IncidentId + "', 500, 400);";
				subItem.Text = LocRM2.GetString("EditGeneralInfo");
				editItem.Items.Add(subItem);
				#endregion

				#region Edit Categories
				if (PortalConfig.CommonIncidentAllowEditGeneralCategoriesField ||
					PortalConfig.IncidentAllowEditIncidentCategoriesField)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.ClientSideCommand = "javascript:ShowWizard('EditCategories.aspx?IncidentId=" + IncidentId + "', 300, 350);";
					subItem.Text = LocRM2.GetString("EditCategories");
					editItem.Items.Add(subItem);
				}
				#endregion
				/*
				#region Edit State Info
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = "javascript:ShowWizard('EditStateInfo.aspx?ProjectId=" + ProjectId + "', 350, 250);";
				subItem.Text = LocRM.GetString("EditStateInfo");
				editItem.Items.Add(subItem);
				#endregion

*/
				topMenuItem.Items.Add(editItem);
			}
			#endregion

			#region Modyfy Recipients
			if (!isExternal && Mediachase.IBN.Business.Incident.CanUpdateExternalRecipients(IncidentId))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/editgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_HDM_RecipEdit");
				cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				//subItem.ClientSideCommand = "javascript:ShowWizard('RecipientsEditor.aspx?IncidentId=" + IncidentId + "', 450, 350);";
				subItem.Text = LocRM.GetString("AddRecipients");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Modyfy Resources
			if (canAddResBox && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/editgroup.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_HDM_ResEdit");
				cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				//subItem.ClientSideCommand = "javascript:ShowWizard('ResourcesEditor.aspx?IncidentId=" + IncidentId + "', 650, 350);";
				subItem.Text = LocRM.GetString("AddResources");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Delete
			if (canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/incident_delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:DeleteIncident()";
				subItem.Text = LocRM.GetString("Delete");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region MarkAsSpam
			if (canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/red_denied.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_HDM_MarkAsSpam");
				cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				subItem.Text = GetGlobalResourceObject("IbnFramework.Incident", "MarkAsSpam").ToString();
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Seperator ---
			if (topMenuItem.Items.Count > 0)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Copy to Clipboard
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/xp-copy.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:try{_XMLReqForClip('AddClip=Issue&IncidentId=" + IncidentId.ToString() + "', '" + LocRM2.GetString("tXMLError") + "')}catch(e){}";
				subItem.Text = LocRM.GetString("tCopyToClipboard");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Add from Clipboard
			if (canUpdate && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/xp-paste.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_HDM_RelatedIssClip");
				cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				subItem.Text = LocRM.GetString("tPasteFromClipboard");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Clear Clipboard
			if (canUpdate && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/clearbuffer.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:try{_XMLReqForClip('ClearClip=Issue', '" + LocRM2.GetString("tXMLError") + "')}catch(e){}";
				subItem.Text = LocRM2.GetString("tClearClipboard");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Seperator ---
			if (topMenuItem.Items.Count > 0)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Add Related Issue
			if (canUpdate && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/relincidents.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);

				cp = new CommandParameters("MC_HDM_RelatedIss");
				cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				//subItem.ClientSideCommand = String.Format("javascript:OpenPopUpNoScrollWindow(\"../Common/SelectIncident.aspx?btn={0}&exclude={1}\", 640, 480);",
				//    Page.ClientScript.GetPostBackEventReference(btnAddRelatedIss, "xxxtypeid;xxxid"), IncidentId.ToString());
				subItem.Text = LocRM.GetString("tAdd");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Timesheet
			// OR [2007-08-23]: We should use IbnNext TimeTracking
			/*
			if (Configuration.ProjectManagementEnabled)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icons/timesheet.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				string sPath = (Security.CurrentUser.IsExternal) ? "../External/ExternalTimeTracking.aspx" : "../TimeTracking/TimeTrackingWeek.aspx";
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?IncidentId={1}', {2});",
				  sPath, IncidentId, (Security.CurrentUser.IsExternal) ? "800,600" : "450, 200");
				subItem.Text = LocRM.GetString("tbAddTimeSheet");
				topMenuItem.Items.Add(subItem);
			}
			 */ 
			#endregion

			#region UpdateHistory
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/SystemEventsByObject.aspx?ObjectId={0}&ObjectTypeId={1}', 750, 466);", IncidentId, (int)ObjectTypes.Issue);
				subItem.Text = LocRM3.GetString("UpdateHistory");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Latest Visitors
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Common/LatestVisitors.aspx?ObjectId={0}&ObjectTypeId={1}', 450, 266);", IncidentId, (int)ObjectTypes.Issue);
				subItem.Text = LocRM3.GetString("LatestVisitors");
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
				subItem.NavigateUrl = String.Format("../Directory/SystemNotificationForObject.aspx?ObjectId={0}&ObjectTypeId={1}", IncidentId, ((int)ObjectTypes.Issue).ToString());
				subItem.Text = LocRM2.GetString("SystemNotifications");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Favorites
			if (!Incident.CheckFavorites(IncidentId) && !isExternal)
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

			#region PrintPreviewSettings
			if (!isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/printPreviewButton.png";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_HDM_PrintPreviewSettings");
				cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("IncidentId", IncidentId.ToString());
				cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				subItem.Text = LocRM.GetString("PrintSettings");
				topMenuItem.Items.Add(subItem);

				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/print.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				cp = new CommandParameters("MC_HDM_PrintIssue");
				cp.CommandArguments = new System.Collections.Generic.Dictionary<string, string>();
				cp.AddCommandArgument("IncidentId", IncidentId.ToString());
				cmd = cm.AddCommand("Incident", "", "IncidentView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				subItem.ClientSideCommand = "javascript:" + cmd;
				subItem.Text = LocRM.GetString("PrintIssue");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			if (topMenuItem.Items.Count > 0)
				secHeader.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{
			if (Tab != null && (Tab == "Finance" || Tab == "General" || Tab == "Forum"))
				pc["IncidentView_CurrentTab"] = Tab;
			else if (ViewState["CurrentTab"] != null)
				pc["IncidentView_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["IncidentView_CurrentTab"] == null)
				pc["IncidentView_CurrentTab"] = "Forum";

			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentView", typeof(IncidentView1).Assembly);

			//int MessCount = Incident.GetForumThreadNodes(IncidentId).Length;
			//			ctrlTopTab.AddTab(
			//				String.Format("{0} <span class='ibn-number'>({1})</span>", LocRM.GetString("tabForum"), MessCount),
			//				"Forum");
			ctrlTopTab.AddTab(LocRM.GetString("tabForum"), "Forum");
			ctrlTopTab.AddTab(LocRM.GetString("tabGeneral"), "General");
			//			ctrlTopTab.AddTab(LocRM.GetString("tbDetails"),"Details");
			//			ctrlTopTab.AddTab(LocRM.GetString("tabLibrary"),"FileLibrary");
			if (Incident.CanViewFinances(IncidentId) && !Security.CurrentUser.IsExternal)
				ctrlTopTab.AddTab(LocRM.GetString("tabFinance"), "Finance");
			else if (pc["IncidentView_CurrentTab"] == "Finance")
				pc["IncidentView_CurrentTab"] = "Forum";

			//			ctrlTopTab.AddTab(LocRM.GetString("tabDiscussions"),"Discussions");

			ctrlTopTab.SelectItem(pc["IncidentView_CurrentTab"]);


			string controlName = "Forum.ascx";
			switch (pc["IncidentView_CurrentTab"])
			{
				case "Forum":
					controlName = "Forum.ascx";
					break;
				case "General":
					controlName = "IncidentGeneral.ascx";
					break;
				case "Finance":
					controlName = "Finance.ascx";
					break;
				/*			
								case "Details":
									controlName = "IncidentDetails.ascx";
									break;	
								case "FileLibrary":
									controlName = "FileLibrary.ascx";
									break;
								case "Discussions":
									controlName = "Discussions.ascx";
									break;*/
				default:
					break;
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}
		#endregion

		#region btnAddToFavorites_Click
		private void btnAddToFavorites_Click(object sender, EventArgs e)
		{
			Incident.AddFavorites(IncidentId);
			Util.CommonHelper.ReloadTopFrame("Favorites.ascx", "../Incidents/IncidentView.aspx?IncidentId=" + IncidentId, Response);
		}
		#endregion

		#region Add Related
		protected void btnAddRelatedIss_Click(object sender, EventArgs e)
		{
			string param = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(param))
			{
				string[] mas = param.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
				if (mas.Length < 2 || !mas[0].Equals("7"))
					return;
				int iRelId = int.Parse(mas[1]);
				Issue2.AddRelation(IncidentId, iRelId);
				Response.Redirect("~/Incidents/IncidentView.aspx?IncidentId=" + IncidentId + "&Tab=General", true);
			}
		}
		#endregion

		#region lbDeleteIncidentAll_Click
		protected void lbDeleteIncidentAll_Click(object sender, System.EventArgs e)
		{
			string link = GetLink();

			Incident.Delete(IncidentId);

			Response.Redirect(link, true);
		}
		#endregion

		#region GetLink
		private string GetLink()
		{
			int ProjectId = -1;
			//			int StateId = (int)ObjectStates.Upcoming;
			using (IDataReader reader = Incident.GetIncident(IncidentId, false))
			{
				///  IncidentId, ProjectId, ProjectTitle, CreatorId, 
				///  Title, Description, Resolution, Workaround, CreationDate, 
				///  TypeId, TypeName, PriorityId, PriorityName, 
				///  SeverityId, SeverityName, IsEmail, MailSenderEmail, StateId
				if (reader.Read())
				{
					if (reader["ProjectId"] != DBNull.Value)
						ProjectId = (int)reader["ProjectId"];
					//					StateId = (int)reader["StateId"];
				}
			}

			string link;
			if (ProjectId > 0)
				link = String.Format("~/Projects/ProjectView.aspx?ProjectId={0}&Tab=5", ProjectId);
			else
			{
				string backlink = ResolveClientUrl("~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx");
				link = backlink;
			}
			return link;
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
