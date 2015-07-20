using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;


namespace Mediachase.Ibn.Web.UI.ReportManagement.CommandHandlers
{
	public class CustomSqlReportEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			return SqlReport.AskCustomSqlReportAccess();
		}

		#endregion
	}
}
