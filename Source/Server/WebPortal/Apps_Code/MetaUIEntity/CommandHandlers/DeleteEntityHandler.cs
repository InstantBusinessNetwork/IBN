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
	public class DeleteEntityHandler : ICommand
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
					((Control)Sender).Page.Response.Redirect(CHelper.GetLinkEntityList(className));
			}
		}

		#endregion
	}
}
