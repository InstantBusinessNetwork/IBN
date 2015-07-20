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

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class RelatedProjectAddHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				int prjId = int.Parse(cp.CommandArguments["ObjectId"]);

				if (cp.CommandArguments.ContainsKey("SelectedValue"))
				{
					string[] elemsToAdd = cp.CommandArguments["SelectedValue"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
					using (DbTransaction tran = DbTransaction.Begin())
					{
						foreach (string elem in elemsToAdd)
						{
							int selectedValue = 0;
							if (int.TryParse(elem, out selectedValue))
								Project2.AddRelation(prjId, selectedValue);
						}
						tran.Commit();
					}
				}

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
