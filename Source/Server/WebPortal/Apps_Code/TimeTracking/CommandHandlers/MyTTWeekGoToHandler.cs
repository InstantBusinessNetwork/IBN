using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class MyTTWeekGoToHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				if (cp.CommandArguments == null)
					return;
				DateTime dtStart = DateTime.Parse(cp.CommandArguments["primaryKeyId"], CultureInfo.InvariantCulture);

				MetaView currentView = DataContext.Current.MetaModel.MetaViews["TT_MyGroupByWeekProject"];
				McMetaViewPreference pref = CHelper.GetMetaViewPreference(currentView);
				pref.SetAttribute<DateTime>("TTFilter_DTCWeek", "TTFilter_DTCWeek", dtStart);
				Mediachase.Ibn.Core.UserMetaViewPreference.Save(Mediachase.IBN.Business.Security.CurrentUser.UserID, pref);

				((System.Web.UI.Control)(Sender)).Page.Response.Redirect("~/Apps/TimeTracking/Pages/Public/ListTimeTrackingNew.aspx?ViewName=TT_MyGroupByWeekProject", true);
			}
		}

		#endregion
	}
}
