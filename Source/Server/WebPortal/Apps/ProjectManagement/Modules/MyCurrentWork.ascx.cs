using System;
using System.Collections.Generic;
using System.Data;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.UI.Web.Util;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class MyCurrentWork : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(MyCurrentWork).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindData();
		}

		#region BindData
		private void BindData()
		{
			int userId = Mediachase.IBN.Business.Security.CurrentUser.UserID;
			ResourceWorkCtrl.Visible = true;
			ResourceWorkCtrl.UserId = userId;
			ResourceWorkCtrl.BindData(true);

			GraphControlMain.Visible = true;
			GraphControlMain.Users = userId.ToString();
			GraphControlMain.IntervalDuration = 7;	// one week
			GraphControlMain.StartDate = Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date;
			GraphControlMain.CurDate = Mediachase.IBN.Business.Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
			GraphControlMain.ObjectTypes = string.Concat(
				((int)ObjectTypes.CalendarEntry).ToString(), ",",
				((int)ObjectTypes.ToDo).ToString(), ",",
				((int)ObjectTypes.Task).ToString(), ",",
				((int)ObjectTypes.Document).ToString(), ",",
				((int)ObjectTypes.Issue).ToString());
			GraphControlMain.DataBind();
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			secHeader.AddLink(GetGlobalResourceObject("IbnFramework.Calendar", "tLegend").ToString(), "javascript:ShowLegend()");
		}
		#endregion
	}
}