using System;
using System.Data;
using System.Collections;
using Mediachase.IBN.Database;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Common;
using Mediachase.MetaDataPlus.Configurator;

namespace Mediachase.IBN.Business
{
	#region DictionaryTypes
	public enum DictionaryTypes
	{
		Categories = 1,
		ProjectPhases = 2,
		RiskLevels = 3,
		IncidentSeverities = 4,
		IncidentTypes = 5,
		ProjectTypes = 6,
		//Clients = 7,
		ProjectCategories = 8,
		IncidentCategories = 9,
		Currency = 10,
		DocumentStatus = 11,
		ListTypes = 12
	}
	#endregion

	/// <summary>
	/// Summary description for Dictionaries.
	/// </summary>
	public class Dictionaries
	{
		#region GetList
		/// <summary>
		///		ItemId, ItemName, Weight, StateId, CanDelete (int)
		/// </summary>
		public static DataTable GetList(DictionaryTypes dict)
		{
			DataTable dt = null;
			switch(dict)
			{
				case DictionaryTypes.Categories:
					dt = DBCommon.GetListCategoriesForDictionaries();
					break;
				case DictionaryTypes.IncidentSeverities:
					dt = DBIncident.GetListIncidentSeverityForDictionaries();
					break;
				case DictionaryTypes.IncidentTypes:
					dt = DBIncident.GetListIncidentTypesForDictionaries();
					break;
				case DictionaryTypes.ProjectTypes:
					dt = DBProject.GetListProjectTypesForDictionaries();
					break;
				case DictionaryTypes.ProjectCategories:
					dt = DBProject.GetListProjectCategoriesForDictionaries();
					break;
				case DictionaryTypes.IncidentCategories:
					dt = DBIncident.GetListIncidentCategoriesForDictionaries();
					break;
				case DictionaryTypes.Currency:
					dt = DBCommon.GetListCurrencyForDictionaries();
					break;
				case DictionaryTypes.DocumentStatus:
					dt = DBDocument.GetListDocumentStatusForDictionaries(Security.CurrentUser.LanguageId);
					break;
				case DictionaryTypes.ProjectPhases:
					dt = DBProject.GetListProjectPhasesForDictionaries();
					break;
				case DictionaryTypes.RiskLevels:
					dt = DBProject.GetListRiskLevelsForDictionaries();
					break;
			}

			if (dict != DictionaryTypes.ProjectPhases && dict != DictionaryTypes.RiskLevels && dt != null)
				dt.Columns.Add("Weight", typeof(int));

			return dt;
		}
		#endregion

		#region AddItem
		public static void AddItem(string ItemName, DictionaryTypes dict)
		{
			AddItem(ItemName, 0, dict);
		}

		public static void AddItem(string ItemName, int Param, DictionaryTypes dict)
		{
			if (!Company.CheckDiskSpace())
				throw new MaxDiskSpaceException();

			switch(dict) 
			{
				case DictionaryTypes.Categories:
					DBCommon.AddCategory(ItemName);
					break;
				case DictionaryTypes.IncidentSeverities:
					DBIncident.AddIncidentSeverity(ItemName);
					break;
				case DictionaryTypes.IncidentTypes:
					DBIncident.AddIncidentType(ItemName);
					break;
				case DictionaryTypes.ProjectTypes:
					AddProjectType(ItemName);
					break;
				case DictionaryTypes.ProjectCategories:
					DBProject.AddProjectCategory(ItemName);
					break;
				case DictionaryTypes.IncidentCategories:
					DBIncident.AddIncidentCategory(ItemName);
					break;
				case DictionaryTypes.Currency:
					DBCommon.AddCurrency(ItemName);
					break;
				case DictionaryTypes.DocumentStatus:
					DBDocument.AddDocumentStatus(ItemName, Param);
					break;
				case DictionaryTypes.ProjectPhases:
					DBProject.AddProjectPhase(ItemName, Param);
					break;
				case DictionaryTypes.RiskLevels:
					DBProject.AddRiskLevel(ItemName, Param);
					break;
			}
		}
		#endregion

		#region UpdateItem
		public static void UpdateItem(int ItemId, string ItemName, DictionaryTypes dict)
		{
			UpdateItem(ItemId, ItemName, 0, dict);
		}

