using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Reporting;

namespace Mediachase.Ibn.Web.UI.ReportManagement.FilterControls
{
	public partial class DefaultFilter : System.Web.UI.UserControl, IReportFilter
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			btnApply.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Apply").ToString();
		}

		protected void btnApply_Click(object sender, EventArgs e)
		{
			FilterEventArgs fe = new FilterEventArgs(String.Empty, String.Empty);

			EventHandler<FilterEventArgs> filterChanged = FilterChanged;
			if (filterChanged != null)
				filterChanged(this, fe);
		}


		#region IReportFilter Members

		public event EventHandler<FilterEventArgs> FilterChanged;

		public string FilterSql
		{
			get { return String.Empty; }
		}

		public string FilterXml
		{
			get
			{
				return String.Empty;
			}
			set
			{
			}
		}

		public Uri WebServiceUri
		{
			get
			{
				return new Uri(String.Empty);
			}
			set
			{
			}
		}

		#endregion
	}
}