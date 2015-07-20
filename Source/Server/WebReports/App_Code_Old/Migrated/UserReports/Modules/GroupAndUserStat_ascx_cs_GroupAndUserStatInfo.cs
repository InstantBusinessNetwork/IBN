//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'GroupAndUserStatInfo' in the code behind file in 'UserReports\Modules\GroupAndUserStat.ascx.cs' is moved to this file.
//====================================================================




namespace Mediachase.UI.Web.UserReports.Modules
 {

	using System;
	using System.Data;
	using System.Text;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.UI.Web.UserReports.Util;
	using System.Globalization;
	using ComponentArt.Web.UI;
	using Mediachase.IBN.DefaultUserReports;

	public class GroupAndUserStatInfo: Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		#region IUserReportInfo Members
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strGroupAndUserStat", typeof(GroupAndUserStatInfo).Assembly);
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
				return LocRM.GetString("tGrAndUs");
			}
		}

		#endregion

	}

}