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
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using Mediachase.IBN.Business.Documents;

namespace Mediachase.Ibn.Web.UI.DocumentManagement.CommandHandlers
{
	public class StateDraftHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
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
							DocumentContentVersionEntity dcve = (DocumentContentVersionEntity)BusinessManager.Load(DocumentContentVersionEntity.GetAssignedMetaClassName(), key);
							dcve.State = (int)DocumentContentVersionState.Draft;
							UpdateStateRequest usr = new UpdateStateRequest(dcve);
							BusinessManager.Execute(usr);
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

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
