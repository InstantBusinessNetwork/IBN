using System;
using System.Collections.Generic;
using System.Web;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business;
using System.Web.UI;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.CommandHandlers
{
	public class MarkAsSpamSelectedIssuesHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				bool fl = false;
				if (cp.CommandArguments["action"] == "delete")
					fl = true;

				if (cp.CommandArguments.ContainsKey("GridId"))
				{
					string[] elemsToDelete = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

					int error = 0;
					using (DbTransaction tran = DbTransaction.Begin())
					{
						foreach (string elem in elemsToDelete)
						{
							int id = Convert.ToInt32(elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
							if (id > 0)
							{
								Issue2.MarkAsSpam(id, fl);
							}
						}
						tran.Commit();
					}
					if (error > 0)
					{
					}
					CHelper.RequireBindGrid();
				}
				else if (((Control)Sender).Page.Request["IncidentId"] != null)
				{
					Issue2.MarkAsSpam(int.Parse(((Control)Sender).Page.Request["IncidentId"]), fl);
					if(fl)
						((Control)Sender).Page.Response.Redirect("~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx", true);
					else
						((Control)Sender).Page.Response.Redirect(((Control)Sender).Page.Request.RawUrl, true);
				}
			}
		}

		#endregion
	}
}
