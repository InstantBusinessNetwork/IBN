using System;
using System.Data;
using System.Resources;

using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QToDo.
	/// </summary>
	public class QToDo : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		public QToDo()
		{
		}

		//		public override QObject[] GetExtensions()
		//		{
		//			return new QObject[]{_Tasks};
		//		}

		protected override void LoadScheme()
		{
			OwnerTable = "TODO";

			this.Fields.Add(new QField("ToDoId", LocRM.GetString("ToDoId"), "ToDoId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));
			this.Fields.Add(new QField("ToDoTitle", LocRM.GetString("ToDoTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			//this.Fields.Add(new QField("ToDoResources",LocRM.GetString("ToDoResources"),"{0}.FirstName + ' ' + {0}.LastName",DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter,
			//    new QFieldJoinRelation[]
			//    {
			//        new QFieldJoinRelation(this.OwnerTable,"TODO_RESOURCES","ToDoId","ToDoId"),
			//        new QFieldJoinRelation("TODO_RESOURCES","USERS","PrincipalId","PrincipalId")
			//    }));

			//this.Fields.Add(new QField("ToDoResourcesId","ToDoResourcesId","PrincipalId",DbType.Int32,QFieldUsingType.Abstract,
			//    new QFieldJoinRelation[]
			//    {
			//        new QFieldJoinRelation(this.OwnerTable,"TODO_RESOURCES","ToDoId","ToDoId"),
			//        new QFieldJoinRelation("TODO_RESOURCES","USERS","PrincipalId","PrincipalId")
			//    }));

			this.Fields.Add(new QField("ToDoResources", LocRM.GetString("ToDoResources"),

				"CASE {0}.IsGroup " +
				"WHEN 0 THEN (SELECT UR.LastName + ' ' + UR.FirstName FROM USERS UR WHERE UR.PrincipalId = {0}.PrincipalId) " +
				"WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = {0}.PrincipalId) END",

				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"TODO_RESOURCES","ToDoId","ToDoId"),
					new QFieldJoinRelation("TODO_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			this.Fields.Add(new QField("ToDoResourcesId", "ToDoResourcesId",
				"PrincipalId",
				DbType.String, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"TODO_RESOURCES","ToDoId","ToDoId"),
					new QFieldJoinRelation("TODO_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoResourcesId"], this.Fields["ToDoResources"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT PrincipalId FROM TODO_RESOURCES) ORDER BY [Value]"));

			this.Fields.Add(new QField("ToDoCreator", LocRM.GetString("ToDoCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Fields.Add(new QField("ToDoCreatorId", LocRM.GetString("ToDoCreatorId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoCreatorId"], this.Fields["ToDoCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM TASKS) ORDER BY [Value]"));

			this.Fields.Add(new QField("ToDoManager", LocRM.GetString("ToDoManager"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));
			this.Fields.Add(new QField("ToDoManagerId", LocRM.GetString("ToDoManagerId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoManagerId"], this.Fields["ToDoManager"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT ManagerId FROM PROJECTS) ORDER BY [Value]"));

			if (Mediachase.Ibn.License.ProjectManagement)
			{
				this.Fields.Add(new QField("ToDoProject", LocRM.GetString("ToDoProject"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
				this.Fields.Add(new QField("ToDoProjectId", LocRM.GetString("ToDoProjectId"), "ProjectId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));

				this.Dictionary.Add(new QDictionary(this.Fields["ToDoProjectId"], this.Fields["ToDoProject"], "SELECT ProjectId as Id, Title as [Value] FROM PROJECTS ORDER BY [Value]"));

				// 2009-09-30
				this.Fields.Add(new QField("PrjCode", LocRM.GetString("PrjCode"), "ProjectCode", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
			}

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
				"WHEN {0}.StartDate > GetUtcDate() THEN N'" + LocRM.GetString("ToDoStatus_Upcoming") + "' ELSE N'" + LocRM.GetString("ToDoStatus_Active") + "' END	) " +
				"END)",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("ToDoStatusId", "ToDoStatusId",
				"(CASE {0}.IsCompleted " +
				"WHEN 1 THEN ( (SELECT CR.ReasonId FROM COMPLETION_REASON_LANGUAGE AS CR WHERE CR.ReasonId = {0}.ReasonId AND LanguageId = {1})) " +
				"WHEN 0 THEN (CASE WHEN {0}.FinishDate < GetUtcDate() THEN -100 " +
				"WHEN {0}.StartDate > GetUtcDate() THEN -101 ELSE -102 END	) " +
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

			//this.Fields.Add(new QField("ToDoType",LocRM.GetString("ToDoType"),"ToDo"));

			this.Fields.Add(new QField("ToDoGeneralCategories", LocRM.GetString("ToDoGeneralCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","ToDoId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.ToDo).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("ToDoGeneralCategoriesId", "ToDoGeneralCategoriesId", "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","ToDoId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.ToDo).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoGeneralCategoriesId"], this.Fields["ToDoGeneralCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM CATEGORIES"));

			// New Addod 2006-10-18
			//this.Fields.Add(new QField("ToDoTaskTime",LocRM.GetString("ToDoTaskTime"),"TaskTime", DbType.Int32,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort,false));

			// 2006-12-13 OZ: Client
			//this.Fields.Add(new QField("ToDoClient",LocRM.GetString("PrjClient"), 
			//    "(ISNULL((SELECT O.OrgName FROM Organizations O WHERE O.OrgId = {0}.OrgId),(SELECT V.FullName FROM VCard V WHERE V.VCardId = {0}.VCardId)))", 
			//    DbType.String,
			//    QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort));

			//this.Fields.Add(new QField("ToDoClientId","ToDoClientId", 
			//    "(ISNULL({0}.OrgId,-{0}.VCardId))", 
			//    DbType.Int32,
			//    QFieldUsingType.Abstract));

			//this.Dictionary.Add(new QDictionary(this.Fields["ToDoClientId"],this.Fields["ToDoClient"],
			//    "SELECT (O.OrgId) AS Id , O.OrgName as Value FROM Organizations O UNION SELECT (-V.VCardId) AS Id , V.FullName as Value FROM VCard V"));

			// 2007-10-17 New Field is assigned with TimeTracking
			this.Fields.Add(new QField("ToDoTaskTime", LocRM.GetString("TaskTime"), "TaskTime", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoTotalMinutes", LocRM.GetString("TotalMinutes"), "TotalMinutes", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("ToDoTotalApproved", LocRM.GetString("TotalApproved"), "TotalApproved", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2008-07-28 Add Resources With Percen 
			this.Fields.Add(new QField("ToDoResourcesWithPercent", LocRM.GetString("ToDoResourcesWithPercent"),
				"(CASE (SELECT PP.IsGroup FROM PRINCIPALS PP WHERE PP.PrincipalId =  {0}.PrincipalId) " +
				"WHEN 0 THEN (SELECT UR.LastName + ' ' + UR.FirstName FROM USERS UR WHERE UR.PrincipalId = {0}.PrincipalId) " +
				"WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = {0}.PrincipalId) END) + " +
				"+ (CASE AL1.CompletionTypeId WHEN 1 THEN (N' (' + CAST({0}.PercentCompleted AS NVARCHAR(20)) + N'%)')	WHEN 2 THEN N''	END)",

				DbType.String, QFieldUsingType.Field,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"TODO_RESOURCES","ToDoId","ToDoId")//,
					//new QFieldJoinRelation("TODO_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			/*	this.Fields.Add(new QField("ToDoResourcesWithPercentId", "ToDoResourcesWithPercentId",
					"PrincipalId",
					DbType.String, QFieldUsingType.Abstract,
					new QFieldJoinRelation[]
					{
						new QFieldJoinRelation(this.OwnerTable,"TODO_RESOURCES","ToDoId","ToDoId"),
						new QFieldJoinRelation("TODO_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
					}));

				this.Dictionary.Add(new QDictionary(this.Fields["ToDoResourcesWithPercentId"], this.Fields["ToDoResourcesWithPercent"], "SELECT DISTINCT TR.PrincipalId as Id, (CASE P.IsGroup WHEN 0 THEN (SELECT UR.FirstName + ' ' + UR.LastName FROM USERS UR WHERE UR.PrincipalId = TR.PrincipalId) WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = TR.PrincipalId) END) as Value FROM TODO_RESOURCES TR INNER JOIN PRINCIPALS P ON P.PrincipalId = TR.PrincipalId"));
			*/

			///////////////////////////
			// 2008-10-20 New Clients
			// Organization OrgUid
			this.Fields.Add(new QField("ToDoOrg", LocRM.GetString("PrjOrganization"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));
			this.Fields.Add(new QField("ToDoOrgUid", "ToDoOrgUid", "OrganizationId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoOrgUid"], this.Fields["ToDoOrg"],
				"SELECT DISTINCT OrganizationId as Id, Name as Value FROM cls_Organization ORDER BY Value"));

			// Contact ContactUid
			this.Fields.Add(new QField("ToDoContact", LocRM.GetString("PrjContact"), "FullName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));
			this.Fields.Add(new QField("ToDoContactUid", "ToDoContactUid", "ContactId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoContactUid"], this.Fields["ToDoContact"],
				"SELECT DISTINCT ContactId as Id, FullName as Value FROM cls_Contact ORDER BY Value"));

			// Client
			this.Fields.Add(new QField("ToDoClient", LocRM.GetString("PrjClient"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "ToDoClients", "ToDoId", "ToDoId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Fields.Add(new QField("ToDoClientUid", "ClientId", "ClientId", DbType.Guid, QFieldUsingType.Abstract,
							new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "ToDoClients", "ToDoId", "ToDoId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoClientUid"], this.Fields["ToDoClient"],
				"SELECT DISTINCT ClientId as Id, Name as Value FROM Clients ORDER BY Value"));

			///////////////////////////
			// 2008-12-01
			this.Fields.Add(new QField("ToDoPriority", LocRM.GetString("ToDoPriority"), "PriorityName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));
			this.Fields.Add(new QField("ToDoPriorityId", LocRM.GetString("ToDoPriority"), "PriorityId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Dictionary.Add(new QDictionary(this.Fields["ToDoPriorityId"], this.Fields["ToDoPriority"], "SELECT PriorityId as Id, PriorityName as Value FROM PRIORITY_LANGUAGE WHERE LanguageId = {0}"));

			// 2010-06-10
			this.Fields.Add(new QField("ToDoDescription", LocRM.GetString("ToDoDescription"), "Description", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter, false));

			QMetaLoader.LoadMetaField(this, "ToDoEx");
		}
	}
}
