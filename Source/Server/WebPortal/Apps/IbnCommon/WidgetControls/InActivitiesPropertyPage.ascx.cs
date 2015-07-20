using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.Apps.WidgetEngine;

namespace Mediachase.Ibn.Web.UI.Shell.Modules
{
	public partial class InActivitiesPropertyPage : System.Web.UI.UserControl, IPropertyPageControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strListView", Assembly.GetExecutingAssembly());

		private string keyShowActive = "showActive";
		private string keyType = "type";
		private string keyFrom = "from";
		private string keyTo = "to";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindTexts();
				BindValues();
			}

			listType.Attributes.Add("onchange", "ShowHideCalendars()");
		}

		#region BindTexts
		private void BindTexts()
		{
			buttonActive.Text = LocRM.GetString("ShowActive");
			buttonAll.Text = LocRM.GetString("ShowAll");

			listType.Items.Add(new ListItem(LocRM.GetString("ShowLastWeek"), "7"));
			listType.Items.Add(new ListItem(LocRM.GetString("ShowLastMonth"), "30"));
			listType.Items.Add(new ListItem(LocRM.GetString("ShowLastYear"), "365"));
			listType.Items.Add(new ListItem(LocRM.GetString("tCustom"), "0"));
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			ControlPropertiesBase properties = ControlProperties.Provider;

			// Active/All
			if (properties.GetValue(this.ID, keyShowActive) == null)
				properties.SaveValue(this.ID, keyShowActive, true);

			if ((bool)properties.GetValue(this.ID, keyShowActive))
				buttonActive.Checked = true;
			else
				buttonAll.Checked = true;

			// Period
			if (properties.GetValue(this.ID, keyType) == null)
				properties.SaveValue(this.ID, keyType, "365");	// week

			if ((string)properties.GetValue(this.ID, keyType) == "0" && (properties.GetValue(this.ID, keyFrom) == null || properties.GetValue(this.ID, keyTo) == null))
				properties.SaveValue(this.ID, keyType, "365");	// week

			if ((string)properties.GetValue(this.ID, keyType) != "0")
			{
				rowFrom.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
				rowTo.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");

				dateFrom.SelectedDate = DateTime.Now.Date.AddDays(-7);
				dateTo.SelectedDate = DateTime.Now.Date;
			}
			else
			{
				dateFrom.SelectedDate = (DateTime)properties.GetValue(this.ID, keyFrom);
				dateTo.SelectedDate = (DateTime)properties.GetValue(this.ID, keyTo);
			}

			CommonHelper.SafeSelect(listType, (string)properties.GetValue(this.ID, keyType));
		}
		#endregion

		#region IPropertyPageControl Members
		public void Save()
		{
			ControlPropertiesBase properties = ControlProperties.Provider;

			properties.SaveValue(this.ID, keyShowActive, buttonActive.Checked);

			properties.SaveValue(this.ID, keyType, listType.SelectedValue);

			if (listType.SelectedValue == "0")
			{
				properties.SaveValue(this.ID, keyFrom, dateFrom.SelectedDate);
				properties.SaveValue(this.ID, keyTo, dateTo.SelectedDate);
			}
		}
		#endregion
	}
}