using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Apps.MetaUI.Grid;

namespace Mediachase.Ibn.Web.UI.Administration.CommandHandlers
{
	public class DeleteSelectedCustomizationProfileHandler : ICommand
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
							string className = parts[1];

							if ((int)key == -1)	// we mustn't delete default profile
							{
								errorCount++;
								continue;
							}

							try
							{
								BusinessManager.Delete(className, key);
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
