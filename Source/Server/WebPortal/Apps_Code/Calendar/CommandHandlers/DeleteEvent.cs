using System;
using System.Collections.Generic;
using System.Web;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core.Business;
using System.Web.UI;

namespace Mediachase.Ibn.Web.UI.Calendar.CommandHandlers
{
	public class DeleteEvent : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			object objectid = CHelper.GetFromContext("ObjectId");
			object classNameObject = CHelper.GetFromContext("ClassName");
			if (objectid != null && classNameObject != null)
			{
				PrimaryKeyId key = (PrimaryKeyId)objectid;
				string className = (string)classNameObject;

				int errorCount = 0;
				try
				{
					BusinessManager.Delete(className, key);
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
