using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBIMGroup.
	/// </summary>
	public class DBIMGroup
	{
		#region CreateUpdate
		public static int CreateUpdate(int IMGroupId, string IMGroupName, string Color, bool IsPartner)
		{
			return DbHelper2.RunSpInteger("ASP_ADD_IMGROUP",
				DbHelper2.mp("@IMGROUP_ID",	SqlDbType.Int, IMGroupId),
				DbHelper2.mp("@IMGROUP_NAME",	SqlDbType.NVarChar, 50,	IMGroupName),
				DbHelper2.mp("@COLOR",	SqlDbType.Char, 6,	Color),
				DbHelper2.mp("@IS_PARTNER",	SqlDbType.Bit, IsPartner));
		}
		#endregion

		#region Delete
		public static void Delete(int IMGroupId)
		{
			DbHelper2.RunSp("ASP_DELETE_IMGROUP",
				DbHelper2.mp("@IMGROUP_ID",	SqlDbType.Int, IMGroupId));
		}
		#endregion

		#region GetIMGroup
		/// <summary>
		/// DataTable contains columns:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static DataTable GetIMGroup(int imGroupId, bool includeInternal)
		{
			return DbHelper2.RunSpDataTable("ASP_GET_IMGROUPS", 
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, imGroupId),
				DbHelper2.mp("@include_internal", SqlDbType.Bit, includeInternal));
		}
		#endregion

		#region GetListIMGroupsWithoutPartners
		/// <summary>
		/// DataTable contains columns:
		///		IMGroupId, IMGroupName, color, logo_version, is_partner
		/// </summary>
		public static DataTable GetListIMGroupsWithoutPartners()
		{
			return DbHelper2.RunSpDataTable("ASP_GET_IMGROUPS_WITHOUT_PARTNERS");
		}
		#endregion

		#region GetListUsers
		/// <summary>
		/// Reader returns fields:
		///		UserId, Login, FirstName, LastName, Email, IsActive, IMGroupId, OriginalId
		/// </summary>
		public static IDataReader GetListUsers(int GroupId)
		{
			return DbHelper2.RunSpDataReader("UsersGetByIMGroup", 
				DbHelper2.mp("@IMGroupId", SqlDbType.Int, GroupId));
		}
		#endregion

		#region UpdateIMGroupLogo
		public static void UpdateIMGroupLogo(int IMGroupId, byte[] IMGroupLogo)
		{
			DbHelper2.RunSp("ASP_UPDATE_IMGROUP_LOGO",
				DbHelper2.mp("@IMGROUP_ID", SqlDbType.Int, IMGroupId), 
				DbHelper2.mp("@IMGROUP_LOGO", SqlDbType.Image, IMGroupLogo.Length, IMGroupLogo));
		}
		#endregion

		#region GetListIMGroupsYouCanSee
		/// <summary>
		/// DataTable contains columns:
		///  IMGroupId, IMGroupName
		/// </summary>
		/// <param name="IMGroupId"></param>
		/// <param name="IncludeSelf"></param>
		/// <returns></returns>
		public static DataTable GetListIMGroupsYouCanSee(int imGroupId, bool includeSelf)
		{
			return DbHelper2.RunSpDataTable("ASP_GET_IMGROUPS_YOU_CAN_SEE",
				DbHelper2.mp("@IMGROUP_ID", SqlDbType.Int, imGroupId), 
				DbHelper2.mp("@include_self",	SqlDbType.Bit, includeSelf));
		}
		#endregion

		#region GetListIMGroupsCanSeeYou
		/// <summary>
		/// DataTable contains columns:
		///  IMGroupId, IMGroupName
		/// </summary>
		/// <param name="IMGroupId"></param>
		/// <param name="CompanyId"></param>
		/// <returns></returns>
		public static DataTable GetListIMGroupsCanSeeYou(int imGroupId)
		{
			return DbHelper2.RunSpDataTable("ASP_GET_IMGROUPS_CAN_SEE_YOU",
				DbHelper2.mp("@IMGROUP_ID", SqlDbType.Int, imGroupId));
		}
		#endregion

		#region AddDependences
		public static void AddDependences(int IMGroupId, int DepIMGroupId)
		{
			DbHelper2.RunSp("ASP_ADD_DEPENDENCES",
				DbHelper2.mp("@IMGROUP_ID",	SqlDbType.Int, IMGroupId),
				DbHelper2.mp("@DEP_IMGROUP_ID",	SqlDbType.Int, DepIMGroupId));
		}
		#endregion

		#region DeleteDependences
		public static void DeleteDependences(int IMGroupId, int DepIMGroupId)
		{
			DbHelper2.RunSp("ASP_DELETE_DEPENDENCES",
				DbHelper2.mp("@IMGROUP_ID",	SqlDbType.Int, IMGroupId),
				DbHelper2.mp("@DEP_IMGROUP_ID",	SqlDbType.Int, DepIMGroupId));
		}
		#endregion

		#region GetBinaryClientLogo
		/// <summary>
		/// Reader returns fields:
		///		client_logo
		/// </summary>
		public static IDataReader GetBinaryClientLogo(int IMGroupId)
		{
			return DbHelper2.RunSpDataReaderBlob("ASP_GET_BINARY_CLIENT_LOGO", 
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, IMGroupId));
		}
		#endregion

		#region GetListIMGroupsByIMGroups
		/// <summary>
		/// DataTable contains columns:
		///		imgroup_id
		/// </summary>
		public static DataTable GetListIMGroupsByIMGroups(int OldIMGroupId, int IMGroupId)
		{
			return DbHelper2.RunSpDataTable("ASP_GET_IMGROUPS_BY_IMGROUPS", 
				DbHelper2.mp("@old_imgroup_id", SqlDbType.Int, OldIMGroupId),
				DbHelper2.mp("@imgroup_id", SqlDbType.Int, IMGroupId));
		}
		#endregion

		#region CloneIMGroupLogo
		public static void CloneIMGroupLogo(int FromIMGroupId, int ToIMGroupId)
		{
			DbHelper2.RunSp("ASP_CLONE_IMGROUP_LOGO",
				DbHelper2.mp("@FROM_IMGROUP_ID", SqlDbType.Int, FromIMGroupId), 
				DbHelper2.mp("@TO_IMGROUP_ID", SqlDbType.Int, ToIMGroupId));
		}
		#endregion

		public static DataTable GetListIMSessionsByUserAndDate(int UserId,DateTime StartDate, DateTime EndDate,int Bias)
		{
			return DbHelper2.RunSpDataTable("ASP_REP_GET_USER_IM_SESSIONS",
				DbHelper2.mp("@user_id", SqlDbType.Int, UserId),
				DbHelper2.mp("@fromdate", SqlDbType.DateTime, StartDate),
				DbHelper2.mp("@todate", SqlDbType.DateTime, EndDate),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, Bias*60));
		}
	}
}
