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
using Mediachase.IBN.Database;
using Mediachase.IBN.Business;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class UpPrioritySelectedItemsHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string[] elemsToDelete = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

				int error = 0;
				using (DbTransaction tran = DbTransaction.Begin())
				{
					foreach (string elem in elemsToDelete)
					{
						int id = Convert.ToInt32(elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
						if (id > 0)
						{
							try
							{
								Project2.UpPriority(id);
							}
							catch (AccessDeniedException)
							{
								error++;
							}
						}
					}
					tran.Commit();
				}
				if (error > 0)
				{
					ClientScript.RegisterStartupScript(((Control)Sender).Page, ((Control)Sender).Page.GetType(), Guid.NewGuid().ToString("N"),
						String.Format("alert('{0}');", CHelper.GetResFileString("{IbnFramework.ListInfo:RefItemException}")), true);
				}

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
