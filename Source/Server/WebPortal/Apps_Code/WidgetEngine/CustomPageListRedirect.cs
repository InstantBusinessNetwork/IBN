using System;
using System.Collections.Generic;
using System.Web;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Apps_Code.WidgetEngine
{
	public class CustomPageListRedirect : ICommand
	{
		#region ICommand Members

		public void Invoke(object sender, object element)
		{
			if (element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)element;
				//if (!cp.CommandArguments.ContainsKey("ControlId"))
				//    throw new ArgumentNullException("CommandParameters.ControlId @ GoogleGadgetRefresh");

				//string _cid = cp.CommandArguments["ControlId"];
				if (HttpContext.Current.Session["ControlId"] == null)
					throw new ArgumentNullException("Session.ControlId @ GoogleGadgetRefresh");

				string _cid = HttpContext.Current.Session["ControlId"].ToString();
				//string id = MetaViewGroupUtil.GetIdFromUniqueKey(cp.CommandArguments["primaryKeyId"]);
				HttpRequest request = HttpContext.Current.Request;

				if (request != null)
				{
					//GoogleGadgetEntity gge = (GoogleGadgetEntity)BusinessManager.Load("GoogleGadget", PrimaryKeyId.Parse(id));
					//ControlProperties.Provider.SaveValue(_cid, "PageSource", id);

					//if (gge != null)
					//    ControlProperties.Provider.SaveValue(_cid, IbnControlPlace.titleKey, CHelper.GetResFileString(gge.Title));

					CommandParameters cp2 = new CommandParameters("MC_GG_SelectItem");
					Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(((CommandManager)sender).Page, cp2.ToString());
					//((CommandManager)sender).Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "", true);

				}
			}
		}

		#endregion
	}
}
