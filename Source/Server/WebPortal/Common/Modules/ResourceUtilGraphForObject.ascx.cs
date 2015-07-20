using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Common.Modules
{
	public partial class ResourceUtilGraphForObject : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourceUtilGraphForObject).Assembly);
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		#region Users
		protected string Users
		{
			get
			{
				string retval = String.Empty;
				if (!String.IsNullOrEmpty(Request["users"]))
					retval = Request["users"];
				return retval;
			}
		}
		#endregion

		#region ObjectId
		protected string ObjectId
		{
			get
			{
				string retval = String.Empty;
				if (!String.IsNullOrEmpty(Request["ObjectId"]))
					retval = Request["ObjectId"];
				return retval;
			}
		}
		#endregion

		#region ObjectTypeId
		protected string ObjectTypeId
		{
			get
			{
				string retval = String.Empty;
				if (!String.IsNullOrEmpty(Request["ObjectTypeId"]))
					retval = Request["ObjectTypeId"];
				return retval;
			}
		}
		#endregion

		#region StartDate
		protected DateTime StartDate
		{
			get
			{
				if (ViewState["StartDate"] == null)
					ViewState["StartDate"] = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date;

				return (DateTime)ViewState["StartDate"];
			}
			set
			{
				ViewState["StartDate"] = value;
			}
		}
		#endregion

		#region CurDate
		protected DateTime CurDate
		{
			get
			{
				if (ViewState["CurDate"] == null)
					ViewState["CurDate"] = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);

				return (DateTime)ViewState["CurDate"];
			}
			set
			{
				ViewState["CurDate"] = value;
			}
		}
		#endregion

		#region IntervalDuration (days)
		protected int IntervalDuration
		{
			get
			{
				return int.Parse(_pc["MV_Weeks"], CultureInfo.InvariantCulture) * 7;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_pc["MV_Weeks"] == null)
				_pc["MV_Weeks"] = "1";

			if (!Page.IsPostBack)
			{
				BindData();
			}
		}

		#region ResetCurDate
		private void ResetCurDate()
		{
			CurDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
		}
		#endregion

		#region ResetStartDate
		private void ResetStartDate()
		{
			StartDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date;
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tResUtiliz");
			RenderMenu(secHeader.ActionsMenu);
		}

		private void RenderMenu(ComponentArt.Web.UI.Menu actionsMenu)
		{
			ComponentArt.Web.UI.MenuItem topMenuItem;

			actionsMenu.Items.Clear();

			#region Legend Item
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = GetGlobalResourceObject("IbnFramework.Calendar", "tLegend").ToString();
			//			topMenuItem.Look.LeftIconUrl = ResolveClientUrl("~/Layouts/Images/downbtn1.gif");
			//			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			//			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";
			topMenuItem.ClientSideCommand = "ShowLegend()";
			actionsMenu.Items.Add(topMenuItem);
			#endregion

			#region View Menu Items
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("tView");
			topMenuItem.Look.LeftIconUrl = ResolveClientUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;
			string graphPeriod = _pc["MV_Weeks"];

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (graphPeriod == "1")	// 1 week
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(ViewButton, "1");
			subItem.Text = LocRM.GetString("Week1");
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (graphPeriod == "3")	// 3 week
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(ViewButton, "3");
			subItem.Text = LocRM.GetString("Week3");
			topMenuItem.Items.Add(subItem);

			actionsMenu.Items.Add(topMenuItem);
			#endregion
		}
		#endregion

		#region ViewButton_Command
		protected void ViewButton_Command(object sender, CommandEventArgs e)
		{
			if (Request["__EVENTARGUMENT"] != null)
				_pc["MV_Weeks"] = Request["__EVENTARGUMENT"];

			ResetStartDate();
			ResetCurDate();

			BindData();
		}
		#endregion

		#region BindData
		private void BindData()
		{
			string objectTypes = String.Format(CultureInfo.InvariantCulture,
				"{0},{1},{2},{3},{4}",
				(int)ObjectTypes.CalendarEntry,
				(int)ObjectTypes.Task,
				(int)ObjectTypes.ToDo,
				(int)ObjectTypes.Document,
				(int)ObjectTypes.Issue);

			GraphControlMain.Users = Users;
			GraphControlMain.IntervalDuration = IntervalDuration;
			GraphControlMain.StartDate = StartDate;
			GraphControlMain.CurDate = CurDate;
			GraphControlMain.ObjectTypes = objectTypes;
			GraphControlMain.HObjects = ObjectId;
			GraphControlMain.HTypes = ObjectTypeId;
			GraphControlMain.LegendTop = 0;
			GraphControlMain.UsersAsLinks = false;
			GraphControlMain.DataBind();
		}
		#endregion
	}
}