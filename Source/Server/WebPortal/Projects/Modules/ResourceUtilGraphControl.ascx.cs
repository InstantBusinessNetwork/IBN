using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class ResourceUtilGraphControl : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourceUtilGraphControl).Assembly);
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		#region ImageHeight
		protected int ImageHeight
		{
			get
			{
				return (int)ViewState["ImageHeight"];
			}
			set
			{
				ViewState["ImageHeight"] = value;
			}
		}
		#endregion

		#region Users
		public string Users
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["Users"] != null)
					retval = ViewState["Users"].ToString();
				return retval;
			}
			set
			{
				ViewState["Users"] = value;
			}
		}
		#endregion

		#region ObjectTypes
		public string ObjectTypes
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["ObjectTypes"] != null)
					retval = ViewState["ObjectTypes"].ToString();
				return retval;
			}
			set
			{
				ViewState["ObjectTypes"] = value;
			}
		}
		#endregion

		#region HObjects
		public string HObjects
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["HObjects"] != null)
					retval = ViewState["HObjects"].ToString();
				return retval;
			}
			set
			{
				ViewState["HObjects"] = value;
			}
		}
		#endregion

		#region HTypes
		public string HTypes
		{
			get
			{
				string retval = String.Empty;
				if (ViewState["HTypes"] != null)
					retval = ViewState["HTypes"].ToString();
				return retval;
			}
			set
			{
				ViewState["HTypes"] = value;
			}
		}
		#endregion

		#region IntervalDuration (days)
		public int IntervalDuration
		{
			get
			{
				int retval = 1;
				if (ViewState["IntervalDuration"] != null)
					retval = (int)ViewState["IntervalDuration"];
				return retval;
			}
			set
			{
				ViewState["IntervalDuration"] = value;
			}
		}
		#endregion

		#region StartDate
		public DateTime StartDate
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
		public DateTime CurDate
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

		#region LegendTop
		public int LegendTop
		{
			get
			{
				int retval = 27;
				if (ViewState["LegendTop"] != null)
					retval = (int)ViewState["LegendTop"];
				return retval;
			}
			set
			{
				ViewState["LegendTop"] = value;
			}
		}
		#endregion

		#region UsersAsLinks
		public bool UsersAsLinks
		{
			get
			{
				bool retval = true;
				if (ViewState["UsersAsLinks"] != null)
					retval = (bool)ViewState["UsersAsLinks"];
				return retval;
			}
			set
			{
				ViewState["UsersAsLinks"] = value;
			}
		}
		#endregion

		#region ShowUsers
		public bool ShowUsers
		{
			get
			{
				bool retval = true;
				if (ViewState["ShowUsers"] != null)
					retval = (bool)ViewState["ShowUsers"];
				return retval;
			}
			set
			{
				ViewState["ShowUsers"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Gantt.css");
			UtilHelper.RegisterScript(Page, "~/Scripts/resUtilGraph.js");

			BindLegend();
		}

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			int lx = 0;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "f_onload(" + lx + ", " + ResourceChart.PortionWidth + ", " + ImageHeight + ");", true);

			divLegendMenu.Style.Add(HtmlTextWriterStyle.Top, LegendTop + "px");

			if (!ShowUsers)
			{
				UsersCell.Visible = false;
			}
		}
		#endregion

		#region BindLegend
		private void BindLegend()
		{
			tblLegend.Rows.Clear();

			string[] items = ResourceChart.GetLegendItems();
			foreach (string item in items)
			{
				TableRow tr = new TableRow();
				TableCell tc1 = new TableCell();
				TableCell tc2 = new TableCell();
				tc1.VerticalAlign = VerticalAlign.Top;
				tc2.VerticalAlign = VerticalAlign.Top;
				Image img = new Image();
				img.ImageUrl = String.Format("~/Projects/ResourceChartLegendImage.aspx?Type={0}",
					item);
				tc1.Controls.Add(img);
				Label lbl = new Label();
				lbl.Text = LocRM.GetString("TimeType" + item);
				tc2.Controls.Add(lbl);
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				tblLegend.Rows.Add(tr);
			}
		}
		#endregion

		#region DataBind
		public override void DataBind()
		{
			string[] users = Users.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

			if (ShowUsers)
			{
				DataTable userTable = new DataTable();
				userTable.Locale = CultureInfo.InvariantCulture;
				userTable.Columns.Add(new DataColumn("UserId", typeof(int)));
				userTable.Columns.Add(new DataColumn("Details", typeof(string)));

				foreach (string s in users)
				{
					int userId = int.Parse(s, CultureInfo.InvariantCulture);

					string details = String.Empty;
					if (!Mediachase.IBN.Business.Calendar.CheckUserCalendar(userId))
						details = LocRM.GetString("NoCalendar");

					DataRow dr = userTable.NewRow();
					dr["UserId"] = userId;
					dr["Details"] = details;
					userTable.Rows.Add(dr);
				}

				UsersGrid.DataSource = userTable.DefaultView;
				UsersGrid.DataBind();
			}

			ImageHeight = users.Length * ResourceChart.ItemHeight;// +ResourceChart.HeaderHeight;
			divImg.Style.Add(HtmlTextWriterStyle.Height, (ImageHeight + ResourceChart.HeaderHeight).ToString() + "px");

			LinkToGraph.Value = String.Format(CultureInfo.InvariantCulture,
				"{0}?Users={1}&Vast={2}&StartDate={3}&CurDate={4}&ObjectTypes={5}&HObjects={6}&HTypes={7}",
				ResolveClientUrl("~/Projects/ResourceChartImage.aspx"),
				Users,
				IntervalDuration == 7,
				StartDate.ToString("yyyy-MM-dd"),
				Server.UrlEncode(CurDate.ToString("yyyy-MM-dd HH:mm")),
				ObjectTypes,
				HObjects,
				HTypes);

			base.DataBind();
		}
		#endregion
	}
}
