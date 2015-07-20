using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Shell.CommandHandlers
{
	public class ActiveEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool isVisible = false;

			DataTable dt;
			// Projects
			if (Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
			{
				dt = Project.GetListActiveProjectsByUserOnlyDataTable();
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Todo and Tasks
			if (!isVisible)
			{
				dt = ToDo.GetListActiveToDoAndTasksByUserOnlyDataTable();
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Incidents
			if (!isVisible && Mediachase.IBN.Business.Configuration.HelpDeskEnabled)
			{
				dt = Incident.GetListOpenIncidentsByUserOnlyDataTable();
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Documents
			if (!isVisible)
			{
				dt = Document.GetListActiveDocumentsByUserOnlyDataTable();
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			return isVisible;
		}
		#endregion
	}
}
