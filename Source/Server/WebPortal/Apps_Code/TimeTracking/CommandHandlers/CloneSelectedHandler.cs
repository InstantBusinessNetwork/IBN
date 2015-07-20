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
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Core;

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class CloneSelectedHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string[] elems = MetaGridServer.GetCheckedCollection(((CommandManager)Sender).Page, cp.CommandArguments["GridId"]);

				DateTime weekStart = CHelper.GetRealWeekStartByDate(DateTime.Now).Date;
				List<int> entryIds = new List<int>();

				foreach (string elem in elems)
				{
					//int id = Convert.ToInt32(elem, CultureInfo.InvariantCulture);
					string type = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1];
					if (type == MetaViewGroupUtil.keyValueNotDefined)
						continue; 

					int id = Convert.ToInt32(elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
					

					if (type == TimeTrackingEntry.GetAssignedMetaClass().Name)
					{
						//if clone current week then abort
						TimeTrackingEntry tte = MetaObjectActivator.CreateInstance<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), id);
						if (weekStart == tte.StartDate)
						{
							throw new ArgumentException(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_UnableToCloneMsg}"));
						}

						entryIds.Add(id);
					}
				}

				if (entryIds.Count > 0)
					TimeTrackingManager.AddEntries(weekStart, Mediachase.IBN.Business.Security.CurrentUser.UserID, entryIds);
			}
		}
		#endregion
	}
}
