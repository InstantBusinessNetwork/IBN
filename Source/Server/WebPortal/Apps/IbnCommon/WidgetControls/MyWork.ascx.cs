using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class MyWork : System.Web.UI.UserControl
	{
		private const string keyPage = "MyWork_page";
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strListView", Assembly.GetExecutingAssembly());

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");

			ctrlGrid.ChangingMCGridColumnHeader += new Mediachase.Ibn.Web.UI.ChangingMCGridColumnHeaderEventHandler(ctrlGrid_ChangingMCGridColumnHeader);
			ctrlGrid.InnerGrid.RowDataBound += new GridViewRowEventHandler(InnerGrid_RowDataBound);

			if (Mediachase.IBN.Business.Calendar.CheckUserCalendar(Security.CurrentUser.UserID))
			{
				BindGrid();

				string scriptKey = "MyWorkRefresh";
				if (!Page.ClientScript.IsClientScriptBlockRegistered(scriptKey))
				{
					StringBuilder builder = new StringBuilder(152);
					builder.Append("<script type=\"text/javascript\">\r\n");
					builder.Append("\tfunction MyWorkRefresh(params) {\r\n");
					builder.Append("\t\t");
					builder.Append(Page.ClientScript.GetPostBackEventReference(RefreshGridButton, ""));
					builder.Append("\r\n");
					builder.Append("\t}\r\n");
					builder.Append("</script>");

					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), scriptKey, builder.ToString());
				}

			}
			else
				ctrlGrid.Visible = false;
		}

		void ctrlGrid_ChangingMCGridColumnHeader(object sender, Mediachase.Ibn.Web.UI.ChangingMCGridColumnHeaderEventArgs e)
		{
			if (e.FieldName == "PriorityId")
			{
				e.ControlField.HeaderText = string.Format(CultureInfo.InvariantCulture, "<span title='{0}'>!!!</span>", GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString());
				//e.ControlField.HeaderText = String.Format("<img width='16' height='16' src='{0}' title='{1}'>",
				//    this.Page.ResolveClientUrl("~/layouts/images/PriorityHeader.gif"),
				//    GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString());
			}
		}

		#region BindGrid
		private void BindGrid()
		{
			//MainGrid.Columns[1].HeaderText = LocRM.GetString("Title");
			//MainGrid.Columns[2].HeaderText = "%";
			//MainGrid.Columns[3].HeaderText = LocRM.GetString("EndDate");

			ArrayList objectTypes = new ArrayList();
			objectTypes.Add(ObjectTypes.Task);
			objectTypes.Add(ObjectTypes.ToDo);
			objectTypes.Add(ObjectTypes.Issue);
			objectTypes.Add(ObjectTypes.Document);
			//			objectTypes.Add(ObjectTypes.CalendarEntry);

			DateTime userNow = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
			DataTable dtSource = Mediachase.IBN.Business.Calendar.GetResourceUtilization(
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

			// O.R. [2009-09-11] Remove Events and merge repeated objects
			DataTable dt = dtSource.Clone();
			int lastObjectId = -1;
			int lastObjectTypeId = (int)ObjectTypes.UNDEFINED;
			string lastAssignmentId = string.Empty;
			foreach (DataRow row in dtSource.Rows)
			{
				if ((int)row["ObjectTypeId"] != (int)ObjectTypes.CalendarEntry)
				{
					if (lastObjectId != (int)row["ObjectId"] || lastObjectTypeId != (int)row["ObjectTypeId"] || lastAssignmentId != row["AssignmentId"].ToString())
					{
						dt.ImportRow(row);
						lastObjectId = (int)row["ObjectId"];
						lastObjectTypeId = (int)row["ObjectTypeId"];
						lastAssignmentId = row["AssignmentId"].ToString();
					}
					else // merging
					{
						dt.Rows[dt.Rows.Count - 1]["Finish"] = row["Finish"];
					}
				}
			}
			//

			DataView dv = dt.DefaultView;

			//int curPage = (int)ViewState[keyPage];
			//int maxPage = (dv.Count - 1) / ctrlGrid.PageSize + 1;
			//if (curPage <= maxPage)
			//    ctrlGrid.PageIndex = curPage;
			//else
			//    ctrlGrid.PageIndex = 0;

			ctrlGrid.ClassName = "MyWork";
			ctrlGrid.PlaceName = "Workspace";

			ctrlGrid.DataSource = dv;
			//ctrlGrid.DataBind();
		}
		#endregion

		#region InnerGrid_RowDataBound
		void InnerGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow && ((System.Data.DataRowView)(e.Row.DataItem)).Row["Type"] != DBNull.Value
				&& ((int)((System.Data.DataRowView)(e.Row.DataItem)).Row["Type"]) == (int)Mediachase.IBN.Business.Calendar.TimeType.WorkPinnedUp)
				e.Row.CssClass = "sticked";
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (ctrlGrid.Visible)
			{
				NoCalendarLabel.Visible = false;
				if (ctrlGrid.Count == 0)
				{
					ctrlGrid.Visible = false;
					NoCalendarLabel.Text = String.Format(CultureInfo.InvariantCulture,
						"<div style='padding:7px;'>{0}</div>",
						LocRM.GetString("tNoItems"));
					NoCalendarLabel.Visible = true;
				}
				else
				{
					ctrlGrid.Visible = true;
					NoCalendarLabel.Visible = false;
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
				ctrlGrid.Visible = false;
			}
		}
		#endregion

		#region RefreshGridButton_Click
		protected void RefreshGridButton_Click(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion
	}
}