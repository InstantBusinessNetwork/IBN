using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Modules;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Workspace.Modules
{
	public partial class OutActivities : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strListView", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		private const string prefix = "Out";
		private const string controlId = "25";
		private const string keyPage = prefix + "Activities_page";
		private const string keySort = prefix + "Activities_sort";
		private const string keyShowActive = prefix + "Activities_showActive";
		private const string keyType = prefix + "Activities_type";
		private const string keyFrom = prefix + "Activities_from";
		private const string keyTo = prefix + "Activities_to";
		private const int pageSize = 10;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ViewState[keyPage] == null)
				ViewState[keyPage] = 0;

			BindGrid();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();

			if (gridActivities.Items.Count == 0)
			{
				gridActivities.Visible = false;
				labelNoItems.Text = String.Format(CultureInfo.InvariantCulture,
					"<div style='padding:7'>{0}</div>",
					LocRM.GetString("tNoItems"));
				labelNoItems.Visible = true;
			}
			else
			{
				gridActivities.Visible = true;
				labelNoItems.Visible = false;
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString(prefix + "Activities");

			string text = string.Format("<img src='{0}' border='0' width=16 height=16 align='absmiddle' >", this.ResolveUrl("~/Layouts/Images/b4.gif"));
			string url = String.Format(CultureInfo.InvariantCulture, "javascript:{0}", Page.ClientScript.GetPostBackEventReference(buttonHide, ""));

			secHeader.AddLink(text, url);
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			gridActivities.Columns[0].HeaderText = LocRM.GetString("Title");
			gridActivities.Columns[1].HeaderText = LocRM.GetString("Resources");
			//			gridActivities.Columns[2].HeaderText = LocRM.GetString("Type");
			gridActivities.Columns[2].HeaderText = LocRM.GetString("EndDate");

			if (pc[keyShowActive] == null)
				pc[keyShowActive] = Boolean.TrueString;

			if (pc[keyType] == null)
				pc[keyType] = "7";	// week

			if (pc[keyType] == "0" && (pc[keyFrom] == null || pc[keyType] == null))
				pc[keyType] = "7";	// week

			DateTime fromDate = DateTime.Now;
			DateTime toDate = DateTime.Now;

			if (pc[keyType] == "0")	// period
			{
				fromDate = DateTime.Parse(pc[keyFrom], CultureInfo.InvariantCulture);
				toDate = DateTime.Parse(pc[keyTo], CultureInfo.InvariantCulture);
			}
			else
			{
				int days = int.Parse(pc[keyType], CultureInfo.InvariantCulture);
				fromDate = fromDate.AddDays(-days);
			}

			DataTable dt = Mediachase.IBN.Business.ToDo.GetListToDoAndTasksAssignedByUserDataTable(bool.Parse(pc[keyShowActive]), fromDate, toDate);
			DataView dv = dt.DefaultView;

			if (pc[keySort] == null)
				pc[keySort] = "FinishDate DESC";
			dv.Sort = pc[keySort];

			if (pageSize >= dv.Count)
				gridActivities.PagerStyle.Visible = false;
			else
				gridActivities.PagerStyle.Visible = true;
			gridActivities.PageSize = pageSize;

			int iPageIndex = (int)ViewState[keyPage];
			int ppi = dv.Count / gridActivities.PageSize;
			if (dv.Count % gridActivities.PageSize == 0)
				ppi = ppi - 1;
			if (iPageIndex <= ppi)
				gridActivities.CurrentPageIndex = iPageIndex;
			else
				gridActivities.CurrentPageIndex = 0;

			gridActivities.DataSource = dv;
			gridActivities.DataBind();

			gridActivities.Style.Add("table-layout", "fixed");

			BindLegend();
		}
		#endregion

		#region BindLegend
		private void BindLegend()
		{
			imageFilter.ToolTip = LocRM.GetString("Filter");

			if (pc[keyShowActive] == Boolean.TrueString)
				labelLegend.Text = LocRM.GetString("ShowActive");
			else
				labelLegend.Text = LocRM.GetString("ShowAll");

			labelLegend.Text += ", ";

			if (pc[keyType] == "7")
			{
				labelLegend.Text += LocRM.GetString("ShowLastWeek");
			}
			else if (pc[keyType] == "30")
			{
				labelLegend.Text += LocRM.GetString("ShowLastMonth");
			}
			else if (pc[keyType] == "365")
			{
				labelLegend.Text += LocRM.GetString("ShowLastYear");
			}
			else
			{
				labelLegend.Text += String.Format(CultureInfo.InvariantCulture,
					"{0} {1} {2} {3}",
					LocRM.GetString("ShowFrom"),
					DateTime.Parse(pc[keyFrom], CultureInfo.InvariantCulture).ToShortDateString(),
					LocRM.GetString("ShowTo"),
					DateTime.Parse(pc[keyTo], CultureInfo.InvariantCulture).ToShortDateString());
			}

			labelLegend.ToolTip = LocRM.GetString("Filter");
		}
		#endregion

		#region GridActivities_PageIndexChanged
		protected void GridActivities_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			ViewState[keyPage] = e.NewPageIndex;
			BindGrid();
		}
		#endregion

		#region GridActivities_SortCommand
		protected void GridActivities_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			if (pc[keySort] == e.SortExpression)
				pc[keySort] += " DESC";
			else
				pc[keySort] = e.SortExpression;

			ViewState[keyPage] = 0;
			BindGrid();
		}
		#endregion

		#region ButtonHide_Click
		protected void ButtonHide_Click(object sender, EventArgs e)
		{
			//Util.CommonHelper.HideWorkspaceControl(controlId);
			Response.Redirect("../Workspace/default.aspx?BTab=Workspace");
		}
		#endregion

		#region ButtonRefresh_Click
		protected void ButtonRefresh_Click(object sender, EventArgs e)
		{
			BindGrid();
		}
		#endregion

		#region GetResources
		protected static string GetResources(int completionType, int isToDo, int itemId, int overallPercentCompleted)
		{
			StringBuilder sb = new StringBuilder();

			int count = 0;
			using (IDataReader reader = ((isToDo == 0) ? Task.GetListResources(itemId) : Mediachase.IBN.Business.ToDo.GetListResources(itemId)))
			{
				while (reader.Read())
				{
					string percents = String.Empty;
					if (completionType == (int)CompletionType.All)
						percents = String.Format(CultureInfo.InvariantCulture, "<td style='padding-left:5px; width:45px'>{0}%</td>", reader["PercentCompleted"].ToString());
					else
						percents = String.Format(CultureInfo.InvariantCulture, "<td style='padding-left:5px; width:45px'>{0}%</td>", overallPercentCompleted.ToString());

					if(count == 0)
						sb.AppendLine("<table class='text' cellspacing='0' cellpadding='1' border='0' width='100%'>");

					sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), percents));

					count++;
				}
			}

			if (count > 0)
				sb.AppendLine("</table>");

			return sb.ToString();
		}
		#endregion
	}
}