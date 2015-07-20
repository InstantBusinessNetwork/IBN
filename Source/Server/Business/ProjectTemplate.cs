using System;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for ProjectTemplate.
	/// </summary>
	public class ProjectTemplate
	{
		#region AddProjectTemplate
		public static int AddProjectTemplate(string TemplateName, string TemplateData)
		{
			return DBProjectTemplate.AddProjectTemplate(TemplateName, Security.CurrentUser.UserID,
				DateTime.UtcNow, TemplateData);
		}
		#endregion

		#region UpdateProjectTemplate
		public static void UpdateProjectTemplate(int TemplateId, string TemplateName, 
			string TemplateData)
		{
			DBProjectTemplate.UpdateProjectTemplate(TemplateId, TemplateName, 
				Security.CurrentUser.UserID, DateTime.UtcNow, TemplateData);
		}
		#endregion

		#region DeleteProjectTemplate
		public static void DeleteProjectTemplate(int TemplateId)
		{
			DBProjectTemplate.DeleteProjectTemplate(TemplateId);
		}
		#endregion

		#region GetProjectTemplate
		/// <summary></summary>
		/// <returns>
		///  TemplateId, TemplateName, CreatorId, LastEditorId, CreationDate, LastSavedDate, TemplateData
		/// </returns>
		public static IDataReader GetProjectTemplate(int TemplateId)
		{
			return DBProjectTemplate.GetProjectTemplate(TemplateId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListProjectTemplate
		/// <summary></summary>
		/// <returns>
		///  TemplateId, TemplateName, CreatorId, LastEditorId, CreationDate, LastSavedDate, TemplateData
		/// </returns>
		public static IDataReader GetListProjectTemplate()
		{
			return DBProjectTemplate.GetListProjectTemplate(Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region GetListProjectTemplateDataTable
		/// <summary></summary>
		/// <returns>
		///  TemplateId, TemplateName, CreatorId, LastEditorId, CreationDate, LastSavedDate, TemplateData
		/// </returns>
		public static DataTable GetListProjectTemplateDataTable()
		{
			return DBProjectTemplate.GetListProjectTemplateDataTable(Security.CurrentUser.TimeZoneId);
		}
		#endregion
	}
}
