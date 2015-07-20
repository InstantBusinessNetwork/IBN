using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	public class DbTimeTracking
	{
		#region ResetObjectId
		public static void ResetObjectId(int objectTypeId, int objectId)
		{
			DbHelper2.RunSp("TimeTrackingEntryResetObjectId",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId));
		}
		#endregion

		#region GetListTimeTrackingItemsForAdd
		/// <summary>
		///  ObjectId, ObjectTypeId, ObjectName, BlockTypeInstanceId
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTimeTrackingItemsForAdd(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbHelper2.RunSpDataReader("TimeTrackingItemsGetForAdd",
				DbHelper2.mp("@BlockTypeInstanceId", SqlDbType.Int, blockTypeInstanceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		public static DataTable GetListTimeTrackingItemsForAdd_DataTable(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbHelper2.RunSpDataTable("TimeTrackingItemsGetForAdd",
				DbHelper2.mp("@BlockTypeInstanceId", SqlDbType.Int, blockTypeInstanceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region GetListTimeTrackingItemsForFinances
		/// <summary>
		///  TimeTrackingEntryId, UserId, ObjectTypeId, ObjectId, Title, 
		///  Day1, Day2, Day3, Day4, Day5, Day6, Day7, Total, TotalApproved, Rate, Cost, TaskTime
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListTimeTrackingItemsForFinances(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbHelper2.RunSpDataReader("TimeTrackingItemsGetForFinances",
				DbHelper2.mp("@BlockTypeInstanceId", SqlDbType.Int, blockTypeInstanceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		public static DataTable GetListTimeTrackingItemsForFinances_DataTable(int blockTypeInstanceId, DateTime startDate, int userId)
		{
			return DbHelper2.RunSpDataTable("TimeTrackingItemsGetForFinances",
				DbHelper2.mp("@BlockTypeInstanceId", SqlDbType.Int, blockTypeInstanceId),
				DbHelper2.mp("@StartDate", SqlDbType.DateTime, startDate),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId));
		}
		#endregion

		#region RecalculateProject
		public static void RecalculateProject(int projectId)
		{
			DbHelper2.RunSp("TimeTrackingRecalculateProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion

		#region RecalculateProjectTaskTime
		public static void RecalculateProjectTaskTime(int projectId)
		{
			DbHelper2.RunSp("TimeTrackingRecalculateProjectTaskTime",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion

		#region RecalculateObject
		public static int RecalculateObject(int objectId, int objectTypeId, int projectId)
		{
			return DbHelper2.RunSpInteger("TimeTrackingRecalculateObject",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion

		#region RecalculateAllObjectsByProject
		public static void RecalculateAllObjectsByProject(int projectId)
		{
			DbHelper2.RunSp("TimeTrackingRecalculateAllObjectsByProject",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, projectId));
		}
		#endregion
	}
}
