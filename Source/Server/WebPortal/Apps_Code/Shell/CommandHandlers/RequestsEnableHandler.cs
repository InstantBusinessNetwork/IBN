using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Bus = Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business.EMail;

namespace Mediachase.Ibn.Web.UI.Shell.CommandHandlers
{
	public class RequestsEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool isVisible = false;

			// Pending users
			if (Bus.Security.IsUserInGroup(Bus.InternalSecureGroups.Administrator))
			{
				using (IDataReader reader = Bus.User.GetListPendingUsers())
				{
					if (reader.Read())
						isVisible = true;
				}
			}

			// Not assigned incidents
			if (!isVisible &&
				(Bus.Security.IsUserInGroup(Bus.InternalSecureGroups.PowerProjectManager) ||
				Bus.Security.IsUserInGroup(Bus.InternalSecureGroups.ProjectManager) ||
				Bus.Security.IsUserInGroup(Bus.InternalSecureGroups.ExecutiveManager) ||
				Bus.Security.IsUserInGroup(Bus.InternalSecureGroups.HelpDeskManager)))
			{
				using (IDataReader reader = Bus.Incident.GetListNotAssignedIncidents(0))
				{
					if (reader.Read())
						isVisible = true;
				}
			}

			// Mail incidents
			if (!isVisible)
			{
				int[] pendList = EMailMessage.ListPendigEMailMessageIds();
				if (pendList.Length > 0)
					isVisible = true;
			}

			// Not Approved ToDo And Tasks
			if (!isVisible)
			{
				DataTable dt = Bus.ToDo.GetListNotApprovedToDoAndTasks(0);
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Pending ToDo And Tasks
			if (!isVisible)
			{
				DataTable dt = Bus.ToDo.GetListPendingToDoAndTasks(0);
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Pending Incidents
			if (!isVisible)
			{
				DataTable dt = Bus.Incident.GetListPendingIncidentsDataTable(0);
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Pending Events
			if (!isVisible)
			{
				DataTable dt = Bus.CalendarEntry.GetListPendingEventsDataTable(0);
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// ToDo And Tasks without Resources
			if (!isVisible)
			{
				DataTable dt = Bus.ToDo.GetListToDoAndTasksWithoutResources(0);
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Not Assigned Documents
			if (!isVisible)
			{
				DataTable dt = Bus.Document.GetListNotAssignedDocumentsDataTable(0);
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Pending Documents
			if (!isVisible)
			{
				DataTable dt = Bus.Document.GetListPendingDocumentsDataTable(0);
				if (dt.Rows.Count > 0)
					isVisible = true;
			}

			// Declined Requests
			if (!isVisible)
			{
				using (IDataReader reader = Bus.Common.GetListDeclinedRequests(0))
				{
					if (reader.Read())
						isVisible = true;
				}
			}

			return isVisible;
		}
		#endregion
	}
}
