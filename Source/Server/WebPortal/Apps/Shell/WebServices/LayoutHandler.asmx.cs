using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;

using Mediachase.IBN.Business.WidgetEngine;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;
using System.Collections.Generic;
using Mediachase.Ibn.Web.UI.Apps.WidgetEngine;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.WebServices
{
	/// <summary>
	/// Summary description for LayoutHandler
	/// </summary>
	[WebService(Namespace = "http://ibn46.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	[ScriptService]
	public class LayoutHandler : System.Web.Services.WebService
	{
		[WebMethod]
		public void OrderChange(string newLayout, string contextKey)
		{
			LayoutContextKey key = UtilHelper.JsonDeserialize<LayoutContextKey>(contextKey);

			if (!key.IsAdmin)
			{
				key.ProfileId = null;
				key.UserId = Mediachase.IBN.Business.Security.CurrentUser.UserID;
			}

			CustomPageEntity page = CustomPageManager.GetCustomPage(key.PageUid, key.ProfileId, key.UserId);
			if (page == null)
				throw new ArgumentException(String.Format("Cant read setting for page: {0}", key.PageUid));

			if (!(key.IsAdmin || Mediachase.Ibn.Business.Customization.ProfileManager.CheckPersonalization()))
			{
				UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				pc["userCollapseExpand_" + key.PageUid.ToString("N")] = newLayout;
			}
			else
			{
				CustomPageManager.UpdateCustomPage(key.PageUid, newLayout, page.TemplateId, key.ProfileId, key.UserId);
			}
		}

		[WebMethod]
		public void Delete(string newLayout, string contextKey)
		{
			//ToDo: Clear ControlProperties collection for deleted node (do it in provider)
			LayoutContextKey key = UtilHelper.JsonDeserialize<LayoutContextKey>(contextKey);

			if (!key.IsAdmin)
			{
				key.ProfileId = null;
				key.UserId = Mediachase.IBN.Business.Security.CurrentUser.UserID;
			}

			CustomPageEntity page = CustomPageManager.GetCustomPage(key.PageUid, key.ProfileId, key.UserId);
			if (page == null)
				throw new ArgumentException(String.Format("Cant read setting for page: {0}", key.PageUid));


			if (!key.IsAdmin)
			{
				List<CpInfo> list = UtilHelper.JsonDeserialize<List<CpInfo>>(page.JsonData);
				foreach (CpInfo item in list)
				{
					foreach (CpInfoItem internalItem in item.Items)
					{
						if (!newLayout.Contains(internalItem.InstanseUid))
						{
							//ToDo: make method to get "wrapControl{0}_{1}"
							ControlProperties.Provider.DeleteValue(String.Format("wrapControl{0}_{1}", internalItem.Id.ToLower().Replace("-", ""), internalItem.InstanseUid));
						}
					}
				}
			}

			CustomPageManager.UpdateCustomPage(key.PageUid, newLayout, page.TemplateId, key.ProfileId, key.UserId);
		}
	}
}
