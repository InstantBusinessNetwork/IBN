using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Database.WebDAV;
using Mediachase.IBN.Business.WebDAV;
using Mediachase.Net.Wdom;
using System.Reflection;
using System.Resources;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class WebDavSessionsOverview : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			BindToolbar();

			if (!Page.IsPostBack)
				BindDataGrid();
			
		}

		private void BindToolbar()
		{
		}

		private void BindDataGrid()
		{
			dgWebDavSessions.DataSource = WebDavSessionManager.GetActiveLocksInfo();
			dgWebDavSessions.DataBind();
		}

		protected  string GetNormalizedDateTime(DateTime dt)
		{
			return Security.CurrentUser.CurrentTimeZone.ToLocalTime(dt).ToString();
		}
		protected  string GetNormalizedDuration(TimeSpan duration)
		{
			DateTime retVal = new DateTime(duration.Ticks, DateTimeKind.Utc);
			return retVal.ToString("HH:MM:ss");
		}

		protected void dgWebDavSessions_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "unlock")
			{
				int propertyId = Convert.ToInt32(e.CommandArgument);
				WebDavSessionManager.UnlockResource(propertyId);
				BindDataGrid();
			}
		}
		
	}
}