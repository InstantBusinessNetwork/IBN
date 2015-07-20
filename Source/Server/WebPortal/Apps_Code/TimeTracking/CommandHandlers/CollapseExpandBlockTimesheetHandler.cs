using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class CollapseExpandBlockTimesheetHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (cp.CommandArguments["groupType"] == null || cp.CommandArguments["primaryKeyId"] == null)
					throw new ArgumentException("Some expected parameters are null for CollapseExpandBlockTimesheetHandler");

				string ViewName = CHelper.GetFromContext("MetaViewName").ToString();

				if (ViewName != null && ViewName != string.Empty)
				{
					MetaView CurrentView = Mediachase.Ibn.Data.DataContext.Current.MetaModel.MetaViews[ViewName];
					if (CurrentView == null)
						throw new ArgumentException(String.Format("Cant find MetaView: {0}", ViewName));

					McMetaViewPreference mvPref = Mediachase.Ibn.Core.UserMetaViewPreference.Load(CurrentView, Mediachase.IBN.Business.Security.CurrentUser.UserID);
					if (cp.CommandArguments["groupType"] == MetaViewGroupByType.Primary.ToString())
					{
						MetaViewGroupUtil.CollapseOrExpand(MetaViewGroupByType.Primary, mvPref, cp.CommandArguments["primaryKeyId"]);
					}
					else
					{
						MetaViewGroupUtil.CollapseOrExpand(MetaViewGroupByType.Secondary, mvPref, cp.CommandArguments["primaryKeyId"]);
					}

					CHelper.RequireBindGrid();
					CHelper.AddToContext("DontShowEditPopup", 1);
					UserMetaViewPreference.Save(Mediachase.IBN.Business.Security.CurrentUser.UserID, mvPref);
				}
			}
		}

		#endregion
	}
}
