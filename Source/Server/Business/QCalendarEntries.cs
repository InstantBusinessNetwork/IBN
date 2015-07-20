using System;
using System.Data;
using System.Resources;

using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QCalendarEntries.
	/// </summary>
	public class QCalendarEntries : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		public QCalendarEntries()
		{
		}

		public override QField[] GetFieldExtensions(QField Field)
		{
			if (Field == this.Fields["EventParticipants"])
				return new QField[]{new QField("EventParticipants","Participants", "{0}.LastName + ' ' + {0}.FirstName", DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter,
									   new QFieldJoinRelation(this.OwnerTable,"USERS","ManagerId","PrincipalId"))};
			return null;
		}

		protected override void LoadScheme()
		{
			OwnerTable = "EVENTS";

			//id
			this.Fields.Add(new QField("EventId", LocRM.GetString("EventId"), "EventId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));
			//			Title 1
			this.Fields.Add(new QField("EventTitle", LocRM.GetString("EventTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			//			Participants 0..N

			//			this.Fields.Add(new QField("EventParticipants","Participants","{0}.FirstName + ' ' + {0}.LastName",DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter,
			//				new QFieldJoinRelation[]
			//				{
			//					new QFieldJoinRelation(this.OwnerTable,"EVENT_RESOURCES","EventId","EventId"),
			//					new QFieldJoinRelation("EVENT_RESOURCES","USERS","PrincipalId","PrincipalId")
			//				}));

			this.Fields.Add(new QField("EventParticipants", LocRM.GetString("EventParticipants"),

				"CASE {0}.IsGroup " +
				"WHEN 0 THEN (SELECT UR.LastName + ' ' + UR.FirstName FROM USERS UR WHERE UR.PrincipalId = {0}.PrincipalId) " +
				"WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = {0}.PrincipalId) END",

				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"EVENT_RESOURCES","EventId","EventId"),
					new QFieldJoinRelation("EVENT_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			this.Fields.Add(new QField("EventParticipantsId", "EventParticipantsId",
				"PrincipalId",
				DbType.String, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"EVENT_RESOURCES","EventId","EventId"),
					new QFieldJoinRelation("EVENT_RESOURCES","PRINCIPALS","PrincipalId","PrincipalId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["EventParticipantsId"], this.Fields["EventParticipants"], "SELECT DISTINCT TR.PrincipalId as Id, (CASE P.IsGroup WHEN 0 THEN (SELECT UR.LastName + ' ' + UR.FirstName FROM USERS UR WHERE UR.PrincipalId = TR.PrincipalId) WHEN 1 THEN (SELECT GR.GroupName FROM GROUPS GR WHERE GR.PrincipalId = TR.PrincipalId) END) as Value FROM EVENT_RESOURCES TR INNER JOIN PRINCIPALS P ON P.PrincipalId = TR.PrincipalId"));

			//			Organizer 1
			this.Fields.Add(new QField("EventOrganizer", LocRM.GetString("EventOrganizer"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));
			this.Fields.Add(new QField("EventOrganizerId", LocRM.GetString("EventOrganizerId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["EventOrganizerId"], this.Fields["EventOrganizer"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT ManagerId FROM EVENTS) ORDER BY [Value]"));

			//			Creator 1
			this.Fields.Add(new QField("EventCreator", LocRM.GetString("EventCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Fields.Add(new QField("EventCreatorId", LocRM.GetString("EventCreatorId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["EventCreatorId"], this.Fields["EventCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM EVENTS) ORDER BY [Value]"));

			//			Type (event, meeting, appointment)
			this.Fields.Add(new QField("EventType", LocRM.GetString("EventType"), "TypeName", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "EVENT_TYPE_LANGUAGE", "TypeId", "TypeId", "LanguageId")));

			this.Fields.Add(new QField("EventTypeId", "EventTypeId", "TypeId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "EVENT_TYPE_LANGUAGE", "TypeId", "TypeId", "LanguageId")));

			this.Dictionary.Add(new QDictionary(this.Fields["EventTypeId"], this.Fields["EventType"], "SELECT TypeId as Id, TypeName as Value FROM EVENT_TYPE_LANGUAGE WHERE LanguageId = {0}"));

			//			Project 0..1
			if (Mediachase.Ibn.License.ProjectManagement)
			{
				this.Fields.Add(new QField("EventProject", LocRM.GetString("EventProject"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));

				this.Fields.Add(new QField("EventProjectId", LocRM.GetString("EventProjectId"), "ProjectId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));

				this.Dictionary.Add(new QDictionary(this.Fields["EventProjectId"], this.Fields["EventProject"], "SELECT ProjectId as Id, Title as [Value] FROM PROJECTS ORDER BY [Value]"));

				// 2009-09-30
				this.Fields.Add(new QField("PrjCode", LocRM.GetString("PrjCode"), "ProjectCode", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter | QFieldUsingType.Sort, 
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
			}

			//			Start date
			this.Fields.Add(new QField("EventStartDate", LocRM.GetString("EventStartDate"), "StartDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			//			Finish date
			this.Fields.Add(new QField("EventFinishDate", LocRM.GetString("EventFinishDate"), "FinishDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			//			General category 0..n
			this.Fields.Add(new QField("EventGeneralCategories", LocRM.GetString("EventGeneralCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","EventId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.CalendarEntry).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("EventGeneralCategoriesId", "EventGeneralCategoriesId", "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","EventId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.CalendarEntry).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));


			this.Dictionary.Add(new QDictionary(this.Fields["EventGeneralCategoriesId"], this.Fields["EventGeneralCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM CATEGORIES"));

			// 2006-12-13 OZ: Client
			//this.Fields.Add(new QField("EventClient",LocRM.GetString("PrjClient"), 
			//    "(ISNULL((SELECT O.OrgName FROM Organizations O WHERE O.OrgId = {0}.OrgId),(SELECT V.FullName FROM VCard V WHERE V.VCardId = {0}.VCardId)))", 
			//    DbType.String,
			//    QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort));

			//this.Fields.Add(new QField("EventClientId","EventClientId", 
			//    "(ISNULL({0}.OrgId,-{0}.VCardId))", 
			//    DbType.Int32,
			//    QFieldUsingType.Abstract));

			//this.Dictionary.Add(new QDictionary(this.Fields["EventClientId"],this.Fields["EventClient"],
			//    "SELECT (O.OrgId) AS Id , O.OrgName as Value FROM Organizations O UNION SELECT (-V.VCardId) AS Id , V.FullName as Value FROM VCard V"));

			// 2007-10-17 New Field is assigned with TimeTracking
			this.Fields.Add(new QField("EventTaskTime", LocRM.GetString("TaskTime"), "TaskTime", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("EventTotalMinutes", LocRM.GetString("TotalMinutes"), "TotalMinutes", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("EventTotalApproved", LocRM.GetString("TotalApproved"), "TotalApproved", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			///////////////////////////
			// 2008-10-20 New Clients
			// Organization OrgUid
			this.Fields.Add(new QField("EventOrg", LocRM.GetString("PrjOrganization"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));
			this.Fields.Add(new QField("EventOrgUid", "EventOrgUid", "OrganizationId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));

			this.Dictionary.Add(new QDictionary(this.Fields["EventOrgUid"], this.Fields["EventOrg"],
				"SELECT DISTINCT OrganizationId as Id, Name as Value FROM cls_Organization ORDER BY Value"));

			// Contact ContactUid
			this.Fields.Add(new QField("EventContact", LocRM.GetString("PrjContact"), "FullName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));
			this.Fields.Add(new QField("EventContactUid", "EventContactUid", "ContactId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));

			this.Dictionary.Add(new QDictionary(this.Fields["EventContactUid"], this.Fields["EventContact"],
				"SELECT DISTINCT ContactId as Id, FullName as Value FROM cls_Contact ORDER BY Value"));

			// Client
			this.Fields.Add(new QField("EventClient", LocRM.GetString("PrjClient"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "EventClients", "EventId", "EventId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Fields.Add(new QField("EventClientUid", "ClientId", "ClientId", DbType.Guid, QFieldUsingType.Abstract,
							new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "EventClients", "EventId", "EventId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["EventClientUid"], this.Fields["EventClient"],
				"SELECT DISTINCT ClientId as Id, Name as Value FROM Clients ORDER BY Value"));
			///////////////////////////

			// 2009-04-28 New CreationDate Field is assigned with TimeTracking
			this.Fields.Add(new QField("EventCreationDate", LocRM.GetString("ToDoCreationDate"), "CreationDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// 2009-06-22 New Field Location
			this.Fields.Add(new QField("EventLocation", LocRM.GetString("EventLocation"), "Location", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("EventDescription", LocRM.GetString("EventDescription"), "Description", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			QMetaLoader.LoadMetaField(this, "EventsEx");
		}
	}
}
