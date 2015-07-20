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
using Mediachase.IBN.Business;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists
{
	public partial class ListTimeTrackingWeek : System.Web.UI.UserControl
	{
		private const string _className = "TimeTrackingWeek";
		private const string _viewName = "";
		private const string _placeName = "";

		protected UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			grdMain.ChangingMCGridColumnHeader += new ChangingMCGridColumnHeaderEventHandler(grdMain_ChangingMCGridColumnHeader);

			//инициализируем грид
			grdMain.ClassName = _className;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;

			//инициализируем тулбар
			MainMetaToolbar.ClassName = _className;
			MainMetaToolbar.ViewName = _viewName;
			MainMetaToolbar.PlaceName = _placeName;

			if (!Page.IsPostBack)
				BindBlockHeader();

			BindDataGrid(!Page.IsPostBack);

		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			if (CHelper.NeedToBindGrid())
			{
				BindDataGrid(true);
				grdMainPanel.Update();
			}

			base.OnPreRender(e);
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.TimeTracking", "_mc_TimeTrackingWeek").ToString();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			WeekItemInfo[] mas = Mediachase.IBN.Business.TimeTracking.GetWeekItemsForCurrentUser(
				Mediachase.IBN.Business.TimeTracking.GetTimeTrackingBlockMinStartDate(), 
				DateTime.Now);
			
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("uid", typeof(string)));
			dt.Columns.Add(new DataColumn("WeekNumber", typeof(string)));
			dt.Columns.Add(new DataColumn("WeekNumberSort", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("Week", typeof(string)));
			dt.Columns.Add(new DataColumn("Status", typeof(string)));
			dt.Columns.Add(new DataColumn("Day1", typeof(string)));
			dt.Columns.Add(new DataColumn("Day2", typeof(string)));
			dt.Columns.Add(new DataColumn("Day3", typeof(string)));
			dt.Columns.Add(new DataColumn("Day4", typeof(string)));
			dt.Columns.Add(new DataColumn("Day5", typeof(string)));
			dt.Columns.Add(new DataColumn("Day6", typeof(string)));
			dt.Columns.Add(new DataColumn("Day7", typeof(string)));
			dt.Columns.Add(new DataColumn("DayT", typeof(string)));
			DataRow dr;
			foreach (WeekItemInfo wii in mas)
			{
				dr = dt.NewRow();
				dr["uid"] = wii.StartDate.ToString(CultureInfo.InvariantCulture);
				dr["WeekNumber"] = wii.WeekNumber.ToString();
				dr["WeekNumberSort"] = wii.StartDate;
				dr["Week"] = String.Format("{0} - {1}", wii.StartDate.ToShortDateString(), wii.StartDate.AddDays(6).ToShortDateString());
				dr["Status"] = wii.Status.ToString();
				dr["Day1"] = String.Format("{0:D2}:{1:D2}", Convert.ToInt32(wii.Day1) / 60, Convert.ToInt32(wii.Day1) % 60);
				dr["Day2"] = String.Format("{0:D2}:{1:D2}", Convert.ToInt32(wii.Day2) / 60, Convert.ToInt32(wii.Day2) % 60);
				dr["Day3"] = String.Format("{0:D2}:{1:D2}", Convert.ToInt32(wii.Day3) / 60, Convert.ToInt32(wii.Day3) % 60);
				dr["Day4"] = String.Format("{0:D2}:{1:D2}", Convert.ToInt32(wii.Day4) / 60, Convert.ToInt32(wii.Day4) % 60);
				dr["Day5"] = String.Format("{0:D2}:{1:D2}", Convert.ToInt32(wii.Day5) / 60, Convert.ToInt32(wii.Day5) % 60);
				dr["Day6"] = String.Format("{0:D2}:{1:D2}", Convert.ToInt32(wii.Day6) / 60, Convert.ToInt32(wii.Day6) % 60);
				dr["Day7"] = String.Format("{0:D2}:{1:D2}", Convert.ToInt32(wii.Day7) / 60, Convert.ToInt32(wii.Day7) % 60);
				dr["DayT"] = String.Format("{0}{1:D2}:{2:D2}{3}", "<b>", Convert.ToInt32(wii.DayT) / 60, Convert.ToInt32(wii.DayT) % 60, "</b>");
				dt.Rows.Add(dr);
			}

			if (_pc[grdMain.GetPropertyKey(MCGrid.SortingPropertyKey)] == null)
				_pc[grdMain.GetPropertyKey(MCGrid.SortingPropertyKey)] = "WeekNumberSort DESC";
			if (_pc[grdMain.GetPropertyKey(MCGrid.PageSizePropertyKey)] == null || _pc[grdMain.GetPropertyKey(MCGrid.PageSizePropertyKey)] == "-1")
				_pc[grdMain.GetPropertyKey(MCGrid.PageSizePropertyKey)] = "100";
			
			grdMain.DataSource = dt.DefaultView;
			if(dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region grdMain_ChangingMCGridColumnHeader
		void grdMain_ChangingMCGridColumnHeader(object sender, ChangingMCGridColumnHeaderEventArgs e)
		{
			string fieldName = e.FieldName;
			if (fieldName == "Day1" || fieldName == "Day2" || fieldName == "Day3" || fieldName == "Day4" || fieldName == "Day5" || fieldName == "Day6" || fieldName == "Day7")
			{
				DateTime dt = CHelper.GetRealWeekStartByDate(DateTime.UtcNow);

				if (fieldName == "Day2")
					dt = dt.AddDays(1);
				else if (fieldName == "Day3")
					dt = dt.AddDays(2);
				else if (fieldName == "Day4")
					dt = dt.AddDays(3);
				else if (fieldName == "Day5")
					dt = dt.AddDays(4);
				else if (fieldName == "Day6")
					dt = dt.AddDays(5);
				else if (fieldName == "Day7")
					dt = dt.AddDays(6);

				e.ControlField.HeaderText = GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString();
			}
		}
		#endregion
	}
}