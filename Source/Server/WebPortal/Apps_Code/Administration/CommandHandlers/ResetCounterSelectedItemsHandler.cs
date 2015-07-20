using System;
using System.Collections.Generic;
using System.Web;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Business.Messages;

namespace Mediachase.Ibn.Web.UI.Administration.CommandHandlers
{
	public class ResetCounterSelectedItemsHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string[] selectedElements = EntityGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

				if (selectedElements != null && selectedElements.Length > 0)
				{
					int errorCount = 0;
					using (TransactionScope tran = DataContext.Current.BeginTransaction())
					{
						foreach (string elem in selectedElements)
						{
							string[] parts = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
							string id = parts[0];
							PrimaryKeyId key = PrimaryKeyId.Parse(id);

							try
							{
								ResetDeliveryAttemptsRequest req = new ResetDeliveryAttemptsRequest(key);
								BusinessManager.Execute(req);
							}
							catch (Exception ex)
							{
								CHelper.GenerateErrorReport(ex);
								errorCount++;
							}
						}

						tran.Commit();
					}

					if (errorCount > 0)
						((CommandManager)Sender).InfoMessage = CHelper.GetResFileString("{IbnFramework.Common:NotAllSelectedItemsWereProcessed}");
				}
			}
			CHelper.RequireDataBind();
		}

		#endregion
	}
}
