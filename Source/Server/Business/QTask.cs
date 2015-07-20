using System;
using System.Data;
using System.Resources;

using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QTask.
	/// </summary>
	public class QTask : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		protected override void LoadScheme()
		{
			OwnerTable = "TASKS";

			this.Fields.Add(new QField("ToDoId", LocRM.GetString("ToDoId"), "TaskId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));
			this.Fields.Add(new QField("ToDoTitle", LocRM.GetString("ToDoTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			//				this.Fields.Add(new QField("ToDoResources","Resources","{0}.FirstName + ' ' + {0}.LastName",DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter,
			//					new QFieldJoinRelation[]
			//				{
			//					new QFieldJoinRelation(this.OwnerTable,"TASK_RESOURCES","TaskId","TaskId"),
			//					new QFieldJoinRelation("TASK_RESOURCES","USERS","PrincipalId","PrincipalId")
			//				}));

			this.Fields.Add(new QField("ToDoResources", LocRM.GetString("ToDoResources"),
				"CASE {0}.IsGroup " +
				"WHEN 0 THEN (SELECT UR.LastName + ' ' + UR.FirstName FROM USERS UR WHERE UR.PrincipalId = {0}.PrincipalId) " +
				"WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = {0}.PrincipalId) END",

				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"TASK_RESOURCES","TaskId","TaskId"),
					new QFieldJoinRelation("TASK_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			this.Fields.Add(new QField("ToDoResourcesId", "ToDoResourcesId",
				"PrincipalId",
				DbType.String, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"TASK_RESOURCES","TaskId","TaskId"),
					new QFieldJoinRelation("TASK_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoResourcesId"], this.Fields["ToDoResources"], "SELECT DISTINCT TR.PrincipalId as Id, (CASE P.IsGroup WHEN 0 THEN (SELECT UR.LastName + ' ' + UR.FirstName FROM USERS UR WHERE UR.PrincipalId = TR.PrincipalId) WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = TR.PrincipalId) END) as Value FROM TASK_RESOURCES TR INNER JOIN PRINCIPALS P ON P.PrincipalId = TR.PrincipalId"));

			this.Fields.Add(new QField("ToDoCreator", LocRM.GetString("ToDoCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Fields.Add(new QField("ToDoCreatorId", LocRM.GetString("ToDoCreatorId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoCreatorId"], this.Fields["ToDoCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName +' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM TASKS) ORDER BY [Value]"));

			this.Fields.Add(new QField("ToDoManager", LocRM.GetString("ToDoManager"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECTS","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECTS","USERS","ManagerId","PrincipalId")
				}));
			this.Fields.Add(new QField("ToDoManagerId", LocRM.GetString("ToDoManagerId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECTS","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECTS","USERS","ManagerId","PrincipalId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoManagerId"], this.Fields["ToDoManager"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT ManagerId FROM PROJECTS) ORDER BY [Value]"));

			this.Fields.Add(new QField("ToDoProject", LocRM.GetString("ToDoProject"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
			this.Fields.Add(new QField("ToDoProjectId", LocRM.GetString("ToDoProjectId"), "ProjectId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoProjectId"], this.Fields["ToDoProject"], "SELECT ProjectId as Id, Title as [Value] FROM PROJECTS ORDER BY [Value]"));

			// 2009-09-30
			this.Fields.Add(new QField("PrjCode", LocRM.GetString("PrjCode"), "ProjectCode", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));

			this.Fields.Add(new QField("ToDoCreationDate", LocRM.GetString("ToDoCreationDate"), "CreationDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoStartDate", LocRM.GetString("ToDoStartDate"), "StartDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoDueDate", LocRM.GetString("ToDoDueDate"), "FinishDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoActualStartDate", LocRM.GetString("ToDoActualStartDate"), "ActualStartDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoActualDueDate", LocRM.GetString("ToDoActualDueDate"), "ActualFinishDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("ToDoOverallStatus", LocRM.GetString("ToDoOverallStatus"), "PercentCompleted", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("ToDoStatus", LocRM.GetString("ToDoStatus"),
				"(CASE {0}.IsCompleted " +
				"WHEN 1 THEN ( (SELECT ReasonName FROM COMPLETION_REASON_LANGUAGE AS CR WHERE CR.ReasonId = {0}.ReasonId AND LanguageId = {1})) " +
				"WHEN 0 THEN (CASE WHEN {0}.FinishDate < GetUtcDate() THEN N'" + LocRM.GetString("ToDoStatus_Overdue") + "' " +
				"WHEN {0}.StartDate > GetUtcDate() THEN N'" + LocRM.GetString("ToDoStatus_Upcoming") + "' ELSE N'" + LocRM.GetString("ToDoStatus_Active") + "' END ) " +
				"END)",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("ToDoStatusId", "ToDoStatusId",
				"(CASE {0}.IsCompleted " +
				"WHEN 1 THEN ( (SELECT CR.ReasonId FROM COMPLETION_REASON_LANGUAGE AS CR WHERE CR.ReasonId = {0}.ReasonId AND LanguageId = {1})) " +
				"WHEN 0 THEN (CASE WHEN {0}.FinishDate < GetUtcDate() THEN -100 " +
				"WHEN {0}.StartDate > GetUtcDate() THEN -101 ELSE -102 END ) " +
				"END)",
				DbType.Int32, QFieldUsingType.Abstract));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoStatusId"], this.Fields["ToDoStatus"],
					" SELECT ReasonId As Id, ReasonName As Value FROM COMPLETION_REASON_LANGUAGE WHERE LanguageId = {0}" +
					" UNION " +
					" SELECT -100 As Id, N'" + LocRM.GetString("ToDoStatus_Overdue") + "' As Value" +
					" UNION " +
					" SELECT -101 As Id, N'" + LocRM.GetString("ToDoStatus_Upcoming") + "' As Value" +
					" UNION " +
					" SELECT -102 As Id, N'" + LocRM.GetString("ToDoStatus_Active") + "' As Value"
				));

			//this.Fields.Add(new QField("ToDoType",LocRM.GetString("ToDoType"),"Task"));

			this.Fields.Add(new QField("ToDoGeneralCategories", LocRM.GetString("ToDoGeneralCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","TaskId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Task).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("ToDoGeneralCategoriesId", "ToDoGeneralCategoriesId", "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","TaskId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Task).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoGeneralCategoriesId"], this.Fields["ToDoGeneralCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM CATEGORIES"));

			// New Addod 2006-10-18
			this.Fields.Add(new QField("ToDoDuration", LocRM.GetString("ToDoDuration"), "Duration", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			//this.Fields.Add(new QField("ToDoTaskTime",LocRM.GetString("ToDoTaskTime"),"TaskTime", DbType.Int32,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort,false));

			// 2007-10-17 New Field is assigned with TimeTracking
			this.Fields.Add(new QField("ToDoTaskTime", LocRM.GetString("TaskTime"), "TaskTime", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoTotalMinutes", LocRM.GetString("TotalMinutes"), "TotalMinutes", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoTotalApproved", LocRM.GetString("TotalApproved"), "TotalApproved", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2008-02-27 Priority
			this.Fields.Add(new QField("ToDoPriority", LocRM.GetString("ToDoPriority"), "PriorityName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));
			this.Fields.Add(new QField("ToDoPriorityId", LocRM.GetString("ToDoPriority"), "PriorityId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoPriorityId"], this.Fields["ToDoPriority"], "SELECT PriorityId as Id, PriorityName as Value FROM PRIORITY_LANGUAGE WHERE LanguageId = {0}"));

			// 2008-06-01 Outline Number + Outline Level
			this.Fields.Add(new QField("TaskOutlineNumber", LocRM.GetString("TaskOutlineNumber"), "OutlineNumber", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("TaskOutlineLevel", LocRM.GetString("TaskOutlineLevel"), "OutlineLevel", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2008-07-28 Add Resources With Percen 
			this.Fields.Add(new QField("ToDoResourcesWithPercent", LocRM.GetString("ToDoResourcesWithPercent"),
				"(CASE (SELECT PP.IsGroup FROM PRINCIPALS PP WHERE PP.PrincipalId =  {0}.PrincipalId) " +
				"WHEN 0 THEN (SELECT UR.LastName + ' ' + UR.FirstName FROM USERS UR WHERE UR.PrincipalId = {0}.PrincipalId) " +
				"WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = {0}.PrincipalId) END) + " +
				"+ (CASE AL1.CompletionTypeId WHEN 1 THEN (N' (' + CAST({0}.PercentCompleted AS NVARCHAR(20)) + N'%)')	WHEN 2 THEN N''	END)",

				DbType.String, QFieldUsingType.Field,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"TASK_RESOURCES","TaskId","TaskId")//,
					//new QFieldJoinRelation("TASK_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			// 2008-09-03 TaskNum
			this.Fields.Add(new QField("TaskNum", LocRM.GetString("TaskNum"), "TaskNum", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2009-01-20 OZ New Columns Type (Common, Milestone, Summary)
			this.Fields.Add(new QField("TaskType", LocRM.GetString("TaskType"),
				"(CASE ({0}.IsMileStone + 2*{0}.IsSummary) " +
				"WHEN 1 THEN N'" + LocRM.GetString("TaskTypeMilestone") + "' " +
				"WHEN 2 THEN N'" + LocRM.GetString("TaskTypeSummary") + "' " +
				"ELSE N'" + LocRM.GetString("TaskTypeCommon") + "' END)",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("TaskTypeId", "TaskTypeId", "({0}.IsMileStone + 2*{0}.IsSummary)", DbType.Int32, QFieldUsingType.Abstract, false));

			this.Dictionary.Add(new QDictionary(this.Fields["TaskTypeId"], this.Fields["TaskType"], "SELECT 0 As Id, N'" + LocRM.GetString("TaskTypeCommon") + "' As Value UNION SELECT 1 As Id, N'" + LocRM.GetString("TaskTypeMilestone") + "'  As Value UNION SELECT 2 As Id, N'" + LocRM.GetString("TaskTypeSummary") + "' As Value"));


			/*	this.Fields.Add(new QField("ToDoResourcesWithPercentId", "ToDoResourcesWithPercentId",
					"PrincipalId",
					DbType.String, QFieldUsingType.Abstract,
					new QFieldJoinRelation[]
					{
						new QFieldJoinRelation(this.OwnerTable,"TASK_RESOURCES","TaskId","TaskId"),
						new QFieldJoinRelation("TASK_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
					}));

				this.Dictionary.Add(new QDictionary(this.Fields["ToDoResourcesWithPercentId"], this.Fields["ToDoResourcesWithPercent"], "SELECT DISTINCT TR.PrincipalId as Id, (CASE P.IsGroup WHEN 0 THEN (SELECT UR.FirstName + ' ' + UR.LastName FROM USERS UR WHERE UR.PrincipalId = TR.PrincipalId) WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = TR.PrincipalId) END) as Value FROM TASK_RESOURCES TR INNER JOIN PRINCIPALS P ON P.PrincipalId = TR.PrincipalId"));
			*/


			// 2006-12-13 OZ: Client
			//			this.Fields.Add(new QField("ToDoClient",LocRM.GetString("PrjClient"), 
			//				"(ISNULL((SELECT O.OrgName FROM Organizations O WHERE O.OrgId = {0}.OrgId),(SELECT V.FullName FROM VCard V WHERE V.VCardId = {0}.VCardId)))", 
			//				DbType.String,
			//				QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort));
			//
			//			this.Fields.Add(new QField("ToDoClientId","ToDoClientId", 
			//				"(ISNULL({0}.OrgId,-{0}.VCardId))", 
			//				DbType.Int32,
			//				QFieldUsingType.Abstract));
			//
			//			this.Dictionary.Add(new QDictionary(this.Fields["ToDoClientId"],this.Fields["ToDoClient"],
			//				"SELECT (O.OrgId) AS Id , O.OrgName as Value FROM Organizations O UNION SELECT (-V.VCardId) AS Id , V.FullName as Value FROM VCard V"));

			// 2010-06-10
			this.Fields.Add(new QField("ToDoDescription", LocRM.GetString("ToDoDescription"), "Description", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter, false));


			QMetaLoader.LoadMetaField(this, "TaskEx");
		}
	}


}
