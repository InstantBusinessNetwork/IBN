using System;
using System.Collections.Generic;
using System.Data;
using System.Resources;
using System.Text;

using Mediachase.IBN.Business.Reports;
using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business
{
	public class QDocument : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QDocument).Assembly);

		public QDocument()
		{
		}

		protected override void LoadScheme()
		{
			OwnerTable = "DOCUMENTS";

			//Код документа
			this.Fields.Add(new QField("DocId", LocRM.GetString("DocId"), "DocumentId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));

			//Название
			this.Fields.Add(new QField("DocTitle", LocRM.GetString("DocTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			//Приоритет
			this.Fields.Add(new QField("DocPriority", LocRM.GetString("DocPriority"), "PriorityName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
	new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Fields.Add(new QField("DocPriorityId", LocRM.GetString("DocPriority"), "PriorityId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Dictionary.Add(new QDictionary(this.Fields["DocPriorityId"], this.Fields["DocPriority"], "SELECT PriorityId as Id, PriorityName as Value FROM PRIORITY_LANGUAGE WHERE LanguageId = {0}"));

			//Проект
			if (Mediachase.Ibn.License.ProjectManagement)
			{
				this.Fields.Add(new QField("DocProject", LocRM.GetString("DocProject"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
				this.Fields.Add(new QField("DocProjectId", LocRM.GetString("DocProjectId"), "ProjectId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));

				this.Dictionary.Add(new QDictionary(this.Fields["DocProjectId"], this.Fields["DocProject"], "SELECT ProjectId as Id, Title as [Value] FROM PROJECTS ORDER BY [Value]"));

				// 2009-09-30
				this.Fields.Add(new QField("PrjCode", LocRM.GetString("PrjCode"), "ProjectCode", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "PROJECTS", "ProjectId", "ProjectId")));
			}

			// Создал
			this.Fields.Add(new QField("DocCreator", LocRM.GetString("DocCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Fields.Add(new QField("DocCreatorId", "DocCreatorId", "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["DocCreatorId"], this.Fields["DocCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM DOCUMENTS) ORDER BY [Value]"));

			// Клиент
			//this.Fields.Add(new QField("DocClient", LocRM.GetString("DocClient"),
			//     "(ISNULL((SELECT O.OrgName FROM Organizations O WHERE O.OrgId = {0}.OrgId),(SELECT V.FullName FROM VCard V WHERE V.VCardId = {0}.VCardId)))",
			//     DbType.String,
			//     QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			//this.Fields.Add(new QField("DocClientId", "DocClientId",
			//    "(ISNULL({0}.OrgId,-{0}.VCardId))",
			//    DbType.Int32,
			//    QFieldUsingType.Abstract));

			//this.Dictionary.Add(new QDictionary(this.Fields["DocClientId"], this.Fields["DocClient"],
			//    "SELECT (O.OrgId) AS Id , O.OrgName as Value FROM Organizations O UNION SELECT (-V.VCardId) AS Id , V.FullName as Value FROM VCard V"));

			// Общие категории
			this.Fields.Add(new QField("DocGeneralCategories", LocRM.GetString("DocGeneralCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
	 new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","DocumentId","ObjectId", 
						new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Document).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("DocGeneralCategoriesId", "DocGeneralCategoriesId", "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","DocumentId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Document).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["DocGeneralCategoriesId"], this.Fields["DocGeneralCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM CATEGORIES"));

			// Дата создания
			this.Fields.Add(new QField("DocCreationDate", LocRM.GetString("DocCreationDate"), "CreationDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			// Менеджер
			this.Fields.Add(new QField("DocManager", LocRM.GetString("DocManager"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
	new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));
			this.Fields.Add(new QField("DocManagerId", "DocManagerId", "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["DocManagerId"], this.Fields["DocManager"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT ManagerId FROM DOCUMENTS) ORDER BY [Value]"));

			// Статус
			this.Fields.Add(new QField("DocStatus", LocRM.GetString("DocStatus"), "StatusName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "DOCUMENT_STATUS", "StatusId", "StatusId")));

			this.Fields.Add(new QField("DocStatusId", LocRM.GetString("DocStatusId"), "StatusId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "DOCUMENT_STATUS", "StatusId", "StatusId")));

			this.Dictionary.Add(new QDictionary(this.Fields["DocStatusId"], this.Fields["DocStatus"], "SELECT StatusId as Id, StatusName as Value FROM DOCUMENT_STATUS"));

			// 2007-10-17 New Field is assigned with TimeTracking
			this.Fields.Add(new QField("DocTaskTime", LocRM.GetString("TaskTime"), "TaskTime", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("DocTotalMinutes", LocRM.GetString("TotalMinutes"), "TotalMinutes", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("DocTotalApproved", LocRM.GetString("TotalApproved"), "TotalApproved", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			///////////////////////////
			// 2008-10-20 New Clients
			// Organization OrgUid
			this.Fields.Add(new QField("DocOrg", LocRM.GetString("PrjOrganization"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));
			this.Fields.Add(new QField("DocOrgUid", "DocOrgUid", "OrganizationId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));

			this.Dictionary.Add(new QDictionary(this.Fields["DocOrgUid"], this.Fields["DocOrg"],
				"SELECT DISTINCT OrganizationId as Id, Name as Value FROM cls_Organization ORDER BY Value"));

			// Contact ContactUid
			this.Fields.Add(new QField("DocContact", LocRM.GetString("PrjContact"), "FullName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));
			this.Fields.Add(new QField("DocContactUid", "DocContactUid", "ContactId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));

			this.Dictionary.Add(new QDictionary(this.Fields["DocContactUid"], this.Fields["DocContact"],
				"SELECT DISTINCT ContactId as Id, FullName as Value FROM cls_Contact ORDER BY Value"));

			// Client
			this.Fields.Add(new QField("DocClient", LocRM.GetString("PrjClient"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "DocumentClients", "DocumentId", "DocumentId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Fields.Add(new QField("DocClientUid", "ClientId", "ClientId", DbType.Guid, QFieldUsingType.Abstract,
							new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "DocumentClients", "DocumentId", "DocumentId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["DocClientUid"], this.Fields["DocClient"],
				"SELECT DISTINCT ClientId as Id, Name as Value FROM Clients ORDER BY Value"));
			///////////////////////////


			// Мета поля
			QMetaLoader.LoadMetaField(this, "DocumentsEx");
		}
	}
}
