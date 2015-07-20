using System;
using System.Data;


namespace Mediachase.IBN.Database
{
	public class DbProject2
	{
		#region UpdateActualFinishDate()
		public static int UpdateActualFinishDate(int projectId, DateTime date)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateActualFinishDate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@Date", SqlDbType.DateTime, date));
		}
		#endregion

		#region UpdateActualStartDate()
		public static int UpdateActualStartDate(int projectId, DateTime date)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateActualStartDate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@Date", SqlDbType.DateTime, date));
		}
		#endregion

		#region UpdateClient()
		public static int UpdateClient(int projectId, object contactUid, object orgUid)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateClient",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ContactUid", SqlDbType.UniqueIdentifier, contactUid),
				DbHelper2.mp("@orgUid", SqlDbType.UniqueIdentifier, orgUid));
		}
		#endregion

		#region UpdateConfigurationInfo()
		public static int UpdateConfigurationInfo(int projectId, int calendarId, int currencyId, int typeId, int initialPhaseId, int blockTypeId)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateConfigurationInfo",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@CalendarId", SqlDbType.Int, calendarId),
				DbHelper2.mp("@CurrencyId", SqlDbType.Int, currencyId),
				DbHelper2.mp("@TypeId", SqlDbType.Int, typeId), 
				DbHelper2.mp("@InitialPhaseId", SqlDbType.Int, initialPhaseId),
				DbHelper2.mp("@BlockTypeId", SqlDbType.Int, blockTypeId));
		}
		#endregion

		#region UpdateExecutiveManager()
		public static void UpdateExecutiveManager(int projectId, int valueId)
		{
			object oValueId = valueId > 0 ? (object)valueId : null;

			DbHelper2.RunSp("Act_ProjectUpdateExecutiveManager",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, oValueId));
		}
		#endregion

		#region UpdateGeneralInfo()
		public static void UpdateGeneralInfo(int projectId,
			string title,
			string description,
			string goals,
			string scope,
			string deliverables)
		{
			DbHelper2.RunSp("Act_ProjectUpdateGeneralInfo",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, title),
				DbHelper2.mp("@Description", SqlDbType.NText, description),
				DbHelper2.mp("@Goals", SqlDbType.NText, goals),
				DbHelper2.mp("@Scope", SqlDbType.NText, scope),
				DbHelper2.mp("@Deliverables", SqlDbType.NText, deliverables));
		}
		#endregion

		#region UpdateManager()
		public static void UpdateManager(int projectId, int valueId)
		{
			DbHelper2.RunSp("Act_ProjectUpdateManager",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdatePhase()
		public static int UpdatePhase(int projectId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdatePhase",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdatePriority()
		public static int UpdatePriority(int projectId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdatePriority",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdateProgress()
		public static int UpdateProgress(int projectId, int value)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateProgress",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@Value", SqlDbType.Int, value));
		}
		#endregion

		#region UpdateStatus()
		public static int UpdateStatus(int projectId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateStatus",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdateRiskLevel()
		public static int UpdateRiskLevel(int projectId, int valueId)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateRiskLevel",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdateTargetDates()
		/// <summary>
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="startDate"></param>
		/// <param name="finishDate"></param>
		/// <returns>
		///		0 if no dates were changed
		///		1 if only startDate was changed
		///		2 if only finishDate was changed
		///		3 if both startDate and finishDate were changed
		/// </returns>
		public static int UpdateTargetDates(int projectId, DateTime startDate, DateTime finishDate)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateTargetDates",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, finishDate));
		}
		#endregion

		#region AddTeamMember
		public static void AddTeamMember(int projectId, int principalId, string code, decimal rate)
		{
			DbHelper2.RunSp("ProjectMemberAddMember",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@Code", SqlDbType.NChar, 2, code),
				DbHelper2.mp("@Rate", SqlDbType.Money, rate));
		}
		#endregion

		#region UpdateTeamMember
		public static int UpdateTeamMember(int projectId, int principalId, string code, decimal rate)
		{
			return DbHelper2.RunSpInteger("Act_ProjectMemberUpdateTeamMember",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@Code", SqlDbType.NChar, 2, code),
				DbHelper2.mp("@Rate", SqlDbType.Money, rate));
		}
		#endregion

		#region UpdateMsProjectResourceId
		//Act_ProjectMemberUpdateMsProjectResourceId
		public static int UpdateMsProjectResourceId(int projectId, int principalId, int msProjectResId)
		{
			return DbHelper2.RunSpInteger("Act_ProjectMemberUpdateMsProjectResourceId",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MsProjectResourceId", SqlDbType.Int, msProjectResId));
		}
		#endregion

		#region UpdateCalendar()
		public static void UpdateCalendar(int projectId, int valueId)
		{
			DbHelper2.RunSp("Act_ProjectUpdateCalendar",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ValueId", SqlDbType.Int, valueId));
		}
		#endregion

		#region UpdateProjectCode()
		/// <summary>
		/// Updates the project code.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="projectCode">The project code.</param>
		/// <returns></returns>
		public static int UpdateProjectCode(int projectId, string projectCode)
		{
			return DbHelper2.RunSpInteger("Act_ProjectUpdateProjectCode",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@ProjectCode", SqlDbType.NVarChar, 250, projectCode));
		}
		#endregion
	}
}
