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
using System.Collections;
using System.Globalization;
using Mediachase.IBN.Business.EMail;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.CommandHandlers
{
	public class AddToExistSelectedPendingHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				int issueId = int.Parse(cp.CommandArguments["SelectedValue"]);

				if (cp.CommandArguments.ContainsKey("GridId"))
				{
					string[] checkedElems = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);
				
					ArrayList alIds = new ArrayList();
					foreach (string elem in checkedElems)
					{
						int id = Convert.ToInt32(elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
						if (id > 0)
						{
							alIds.Add(id);
						}
					}

					EMailMessage.CopyToIncident(alIds, issueId);
				}
				else if (cp.CommandArguments.ContainsKey("ObjectId"))
				{
					int emailMessageId = int.Parse(cp.CommandArguments["ObjectId"]);

					EMailMessage.CopyToIncident(emailMessageId, issueId);
				}

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
