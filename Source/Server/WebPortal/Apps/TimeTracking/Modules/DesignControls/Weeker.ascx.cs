using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using System.Globalization;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking
{
	public partial class Weeker : System.Web.UI.UserControl
	{

		#region event: OnValueChange
		public delegate void ValueChanged(Object sender, EventArgs e);

		public event ValueChanged ValueChange;

		protected virtual void OnValueChange(Object sender, EventArgs e)
		{
			if (ValueChange != null)
			{
				ValueChange(this, e);
			}
		}
		#endregion

		#region SelectedDate
		public DateTime SelectedDate
		{
			get
			{
				return dtcWeek.SelectedDate;
			}
			set
			{
				dtcWeek.SelectedDate = value;
			}
		} 
		#endregion

		#region prop: ShowWeekNumber
		/// <summary>
		/// Gets or sets a value indicating whether [show week number].
		/// </summary>
		/// <value><c>true</c> if [show week number]; otherwise, <c>false</c>.</value>
		public bool ShowWeekNumber
		{
			get
			{
				if (ViewState[this.ID + "_ShowWeekNumber"] == null)
					return false;

				return Convert.ToBoolean(ViewState[this.ID + "_ShowWeekNumber"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState[this.ID + "_ShowWeekNumber"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			dtcWeek.ValueChange += new PickerControl.ValueChanged(dtcWeek_ValueChange);
			dtcWeek.ShowWeekNumber = this.ShowWeekNumber;
			ApplyLocalization();

			imgPrev.Click += new ImageClickEventHandler(imgPrev_Click);
			imgNext.Click += new ImageClickEventHandler(imgNext_Click);
			btnCurrentWeek.Click += new EventHandler(btnCurrentWeek_Click);

			imgNext.ImageUrl = this.Page.ResolveUrl("~/Images/TimeTracking/right.gif");
			imgPrev.ImageUrl = this.Page.ResolveUrl("~/Images/TimeTracking/left.gif");

			if (Request.Browser.Browser.Contains("IE"))
			{
				cellNext.Attributes.Add("class", "iePaddingBottom2px");
				cellPrev.Attributes.Add("class", "iePaddingBottom2px");
			}

			
		}

        string generateExcelScript()
        {
            StringBuilder sb = new StringBuilder();
			sb.Append("function excelExportActionScript(obj, showModal) {");
            
			string url = this.Page.ResolveUrl("~/Apps/TimeTracking/Pages/Public/TimeTrackingExport.aspx");
            url += String.Format("?ViewName={0}&StartDate={1}", CHelper.GetFromContext("MetaViewName"), dtcWeek.SelectedDate);

            sb.AppendFormat("window.open('{0}', 'ExportExcel', 'resizible=0,scrollbars=0,height=300,width=300,top=100,left=100'); return false;", url);
			sb.Append("}");
            return sb.ToString();
        }

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			DateTime dt = CHelper.GetRealWeekStartByDate(DateTime.Now);
			btnCurrentWeek.Enabled = (dt != dtcWeek.SelectedDate);
			this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), new Guid().ToString(), generateExcelScript(), true);
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnCurrentWeek.Text = CHelper.GetResFileString("{IbnFramework.TimeTracking:CurrentWeek}");
		}
		#endregion

		#region WeekEvents

        #region btnCurrentWeek_Click
        void btnCurrentWeek_Click(object sender, EventArgs e)
        {
			DateTime dt = CHelper.GetRealWeekStartByDate(DateTime.Now);
            dtcWeek.SelectedDate = dt;

            OnValueChange(sender, e);
        }
        #endregion

        #region imgNext_Click
        void imgNext_Click(object sender, ImageClickEventArgs e)
        {
			UpdateCM();
			DateTime dt = CHelper.GetRealWeekStartByDate(dtcWeek.SelectedDate.AddDays(7));
            dtcWeek.SelectedDate = dt;

            OnValueChange(sender, e);
        }
        #endregion

        #region imgPrev_Click
        void imgPrev_Click(object sender, ImageClickEventArgs e)
        {
			UpdateCM();

			DateTime dt = CHelper.GetRealWeekStartByDate(dtcWeek.SelectedDate.AddDays(-7));
            dtcWeek.SelectedDate = dt;

            OnValueChange(sender, e);
        }
        #endregion

		#region UpdateCM
		/// <summary>
		/// Updates the CM.
		/// </summary>
		private void UpdateCM()
		{
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			if (cm != null)
			{
				cm.ForceUpdate();
			}
		}
		#endregion

		#region dtcWeek_ValueChange
		/// <summary>
		/// Handles the ValueChange event of the dtcWeek control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void dtcWeek_ValueChange(object sender, EventArgs e)
		{
			DateTime dt = CHelper.GetRealWeekStartByDate(dtcWeek.SelectedDate);
			dtcWeek.SelectedDate = dt;

			OnValueChange(sender, e);
		} 
		#endregion
		#endregion
	}
}