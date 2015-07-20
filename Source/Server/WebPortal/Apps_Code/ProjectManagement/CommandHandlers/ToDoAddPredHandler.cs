using System;
using System.Collections.Generic;
using System.Web;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Database;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class ToDoAddPredHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				int todoId = int.Parse(cp.CommandArguments["ObjectId"]);

				if (cp.CommandArguments.ContainsKey("SelectedValue"))
				{
					string[] elemsToAdd = cp.CommandArguments["SelectedValue"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
					using (DbTransaction tran = DbTransaction.Begin())
					{
						foreach (string elem in elemsToAdd)
						{
							int selectedValue = 0;
							if (int.TryParse(elem, out selectedValue))
							{
								if (ToDo.CanUpdate(selectedValue))
									ToDo.CreateToDoLink(selectedValue, todoId);
							}
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
