//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'MessageHistoryAdminInfo' in the code behind file in 'UserReports\Modules\MessageHistory.ascx.cs' is moved to this file.
//====================================================================




namespace Mediachase.UI.Web.UserReports.Modules
 {

	using System;
	using System.Data;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.DefaultUserReports;
	using Mediachase.UI.Web.UserReports.Util;

	public class MessageHistoryAdminInfo: Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		#region IUserReportInfo Members
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strMessageHistory", typeof(MessageHistoryAdminInfo).Assembly);
		public string Description
		{
			get
			{
				return string.Empty;
			}
		}

		public string ShowName
		{
			get
			{
				return LocRM.GetString("tAdminMesHist");
			}
		}

		#endregion

	}

}