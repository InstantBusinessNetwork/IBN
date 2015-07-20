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
using Mediachase.Ibn.Events;

namespace Mediachase.Ibn.Web.UI.Calendar.CommandHandlers
{
	public class DeleteEventSeries : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			string objectid = ((Control)Sender).Page.Request["ObjectId"];

			if (!String.IsNullOrEmpty(objectid))
			{
				PrimaryKeyId key = PrimaryKeyId.Parse(objectid);
				key = ((VirtualEventId)key).RealEventId;

				int errorCount = 0;
				try
				{
					BusinessManager.Delete(CalendarEventEntity.ClassName, key);
				}
				catch (Exception ex)
				{
					CHelper.GenerateErrorReport(ex);
					errorCount++;
				}

				if (errorCount > 0)
					((CommandManager)Sender).InfoMessage = CHelper.GetResFileString("{IbnFramework.Common:ActionWasNotProcessed}");
				else
					((Control)Sender).Page.Response.Redirect("~/Apps/Calendar/Pages/Calendar.aspx", true);
			}
		}

		#endregion
	}
}
