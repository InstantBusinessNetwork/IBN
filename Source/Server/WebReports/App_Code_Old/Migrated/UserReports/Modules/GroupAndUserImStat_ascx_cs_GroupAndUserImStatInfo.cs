//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'GroupAndUserImStatInfo' in the code behind file in 'UserReports\Modules\GroupAndUserImStat.ascx.cs' is moved to this file.
//====================================================================




namespace Mediachase.UI.Web.UserReports.Modules
 {

	using System;
	using System.Data;
	using System.Text;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.UI.Web.UserReports.Util;
	using System.Globalization;
	using Mediachase.UI.Web.UserReports.GlobalModules.PageTemplateExtension;
	using Mediachase.IBN.DefaultUserReports;
	 using Mediachase.Ibn;

	public class GroupAndUserImStatInfo: Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		#region IUserReportInfo Members
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strGroupAndUserStat", typeof(GroupAndUserImStatInfo).Assembly);
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
				return String.Format(LocRM.GetString("GroupAndUserIM"), IbnConst.ProductFamilyShort);
			}
		}

		#endregion

	}

}