		public static void UpdateItem(int ItemId, string ItemName, int Param, DictionaryTypes dict)
		{
			switch(dict) 
			{
				case DictionaryTypes.Categories:
					DBCommon.UpdateCategory(ItemId, ItemName);
					break;
				case DictionaryTypes.IncidentSeverities:
					DBIncident.UpdateIncidentSeverity(ItemId, ItemName);
					break;
				case DictionaryTypes.IncidentTypes:
					DBIncident.UpdateIncidentType(ItemId, ItemName);
					break;
				case DictionaryTypes.ProjectTypes:
					UpdateProjectType(ItemId, ItemName);
					break;
//				case DictionaryTypes.Clients:
//					DBProject.UpdateClient(ItemId, ItemName);
//					break;
				case DictionaryTypes.ProjectCategories:
					DBProject.UpdateProjectCategory(ItemId, ItemName);
					break;
				case DictionaryTypes.IncidentCategories:
					DBIncident.UpdateIncidentCategory(ItemId, ItemName);
					break;
				case DictionaryTypes.Currency:
					DBCommon.UpdateCurrency(ItemId, ItemName);
					break;
				case DictionaryTypes.DocumentStatus:
					DBDocument.UpdateDocumentStatus(ItemId, ItemName, Param);
					break;
				case DictionaryTypes.ProjectPhases:
					DBProject.UpdateProjectPhase(ItemId, ItemName, Param);
					break;
				case DictionaryTypes.RiskLevels:
					DBProject.UpdateRiskLevel(ItemId, ItemName, Param);
					break;
			}
		}
		#endregion

		#region DeleteItem
		public static void DeleteItem(int ItemId, DictionaryTypes dict)
		{
			switch(dict) 
			{
				case DictionaryTypes.Categories:
					DBCommon.DeleteCategory(ItemId);
					break;
				case DictionaryTypes.IncidentSeverities:
					DBIncident.DeleteIncidentSeverity(ItemId);
					break;
				case DictionaryTypes.IncidentTypes:
					DBIncident.DeleteIncidentType(ItemId);
					break;
				case DictionaryTypes.ProjectTypes:
					DeleteProjectType(ItemId);
					break;
//				case DictionaryTypes.Clients:
//					DBProject.DeleteClient(ItemId);
//					break;
				case DictionaryTypes.ProjectCategories:
					DBProject.DeleteProjectCategory(ItemId);
					break;
				case DictionaryTypes.IncidentCategories:
					DBIncident.DeleteIncidentCategory(ItemId);
					break;
				case DictionaryTypes.Currency:
					DBCommon.DeleteCurrency(ItemId);
					break;
				case DictionaryTypes.DocumentStatus:
					DBDocument.DeleteDocumentStatus(ItemId);
					break;
				case DictionaryTypes.ProjectPhases:
					DBProject.DeleteProjectPhase(ItemId);
					break;
				case DictionaryTypes.RiskLevels:
					DBProject.DeleteRiskLevel(ItemId);
					break;
			}
		}
		#endregion

		#region AddProjectType
		private static void AddProjectType(string ProjectTypeName)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				int ProjectTypeId =	DBProject.AddProjectType(ProjectTypeName);
				string MetaClassName = String.Format("ProjectsEx_{0}", ProjectTypeId);

				MetaClass metaProjectTypeEx = MetaClass.Create(MetaClassName, ProjectTypeName, MetaClassName, MetaClass.Load("Projects"), false, "Project Type Extension");

				RebuildProjectsSearch(metaProjectTypeEx);

