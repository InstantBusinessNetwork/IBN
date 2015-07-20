using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Microsoft.Reporting.WebForms;

using Mediachase.IBN.Business.ReportSystem;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Reporting;
using Mediachase.IBN.Business.WidgetEngine;

namespace Mediachase.Ibn.Web.UI.Shell.Modules
{
	public partial class RdlReport : System.Web.UI.UserControl
	{
		private const string _filterDirPath = "~/Apps/ReportManagement/FilterControls";
		private string keyReport = "report";

		#region ReportId
		private PrimaryKeyId? reportId = null;
		protected PrimaryKeyId ReportId
		{
			get
			{
				if (!reportId.HasValue)
				{
					reportId = PrimaryKeyId.Empty;

					ControlPropertiesBase properties = ControlProperties.Provider;
					if (properties.GetValue(this.ID, keyReport) != null)
					{
						PrimaryKeyId savedId = PrimaryKeyId.Parse((string)properties.GetValue(this.ID, keyReport));

						// Check Security
						EntityObject[] reports = BusinessManager.List(ReportEntity.GetAssignedMetaClassName(), new FilterElementCollection(FilterElement.EqualElement("ReportId", savedId)).ToArray());
						if (reports != null && reports.Length > 0)
							reportId = savedId;
					}

					// If report was not found, then take the first available report
					if (reportId == PrimaryKeyId.Empty)
					{
						EntityObject[] reports = BusinessManager.List(ReportEntity.GetAssignedMetaClassName(), new FilterElementCollection().ToArray());
						if (reports != null && reports.Length > 0)
						{
							ReportEntity re = (ReportEntity)reports[0];
							reportId = re.PrimaryKeyId;
							properties.SaveValue(this.ID, keyReport, reportId.ToString());
						}
					}
				}
				return reportId.Value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			ErrorLabel.Visible = false;

			if (ReportId != PrimaryKeyId.Empty)
			{
				rvMain.Visible = true;
				NoReportsLiteral.Visible = false;

				try
				{
					ReportEntity re = (ReportEntity)BusinessManager.Load(ReportEntity.GetAssignedMetaClassName(), ReportId);
					ReportLabel.Text = re.Title;

					string sql = BindFilterControl(re);

					if (!Page.IsPostBack)
					{
						BindReport(re, sql);
					}
				}
				catch (Exception)
				{
					rvMain.Visible = false;
					FilterRow.Visible = false;
					TitleRow.Visible = false;
					ErrorLabel.Visible = true;
				}
			}
			else
			{
				rvMain.Visible = false;
				NoReportsLiteral.Visible = true;
			}
		}

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
			catch { }
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

		#region BindReport
		private void BindReport(ReportEntity reportEntity, string filter)
		{
			MemoryStream memStream = new MemoryStream();
			StreamWriter writer = new StreamWriter(memStream);
			writer.Write(reportEntity.RdlText);
			writer.Flush();
			memStream.Seek(0, SeekOrigin.Begin);

			rvMain.LocalReport.LoadReportDefinition(memStream);

			ReportDataResponse rdr = BusinessManager.Execute<ReportDataRequest, ReportDataResponse>(new ReportDataRequest(reportEntity, filter));
			foreach (DataTable dt in rdr.Data)
			{
				rvMain.LocalReport.DataSources.Add(new ReportDataSource(dt.TableName, dt));
			}
		}
		#endregion

		#region ReportView_FilterChanged
		void ReportView_FilterChanged(object sender, FilterEventArgs e)
		{
			ReportEntity re = (ReportEntity)BusinessManager.Load(ReportEntity.GetAssignedMetaClassName(), ReportId);
			BindReport(re, e.FilterSql);
		}
		#endregion
	}
}