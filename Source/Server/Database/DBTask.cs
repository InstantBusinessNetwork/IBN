using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBTask.
	/// </summary>
	public class DBTask
	{
		#region Create
		public static int Create(
			int ProjectId, int CreatorId,
			string Title, string Description, 
			DateTime CreationDate, 
			DateTime StartDate, DateTime FinishDate,
			int Duration, int PriorityId, 
			bool IsMilestone,
			int ConstraintTypeId, DateTime ConstraintDate,
			int ActivationTypeId, 
			int CompletionTypeId, 
			bool MustBeConfirmed,
			int PhaseId,
			int TaskTime)
		{
			return DbHelper2.RunSpInteger("TaskCreate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@IsMilestone", SqlDbType.Bit, IsMilestone),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				DbHelper2.mp("@ConstraintDate", SqlDbType.DateTime, ConstraintDate),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime));
		}

		public static int Create(
			int ProjectId, string Title, string Description,
			int CreatorId,DateTime CreationDate, 
			DateTime StartDate, DateTime FinishDate, DateTime ActualFinishDate, int Duration, 
			int PriorityId, int ConstraintTypeId, 
			int ActivationTypeId, int CompletionTypeId, 
			int TaskNum, bool IsMilestone, bool IsSummary,
			string OutlineNumber, int OutlineLevel, int PercentCompleted, int ReasonId)
		{
			return DbHelper2.RunSpInteger("TaskCreateFromExport",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				ActualFinishDate == DateTime.MinValue ? 
					DbHelper2.mp("@ActualFinishDate", SqlDbType.DateTime, null):
					DbHelper2.mp("@ActualFinishDate", SqlDbType.DateTime, ActualFinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@TaskNum", SqlDbType.Int, TaskNum),
				DbHelper2.mp("@IsMilestone", SqlDbType.Bit, IsMilestone),
				DbHelper2.mp("@IsSummary", SqlDbType.Bit, IsSummary),
				DbHelper2.mp("@OutlineNumber", SqlDbType.VarChar, 255, OutlineNumber),
				DbHelper2.mp("@OutlineLevel", SqlDbType.Int, OutlineLevel),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@ReasonId", SqlDbType.Int, ReasonId));
		}
		#endregion

		#region CreateFromExport2
		public static int CreateFromExport2(
			int ProjectId, string Title, string Description,
			int CreatorId, DateTime CreationDate,
			DateTime StartDate, DateTime FinishDate,
			DateTime? ActualStartDate, DateTime? ActualFinishDate, 
			int Duration,
			int PriorityId, int ConstraintTypeId,
			int ActivationTypeId, int CompletionTypeId,
			int TaskNum, bool IsMilestone, bool IsSummary,
			string OutlineNumber, int OutlineLevel, int PercentCompleted,
			int ReasonId, int MSProjectUID)
		{
			return DbHelper2.RunSpInteger("TaskCreateFromExport2",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@ActualStartDate", SqlDbType.DateTime, ActualStartDate),
				DbHelper2.mp("@ActualFinishDate", SqlDbType.DateTime, ActualFinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@TaskNum", SqlDbType.Int, TaskNum),
				DbHelper2.mp("@IsMilestone", SqlDbType.Bit, IsMilestone),
				DbHelper2.mp("@IsSummary", SqlDbType.Bit, IsSummary),
				DbHelper2.mp("@OutlineNumber", SqlDbType.VarChar, 255, OutlineNumber),
				DbHelper2.mp("@OutlineLevel", SqlDbType.Int, OutlineLevel),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@ReasonId", SqlDbType.Int, ReasonId),
				DbHelper2.mp("@MSProjectUID", SqlDbType.Int, MSProjectUID));
		}
		#endregion

		#region Insert
		public static int Insert(
			int ProjectId, int BeforeTaskId, int CreatorId,
			string Title, string Description, 
			DateTime CreationDate, DateTime StartDate,
			DateTime FinishDate, int Duration, int PriorityId, 
			bool IsMilestone,
			int ConstraintTypeId, DateTime ConstraintDate,
			int ActivationTypeId, int CompletionTypeId, 
			bool MustBeConfirmed, int PhaseId, int TaskTime)
		{
			return DbHelper2.RunSpInteger("TaskInsert",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@BeforeTaskId", SqlDbType.Int, BeforeTaskId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@IsMilestone", SqlDbType.Bit, IsMilestone),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				DbHelper2.mp("@ConstraintDate", SqlDbType.DateTime, ConstraintDate),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime));
		}
		#endregion

		#region InsertTo
		public static int InsertTo(
			int ProjectId, int FromTaskId,
			byte Place , int CreatorId,
			string Title, string Description, 
			DateTime CreationDate, DateTime StartDate,
			DateTime FinishDate, int Duration, int PriorityId, 
			bool IsMilestone,
			int ConstraintTypeId, DateTime ConstraintDate,
			int ActivationTypeId, int CompletionTypeId, 
			bool MustBeConfirmed, int PhaseId, int TaskTime)
		{
			return DbHelper2.RunSpInteger("TaskInsertTo",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@FromTaskId", SqlDbType.Int, FromTaskId),
				DbHelper2.mp("@Place", SqlDbType.TinyInt, Place),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@IsMilestone", SqlDbType.Bit, IsMilestone),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				DbHelper2.mp("@ConstraintDate", SqlDbType.DateTime, ConstraintDate),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime));
		}
		#endregion

		#region Update
        public static void Update(
            int TaskId, string Title, string Description,
            DateTime StartDate, DateTime FinishDate, DateTime? ActualStartDate,
			DateTime? ActualFinishDate, int Duration,
            int PriorityId, int ConstraintTypeId,
            int ActivationTypeId, int CompletionTypeId,
            int TaskNum, bool IsMilestone, bool IsSummary,
            string OutlineNumber, int OutlineLevel, int PercentCompleted,
            int ReasonId, int MSProjectUID)
        {
            DbHelper2.RunSp("TaskUpdate2",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@ActualFinishDate", SqlDbType.DateTime, ActualFinishDate),
				DbHelper2.mp("@ActualStartDate", SqlDbType.DateTime, ActualStartDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@TaskNum", SqlDbType.Int, TaskNum),
				DbHelper2.mp("@IsMilestone", SqlDbType.Bit, IsMilestone),
				DbHelper2.mp("@IsSummary", SqlDbType.Bit, IsSummary),
				DbHelper2.mp("@OutlineNumber", SqlDbType.VarChar, 255, OutlineNumber),
				DbHelper2.mp("@OutlineLevel", SqlDbType.Int, OutlineLevel),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@ReasonId", SqlDbType.Int, ReasonId),
				DbHelper2.mp("@MSProjectUID", SqlDbType.Int, MSProjectUID)
				);
        }

		public static void Update(
			int TaskId, 
			string Title, string Description, 
			DateTime StartDate, DateTime FinishDate,
			int Duration, int PriorityId, 
			bool IsMilestone,	
			int ConstraintTypeId, DateTime ConstraintDate,
			int ActivationTypeId, int CompletionTypeId, 
			bool MustBeConfirmed,
			int PhaseId, int TaskTime)
		{
			DbHelper2.RunSp("TaskUpdate",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DbHelper2.mp("@Description", SqlDbType.NText, Description),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@PriorityId", SqlDbType.Int, PriorityId),
				DbHelper2.mp("@IsMilestone", SqlDbType.Bit, IsMilestone),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				ConstraintDate == DateTime.MinValue ? 
					DbHelper2.mp("@ConstraintDate", SqlDbType.DateTime, null):
					DbHelper2.mp("@ConstraintDate", SqlDbType.DateTime, ConstraintDate),
				DbHelper2.mp("@ActivationTypeId", SqlDbType.Int, ActivationTypeId),
				DbHelper2.mp("@CompletionTypeId", SqlDbType.Int, CompletionTypeId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@PhaseId", SqlDbType.Int, PhaseId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime));
		}

		public static void Update(
			int TaskId, DateTime StartDate, 
			DateTime FinishDate, int Duration, 
			int ConstraintTypeId, DateTime ConstraintDate)
		{
			DbHelper2.RunSp("TaskUpdateDates",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Duration", SqlDbType.Int, Duration),
				DbHelper2.mp("@ConstraintTypeId", SqlDbType.Int, ConstraintTypeId),
				ConstraintDate == DateTime.MinValue ? 
					DbHelper2.mp("@ConstraintDate", SqlDbType.DateTime, null):
					DbHelper2.mp("@ConstraintDate", SqlDbType.DateTime, ConstraintDate));
		}
		#endregion

		#region Delete
		public static void Delete(int TaskId, string UpdateId)
		{
			DbHelper2.RunSp("TaskDelete",
				DbHelper2.mp("@UpdateId", SqlDbType.Char, 36, UpdateId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region CreateTaskLink
		public static int CreateTaskLink(int PredecessorId, int SuccessorId, int Lag)
		{
			return DbHelper2.RunSpInteger("TaskLinkCreate",
				DbHelper2.mp("@PredId", SqlDbType.Int, PredecessorId),
				DbHelper2.mp("@SuccId", SqlDbType.Int, SuccessorId),
				DbHelper2.mp("@Lag", SqlDbType.Int, Lag));
		}
		#endregion

		#region UpdateTaskLink
		public static int UpdateTaskLink(int LinkId, int Lag)
		{
			return DbHelper2.RunSpInteger("TaskLinkUpdate",
				DbHelper2.mp("@LinkId", SqlDbType.Int, LinkId),
				DbHelper2.mp("@Lag", SqlDbType.Int, Lag));
		}
		#endregion

		#region DeleteTaskLink
		public static int DeleteTaskLink(int LinkId)
		{
			return DbHelper2.RunSpInteger("TaskLinkDelete",
				DbHelper2.mp("@LinkId", SqlDbType.Int, LinkId));
		}
		#endregion

		#region DeleteTaskLink
		public static int DeleteTaskLink(int PredId, int SuccId)
		{
			return DbHelper2.RunSpInteger("TaskLinkDeleteByPredSucc",
				DbHelper2.mp("@PredId", SqlDbType.Int, PredId),
				DbHelper2.mp("@SuccId", SqlDbType.Int, SuccId));
		}
		#endregion

		#region TaskInAllChild
		public static bool TaskInAllChild(int TaskId, int ChildTaskId)
		{
			int retval = DbHelper2.RunSpInteger("TaskInAllChild",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@ChildTaskId", SqlDbType.Int, ChildTaskId));
			return (retval > 0)?true:false;
		}
		#endregion

		#region TaskInAllDependOnTasks
		public static bool TaskInAllDependOnTasks(int TaskId, int DepTaskId)
		{
			int retval = DbHelper2.RunSpInteger("TaskInAllDependOnTasks",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@DepTaskId", SqlDbType.Int, DepTaskId));
			return (retval > 0)?true:false;
		}
		#endregion

		#region GetMinPossibleStartDate
		public static DateTime GetMinPossibleStartDate(int TaskId, int CalendarId)
		{
			return DbHelper2.RunSpDateTime("TaskGetMinPossibleStartDate",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion
		
		#region GetTask
		/// <summary>
		/// Reader returns fields:
		///  TaskId, TaskNum, ProjectId, ProjectTitle, ManagerId, CreatorId, CompletedBy, Title, 
		///  Description, CreationDate, StartDate, FinishDate, Duration, 
		///  ActualStartDate, ActualFinishDate, PriorityId, PriorityName, PercentCompleted, OutlineNumber, 
		///  OutlineLevel, IsSummary, IsMilestone, ConstraintTypeId, ConstraintTypeName, 
		///  ConstraintDate, CompletionTypeId, CompletionTypeName, IsCompleted, 
		///  MustBeConfirmed, ReasonId, ActivationTypeId, StateId, ActivatedManually, 
		///  ActivationTypeName, StateName, PhaseId, PhaseName, 
		///  TaskTime, MSProjectUID, TotalMinutes, TotalApproved, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetTask(int TaskId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate","StartDate","FinishDate","ActualFinishDate","ConstraintDate"},
				"TaskGet",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		//22-10-2007 ET: Add method
		public static DataTable GetTaskDataTable(int TaskId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "CreationDate", "StartDate", "FinishDate", "ActualFinishDate", "ConstraintDate" },
				"TaskGet",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetTaskInUTC
		/// <summary>
		/// Reader returns fields:
		///  TaskId, TaskNum, ProjectId, ProjectTitle, ManagerId, CreatorId, CompletedBy, Title, 
		///  [Description],	CreationDate, StartDate, FinishDate, 	Duration, 
		///  ActualStartDate, ActualFinishDate, PriorityId, PriorityName, PercentCompleted, OutlineNumber, 
		///  OutlineLevel, 	IsSummary, IsMilestone, ConstraintTypeId, ConstraintTypeName, 
		///  ConstraintDate, CompletionTypeId, CompletionTypeName, IsCompleted, 
		///  MustBeConfirmed, ReasonId, StateId, ProjectCode
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetTaskInUTC(int TaskId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("TaskGetInUTC",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListTasksByProject
		/// <summary>
		/// TaskId, TaskNum, ProjectId, CreatorId, LastEditorId, Title, Description, 	
		/// CreationDate, LastSavedDate, 	StartDate, FinishDate, 	Duration, 
		/// ActualFinishDate, PriorityId, PercentCompleted, OutlineNumber, OutlineLevel, 	
		/// IsSummary, IsMilestone, ConstraintTypeId, ConstraintDate, CompletionTypeId, 
		/// IsCompleted, MustBeConfirmed, ReasonId, PriorityName, StateId, MSProjectUID
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTasksByProject(int ProjectId, int LanguageId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate", "ActualFinishDate", "ConstraintDate"},
				"TasksGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListTasksByProjectDataTable
		/// <summary>
		/// TaskId, TaskNum, ProjectId, CreatorId, LastEditorId, Title, Description, 	
		/// CreationDate, LastSavedDate, 	StartDate, FinishDate, 	Duration, 
		/// ActualFinishDate, PriorityId, PercentCompleted, OutlineNumber, OutlineLevel, 	
		/// IsSummary, IsMilestone, ConstraintTypeId, ConstraintDate, CompletionTypeId, 
		/// IsCompleted, MustBeConfirmed, ReasonId, PriorityName, StateId, MSProjectUID
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListTasksByProjectDataTable(int ProjectId, int LanguageId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "StartDate", "FinishDate", "ActualFinishDate", "ConstraintDate"},
				"TasksGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListTasksByProjectCollapsedDataTable
		/// <summary>
		/// Datatable contains fields:
		/// TaskId, TaskNum, OutlineNumber, OutlineLevel, IsSummary, IsMilestone, Title, 
		/// PercentCompleted, CompletionTypeId, IsCompleted, IsCollapsed, StartDate, FinishDate, 
		/// Duration, StateId, ActualStartDate, ActualFinishDate
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static DataTable GetListTasksByProjectCollapsedDataTable(int ProjectId, int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "StartDate", "FinishDate", "ActualStartDate", "ActualFinishDate" },
				"TasksGetByProjectCollapsed",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListSummaryTasksByProject
		/// <summary>
		/// Reader returns fields:
		/// TaskId, TaskNum, OutlineNumber, OutlineLevel
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetListSummaryTasksByProject(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("TasksGetSummaryByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region AddCollapsedTask
		public static void AddCollapsedTask(int UserId, int TaskId)
		{
			DbHelper2.RunSp("CollapsedTaskAdd",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region DeleteCollapsedTask
		public static void DeleteCollapsedTask(int UserId, int TaskId)
		{
			DbHelper2.RunSp("CollapsedTaskDelete",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListTaskLinksByProject
		/// <summary>
		/// Reader returns fields:
		/// LinkId, PredId, SuccId, Lag 
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public static IDataReader GetListTaskLinksByProject(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("TaskLinksGetByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetMaxFinishDateOfChild
		public static DateTime GetMaxFinishDateOfChild(int TaskId)
		{
			return DbHelper2.RunSpDateTime("TaskGetMaxFinishDateOfChild",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetMinStartDateOfChild
		public static DateTime GetMinStartDateOfChild(int TaskId)
		{
			return DbHelper2.RunSpDateTime("TaskGetMinStartDateOfChild",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion
		
		#region GetParent
		public static int GetParent(int TaskId)
		{
			return DbHelper2.RunSpInteger("TaskGetParent", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetProject
		public static int GetProject(int TaskId)
		{
			return DbHelper2.RunSpInteger("TaskGetProject", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListSuccessors
		/// <summary>
		/// Reader returns fields:
		///  TaskId
		/// </summary>
		/// <param name="TaskId"></param>
		/// <returns></returns>
		public static IDataReader GetListSuccessors(int TaskId)
		{
			return DbHelper2.RunSpDataReader("TaskGetSuccessors", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListSuccessorsDetails
		/// <summary>
		/// Reader returns fields:
		///  LinkId, TaskId, TaskNum, Title, StartDate, FinishDate, Duration, PriorityId, 
		///	 PercentCompleted, OutlineNumber, OutlineLevel, IsCompleted, Lag, CompletionTypeId, 
		///	 ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListSuccessorsDetails(int TaskId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate","FinishDate"},
				"TaskGetSuccessorsDetails", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListPredecessorsDetails
		/// <summary>
		/// Reader returns fields:
		///  LinkId, TaskId, TaskNum, Title, StartDate, FinishDate, Duration, PriorityId, 
		///	 PercentCompleted, OutlineNumber, OutlineLevel, IsCompleted, Lag,
		///	 IsCompleted, CompletionTypeId, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPredecessorsDetails(int TaskId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate","FinishDate"},
				"TaskGetPredecessorsDetails", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		public static DataTable GetListPredecessorsDetailsDataTable(int TaskId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[] { "StartDate", "FinishDate" },
				"TaskGetPredecessorsDetails",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region MoveLeft
		public static int MoveLeft(int TaskId)
		{
			return DbHelper2.RunSpInteger("TaskMoveLeft", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region MoveRight
		public static int MoveRight(int TaskId)
		{
			return DbHelper2.RunSpInteger("TaskMoveRight", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetSecurityForUser
		/// <summary>
		/// Reader return fields:
		///  TaskId, PrincipalId, IsResource, IsManager, IsRealTaskResource, IsCreator
		/// </summary>
		/// <param name="TaskId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static IDataReader GetSecurityForUser(int TaskId, int UserId)
		{
			return DbHelper2.RunSpDataReader("TaskGetSecurityForUser",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListCompletionTypes
		/// <summary>
		/// Reader returns fields:
		///  CompletionTypeId, CompletionTypeName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListCompletionTypes(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("CompletionTypesGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region MoveTo
		public static int MoveTo(int TaskId, int AfterTaskNum)
		{
			return DbHelper2.RunSpInteger("TaskMoveTo", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@AfterTaskNum", SqlDbType.Int, AfterTaskNum));
		}
		#endregion

		#region GetListChildren
		/// <summary>
		///  TaskId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListChildren(int TaskId)
		{
			return DbHelper2.RunSpDataReader("TaskGetChild", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListTasksForMove
		/// <summary>
		/// Reader returns fields:
		///	 TaskNum, Title
		/// </summary>
		/// <param name="TaskId"></param>
		/// <returns></returns>
		public static IDataReader GetListTasksForMove(int TaskId)
		{
			return DbHelper2.RunSpDataReader("TasksGetForMove",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListVacantPredecessors
		/// <summary>
		/// Reader returns fields:
		///	 TaskId, TaskNum, Title
		/// </summary>
		/// <param name="TaskId"></param>
		/// <returns></returns>
		public static IDataReader GetListVacantPredecessors(int TaskId)
		{
			return DbHelper2.RunSpDataReader("TaskGetVacantPredecessors",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListVacantSuccessors
		/// <summary>
		/// Reader returns fields:
		///	 TaskId, TaskNum, Title
		/// </summary>
		/// <param name="TaskId"></param>
		/// <returns></returns>
		public static IDataReader GetListVacantSuccessors(int TaskId)
		{
			return DbHelper2.RunSpDataReader("TaskGetVacantSuccessors",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListResources
		/// <summary>
		/// Reader returns fields:
		///	 ResourceId, TaskId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, MSProjectResourceId, IsExternal, ResourceName
		/// </summary>
		/// <param name="TaskId"></param>
		/// <returns></returns>
		public static IDataReader GetListResources(int TaskId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"ActualFinishDate","LastSavedDate"},
				"TaskResourcesGet",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListResourcesDataTable
		/// <summary>
		/// DataTable returns fields:
		///	 ResourceId, TaskId, UserId, MustBeConfirmed, ResponsePending, IsConfirmed, 
		///	 PercentCompleted, ActualFinishDate, LastSavedDate, IsGroup, MSProjectResourceId, IsExternal, ResourceName
		/// </summary>
		/// <param name="TaskId"></param>
		/// <returns></returns>
		public static DataTable GetListResourcesDataTable(int TaskId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"ActualFinishDate","LastSavedDate"},
				"TaskResourcesGet",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region AddResource
		public static void AddResource(int TaskId, int UserId, bool MustBeConfirmed, bool CanManage)
		{
			DbHelper2.RunSp("TaskResourceAdd",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@MustBeConfirmed", SqlDbType.Bit, MustBeConfirmed),
				DbHelper2.mp("@CanManage", SqlDbType.Bit, CanManage));
		}
		#endregion

		#region DeleteResource
		public static void DeleteResource(int TaskId, int UserId)
		{
			DbHelper2.RunSp("TaskResourceDelete",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetResourceByUser
		/// <summary>
		/// Reader returns fields:
		///	 MustBeConfirmed, ResponsePending, IsConfirmed, PercentCompleted
		/// </summary>
		/// <param name="TaskId"></param>
		/// <param name="UserId"></param>
		/// <returns></returns>
		public static IDataReader GetResourceByUser(int TaskId, int UserId)
		{
			return DbHelper2.RunSpDataReader("TaskResourceGetByUser",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplyResource
		public static void ReplyResource(int TaskId, int UserId, bool IsConfirmed, DateTime LastSavedDate)
		{
			DbHelper2.RunSp("TaskResourceReply",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IsConfirmed", SqlDbType.Bit, IsConfirmed),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate));
		}
		#endregion

		#region UpdateResourcePercent
		public static void UpdateResourcePercent(int TaskId, int UserId, int PercentCompleted, DateTime LastSavedDate)
		{
			DbHelper2.RunSp("TaskResourceUpdatePercent",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate));
		}
		#endregion

		#region UpdatePercent
		public static void UpdatePercent(int TaskId, int PercentCompleted)
		{
			DbHelper2.RunSp("TaskUpdatePercent",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted));
		}
		#endregion

		#region UpdateCompletion
		public static void UpdateCompletion(int TaskId, bool IsCompleted, int ReasonId)
		{
			DbHelper2.RunSp("TaskUpdateCompletion",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@IsCompleted", SqlDbType.Bit, IsCompleted),
				DbHelper2.mp("@ReasonId", SqlDbType.Int, ReasonId));
		}
		#endregion

		#region GetListTasksByFilter
		/// <summary>
		///  Reader returns fields:
		///		TaskId, Title, Description, IsCompleted, StartDate, FinishDate, CanEdit, CanDelete, 
		///		PriorityId, PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTasksByFilter(int ProjectId, int UserId, 
			int ResourceId, int TimeZoneId, DateTime StartDate, DateTime FinishDate, 
			bool GetAssigned, bool GetManaged, bool GetCreated, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate","FinishDate"},
				"TasksGetByFilter",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate), 
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged),
				DbHelper2.mp("@GetCreated", SqlDbType.Bit, GetCreated),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region GetListTasksInRecalculateOrder
		/// <summary>
		/// Reader returns fields:
		///		TaskId
		/// </summary>
		public static IDataReader GetListTasksInRecalculateOrder(int ProjectId)
		{
			return DbHelper2.RunSpDataReader("ProjectGetTaskInRecalculateOrder", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region ProjectTaskCheckConsistency
		/// <summary>
		/// Reader returns fields:
		///		TaskId
		/// </summary>
		public static bool ProjectTaskCheckConsistency(int ProjectId)
		{
			if (1 == DbHelper2.RunSpInteger("ProjectTaskCheckConsistency", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId)))
				return true;
					else
				return false;
		}
		#endregion

		#region GetListTasksSimple
		/// <summary>
		///  Reader returns fields:
		///		TaskId, Title
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <param name="UserId"></param>
		/// <param name="GetAssigned"></param>
		/// <param name="GetManaged"></param>
		/// <returns></returns>
		public static IDataReader GetListTasksSimple(int ProjectId, 
			int UserId, bool GetAssigned, bool GetManaged)
		{
			return DbHelper2.RunSpDataReader("TasksGetSimple",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@GetAssigned", SqlDbType.Bit, GetAssigned),
				DbHelper2.mp("@GetManaged", SqlDbType.Bit, GetManaged));
		}
		#endregion

		#region DeleteAll
		public static void DeleteAll(int ProjectId)
		{
			DbHelper2.RunSp("TaskDeleteAll",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetListTaskPredecessorDates
		/// <summary>
		///  DataTable returns fields:
		///		LinkId, PredId, FinishDate, Lag, PossibleStartDate 
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListTaskPredecessorDates(int TaskId, int CalendarId)
		{
			return DbHelper2.RunSpDataTable("TaskGetPredecessorDates",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@CalendarId", SqlDbType.Int, CalendarId));
		}
		#endregion

		#region GetListTasksForUserByProject
		/// <summary>
		/// DataTable contains fields:
		///  TaskId, Title, Description, PriorityId, PriorityName, StartDate, FinishDate, 
		///  PercentCompleted, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListTasksForUserByProject(int ProjectId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"TasksGetForUserByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region RecalculateSummaryPercent
		public static void RecalculateSummaryPercent(int TaskId)
		{
			DbHelper2.RunSp("TaskRecalculateSummaryPercent",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetIsCompleted
		public static int GetIsCompleted(int TaskId)
		{
			return DbHelper2.RunSpInteger("TaskGetIsCompleted",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region TaskUpdateAddPossibleSuccessor
		public static int TaskUpdateAddPossibleSuccessor(string UpdateId, int TaskId)
		{
			return DbHelper2.RunSpInteger("TaskUpdateAddPossibleSuccessor",
				DbHelper2.mp("@UpdateId", SqlDbType.Char, 36, UpdateId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region TaskUpdateAddPossibleTask
		public static int TaskUpdateAddPossibleTask(string UpdateId, int TaskId, bool FromChild)
		{
			return DbHelper2.RunSpInteger("TaskUpdateAddPossibleTask",
				DbHelper2.mp("@UpdateId", SqlDbType.Char, 36, UpdateId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@FromChild", SqlDbType.Bit, FromChild));
		}
		#endregion

		#region TaskUpdateDeleteTask
		public static void TaskUpdateDeleteTask(string UpdateId, int TaskId)
		{
			DbHelper2.RunSp("TaskUpdateDeleteTask",
				DbHelper2.mp("@UpdateId", SqlDbType.Char, 36, UpdateId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region TaskUpdateDeleteAll
		public static void TaskUpdateDeleteAll(string UpdateId)
		{
			DbHelper2.RunSp("TaskUpdateDeleteAll",
				DbHelper2.mp("@UpdateId", SqlDbType.Char, 36, UpdateId));
		}
		#endregion

		#region TaskUpdateGetTask
		public static int TaskUpdateGetTask(string UpdateId)
		{
			return DbHelper2.RunSpInteger("TaskUpdateGetTask",
				DbHelper2.mp("@UpdateId", SqlDbType.Char, 36, UpdateId));
		}
		#endregion

		#region GetSharingLevel
		public static int GetSharingLevel(int UserId, int TaskId)
		{
			return DbHelper2.RunSpInteger("SharingGetLevelForTask", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListTasksForScheduling
		/// <summary>
		///  Reader returns fields:
		///		UserId, TaskId, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTasksForScheduling(int UserId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate","FinishDate"},
				"TasksGetForScheduling",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region GetListTasksForEventScheduling
		/// <summary>
		///  Reader returns fields:
		///		UserId, TaskId, StartDate, FinishDate, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTasksForEventScheduling(int EventId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate","FinishDate"},
				"TasksGetForEventScheduling",
				DbHelper2.mp("@EventId", SqlDbType.Int, EventId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate));
		}
		#endregion

		#region GetTaskLink
		/// <summary>
		/// LinkId, PredId, SuccId, Lag 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetTaskLink(int LinkId)
		{
			return DbHelper2.RunSpDataReader("TaskLinkGet",
				DbHelper2.mp("@LinkId", SqlDbType.Int, LinkId));
		}
		#endregion

		#region CheckForChangeableRoles
		public static int CheckForChangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("TasksCheckForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("TasksCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListTasksForChangeableRoles
		/// <summary>
		/// Reader returns fields:
		///  TaskId, Title, IsCompleted, CompletionTypeId, StartDate, FinishDate,
		///  IsResource, CanView, CanEdit, CanDelete, ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTasksForChangeableRoles(int UserId, int CurrentUserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate"},
				"TasksGetForChangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@CurrentUserId", SqlDbType.Int, CurrentUserId));
		}
		#endregion

		#region ReplaceUnchangeableUserToManager
		public static void ReplaceUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("TasksReplaceUnchangeableUserToManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("TasksReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion
	
		#region GetListAllChild
		/// <summary>
		/// Reader returns fields:
		/// TaskId, IsCompleted, ReasonId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListAllChild(int TaskId)
		{
			return DbHelper2.RunSpDataReader("TaskGetAllChild",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListMilestones
		/// <summary>
		///  Reader returns fields:
		///		TaskId, Title, Description, IsCompleted, StartDate, FinishDate, StateId, PriorityId
		///		CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListMilestones(int ProjectId, int UserId, 
			int ResourceId, int TimeZoneId, DateTime StartDate, DateTime FinishDate, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate","FinishDate"},
				"TasksGetMilestones",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ResourceId", SqlDbType.Int, ResourceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region GetListMilestonesDataTable
		/// <summary>
		///  Reader returns fields:
		///		TaskId, Title, Description, IsCompleted, StartDate, FinishDate, StateId, PriorityId,
		///		CanEdit, CanDelete
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListMilestonesDataTable(int ProjectId, int UserId, 
			int TimeZoneId, DateTime StartDate, DateTime FinishDate, string Keyword)
		{
			// O.R. [2008-08-21]: Wildcard chars replacing
			Keyword = DBCommon.ReplaceSqlWildcard(Keyword);
			//

			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate","FinishDate"},
				"TasksGetMilestones",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@FinishDate", SqlDbType.DateTime, FinishDate),
				DbHelper2.mp("@Keyword", SqlDbType.NVarChar, 100, Keyword));
		}
		#endregion

		#region GetListToDo
		/// <summary>
		///	 ToDoId, Title, Description, StartDate, FinishDate, ActualFinishDate, PercentCompleted, 
		///	 IsCompleted, CompletedBy, ReasonId, ReasonName, PriorityId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListToDo(int TaskId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "ActualFinishDate"},
				"ToDoGetByTask",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListToDoDataTable
		/// <summary>
		/// ToDoId, Title, Description, StartDate, FinishDate, ActualFinishDate, PercentCompleted, 
		///	 IsCompleted, CompletedBy, ReasonId, ReasonName, PriorityId, StateId
		/// </summary>
		/// <returns></returns>
		public static DataTable GetListToDoDataTable(int TaskId, int UserId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"StartDate", "FinishDate", "ActualFinishDate"},
				"ToDoGetByTask",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListChildrenInfo
		/// <summary>
		///  TaskId, CreatorId, CompletedBy, Title, Description, 
		///  CreationDate, StartDate, FinishDate, Duration, 
		///  ActualFinishDate, PercentCompleted, IsSummary, IsMilestone, IsCompleted, 
		///  ReasonId, StateId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListChildrenInfo(int TaskId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate","StartDate","FinishDate","ActualFinishDate"},
				"TaskGetChildrenInfo", 
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetTaskSecurity
		/// <summary>
		/// UserId, IsResource, IsManager, IsRealTaskResource, IsRealTaskManager, IsCreator
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetTaskSecurity(int TaskId)
		{
			return DbHelper2.RunSpDataReader("TaskSecurityGet",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetListActivationTypes
		/// <summary>
		/// Reader returns fields:
		///  ActivationTypeId, ActivationTypeName 
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListActivationTypes(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("ActivationTypesGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region UpdateActivation
		public static void UpdateActivation(int TaskId, bool value)
		{
			DbHelper2.RunSp("TaskUpdateActivation",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@Value", SqlDbType.Bit, value));
		}
		#endregion

		#region TaskRecalculateState
		/// <summary>
		/// OldState, NewState, StartDate, FinishDate
		/// </summary>
		/// <returns></returns>
		public static IDataReader TaskRecalculateState(int TaskId, DateTime DT)
		{
			return DbHelper2.RunSpDataReader("TaskRecalculateState",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@DateNow", SqlDbType.DateTime, DT)
				);
		}
		#endregion

		#region UpdateCompletedBy
		public static void UpdateCompletedBy(int TaskId, int UserId)
		{
			DbHelper2.RunSp("TaskUpdateCompletedBy",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ResetCompletedBy
		public static void ResetCompletedBy(int TaskId)
		{
			DbHelper2.RunSp("TaskResetCompletedBy",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region TaskGetMilestonesCount
		public static int TaskGetMilestonesCount(int ProjectId)
		{
			return DbHelper2.RunSpInteger("TasksGetMilestonesCount", DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}
		#endregion

		#region GetState(...)
		public static int GetState(int ObjectId)
		{
			return DBCommon.NullToInt32(
				DbHelper2.RunSpScalar("TaskGetState"
				, DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId)
				));
		}
		#endregion

		#region UpdateTaskTime
		public static void UpdateTaskTime(int TaskId, int TaskTime)
		{
			DbHelper2.RunSp("TaskUpdateTaskTime",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@TaskTime", SqlDbType.Int, TaskTime));
		}
		#endregion

		#region ResetResourcePercent
		public static void ResetResourcePercent(int TaskId, int PercentCompleted, DateTime LastSavedDate)
		{
			DbHelper2.RunSp("TaskResourcesResetPercent",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate));
		}
		#endregion

		#region UpdateActualStart
		public static void UpdateActualStart(int TaskId)
		{
			DbHelper2.RunSp("TaskUpdateActualStart",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion
		
		#region AddResourceForMSProject
		public static void AddResourceForMSProject(int TaskId, int UserId, int MSProjectResourceId)
		{
			AddResourceForMSProject(TaskId, UserId, MSProjectResourceId, 0, null);
		}
		public static void AddResourceForMSProject(int TaskId, int UserId, int MSProjectResourceId, int PercentCompleted, DateTime? ActualFinishDate)
		{
			DbHelper2.RunSp("TaskResourceAddForMSProject",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@MSProjectResourceId", SqlDbType.Int, MSProjectResourceId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@ActualFinishDate", SqlDbType.DateTime, ActualFinishDate));
		}
		#endregion

		#region UpdateResourceForMsProject 
		public static void UpdateResourceForMSProject(int taskId, int principalId,
													  int MSProjectResourceId, 
													  int PercentCompleted, DateTime? ActualFinishDate)
		{
			DbHelper2.RunSp("TaskResourceUpdateForMsProject",
				DbHelper2.mp("@TaskId", SqlDbType.Int, taskId),
				DbHelper2.mp("@PrincipalId", SqlDbType.Int, principalId),
				DbHelper2.mp("@MSProjectResourceId", SqlDbType.Int, MSProjectResourceId),
				DbHelper2.mp("@PercentCompleted", SqlDbType.Int, PercentCompleted),
				DbHelper2.mp("@ActualFinishDate", SqlDbType.DateTime, ActualFinishDate));
		}

		#endregion
		#region GetTaskByMSProjectUID
		/// <summary>
		/// Reader returns fields:
		///  TaskId, TaskNum, ProjectId, CreatorId, CompletedBy, Title, Description, 
		///  CreationDate, StartDate, FinishDate, Duration, ActualFinishDate, 
		///  PriorityId, PercentCompleted, OutlineNumber, OutlineLevel, 
		///  IsSummary, IsMilestone, ConstraintTypeId, ConstraintDate, CompletionTypeId, 
		///  IsCompleted, MustBeConfirmed, ReasonId, ActivationTypeId, StateId, ActivatedManually, 
		///  PhaseId, TaskTime
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetTaskByMSProjectUID(int msProjectUID)
		{
			return DbHelper2.RunSpDataReader("TaskGetByMSProjectUID",
				DbHelper2.mp("@MSProjectUID", SqlDbType.Int, msProjectUID));
		}
		#endregion

		#region DeleteTaskLinksByProject
		public static void DeleteTaskLinksByProject(int projectId)
		{
			DbHelper2.RunSp("TaskLinksDeleteByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion

		#region DeleteSimple
		public static void DeleteSimple(int TaskId)
		{
			DbHelper2.RunSp("TaskDeleteSimple",
				DbHelper2.mp("@TaskId", SqlDbType.Int, TaskId));
		}
		#endregion

		#region GetResourceByMSProjectResourceId
		public static int GetResourceByMSProjectResourceId(int msProjectId,
														   int msProjectResourceId)
		{
			return DbHelper2.RunSpInteger("TaskResourceGetByMSProjectResourceId", 
				DbHelper2.mp("@MSProjectResourceId", SqlDbType.Int, msProjectResourceId),
				DbHelper2.mp("@MSProjectId", SqlDbType.Int, msProjectId));
		}
		#endregion

		#region GetMinStartDate
		public static DateTime GetMinStartDate(int projectId)
		{
			return DbHelper2.RunSpDateTime("TasksGetMinStartDate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion

		#region GetMaxFinishDate
		public static DateTime GetMaxFinishDate(int projectId)
		{
			return DbHelper2.RunSpDateTime("TasksGetMaxFinishDate",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion

		#region AddTaskDates
		/// <summary>
		/// Adds a row into TaskDates.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="taskId">The task id.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="oldDate">The old date.</param>
		/// <param name="newDate">The new date.</param>
		public static void AddTaskDates(int projectId, int taskId, int userId, DateTime oldDate, DateTime newDate)
		{
			DbHelper2.RunSp("TaskDatesAdd",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				DbHelper2.mp("@TaskId", SqlDbType.Int, taskId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@OldDate", SqlDbType.DateTime, oldDate),
				DbHelper2.mp("@NewDate", SqlDbType.DateTime, newDate));
		}
		#endregion

		#region GetTaskDates
		/// <summary>
		/// Gets the task dates.
		/// </summary>
		/// <param name="projectId">The project id.</param>
		/// <param name="fromDate">From date.</param>
		/// <param name="toDate">To date.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="timeZoneId">The time zone id.</param>
		/// <returns>
		/// ProjectId, ProjectName, TaskId, TaskName,  UserId, UserName, OldDate, NewDate, Created
		/// </returns>
		public static IDataReader GetTaskDates(int projectId, DateTime? fromDate, DateTime? toDate, int userId, int timeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				timeZoneId, new string[]{"OldDate", "NewDate", "Created"},
				"TaskDatesGet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId),
				fromDate.HasValue
					? DbHelper2.mp("@FromDate", SqlDbType.DateTime, fromDate.Value) 
					: DbHelper2.mp("@FromDate", SqlDbType.DateTime, null) ,
				toDate.HasValue
					? DbHelper2.mp("@ToDate", SqlDbType.DateTime, toDate.Value) 
					: DbHelper2.mp("@ToDate", SqlDbType.DateTime, null),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion
	}
}