				tran.Commit();
			}
		}
		#endregion

		#region UpdateProjectType
		private static void UpdateProjectType(int ProjectTypeId, string ProjectTypeName)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				string MetaClassName = String.Format("ProjectsEx_{0}", ProjectTypeId);

				MetaClass metaProjectTypeEx = MetaClass.Load(MetaClassName);
				metaProjectTypeEx.FriendlyName = ProjectTypeName;
				DBProject.UpdateProjectType(ProjectTypeId, ProjectTypeName);
		
				tran.Commit();
			}
		}
		#endregion

		#region DeleteProjectType
		private static void DeleteProjectType(int ProjectTypeId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				DBProject.DeleteProjectType(ProjectTypeId);

				string MetaClassName = String.Format("ProjectsEx_{0}", ProjectTypeId);
				MetaClass mc = MetaClass.Load(MetaClassName);

				MetaClass parent = mc.Parent;

				if (mc != null)
					MetaClass.Delete(mc.Id);

				RebuildProjectsSearch(parent);

				tran.Commit();
			}
		}
		#endregion

		#region Rebuild Projects Search
		private static void RebuildProjectsSearch(MetaClass mc)
		{
			string ProjectSearch = @"if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProjectsSearch]') and " +
				@"OBJECTPROPERTY(id, N'IsProcedure') = 1)" + "\r\n" +
				@"drop procedure [dbo].[ProjectsSearch]" + "\r\n" +
				@"GO" + "\r\n" +
				"\r\n" +
				@"SET QUOTED_IDENTIFIER OFF " + "\r\n" +
				@"GO" + "\r\n" +
				@"SET ANSI_NULLS ON " + "\r\n" +
				@"GO" + "\r\n" +
				"\r\n" +

				@"CREATE PROCEDURE [dbo].[ProjectsSearch]" + Environment.NewLine +
				@"	@UserId as int," + Environment.NewLine +
				@"	@Keyword as nvarchar(100)" + Environment.NewLine +
				@"as " + Environment.NewLine +
				@"DECLARE @IsPPM_Exec bit" + Environment.NewLine +
				@"SET @IsPPM_Exec = 0" + Environment.NewLine +
				@"IF EXISTS(SELECT * FROM USER_GROUP WHERE UserId = @UserId AND (GroupId = 4 OR GroupId = 7))		-- PPM " +
				@"or Exec" + Environment.NewLine +
				@"	SET @IsPPM_Exec = 1" + Environment.NewLine +
				@"SET @Keyword = '%' + @Keyword + '%'" + Environment.NewLine +
				@"SELECT P.ProjectId, P.ManagerId, P.Title, P.[Description], P.StatusId, P.PercentCompleted, " + Environment.NewLine +
				@"	P.StartDate, P.FinishDate, P.TargetStartDate, P.TargetFinishDate, P.ActualStartDate, " +
				@"P.ActualFinishDate, ISNULL(P.ProjectCode, '') AS ProjectCode" + Environment.NewLine +
				@"  FROM PROJECTS P " + Environment.NewLine +
				@"	LEFT JOIN PROJECT_SECURITY_ALL PS ON (P.ProjectId = PS.ProjectId AND PS.PrincipalId = @UserId)" + Environment.NewLine +
				@"	-- Auto-Generate Code" + Environment.NewLine +
				@"	[%=ForEach(""SELECT MC.MetaClassId, MC.TableName FROM MetaClass MC JOIN MetaClass PMC ON " +
				@"MC.ParentClassId=PMC.MetaClassId WHERE  PMC.Name = 'Projects'"",""  "",""LEFT JOIN {1} AS MDPO{0} " +
				@"ON P.ProjectId = MDPO{0}.ObjectId"")=%]" + Environment.NewLine +
				@"	-- End Auto-Generate Code" + Environment.NewLine +
				@"  WHERE (@IsPPM_Exec = 1 OR PS.IsManager = 1 OR PS.IsExecutiveManager = 1 OR PS.IsTeamMember = 1 OR " +
				@"PS.IsSponsor = 1 OR PS.IsStakeHolder = 1)" + Environment.NewLine +
				@"	AND " + Environment.NewLine +
				@"	(" + Environment.NewLine +
				@"		P.Title LIKE @Keyword OR " + Environment.NewLine +
				@"		P.[Description] LIKE @Keyword OR " + Environment.NewLine +
				@"		P.ProjectCode LIKE @Keyword OR " + Environment.NewLine +
				@"		CAST(P.ProjectId AS nvarchar(10)) LIKE @Keyword " + Environment.NewLine +
				@"		OR" + Environment.NewLine +
				@"		-- Auto-Generate Code" + Environment.NewLine +
				@"		[%=ForEach(""SELECT MC.MetaClassId, MC.Name FROM MetaClass MC JOIN MetaClass PMC ON " +
				@"MC.ParentClassId=PMC.MetaClassId WHERE  PMC.Name = " +
				@"'Projects'"",""OR"",""[%=SearchByKeyword(MDPO{0},@Keyword,{1})=%]"")=%]		" + Environment.NewLine +
				@"		-- End Auto-Generate Code" + Environment.NewLine +
				@"	)" + Environment.NewLine+

				@"GO" + "\r\n" +
				"\r\n" +
				@"SET QUOTED_IDENTIFIER OFF " + "\r\n" +
				@"GO" + "\r\n" +
				@"SET ANSI_NULLS ON " + "\r\n" +
				@"GO" + "\r\n" +
				"\r\n" +
				@"";;

			mc.FieldListChangedSqlScript = ProjectSearch;
		}
		#endregion
	}
}
