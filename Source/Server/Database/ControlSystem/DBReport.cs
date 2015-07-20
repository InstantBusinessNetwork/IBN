using System;
using System.Data;
using System.Data.SqlClient;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.ControlSystem
{
	/// <summary>
	/// Summary description for DBReport.
	/// </summary>
	public class DBReport
	{
		private DBReport()
		{
		}

		#region -- Get --
		// ReportId, [Name], ReportCategoryId, ReportCategoryName
		public static IDataReader GetById(string ContainerKey, int ReportId)
		{
			return DbHelper2.RunSpDataReader("fsc_ReportGetById",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@ReportId", SqlDbType.Int, ReportId));
		}

		// ReportId, [Name], ReportCategoryId, ReportCategoryName
		public static IDataReader GetByName(string ContainerKey, string ReportName)
		{
			return DbHelper2.RunSpDataReader("fsc_ReportGetByName",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@ReportName", SqlDbType.NVarChar, 255, ReportName));
		}

		// ReportId, [Name], ReportCategoryId, ReportCategoryName
		public static IDataReader GetByCategory(string ContainerKey, string ReportCategoryName)
		{
			return DbHelper2.RunSpDataReader("fsc_ReportGetByCategory",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@ReportCategoryName", SqlDbType.NVarChar, 255, SqlHelper.Null2DBNull(ReportCategoryName) ));
		}
		#endregion 

		#region -- GetList --
		// ReportId, [Name], ReportCategoryId, ReportCategoryName
		public static IDataReader GetList(string ContainerKey)
		{
			return DbHelper2.RunSpDataReader("fsc_ReportGetList",
								DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey));
		}
		#endregion

		#region -- GetCategoryList --
		// ReportCategoryId, [Name]
		public static IDataReader GetCategoryList()
		{
			return DbHelper2.RunSpDataReader("fsc_ReportCategoryGetList");
		}
		#endregion

		#region -- Create --
		public static int Create(string ContainerKey, string Name)
		{
			return DbHelper2.RunSpInteger("fsc_ReportCreate",
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@ReportName", SqlDbType.NVarChar, 255, Name));
		}
		#endregion

		#region -- CreateCategory --
		public static int CreateCategory(string Name)
		{
			return DbHelper2.RunSpInteger("fsc_ReportCreateCategory",
				DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, Name));
		}
		#endregion

		#region -- SetReportCategory --
		public static void SetReportCategory(int ReportId, int ReportCategoryId)
		{
			DbHelper2.RunSp("fsc_ReportSetCategory",
				DbHelper2.mp("@ReportId", SqlDbType.Int, ReportId),
				DbHelper2.mp("@ReportCategoryId", SqlDbType.Int, ReportCategoryId<=0?DBNull.Value:(object)ReportCategoryId ));

		}
		#endregion

		#region -- CanUserRunAction --
		public static bool CanUserRunAction(int UserId, string ContainerKey, int ReportId, string Action)
		{
			return (DbHelper2.RunSpInteger("fsc_ReportCanUserRunAction",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@ContainerKey", SqlDbType.NVarChar, 50, ContainerKey),
				DbHelper2.mp("@ReportId", SqlDbType.Int, ReportId),
				DbHelper2.mp("@Action", SqlDbType.NVarChar, 50, Action))==1);
		}
		#endregion

	}
}
