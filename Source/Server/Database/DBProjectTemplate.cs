using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DBProjectTemplate.
	/// </summary>
	public class DBProjectTemplate
	{
		#region AddProjectTemplate
		public static int AddProjectTemplate(string TemplateName, int CreatorId, 
			DateTime CreationDate, string TemplateData)
		{
			return DbHelper2.RunSpInteger("ProjectTemplateAdd",
				DbHelper2.mp("@TemplateName", SqlDbType.NVarChar, 255, TemplateName),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@TemplateData", SqlDbType.NText, TemplateData));
		}
		#endregion

		#region UpdateProjectTemplate
		public static void UpdateProjectTemplate(int TemplateId, string TemplateName, 
			int LastEditorId, DateTime LastSavedDate, string TemplateData)
		{
			DbHelper2.RunSp("ProjectTemplateUpdate",
				DbHelper2.mp("@TemplateId", SqlDbType.Int, TemplateId),
				DbHelper2.mp("@TemplateName", SqlDbType.NVarChar, 255, TemplateName),
				DbHelper2.mp("@LastEditorId", SqlDbType.Int, LastEditorId),
				DbHelper2.mp("@LastSavedDate", SqlDbType.DateTime, LastSavedDate),
				DbHelper2.mp("@TemplateData", SqlDbType.NText, TemplateData));
		}
		#endregion

		#region DeleteProjectTemplate
		public static void DeleteProjectTemplate(int TemplateId)
		{
			DbHelper2.RunSp("ProjectTemplateDelete",
				DbHelper2.mp("@TemplateId", SqlDbType.Int, TemplateId));
		}
		#endregion

		#region GetProjectTemplate
		/// <summary></summary>
		/// <returns>
		///  TemplateId, TemplateName, CreatorId, LastEditorId, CreationDate, LastSavedDate, TemplateData
		/// </returns>
		public static IDataReader GetProjectTemplate(int TemplateId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "LastSavedDate"},
				"ProjectTemplateGet",
				DbHelper2.mp("@TemplateId", SqlDbType.Int, TemplateId));
		}
		#endregion

		#region GetListProjectTemplate
		/// <summary></summary>
		/// <returns>
		///  TemplateId, TemplateName, CreatorId, LastEditorId, CreationDate, LastSavedDate, TemplateData
		/// </returns>
		public static IDataReader GetListProjectTemplate(int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate", "LastSavedDate"},
				"ProjectTemplateGetList");
		}
		#endregion

		#region GetListProjectTemplateDataTable
		/// <summary></summary>
		/// <returns>
		///  TemplateId, TemplateName, CreatorId, LastEditorId, CreationDate, LastSavedDate, TemplateData
		/// </returns>
		public static DataTable GetListProjectTemplateDataTable(int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"CreationDate", "LastSavedDate"},
				"ProjectTemplateGetList");
		}
		#endregion

		#region CheckForUnchangeableRoles
		public static int CheckForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("ProjectTemplatesCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceUnchangeableUser
		public static void ReplaceUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("ProjectTemplatesReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion
	}
}
