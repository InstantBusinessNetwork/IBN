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
using Mediachase.UI.Web.Apps.MetaUI.Grid;
using System.Globalization;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class MyTTWeekApproveSelectedHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				string[] elems = MCGrid.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

				using (TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					foreach (string elem in elems)
					{
						DateTime startDate = DateTime.Parse(elem, CultureInfo.InvariantCulture);
						TimeTrackingBlock[] mas = TimeTrackingBlock.List(FilterElement.EqualElement("OwnerId", Mediachase.IBN.Business.Security.CurrentUser.UserID), FilterElement.EqualElement("StartDate", startDate));
						foreach (TimeTrackingBlock ttb in mas)
						{
							StateMachineService sms = ((BusinessObject)ttb).GetService<StateMachineService>();

							// process only initial state
							if (sms.StateMachine.GetStateIndex(sms.CurrentState.Name) == 0)
							{
								StateTransition[] availableTransitions = sms.GetNextAvailableTransitions(true);
								if (availableTransitions.Length > 0)
								{
									sms.MakeTransition(availableTransitions[0].Uid);
									ttb.Save();
								}
							}
						}
					}
					tran.Commit();
				}

				CHelper.RequireBindGrid();
			}
		}

		#endregion
	}
}
