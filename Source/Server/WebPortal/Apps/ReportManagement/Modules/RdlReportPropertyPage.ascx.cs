using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.IBN.Business.ReportSystem;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.Apps.WidgetEngine;

namespace Mediachase.Ibn.Web.UI.Shell.Modules
{
	public partial class RdlReportPropertyPage : System.Web.UI.UserControl, IPropertyPageControl
	{
		private string keyReport = "report";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindValues();
			}
		}

		#region BindValues
		private void BindValues()
		{
			EntityObject[] reports = BusinessManager.List(ReportEntity.GetAssignedMetaClassName(), new FilterElementCollection().ToArray());
			if (reports != null && reports.Length > 0)
			{
				ReportList.DataSource = reports;
				ReportList.DataBind();

				ControlPropertiesBase properties = ControlProperties.Provider;
				if (properties.GetValue(this.ID, keyReport) != null)
					CommonHelper.SafeSelect(ReportList, (string)properties.GetValue(this.ID, keyReport));

				ReportsRow.Visible = true;
				NoReportsRow.Visible = false;
			}
			else
			{
				ReportsRow.Visible = false;
				NoReportsRow.Visible = true;
			}
		}
		#endregion

		#region IPropertyPageControl Members
		public void Save()
		{
			if (ReportList.Items.Count > 0)
			{
				ControlPropertiesBase properties = ControlProperties.Provider;
				properties.SaveValue(this.ID, keyReport, ReportList.SelectedValue);
			}
		}
		#endregion
	}
}