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
using System.Globalization;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class RelateSelectedProjectsHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string[] elemsToDelete = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);
				if (elemsToDelete.Length > 1)
				{
					using (DbTransaction tran = DbTransaction.Begin())
					{
						for (int i = 0; i < elemsToDelete.Length - 1; i++)
							for (int j = i + 1; j < elemsToDelete.Length; j++)
							{
								int id1 = Convert.ToInt32(elemsToDelete[i].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
								int id2 = Convert.ToInt32(elemsToDelete[j].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
								if (id1 > 0 && id2 > 0)
									Project2.AddRelation(id1, id2);
							}

						tran.Commit();
					}

					CHelper.RequireBindGrid();
				}
			}
		}

		#endregion
	}
}
