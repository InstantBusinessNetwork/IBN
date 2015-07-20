using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business.ReportSystem;
using System.IO;
using Microsoft.Reporting.WebForms;
using Mediachase.Ibn.Reporting;

namespace Mediachase.Ibn.Web.UI.ReportManagement.Modules
{
	public partial class ReportView : System.Web.UI.UserControl
	{
		private const string _filterDirPath = "~/Apps/ReportManagement/FilterControls";

		protected void Page_Load(object sender, EventArgs e)
		{
			ReportEntity re = (ReportEntity)BusinessManager.Load(ReportEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(Request["ReportId"]));

			string sql = BindFilterControl(re);
			
			if (!Page.IsPostBack)
			{
				BindReport(re, sql);
				BindToolbar();
			}
		}

		#region BindReport
		private void BindReport(ReportEntity reportEntity, string filter)
		{
			MemoryStream memStream = new MemoryStream();
			StreamWriter writer = new StreamWriter(memStream);
			writer.Write(reportEntity.RdlText);
			writer.Flush();
			memStream.Seek(0, SeekOrigin.Begin);

			rvMain.LocalReport.LoadReportDefinition(memStream);
			rvMain.LocalReport.DisplayName = "Report";

			ReportDataResponse rdr = BusinessManager.Execute<ReportDataRequest, ReportDataResponse>(new ReportDataRequest(reportEntity, filter));
			foreach (DataTable dt in rdr.Data)
			{
				rvMain.LocalReport.DataSources.Add(new ReportDataSource(dt.TableName, dt));
			}
		} 
		#endregion

		#region BindFilterControl
		private string BindFilterControl(ReportEntity re)
		{
			string retVal = String.Empty;
			if (String.IsNullOrEmpty(re.FilterControl))
				return retVal;
			string sPath = String.Format("{1}/{0}.ascx", re.FilterControl, _filterDirPath);
			Control c = null;
			try
			{
				c = Page.LoadControl(sPath);
			}
			catch{}
			if (c == null)
				return retVal;
			
			c.ID = "filterCtrl";
			phFilter.Controls.Add(c);
			if (c is IReportFilter)
			{
				((IReportFilter)c).FilterChanged += new EventHandler<FilterEventArgs>(ReportView_FilterChanged);
			}
			if (!Page.IsPostBack && !String.IsNullOrEmpty(re.DefaultFilterXml))
				((IReportFilter)c).FilterXml = re.DefaultFilterXml;
			if (!Page.IsPostBack)
				retVal = ((IReportFilter)c).FilterSql;

			return retVal;
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.Report", "ReportView").ToString();
			BlockHeaderMain.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' /> {1}",
					ResolveClientUrl("~/layouts/images/cancel.gif"),
					GetGlobalResourceObject("IbnFramework.Report", "ReportList").ToString()),
				ResolveUrl("~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=" + ReportEntity.GetAssignedMetaClassName()));
		} 
		#endregion

		void ReportView_FilterChanged(object sender, FilterEventArgs e)
		{
			ReportEntity re = (ReportEntity)BusinessManager.Load(ReportEntity.GetAssignedMetaClassName(), PrimaryKeyId.Parse(Request["ReportId"]));
			BindReport(re, e.FilterSql);
		}
	}
}