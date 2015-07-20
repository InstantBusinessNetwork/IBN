using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.CommandHandlers
{
	public class WFInstanceSaveAsTemplateEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		/// <summary>
		/// Determines whether the specified sender is enable.
		/// </summary>
		/// <param name="Sender">The sender.</param>
		/// <param name="Element">The element.</param>
		/// <returns>
		/// 	<c>true</c> if the specified sender is enable; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEnable(object Sender, object Element)
		{
			// SaveAsTemplate action is available for: Admin, PPM, Exec and project manager
			bool retval = false;

			if (Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				||
				Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)
				||
				Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.Administrator))
			{
				retval = true;
			}
			else if (HttpContext.Current.Items.Contains("OwnerTypeId") && HttpContext.Current.Items.Contains("OwnerId"))
			{
				int ownerTypeId = (int)HttpContext.Current.Items["OwnerTypeId"];
				int ownerId = (int)HttpContext.Current.Items["OwnerId"];

				int projectId = -1;

				if (ownerTypeId == (int)ObjectTypes.Document)
					projectId = Document.GetProject(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.Task)
					projectId = Task.GetProject(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.ToDo)
					projectId = ToDo.GetProject(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.Issue)
					projectId = Incident.GetProject(ownerId);

				if (projectId > 0)
				{
					retval = Project.CanUpdate(projectId);
				}
			}

			return retval;
		}
		#endregion
	}
}
