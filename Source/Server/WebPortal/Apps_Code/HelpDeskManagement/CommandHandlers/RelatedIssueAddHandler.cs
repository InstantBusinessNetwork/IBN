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
using Mediachase.IBN.Business;
using Mediachase.IBN.Database;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.CommandHandlers
{
	public class RelatedIssueAddHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				int issueId = int.Parse(cp.CommandArguments["ObjectId"]);

				if (cp.CommandArguments.ContainsKey("SelectedValue"))
				{
					string[] elemsToAdd = cp.CommandArguments["SelectedValue"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
					using (DbTransaction tran = DbTransaction.Begin())
					{
						foreach (string elem in elemsToAdd)
						{
							int selectedValue = 0;
							if(int.TryParse(elem, out selectedValue))
								Issue2.AddRelation(issueId, selectedValue);
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
