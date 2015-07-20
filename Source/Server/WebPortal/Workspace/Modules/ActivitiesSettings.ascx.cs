using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Modules.PageTemplateExtension;

namespace Mediachase.UI.Web.Workspace.Modules
{
	public partial class ActivitiesSettings : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strListView", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		private string keyShowActive;
		private string keyType;
		private string keyFrom;
		private string keyTo;

		#region RefreshButton
		public string RefreshButton
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["btn"] != null)
					retval = Request.QueryString["btn"];
				return retval;
			}
		}
		#endregion

		#region Prefix
		protected string Prefix
		{
			get
			{
				string retval = "Out";
				if (Request.QueryString["prefix"] != null)
					retval = Request.QueryString["prefix"];
				return retval;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			keyShowActive = Prefix + "Activities_showActive";
			keyType = Prefix + "Activities_type";
			keyFrom = Prefix + "Activities_from";
			keyTo = Prefix + "Activities_to";

			if (!IsPostBack)
			{
				BindTexts();
				BindValues();
			}

			buttonSave.Text = LocRM2.GetString("tbsave_save");
			buttonCancel.Text = LocRM2.GetString("tbsave_cancel");

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
			// Active/All
			if (pc[keyShowActive] == null)
				pc[keyShowActive] = Boolean.TrueString;

			if (pc[keyShowActive] == Boolean.TrueString)
				buttonActive.Checked = true;
			else
				buttonAll.Checked = true;

			// Period
			if (pc[keyType] == null)
				pc[keyType] = "7";	// week

			if (pc[keyType] == "0" && (pc[keyFrom] == null || pc[keyType] == null))
				pc[keyType] = "7";	// week

			if (pc[keyType] != "0")
			{
				rowFrom.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
				rowTo.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");

				dateFrom.SelectedDate = DateTime.Now.Date.AddDays(-7);
				dateTo.SelectedDate = DateTime.Now.Date;
			}
			else
			{
				dateFrom.SelectedDate = DateTime.Parse(pc[keyFrom], CultureInfo.InvariantCulture);
				dateTo.SelectedDate = DateTime.Parse(pc[keyTo], CultureInfo.InvariantCulture);
			}

			Util.CommonHelper.SafeSelect(listType, pc[keyType]);
		}
		#endregion

		#region ButtonSave_ServerClick
		protected void ButtonSave_ServerClick(object sender, EventArgs e)
		{
			if (buttonActive.Checked)
				pc[keyShowActive] = Boolean.TrueString;
			else
				pc[keyShowActive] = Boolean.FalseString;

			pc[keyType] = listType.SelectedValue;

			if (listType.SelectedValue == "0")
			{
				pc[keyFrom] = dateFrom.SelectedDate.ToString(CultureInfo.InvariantCulture);
				pc[keyTo] = dateTo.SelectedDate.ToString(CultureInfo.InvariantCulture);
			}

			// Closing window
			if (String.IsNullOrEmpty(RefreshButton))
			{
				Util.CommonHelper.CloseItAndRefresh(Response);
			}
			else  // Dialog Mode
			{
				Util.CommonHelper.CloseItAndRefresh(Response, RefreshButton);
			}
		}
		#endregion

		#region IPageTemplateTitle Members
		public string Modify(string oldValue)
		{
			return LocRM.GetString(Prefix + "Activities") + " - " + LocRM.GetString("Filter");
		}
		#endregion
	}
}