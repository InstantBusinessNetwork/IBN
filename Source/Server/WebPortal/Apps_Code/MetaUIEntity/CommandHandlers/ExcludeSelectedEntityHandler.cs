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
	public class ExcludeSelectedEntityHandler : ICommand
	{
		private const string _httpContextFilterFieldNameKey = "Reference1N_FieldName";

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

							try
							{
								EntityObject obj = BusinessManager.Load(className, key);

								string fieldName = (string)CHelper.GetFromContext(_httpContextFilterFieldNameKey);
								obj.Properties[fieldName].Value = null;
								BusinessManager.Update(obj);
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
