//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'ChatHistoryInfo' in the code behind file in 'UserReports\Modules\ChatHistory.ascx.cs' is moved to this file.
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

	public class ChatHistoryInfo: Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		#region IUserReportInfo Members
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strChatHistory", typeof(ChatHistoryInfo).Assembly);
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
				return LocRM.GetString("tChatHist");
			}
		}

		#endregion

	}

}