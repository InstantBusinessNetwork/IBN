using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	/// <summary>
	/// Represents ProjectTaskScoreReport result.
	/// </summary>
	public class ReportResult
	{
		#region Const
		#endregion

		#region Fields
		#endregion

		#region .Ctor
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectTaskScoreReportResult"/> class.
		/// </summary>
		public ReportResult()
		{
		}
		#endregion

		#region Properties
		private List<ReportResultItem> _items = new List<ReportResultItem>();

		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <value>The items.</value>
		public List<ReportResultItem> Items
		{
			get { return _items; }
		}
	

		private double _total;

		/// <summary>
		/// Gets or sets the total.
		/// </summary>
		/// <value>The total.</value>
		public double Total
		{
			get { return _total; }
			set { _total = value; }
		}
		#endregion

		#region Methods
		#endregion

		
	}
}
