using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	public class UserReportResultItem : ReportResultItem
	{
		public UserReportResultItem()
		{
		}

		public UserReportResultItem(ObjectTypes objectType, int objectId, string name):base(objectType, objectId, name)
		{
		}

		#region Properties
		private double _approvedByManager;

		/// <summary>
		/// Gets or sets the approved by manager.
		/// </summary>
		/// <value>The approved by manager.</value>
		/// <remarks>Not null in final state only</remarks>
		public double ApprovedByManager
		{
			get { return _approvedByManager; }
			set { _approvedByManager = value; }
		}

		private double _cost;

		/// <summary>
		/// Gets or sets the cost (Total Approved * Rate).
		/// </summary>
		/// <value>The cost.</value>
		public double Cost
		{
			get { return _cost; }
			set { _cost = value; }
		}
	
		#endregion
	}
}
