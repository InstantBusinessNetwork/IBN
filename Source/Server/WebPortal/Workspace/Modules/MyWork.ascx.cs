using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Resources;
using System.Reflection;

using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Workspace.Modules
{
	public partial class MyWork : System.Web.UI.UserControl
	{
		private const string keyPage = "MyWork_page";
		private const int pageSize = 10;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strListView", Assembly.GetExecutingAssembly());

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ViewState[keyPage] == null)
				ViewState[keyPage] = 0;

			if (!IsPostBack)
			{
				if (Mediachase.IBN.Business.Calendar.CheckUserCalendar(Security.CurrentUser.UserID))
					BindGrid();
				else
					MainGrid.Visible = false;
			}
		}

		#region BindGrid
		private void BindGrid()
		{
			MainGrid.Columns[1].HeaderText = LocRM.GetString("Title");
			MainGrid.Columns[2].HeaderText = "%";
			MainGrid.Columns[3].HeaderText = LocRM.GetString("EndDate");

			ArrayList objectTypes = new ArrayList();
			objectTypes.Add(ObjectTypes.Task);
			objectTypes.Add(ObjectTypes.ToDo);
			objectTypes.Add(ObjectTypes.Issue);
			objectTypes.Add(ObjectTypes.Document);
//			objectTypes.Add(ObjectTypes.CalendarEntry);

			DateTime userNow = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
			DataTable dt = Mediachase.IBN.Business.Calendar.GetResourceUtilization(
				Security.CurrentUser.UserID,
				userNow,
				userNow,
				userNow.AddDays(14),
				objectTypes,
				null,
				false,
				false,
				true,
				false);

			DataView dv = dt.DefaultView;
			dv.Sort = "Start";

			if (pageSize >= dv.Count)
				MainGrid.PagerStyle.Visible = false;
			else
				MainGrid.PagerStyle.Visible = true;
			MainGrid.PageSize = pageSize;

			int curPage = (int)ViewState[keyPage];
			int maxPage = (dv.Count - 1) / MainGrid.PageSize + 1;
			if (curPage <= maxPage)
				MainGrid.CurrentPageIndex = curPage;
			else
				MainGrid.CurrentPageIndex = 0;

			MainGrid.DataSource = dv;
			MainGrid.DataBind();

		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (MainGrid.Visible)
			{
				NoCalendarLabel.Visible = false;
				if (MainGrid.Items.Count == 0)
				{
					MainGrid.Visible = false;
					NoItemsLabel.Text = String.Format(CultureInfo.InvariantCulture,
						"<div style='padding:7px;'>{0}</div>",
						LocRM.GetString("tNoItems"));
					NoItemsLabel.Visible = true;
				}
				else
				{
					MainGrid.Visible = true;
					NoItemsLabel.Visible = false;
				}
			}
			else // no user calendar
			{
				string formatString = String.Format(CultureInfo.InvariantCulture,
					"<div style='padding:7px'>{0}</div>",
					GetGlobalResourceObject("IbnFramework.Calendar", "NoUserCalendarWithLinkGap").ToString());
				string link = String.Format(CultureInfo.InvariantCulture,
					"{0}?UserID={1}&Tab=5",
					ResolveClientUrl("~/Directory/UserView.aspx"),
					Security.CurrentUser.UserID);

				NoCalendarLabel.Text = String.Format(CultureInfo.InvariantCulture,
					formatString,
					link);

				NoCalendarLabel.Visible = true;
			}
		}
		#endregion

		#region MainGrid_PageIndexChanged
		protected void MainGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			ViewState[keyPage] = e.NewPageIndex;
			BindGrid();
		}
		#endregion
	}
}