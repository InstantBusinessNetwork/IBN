using System;
using System.Data;
using System.Resources;

using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QIncident.
	/// </summary>
	public class QIncident : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		public QIncident()
		{
		}

		protected override void LoadScheme()
		{
			OwnerTable = "INCIDENTS";

			this.Fields.Add(new QField("IncId", LocRM.GetString("IncId"), "IncidentId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));
			this.Fields.Add(new QField("IncTitle", LocRM.GetString("IncTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("IncType", LocRM.GetString("IncType"), "TypeName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "INCIDENT_TYPES", "TypeId", "TypeId")));
			this.Fields.Add(new QField("IncTypeId", LocRM.GetString("IncType"), "TypeId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "INCIDENT_TYPES", "TypeId", "TypeId")));

			this.Dictionary.Add(new QDictionary(this.Fields["IncTypeId"], this.Fields["IncType"], "SELECT TypeId as Id, TypeName as Value FROM INCIDENT_TYPES"));

			//this.Fields.Add(new QField("IncState",LocRM.GetString("IncState"),"StateName", DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort,
			//new QFieldJoinRelation(this.OwnerTable, "INCIDENT_STATE_LANGUAGE", "StateId", "StateId", "LanguageId")));

			//this.Fields.Add(new QField("IncStateId","IncStateId","StateId", DbType.Int32,QFieldUsingType.Abstract,
			//    new QFieldJoinRelation(this.OwnerTable, "INCIDENT_STATE_LANGUAGE", "StateId", "StateId", "LanguageId")));


			// OZ 2008-11-18 Added Overdue state
			this.Fields.Add(new QField("IncState", LocRM.GetString("IncState"),
				"(SELECT StateName FROM INCIDENT_STATE_LANGUAGE WHERE LanguageId = {1} AND StateId = " +
				"(CASE WHEN " +
				"({0}.StateId IN (1, 2, 6, 7) " +
				"AND " +
				"( " +
					"{0}.ExpectedResolveDate < GetUtcDate() OR (EXISTS(SELECT INM.NewMessage FROM INCIDENT_NEWMESSAGE INM WHERE INM.IncidentId = {0}.IncidentId AND INM.NewMessage = 1) AND {0}.ExpectedResponseDate < GetUtcDate())) " +
				") " +
				"OR " +
				"( " +
					"{0}.StateId IN (1, 2, 6) " +
					"AND {0}.ResponsibleId <= 0 " +
					"AND {0}.ExpectedAssignDate < GetUtcDate() " +
				")" +
				"THEN 3 ELSE {0}.StateId " +
				"END))", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));


			this.Fields.Add(new QField("IncStateId", "IncStateId", "(CASE WHEN " +
				"({0}.StateId IN (1, 2, 6, 7) " +
				"AND " +
				"( " +
					"{0}.ExpectedResolveDate < GetUtcDate() OR (EXISTS(SELECT INM.NewMessage FROM INCIDENT_NEWMESSAGE INM WHERE INM.IncidentId = {0}.IncidentId AND INM.NewMessage = 1) AND {0}.ExpectedResponseDate < GetUtcDate())) " +
				") " +
				"OR " +
				"( " +
					"{0}.StateId IN (1, 2, 6) " +
					"AND {0}.ResponsibleId <= 0 " +
					"AND {0}.ExpectedAssignDate < GetUtcDate() " +
				")" +
				"THEN 3 ELSE {0}.StateId " +
				"END)", DbType.Int32, QFieldUsingType.Abstract, false));

			this.Dictionary.Add(new QDictionary(this.Fields["IncStateId"], this.Fields["IncState"], "SELECT StateId as Id, StateName as Value FROM INCIDENT_STATE_LANGUAGE WHERE LanguageId = {0}"));

			if (Mediachase.Ibn.License.ProjectManagement)
			{
				this.Fields.Add(new QField("IncProject", LocRM.GetString("IncProject"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
				this.Fields.Add(new QField("IncProjectId", LocRM.GetString("IncProjectId"), "ProjectId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));

				this.Dictionary.Add(new QDictionary(this.Fields["IncProjectId"], this.Fields["IncProject"], "SELECT ProjectId as Id, Title as [Value] FROM PROJECTS ORDER BY [Value]"));

				// 2009-09-30
				this.Fields.Add(new QField("PrjCode", LocRM.GetString("PrjCode"), "ProjectCode", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
			}

			this.Fields.Add(new QField("IncPriority", LocRM.GetString("IncPriority"), "PriorityName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Fields.Add(new QField("IncPriorityId", LocRM.GetString("IncPriority"), "PriorityId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Dictionary.Add(new QDictionary(this.Fields["IncPriorityId"], this.Fields["IncPriority"], "SELECT PriorityId as Id, PriorityName as Value FROM PRIORITY_LANGUAGE WHERE LanguageId = {0}"));

			this.Fields.Add(new QField("IncCreator", LocRM.GetString("IncCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Fields.Add(new QField("IncCreatorId", LocRM.GetString("IncCreatorId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["IncCreatorId"], this.Fields["IncCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM INCIDENTS) ORDER BY [Value]"));

			//			this.Fields.Add(new QField("IncManager",LocRM.GetString("IncManager"),"{0}.FirstName + ' ' + {0}.LastName",DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort,
			//				new QFieldJoinRelation(this.OwnerTable,"USERS","ManagerId","PrincipalId")));
			//			this.Fields.Add(new QField("IncManagerId",LocRM.GetString("IncManagerId"),"PrincipalId",DbType.Int32,QFieldUsingType.Abstract,
			//				new QFieldJoinRelation(this.OwnerTable,"USERS","ManagerId","PrincipalId")));
			//
			//			this.Dictionary.Add(new QDictionary(this.Fields["IncManagerId"],this.Fields["IncManager"],"SELECT DISTINCT PrincipalId as Id, (FirstName +' ' + LastName) as Value FROM USERS WHERE PrincipalId IN (SELECT ManagerId FROM INCIDENTS)"));

			this.Fields.Add(new QField("IncCreationDate", LocRM.GetString("IncCreationDate"), "CreationDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			//			this.Fields.Add(new QField("IncActivationDate",LocRM.GetString("IncActivationDate"), "ActivationDate",DbType.DateTime,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort, false));
			//			this.Fields.Add(new QField("IncCloseDate",LocRM.GetString("IncCloseDate"), "CloseDate",DbType.DateTime,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("IncGeneralCategories", LocRM.GetString("IncGeneralCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","IncidentId","ObjectId", 
						new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Issue).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("IncGeneralCategoriesId", "IncGeneralCategoriesId", "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","IncidentId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Issue).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["IncGeneralCategoriesId"], this.Fields["IncGeneralCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM CATEGORIES"));

			this.Fields.Add(new QField("IncIncidentCategories", LocRM.GetString("IncIncidentCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"INCIDENT_CATEGORY","IncidentId","IncidentId"),
					new QFieldJoinRelation("INCIDENT_CATEGORY","INCIDENT_CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("IncIncidentCategoriesId", LocRM.GetString("IncIncidentCategories"), "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"INCIDENT_CATEGORY","IncidentId","IncidentId"),
					new QFieldJoinRelation("INCIDENT_CATEGORY","INCIDENT_CATEGORIES","CategoryId","CategoryId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["IncIncidentCategoriesId"], this.Fields["IncIncidentCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM INCIDENT_CATEGORIES"));

			// New fields 2006-07-07
			//			this.Fields.Add(new QField("Inc	",LocRM.GetString("IncDescription"),"Description",DbType.String,QFieldUsingType.Field|QFieldUsingType.Filter, false));
			//			this.Fields.Add(new QField("IncResolution",LocRM.GetString("IncResolution"),"Resolution",DbType.String,QFieldUsingType.Field|QFieldUsingType.Filter, false));
			//			this.Fields.Add(new QField("IncWorkaround",LocRM.GetString("IncWorkaround"),"Workaround",DbType.String,QFieldUsingType.Field|QFieldUsingType.Filter, false));

			//Commented 2007-10-17
			//// New Addod 2006-10-18
			//this.Fields.Add(new QField("IncTaskTime", LocRM.GetString("IncTaskTime"), "TaskTime", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2006-12-13 OZ: Client
			//this.Fields.Add(new QField("IncClient", LocRM.GetString("PrjClient"),
			//    "(ISNULL((SELECT O.OrgName FROM Organizations O WHERE O.OrgId = {0}.OrgId),(SELECT V.FullName FROM VCard V WHERE V.VCardId = {0}.VCardId)))",
			//    DbType.String,
			//    QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			//this.Fields.Add(new QField("IncClientId", "IncClientId",
			//    "(ISNULL({0}.OrgId,-{0}.VCardId))",
			//    DbType.Int32,
			//    QFieldUsingType.Abstract));

			//this.Dictionary.Add(new QDictionary(this.Fields["IncClientId"], this.Fields["IncClient"],
			//    "SELECT (O.OrgId) AS Id , O.OrgName as Value FROM Organizations O UNION SELECT (-V.VCardId) AS Id , V.FullName as Value FROM VCard V"));

			// 2006-12-13 OZ: IncidentBox
			this.Fields.Add(new QField("IncBox", LocRM.GetString("IncBox"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "IncidentBox", "IncidentBoxId", "IncidentBoxId")));

			this.Fields.Add(new QField("IncBoxId", "IncBoxId", "IncidentBoxId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "IncidentBox", "IncidentBoxId", "IncidentBoxId")));

			this.Dictionary.Add(new QDictionary(this.Fields["IncBoxId"], this.Fields["IncBox"], "SELECT IncidentBoxId as Id, Name as Value FROM IncidentBox"));

			// 2007-07-20 OZ: Incident Responsible
			this.Fields.Add(new QField("IncResponsible", LocRM.GetString("IncResponsible"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ResponsibleId", "PrincipalId")));

			this.Fields.Add(new QField("IncResponsibleId", LocRM.GetString("IncResponsibleId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ResponsibleId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["IncResponsibleId"], this.Fields["IncResponsible"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT ResponsibleId FROM INCIDENTS) ORDER BY [Value]"));

			// 2010-02-16 OR: Current Responsible
            this.Fields.Add(new QField("IncCurrentResponsible", LocRM.GetString("IncCurrentResponsible"),
                "( " + 
                "CASE  " +
					"WHEN {0}.CurrentResponsibleId = -2 THEN '{{IbnFramework.Incident:RespNotSet}}' " +
					"WHEN {0}.CurrentResponsibleId = -3 THEN '{{IbnFramework.Incident:RespGroup}}' " +
					"WHEN {0}.CurrentResponsibleId = -4 THEN '{{IbnFramework.Incident:RespAllDenied}}' " +
                    "ELSE (SELECT UR.FirstName + ' ' + UR.LastName FROM USERS AS UR WHERE UR.PrincipalId = {0}.CurrentResponsibleId) " + 
                "END "  +
                ") ", 
                DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

            //this.Fields.Add(new QField("IncCurrentResponsible", LocRM.GetString("IncCurrentResponsible"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
            //    new QFieldJoinRelation(this.OwnerTable, "USERS", "CurrentResponsibleId", "PrincipalId")));

			this.Fields.Add(new QField("IncCurrentResponsibleId", LocRM.GetString("IncCurrentResponsibleId"), "CurrentResponsibleId", DbType.Int32, QFieldUsingType.Abstract, false));

			this.Dictionary.Add(new QDictionary(this.Fields["IncCurrentResponsibleId"], this.Fields["IncCurrentResponsible"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CurrentResponsibleId FROM INCIDENTS) UNION ALL SELECT -2 as Id, '{{IbnFramework.Incident:RespNotSet}}' as Value UNION ALL SELECT -3 as Id, '{{IbnFramework.Incident:RespGroup}}' as Value UNION ALL SELECT -4 as Id, '{{IbnFramework.Incident:RespAllDenied}}' as Value ORDER BY [Value]"));

			// 2007-10-15: New Fields
			this.Fields.Add(new QField("IncModifiedDate", LocRM.GetString("IncModifiedDate"), "ModifiedDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("IncExpectedResponseDate", LocRM.GetString("IncExpectedResponseDate"), "ExpectedResponseDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("IncExpectedResolveDate", LocRM.GetString("IncExpectedResolveDate"), "ExpectedResolveDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("IncActualOpenDate", LocRM.GetString("IncActualOpenDate"), "ActualOpenDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2007-10-17 New Field is assigned with TimeTracking
			this.Fields.Add(new QField("IncTaskTime", LocRM.GetString("TaskTime"), "TaskTime", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("IncTotalMinutes", LocRM.GetString("TotalMinutes"), "TotalMinutes", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("IncTotalApproved", LocRM.GetString("TotalApproved"), "TotalApproved", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2008-09-09: New Fields
			this.Fields.Add(new QField("IncActualFinishDate", LocRM.GetString("IncActualFinishDate"), "ActualFinishDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			///////////////////////////
			// OZ 2009-05-18 IncOrg = OrgUid || (ClientUID -> OrganizationId)
			// Organization OrgUid
			this.Fields.Add(new QField("IncOrg", LocRM.GetString("PrjOrganization"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));
			this.Fields.Add(new QField("IncOrgUid", "IncOrgUid", "OrganizationId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));

			this.Dictionary.Add(new QDictionary(this.Fields["IncOrgUid"], this.Fields["IncOrg"],
				"SELECT DISTINCT OrganizationId as Id, Name as Value FROM cls_Organization ORDER BY Value"));

			// Contact ContactUid
			this.Fields.Add(new QField("IncContact", LocRM.GetString("PrjContact"), "FullName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));
			this.Fields.Add(new QField("IncContactUid", "IncContactUid", "ContactId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));

			this.Dictionary.Add(new QDictionary(this.Fields["IncContactUid"], this.Fields["IncContact"],
				"SELECT DISTINCT ContactId as Id, FullName as Value FROM cls_Contact ORDER BY Value"));

			// Client
			this.Fields.Add(new QField("IncClient", LocRM.GetString("PrjClient"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter ,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "IncidentClients", "IncidentId", "IncidentId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Fields.Add(new QField("IncClientUid", "ClientId", "ClientId", DbType.Guid, QFieldUsingType.Abstract,
							new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "IncidentClients", "IncidentId", "IncidentId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["IncClientUid"], this.Fields["IncClient"],
				"SELECT DISTINCT ClientId as Id, Name as Value FROM Clients ORDER BY Value"));

			///////////////////////////


			QMetaLoader.LoadMetaField(this, "IncidentsEx");
		}
	}
}
