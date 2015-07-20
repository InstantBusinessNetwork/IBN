using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

//using Mediachase.IBN.Business;
using Mediachase.IBN.DefaultUserReports;
using Mediachase.UI.Web.UserReports.GlobalModules.PageTemplateExtension;
using Mediachase.UI.Web.UserReports.Util;
using Mediachase.Ibn;


namespace Mediachase.UI.Web.UserReports.Modules
{
	/// <summary>
	///		Summary description for GroupAndUserImStat.
	/// </summary>
	/// 

	public partial class GroupAndUserImStat : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strGroupAndUserStat", typeof(GroupAndUserImStat).Assembly);
		protected ResourceManager LocRM1 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strMostActiveReport", typeof(GroupAndUserImStat).Assembly);
		Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		/*
		protected System.Web.UI.WebControls.TextBox fromDate;
		protected System.Web.UI.WebControls.TextBox toDate;
		protected System.Web.UI.WebControls.RangeValidator rvFromDate;
		protected System.Web.UI.WebControls.RangeValidator rvToDate;
		*/
		protected System.Web.UI.WebControls.CustomValidator CustomValidator1;

		IFormatProvider culture = CultureInfo.InvariantCulture;

		private int TotalImSessions = 0;
		private TimeSpan Duration;


		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			lblTotalIM.Text = TotalImSessions.ToString();
			lblTotalDuration.Text = CommonHelper.GetStringInterval(Duration);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindSelect();
				GetIMStat();
			}

			_header.Title = String.Format(LocRM.GetString("GroupAndUserIM"), IbnConst.ProductFamilyShort);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script type='text/javascript'>ChangeModify(document.getElementById('" + ddPeriod.ClientID + "'));</script>");

		}

		private void BindSelect()
		{
			ddPeriod.Items.Add(new ListItem(LocRM1.GetString("tToday"), "1"));
			ddPeriod.Items.Add(new ListItem(LocRM1.GetString("tYesterday"), "2"));
			ddPeriod.Items.Add(new ListItem(LocRM1.GetString("tThisWeek"), "3"));
			ddPeriod.Items.Add(new ListItem(LocRM1.GetString("tLastWeek"), "4"));
			ddPeriod.Items.Add(new ListItem(LocRM1.GetString("tThisMonth"), "5"));
			ddPeriod.Items.Add(new ListItem(LocRM1.GetString("tLastMonth"), "6"));
			ddPeriod.Items.Add(new ListItem(LocRM1.GetString("tCustom"), "9"));
			if (pc["IMStat_ddPeriod"] != null)
			{
				string sVal = pc["IMStat_ddPeriod"];
				ListItem liItem = ddPeriod.Items.FindByValue(sVal);
				if (liItem != null)
					liItem.Selected = true;

				if (pc["IMStat_Start"] != null)
				{
					DateTime _date = DateTime.Parse(pc["IMStat_Start"], culture);
					//fromDate.Text = _date.ToShortDateString();
					dtcStartDate.SelectedDate = _date;
				}
				else
				{
					//fromDate.Text = UserDateTime.UserToday.ToShortDateString();
					dtcStartDate.SelectedDate = UserDateTime.Today;
				}
				if (pc["IMStat_End"] != null)
				{
					DateTime _date = DateTime.Parse(pc["IMStat_End"], culture);
					//toDate.Text = _date.ToShortDateString();
					dtcEndDate.SelectedDate = _date;
				}
				else
				{
					//toDate.Text = UserDateTime.UserNow.ToShortDateString();
					dtcEndDate.SelectedDate = UserDateTime.Now;
				}
			}
			else
			{
				//fromDate.Text = UserDateTime.UserToday.AddMonths(-1).ToShortDateString();
				dtcStartDate.SelectedDate = UserDateTime.Today.AddMonths(-1);
				//toDate.Text = UserDateTime.UserNow.ToShortDateString();
				dtcEndDate.SelectedDate = UserDateTime.Now;
			}

			ddContactGroup.DataSource = UserReport.GetListIMGroup();
			ddContactGroup.DataTextField = "IMGroupName";
			ddContactGroup.DataValueField = "IMGroupId";
			ddContactGroup.DataBind();

			btnAplly.Text = LocRM.GetString("tShow");
			btnExport.Text = LocRM.GetString("tExport");
		}

		private void GetIMStat()
		{
			lblContactGroup.Text = ddContactGroup.SelectedItem.Text;

			repUsers.DataSource = UserReport.GetListUsers(int.Parse(ddContactGroup.SelectedItem.Value));
			repUsers.DataBind();
		}

		protected string GetDateString(DateTime SBegin, Object SEnd)
		{
			String sessionend = String.Empty;
			if (SEnd != DBNull.Value)
				sessionend = ((DateTime)SEnd).ToString("g");
			return String.Concat(SBegin.ToString("g"), " - ", sessionend);
		}

		protected string GetDuration(DateTime StartDate, object EndDate)
		{
			DateTime enddate = UserDateTime.Now;
			if (EndDate != DBNull.Value)
				enddate = (DateTime)EndDate;

			Duration = Duration + (enddate - StartDate);
			return CommonHelper.GetStringInterval(enddate - StartDate);
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.repUsers.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(this.repTypes_Bound);

		}
		#endregion

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return String.Format(LocRM.GetString("GroupAndUserIM"), IbnConst.ProductFamilyShort);
		}
		#endregion

		private void repTypes_Bound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
		{
			int OriginalId = (int)DataBinder.Eval(e.Item.DataItem, "OriginalId");
			Repeater repSessions = (Repeater)e.Item.FindControl("repSessions");
			Label lblNone = (Label)e.Item.FindControl("lblNone");

			if (repSessions != null)
			{
				DateTime StartDate = DateTime.MinValue;
				DateTime EndDate = DateTime.MinValue;
				//SetDates(ddPeriod.Value, out StartDate, out EndDate, fromDate.Text, toDate.Text);
				UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

				string sFilter = "&nbsp;&nbsp;" + LocRM.GetString("tGroupName") + ":&nbsp;" + ddContactGroup.SelectedItem.Text;
				sFilter += "<br/>&nbsp;&nbsp;" + LocRM.GetString("tPeriod") + ":&nbsp;" + StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
				_header.Filter = sFilter;
				repSessions.DataSource = UserReport.GetListIMSessionsByUserAndDate(OriginalId, StartDate, EndDate);
				repSessions.DataBind();
				if (repSessions.Items.Count > 0)
				{
					lblNone.Visible = false;
					TotalImSessions += repSessions.Items.Count;
				}
				else
					lblNone.Text = LocRM.GetString("NoneDuring");
			}
		}

		protected void btnAplly_Click(object sender, System.EventArgs e)
		{
			DateTime StartDate = new DateTime(0);
			DateTime EndDate = new DateTime(0);
			//SetDates(ddPeriod.Value, out StartDate, out EndDate, fromDate.Text, toDate.Text);
			UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			pc["IMStat_ddPeriod"] = ddPeriod.Value;
			if (ddPeriod.Value == "9")
			{
				pc["IMStat_Start"] = StartDate.ToString(culture);
				pc["IMStat_End"] = EndDate.ToString(culture);
			}
			pc["IMStat_ViewBy"] = ddContactGroup.SelectedItem.Value;
			GetIMStat();
		}
	}
}
