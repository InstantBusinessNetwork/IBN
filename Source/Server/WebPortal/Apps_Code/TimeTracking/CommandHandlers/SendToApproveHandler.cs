using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.UI.Web.Apps.MetaUI.Grid;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class SendToApproveHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				string[] elems = null;
				if (cp.CommandArguments.ContainsKey("GridId"))
					elems = MetaGridServer.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

				List<int> ttbIds = new List<int>();

				//fix by DV: 2008-05-14 
				//Esli vyzyvaetsa s grida (iconka v otklonennih listah), to primitiv v gride ustanovit etot flag
				//i togda ID budet bratsa iz parametrov, v protivnom sluchae ID - beretsa iz checkboxes
				if (!cp.CommandArguments.ContainsKey("callFromGrid"))
				{
					foreach (string elem in elems)
					{
						string type = MetaViewGroupUtil.GetMetaTypeFromUniqueKey(elem);// elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1];
						if (type == MetaViewGroupUtil.keyValueNotDefined)
							continue;

						int id = Convert.ToInt32(MetaViewGroupUtil.GetIdFromUniqueKey(elem), CultureInfo.InvariantCulture); //Convert.ToInt32(elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
						

						if (type == TimeTrackingEntry.GetAssignedMetaClass().Name)
						{
							//TimeTrackingEntry tte = MetaObjectActivator.CreateInstance<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), id);

							//if (!ttbIds.Contains(tte.ParentBlockId))
							//    ttbIds.Add(tte.ParentBlockId);
						}
						else
						{
							if (!ttbIds.Contains(id))
								ttbIds.Add(id);
						}
					}
				}
				else
				{
					ttbIds.Add(Convert.ToInt32(MetaViewGroupUtil.GetIdFromUniqueKey(cp.CommandArguments["primaryKeyId"]), CultureInfo.InvariantCulture));
				}

				using (TransactionScope tran = DataContext.Current.BeginTransaction())
				{
					foreach (int ttbId in ttbIds)
					{
						TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), ttbId);

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
					tran.Commit();
				}
 			}
		}
		#endregion
	}
}
