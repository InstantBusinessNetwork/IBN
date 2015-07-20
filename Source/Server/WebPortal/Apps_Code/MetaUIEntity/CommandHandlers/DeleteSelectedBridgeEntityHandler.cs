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

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.CommandHandlers
{
	public class DeleteSelectedBridgeEntityHandler : ICommand
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
					string bridgeClassName = CHelper.GetFromContext("ReferenceNN_BridgeClassName").ToString();
					string field1Name = CHelper.GetFromContext("ReferenceNN_Field1Name").ToString();
					string field2Name = CHelper.GetFromContext("ReferenceNN_Field2Name").ToString();
					PrimaryKeyId field1Value = PrimaryKeyId.Parse(CHelper.GetFromContext("ReferenceNN_Field1Value").ToString());
					using (TransactionScope tran = DataContext.Current.BeginTransaction())
					{
						foreach (string elem in selectedElements)
						{
							string[] parts = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
							string id = parts[0];
							PrimaryKeyId key = PrimaryKeyId.Parse(id);
							string className = parts[1];

							FilterElementCollection fec = new FilterElementCollection();
							fec.Add(FilterElement.EqualElement(field1Name, field1Value));
							fec.Add(FilterElement.EqualElement(field2Name, key));
							EntityObject[] list = BusinessManager.List(bridgeClassName, fec.ToArray());
							if (list.Length > 0)
								foreach (EntityObject eo in list)
								{
									try
									{
										BusinessManager.Delete(bridgeClassName, eo.PrimaryKeyId.Value);
									}
									catch (Exception ex)
									{
										CHelper.GenerateErrorReport(ex);
										errorCount++;
									}
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
