using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Business;
using System.Data;

namespace Mediachase.UI.Web.Apps.ReportManagement.Modules
{
	public partial class UserReportGeneral : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", typeof(UserReportGeneral).Assembly);
		Mediachase.IBN.Business.ControlSystem.ReportStorage rs;
		BaseIbnContainer bic;

		protected void Page_Load(object sender, EventArgs e)
		{
			bic = BaseIbnContainer.Create("Reports", "GlobalReports");
			rs = (ReportStorage)bic.LoadControl("ReportStorage");
			BindToolBar();
			if (!IsPostBack)
				BindRepeaters();
		}

		#region BindToolBar
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindToolBar()
		{
			tbLightGeneral.ActionsMenu.Items.Clear();
			tbLightGeneral.ClearRightItems();
			tbLightGeneral.AddText(LocRM.GetString("GeneralReports"));
		}
		#endregion

		#region BindRepeaters
		/// <summary>
		/// Binds the repeaters.
		/// </summary>
		private void BindRepeaters()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("sortName", typeof(string)));

			DataTable dt1 = new DataTable();
			dt1.Columns.Add(new DataColumn("Name", typeof(string)));
			dt1.Columns.Add(new DataColumn("sortName", typeof(string)));

			DataRow dr;

			ReportInfo[] _ri = rs.GetReports();
			foreach (ReportInfo ri in _ri)
			{
				if (!rs.CanUserRead(Security.CurrentUser.UserID, ri.Id))
					continue;
				if (ri.Name == "ProjectTime" && !Configuration.ProjectManagementEnabled)
					continue;
				if (ri.Type == Mediachase.IBN.Business.UserReport.UserReportType.Personal)
				{
					dr = dt.NewRow();
					dr["Name"] = String.Format("<a href='{0}'>{1}</a>", ResolveUrl("~" + ri.Url), ri.ShowName);
					dr["sortName"] = ri.ShowName;
					dt.Rows.Add(dr);
				}
				else if (ri.Type == Mediachase.IBN.Business.UserReport.UserReportType.Global)
				{
					dr = dt1.NewRow();
					dr["Name"] = String.Format("<a href='{0}'>{1}</a>", ResolveUrl("~" + ri.Url), ri.ShowName);
					dr["sortName"] = ri.ShowName;
					dt1.Rows.Add(dr);
				}
			}

			DataView dv = dt.DefaultView;

			dv = dt1.DefaultView;
			dv.Sort = "sortName";
			repGeneral.DataSource = dv;
			repGeneral.DataBind();
		}
		#endregion
	}
}