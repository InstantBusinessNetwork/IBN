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

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Data;


namespace Mediachase.UI.Web.Apps.TimeTracking.Modules.PublicControls
{
	public partial class GridPopupEditEntry : System.Web.UI.UserControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["primaryKeyId"] != null)
				{
					retval = MetaViewGroupUtil.GetMetaTypeFromUniqueKey(Request.QueryString["primaryKeyId"]);
				}
				return retval;
			}
		}
		#endregion

		#region ObjectId
		public int ObjectId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["primaryKeyId"] != null)
				{
					retval = int.Parse(MetaViewGroupUtil.GetIdFromUniqueKey(Request.QueryString["primaryKeyId"]));
				}
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				Day1Time.Value = DateTime.MinValue;
				Day2Time.Value = DateTime.MinValue;
				Day3Time.Value = DateTime.MinValue;
				Day4Time.Value = DateTime.MinValue;
				Day5Time.Value = DateTime.MinValue;
				Day6Time.Value = DateTime.MinValue;
				Day7Time.Value = DateTime.MinValue;

				BindData();
			}

			if (Request["closeFramePopup"] != null)
			{
				string closeScript = String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
				btnCancelEntry.OnClientClick = closeScript;
				btnCancelBlock.OnClientClick = closeScript;
				btnCancelReadOnly.OnClientClick = closeScript;
			}
		}

		#region BindData
		/// <summary>
		/// Binds the data.
		/// </summary>
		private void BindData()
		{
			if (this.ClassName == TimeTrackingEntry.GetAssignedMetaClass().Name)
			{
				TimeTrackingEntry tte = MetaObjectActivator.CreateInstance<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), this.ObjectId);

				BindDayHeaders(tte.StartDate);
				BindTTEntry(tte);
				frmView.DataItem = tte;
				frmView.DataBind();

				if (TimeTrackingBlock.CheckUserRight(tte.ParentBlockId, Security.RightWrite))
				{
					TTBlock.Visible = false;
					TTEntry.Visible = true;
					ReadOnlyBlock.Visible = false;
				}
				else
				{
					TTBlock.Visible = false;
					TTEntry.Visible = false;
					ReadOnlyBlock.Visible = true;
				}
			}
			else if (this.ClassName == TimeTrackingBlock.GetAssignedMetaClass().Name)
			{
				TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), this.ObjectId);

				frmViewBlock.DataItem = ttb;
				frmViewBlock.DataBind();

				if (TimeTrackingBlock.CheckUserRight(ttb.PrimaryKeyId.Value, Security.RightWrite))
				{
					TTBlock.Visible = true;
					TTEntry.Visible = false;
					ReadOnlyBlock.Visible = false;
				}
				else
				{
					TTBlock.Visible = false;
					TTEntry.Visible = false;
					ReadOnlyBlock.Visible = true;
				}
			}
			else
			{
				TTBlock.Visible = false;
				TTEntry.Visible = false;
				ReadOnlyBlock.Visible = true;
			}
		} 
		#endregion

		#region BindDayHeaders
		private void BindDayHeaders(DateTime startDate)
		{
			DateTime dt;
			DateTime curDate = Mediachase.IBN.Business.User.GetLocalDate(DateTime.UtcNow).Date;

			dt = startDate;
			Day1Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day1Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day2Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day2Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day3Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day3Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day4Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day4Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day5Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day5Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day6Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day6Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day7Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day7Label.CssClass = (curDate == dt) ? "smallSelected" : "small";
		}
		#endregion

		#region BindTTEntry
		/// <summary>
		/// Binds the TT entry.
		/// </summary>
		/// <param name="tte">The tte.</param>
		void BindTTEntry(TimeTrackingEntry tte)
		{
			Day1Time.Value = DateTime.MinValue.AddMinutes(tte.Day1);
			Day2Time.Value = DateTime.MinValue.AddMinutes(tte.Day2);
			Day3Time.Value = DateTime.MinValue.AddMinutes(tte.Day3);
			Day4Time.Value = DateTime.MinValue.AddMinutes(tte.Day4);
			Day5Time.Value = DateTime.MinValue.AddMinutes(tte.Day5);
			Day6Time.Value = DateTime.MinValue.AddMinutes(tte.Day6);
			Day7Time.Value = DateTime.MinValue.AddMinutes(tte.Day7);
		}
		#endregion

		#region btnSaveEntry_Click
		/// <summary>
		/// Handles the Click event of the btnSaveEntry control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnSaveEntry_Click(object sender, EventArgs e)
		{
			double maxMinutes = (double)(24 * 60);
			TimeTrackingEntry tte = MetaObjectActivator.CreateInstance<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), this.ObjectId);
			tte.Day1 = Math.Min((new TimeSpan(Day1Time.Value.Ticks)).TotalMinutes, maxMinutes);
			tte.Day2 = Math.Min((new TimeSpan(Day2Time.Value.Ticks)).TotalMinutes, maxMinutes);
			tte.Day3 = Math.Min((new TimeSpan(Day3Time.Value.Ticks)).TotalMinutes, maxMinutes);
			tte.Day4 = Math.Min((new TimeSpan(Day4Time.Value.Ticks)).TotalMinutes, maxMinutes);
			tte.Day5 = Math.Min((new TimeSpan(Day5Time.Value.Ticks)).TotalMinutes, maxMinutes);
			tte.Day6 = Math.Min((new TimeSpan(Day6Time.Value.Ticks)).TotalMinutes, maxMinutes);
			tte.Day7 = Math.Min((new TimeSpan(Day7Time.Value.Ticks)).TotalMinutes, maxMinutes);

			ProcessCollection(this.frmView.Controls, tte);

			tte.Save();

			CommandParameters cp = new CommandParameters("MC_TimeTracking_EditGridFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region btnSaveBlock_Click
		/// <summary>
		/// Handles the Click event of the btnSaveBlock control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnSaveBlock_Click(object sender, EventArgs e)
		{
			using (TransactionScope tran = DataContext.Current.BeginTransaction())
			{
				TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), this.ObjectId);

				ProcessCollection(this.frmViewBlock.Controls, ttb);

				ttb.Save();

				tran.Commit();
			}

			CommandParameters cp = new CommandParameters("MC_TimeTracking_EditGridFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region ProcessCollection
		private void ProcessCollection(ControlCollection _coll, MetaObject mo)
		{
			foreach (Control c in _coll)
			{
				ProcessControl(c, mo);
				if (c.Controls.Count > 0)
					ProcessCollection(c.Controls, mo);
			}
		}

		private void ProcessControl(Control c, MetaObject mo)
		{
			IEditControl fc = c as IEditControl;
			if (fc != null && !fc.ReadOnly)
			{
				string fieldName = fc.FieldName;
				if (fieldName.ToLower() == "title" && mo.Properties[fieldName] == null)
				{
					fieldName = mo.GetMetaType().TitleFieldName;
				}
				mo.Properties[fieldName].Value = fc.Value;
			}
		}
		#endregion
	}
}