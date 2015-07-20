using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Mediachase.Schedule;
using Mediachase.IBN;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.EMail;
using Mediachase.Net.Mail;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn;

namespace Mediachase.IBN.Business.EMail
{
	/// <summary>
	/// Summary description for XIbnHeaderCommand.
	/// </summary>
	public class XIbnHeaderCommand
	{
		private XIbnHeaderCommand()
		{
		}

		public static void Unknown(int IncidentId, string Value)
		{
		}

		public static void SetPrivateStatus(int ThreadNodeId)
		{
			ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(ThreadNodeId);

			if(settings.Contains(ForumThreadNodeSetting.Outgoing))
				settings.Remove(ForumThreadNodeSetting.Outgoing);
			if(settings.Contains(ForumThreadNodeSetting.Incoming))
				settings.Remove(ForumThreadNodeSetting.Incoming);

			settings.Add(ForumThreadNodeSetting.Internal, "1");
		}

		public static void SetResolutionStatus(int ThreadNodeId)
		{
			ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(ThreadNodeId);

			settings.Add(ForumThreadNodeSetting.Resolution, "1");
		}

		public static void SetWorkaroundStatus(int ThreadNodeId)
		{
			ForumThreadNodeSettingCollection settings = new ForumThreadNodeSettingCollection(ThreadNodeId);

			settings.Add(ForumThreadNodeSetting.Workaround, "1");
		}

		public static void SetIncidentState(ForumStorage.NodeType nodeType, IncidentBoxDocument incidentBoxDocument, int IncidentId, int ThreadNodeId, ObjectStates state)
		{
			Issue2.SetStateByEmail(nodeType, incidentBoxDocument, IncidentId, ThreadNodeId, state);
		}

		public static void ChangeResponsible(int incidentId, int responsibleId)
		{
			try
			{
				DataTable responsibleGroup = Incident.GetResponsibleGroupDataTable(incidentId);
				responsibleGroup.Columns.Add(new DataColumn("IsNew", typeof(bool)));

				foreach (DataRow row in responsibleGroup.Rows)
				{
					row["IsNew"] = false;
				}

				if (responsibleId == -1) // NotSetResponsibleId
				{
					Issue2.UpdateQuickTracking(incidentId, null, -1, false, responsibleGroup);
				}
				else if (responsibleId == -2) // GroupResponsibleId
				{
					Issue2.UpdateQuickTracking(incidentId, null, -1, true, responsibleGroup);
				}
				else
				{
					Issue2.UpdateQuickTracking(incidentId, null, responsibleId, false, responsibleGroup);
				}
			}
			catch (AccessDeniedException)
			{
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.ToString());
			}
		}

	}
}
