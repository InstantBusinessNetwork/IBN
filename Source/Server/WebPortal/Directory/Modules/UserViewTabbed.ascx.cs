namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using ComponentArt.Web.UI;

	/// <summary>
	///		Summary description for UserViewTabbed.
	/// </summary>
	public partial class UserViewTabbed : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strPageTitles", typeof(UserViewTabbed).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(UserViewTabbed).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserView", typeof(UserViewTabbed).Assembly);
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

		#region UserID
		private int UserID
		{
			get
			{
				try
				{
					if (Request["UserID"] != null)
						return int.Parse(Request["UserID"]);
					else
					{
						if (Request["AccountID"] != null)
						{
							int iID = 0;
							using (IDataReader reader = User.GetUserInfoByOriginalId(int.Parse(Request["AccountID"])))
							{
								if (reader.Read())
									iID = (int)reader["UserId"];
							}
							if (iID > 0)
								return iID;
							else
								return Security.CurrentUser.UserID;
						}
						else
							return Security.CurrentUser.UserID;
					}
				}
				catch
				{
					throw new Exception("Invalid User ID");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			BindTabs();
			if (!IsPostBack)
			{
				ViewState["CurrentTab"] = pc["UserView_CurrentTab"];
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

		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tUserView");

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			bool isExternal = false;
			bool isAlertService = false;
			using (IDataReader reader = User.GetUserInfo(UserID, false))
			{
				if (reader.Read())
				{
					isExternal = (bool)reader["IsExternal"];

					if (reader["login"].ToString().ToLower() == "alert")
						isAlertService = true;
				}
			}

			ComponentArt.Web.UI.MenuItem subItem;

			bool canUpdate = User.CanUpdateUserInfo(UserID);
			bool canConvert = isExternal && Security.IsUserInGroup(InternalSecureGroups.Administrator);
			bool canDelete = Security.IsUserInGroup(InternalSecureGroups.Administrator) && UserID != Security.CurrentUser.UserID && !isAlertService;

			#region Edit
			if (canUpdate)
			{
				string sURL = String.Format("~/Directory/UserEdit.aspx?Back=View&UserID={0}", UserID);
				if (isExternal)
					sURL = String.Format("~/Directory/ExternalEdit.aspx?Back=View&UserID={0}", UserID);

				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/edit.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = sURL;
				subItem.Text = LocRM3.GetString("tbEditEdit");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Convert
			if (canConvert)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/upload.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = "~/Directory/UserEdit.aspx?UserId=" + UserID;
				subItem.Text = LocRM3.GetString("tbConvert");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Delete
			if (canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/delete.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('UserDelete.aspx?UserId={0}',450,250)", UserID);
				subItem.Text = LocRM3.GetString("tDelete");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region --- Seperator ---
			if (canUpdate || canConvert || canDelete)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "BreakItem";
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Favorites
			if (!Common.CheckFavorites(UserID, ObjectTypes.User) && !isExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Favorites.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(AddToFavoritesButton, "");
				subItem.Text = LocRM2.GetString("AddToFavorites");
				topMenuItem.Items.Add(subItem);
			}
			#endregion

			#region Back
			subItem = new ComponentArt.Web.UI.MenuItem();
			subItem.Look.LeftIconUrl = "~/Layouts/Images/cancel.gif";
			subItem.Look.LeftIconWidth = Unit.Pixel(16);
			subItem.Look.LeftIconHeight = Unit.Pixel(16);
			subItem.NavigateUrl = "~/Directory/Directory.aspx";
			subItem.Text = LocRM3.GetString("tbBack");
			topMenuItem.Items.Add(subItem);
			#endregion

			secHeader.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{
			if (Tab != null && (Tab == "1" || Tab == "2" || Tab == "WSCustomize" || Tab == "3" || Tab == "4" || Tab == "5"))
				pc["UserView_CurrentTab"] = Tab;
			else if (ViewState["CurrentTab"] != null)
				pc["UserView_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["UserView_CurrentTab"] == null)
				pc["UserView_CurrentTab"] = "1";

			if (UserID != Security.CurrentUser.UserID && (pc["UserView_CurrentTab"] == "WSCustomize" || pc["UserView_CurrentTab"] == "3" || pc["UserView_CurrentTab"] == "4"))
				pc["UserView_CurrentTab"] = "1";

			ctrlTopTab.AddTab(LocRM3.GetString("tabGeneral"), "1");
			ctrlTopTab.AddTab(LocRM3.GetString("tabMetaData"), "2");

			if (UserID == Security.CurrentUser.UserID
				|| Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.TimeManager)
				|| Security.IsUserInGroup(InternalSecureGroups.Administrator))
			{
				ctrlTopTab.AddTab(LocRM3.GetString("tabUserCalendar"), "5");
			}
			else if (pc["UserView_CurrentTab"] == "5")
			{
				pc["UserView_CurrentTab"] = "1";
			}
			
			if (UserID == Security.CurrentUser.UserID)
			{
				//ctrlTopTab.AddTab(LocRM.GetString("tabWSCustomize"),"WSCustomize");
				ctrlTopTab.AddTab(LocRM3.GetString("tabSystemNotifications"), "3");
				ctrlTopTab.AddTab(LocRM3.GetString("tabSystemReminders"), "4");
			}

			ctrlTopTab.SelectItem(pc["UserView_CurrentTab"]);

			if (pc["UserView_CurrentTab"] == "WSCustomize")
			{
				pc["UserView_CurrentTab"] = "2";
			}

			string controlName = "UserView.ascx";
			switch (pc["UserView_CurrentTab"])
			{
				case "2":
					controlName = "MetaDataView.ascx";
					break;
				case "3":
					controlName = "SystemNotification.ascx";
					((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM3.GetString("tabSystemNotifications");
					break;
				case "4":
					controlName = "SystemReminders.ascx";
					((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM3.GetString("tabSystemReminders");
					break;
				case "5":
					controlName = "UserCalendar.ascx";
					((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM3.GetString("tabUserCalendar");
					break;
				default:
					controlName = "UserView.ascx";
					break;
			}

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}
		#endregion

		#region AddToFavoritesButton_Click
		protected void AddToFavoritesButton_Click(object sender, EventArgs e)
		{
			Common.AddFavorites(UserID, ObjectTypes.User);
//			Util.CommonHelper.ReloadTopFrame("Favorites.ascx", String.Concat("../Directory/UserView.aspx?UserID=", UserID), Response);
		}
		#endregion
	}
}
