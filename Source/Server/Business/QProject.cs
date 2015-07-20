using System;
using System.Collections;
using System.Data;
using System.Resources;

using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QProject.
	/// </summary>
	public class QProject : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		protected ArrayList _Extensions = new ArrayList();
		protected int _ExtensionsId = 0;

		public QProject()
		{
			LoadScheme2();
		}

		private QProject(int ExtensionsId)
		{
			_ExtensionsId = ExtensionsId;

			LoadScheme2();
		}

		public override QObject[] GetExtensions()
		{
			// TODO:  Add QProject.GetExtensions implementation
			return (QObject[])_Extensions.ToArray(typeof(QObject));
		}

		protected override void LoadScheme()
		{
			// Fix: couldn't set _ExtensionsId = ExtensionsId before execute LoadScheme method.
		}

		protected void LoadScheme2()
		{
			OwnerTable = "PROJECTS";

			this.Fields.Add(new QField("PrjId", LocRM.GetString("PrjId"), "ProjectId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));
			this.Fields.Add(new QField("PrjTitle", LocRM.GetString("PrjTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("PrjCreator", LocRM.GetString("PrjCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Fields.Add(new QField("PrjCreatorId", LocRM.GetString("PrjCreatorId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjCreatorId"], this.Fields["PrjCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM PROJECTS) ORDER BY [Value]"));

			this.Fields.Add(new QField("PrjManager", LocRM.GetString("PrjManager"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));
			this.Fields.Add(new QField("PrjManagerId", LocRM.GetString("PrjManagerId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjManagerId"], this.Fields["PrjManager"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT ManagerId FROM PROJECTS) ORDER BY [Value]"));

			this.Fields.Add(new QField("PrjExecutiveManager", LocRM.GetString("PrjExecutiveManager"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ExecutiveManagerId", "PrincipalId")));
			this.Fields.Add(new QField("PrjExecutiveManagerId", LocRM.GetString("PrjExecutiveManagerId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "ManagerId", "PrincipalId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjExecutiveManagerId"], this.Fields["PrjExecutiveManager"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT ExecutiveManagerId FROM PROJECTS) ORDER BY [Value]"));

			this.Fields.Add(new QField("PrjCreationDate", LocRM.GetString("PrjCreationDate"), "CreationDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("PrjStartDate", LocRM.GetString("PrjStartDate"), "StartDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("PrjFinishDate", LocRM.GetString("PrjFinishDate"), "FinishDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("PrjTargetFinishDate", LocRM.GetString("PrjTargetFinishDate"), "TargetFinishDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("PrjActualFinishDate", LocRM.GetString("PrjActualFinishDate"), "ActualFinishDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			this.Fields.Add(new QField("PrjStatus", LocRM.GetString("PrjStatus"), "StatusName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PROJECT_STATUS_LANGUAGE", "StatusId", "StatusId", "LanguageId")));

			this.Fields.Add(new QField("PrjStatusId", "PrjStatusId", "StatusId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "PROJECT_STATUS_LANGUAGE", "StatusId", "StatusId", "LanguageId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjStatusId"], this.Fields["PrjStatus"], "SELECT StatusId as Id, StatusName as Value FROM PROJECT_STATUS_LANGUAGE WHERE LanguageId = {0}"));

			//			this.Fields.Add(new QField("PrjClient",LocRM.GetString("PrjClient"),"ClientName", DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort,
			//				new QFieldJoinRelation(this.OwnerTable,"CLIENTS","ClientId","ClientId")));
			//
			//			this.Fields.Add(new QField("PrjClientId","PrjClientId","ClientId", DbType.Int32,QFieldUsingType.Abstract,
			//				new QFieldJoinRelation(this.OwnerTable,"CLIENTS","ClientId","ClientId")));
			//
			//			this.Dictionary.Add(new QDictionary(this.Fields["PrjClientId"],this.Fields["PrjClient"],"SELECT ClientId as Id, ClientName as Value FROM CLIENTS"));

			this.Fields.Add(new QField("PrjGeneralCategories", LocRM.GetString("PrjGeneralCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","ProjectId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Project).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("PrjGeneralCategoriesId", "PrjGeneralCategoriesId", "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"OBJECT_CATEGORY","ProjectId","ObjectId", 
					new SimpleFilterCondition(new QField("ObjectTypeId"),((int)ObjectTypes.Project).ToString(),SimpleFilterType.Equal)),
					new QFieldJoinRelation("OBJECT_CATEGORY","CATEGORIES","CategoryId","CategoryId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjGeneralCategoriesId"], this.Fields["PrjGeneralCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM CATEGORIES"));

			this.Fields.Add(new QField("PrjProjectCategories", LocRM.GetString("PrjProjectCategories"), "CategoryName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECT_CATEGORY","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECT_CATEGORY","PROJECT_CATEGORIES","CategoryId","CategoryId")
				}));

			this.Fields.Add(new QField("PrjProjectCategoriesId", "PrjProjectCategoriesId", "CategoryId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECT_CATEGORY","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECT_CATEGORY","PROJECT_CATEGORIES","CategoryId","CategoryId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjProjectCategoriesId"], this.Fields["PrjProjectCategories"], "SELECT CategoryId As Id, CategoryName As Value FROM PROJECT_CATEGORIES"));

			this.Fields.Add(new QField("PrjResources", LocRM.GetString("PrjResources"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECT_MEMBERS","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECT_MEMBERS","USERS","PrincipalId","PrincipalId")
				}));

			this.Fields.Add(new QField("PrjResourcesId", "PrjResourcesId", "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECT_MEMBERS","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECT_MEMBERS","USERS","PrincipalId","PrincipalId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjResourcesId"], this.Fields["PrjResources"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT PrincipalId FROM PROJECT_MEMBERS) ORDER BY [Value]"));

			//			this.Fields.Add(new QField("PrjTargetBudget",LocRM.GetString("PrjTargetBudget"), "TargetBudget",DbType.Decimal,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter, false));
			//			this.Fields.Add(new QField("PrjEstimatedBudget",LocRM.GetString("PrjEstimatedBudget"), "EstimatedBudget",DbType.Decimal,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter, false));
			//			this.Fields.Add(new QField("PrjActualBudget",LocRM.GetString("PrjActualBudget"), "ActualBudget",DbType.Decimal,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter, false));

			// Project Types [12/17/2004]
			this.Fields.Add(new QField("PrjType", LocRM.GetString("PrjType"), "FormName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "OBJECT_FORMS", "FormId", "FormId", new SimpleFilterCondition(new QField("ObjectTypeId"), ((int)ObjectTypes.Project).ToString(), SimpleFilterType.Equal))));

			this.Fields.Add(new QField("PrjTypeId", "PrjTypeId", "FormId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "OBJECT_FORMS", "FormId", "FormId", new SimpleFilterCondition(new QField("ObjectTypeId"), ((int)ObjectTypes.Project).ToString(), SimpleFilterType.Equal))));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjTypeId"], this.Fields["PrjType"], "SELECT FormId as Id, FormName as Value FROM OBJECT_FORMS WHERE ObjectTypeId = 3"));

			// PercentCompleted [1/13/2005]
			this.Fields.Add(new QField("PrjPercentCompleted", LocRM.GetString("PrjPercentCompleted"), "PercentCompleted", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			// PriorityId [1/13/2005]
			this.Fields.Add(new QField("PrjPriority", LocRM.GetString("PrjPriority"), "PriorityName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Fields.Add(new QField("PrjPriorityId", "PrjPriorityId", "PriorityId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "PRIORITY_LANGUAGE", "PriorityId", "PriorityId", "LanguageId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjPriorityId"], this.Fields["PrjPriority"], "SELECT PriorityId as Id, PriorityName as Value FROM PRIORITY_LANGUAGE WHERE LanguageId = {0}"));

			// Phase [3/35/2005]
			this.Fields.Add(new QField("PrjPhase", LocRM.GetString("PrjPhase"), "PhaseName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "PROJECT_PHASES", "PhaseId", "PhaseId")));

			this.Fields.Add(new QField("PrjPhaseId", "PrjPhaseId", "PhaseId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "PROJECT_PHASES", "PhaseId", "PhaseId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjPhaseId"], this.Fields["PrjPhase"], "SELECT PhaseId as Id, PhaseName as Value FROM PROJECT_PHASES"));

			// Portfolio [3/35/2005]
			this.Fields.Add(new QField("PrjPortfolio", LocRM.GetString("PrjPortfolio"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECT_GROUP","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECT_GROUP","PROJECT_GROUPS","ProjectGroupId","ProjectGroupId")
				}));

			this.Fields.Add(new QField("PrjPortfolioId", "PrjPortfolioId", "ProjectGroupId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"PROJECT_GROUP","ProjectId","ProjectId"),
					new QFieldJoinRelation("PROJECT_GROUP","PROJECT_GROUPS","ProjectGroupId","ProjectGroupId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjPortfolioId"], this.Fields["PrjPortfolio"], "SELECT ProjectGroupId as Id, Title as Value FROM PROJECT_GROUPS"));

			// OZ: 2006-12-12 New Finance by business Score (Current plan)
			foreach (BusinessScore bs in BusinessScore.List())
			{
				this.Fields.Add(new QField(string.Format("PrjBs{0}", bs.Key),
					string.Format("{0} ({1})", bs.Name, LocRM.GetString("PrjCurrentPlan")),
					"(SELECT CAST(SUM(Value) AS MONEY) FROM BusinessScoreData WHERE [Index] = 0 AND BusinessScoreId = " + bs.BusinessScoreId + " AND ProjectId = {0}.ProjectId)",
					DbType.Decimal,
					QFieldUsingType.Field |/*QFieldUsingType.Grouping|*/QFieldUsingType.Filter | QFieldUsingType.Sort,
					new QFieldJoinRelation(this.OwnerTable, "BusinessScoreData", "ProjectId", "ProjectId")));
			}

			// TODO: OZ: 2007-07-12 New Finance by business Score (Fact)
			//foreach (BusinessScore bs in BusinessScore.List())
			//{
			//    this.Fields.Add(new QField(string.Format("PrjBs{0}", bs.Key),
			//        string.Format("{0} ({1})", bs.Name, LocRM.GetString("PrjFact")),
			//        "(SELECT SUM(Value) FROM BusinessScoreData WHERE [Index] = 0 AND BusinessScoreId = " + bs.BusinessScoreId + " AND ProjectId = {0}.ProjectId)",
			//        DbType.Decimal,
			//        QFieldUsingType.Field |/*QFieldUsingType.Grouping|*/QFieldUsingType.Filter | QFieldUsingType.Sort,
			//        new QFieldJoinRelation(this.OwnerTable, "BusinessScoreData", "ProjectId", "ProjectId")));
			//}


			// 2006-12-13 OZ: Client
			//this.Fields.Add(new QField("PrjClient", LocRM.GetString("PrjClient"),
			//    "(ISNULL((SELECT O.OrgName FROM Organizations O WHERE O.OrgId = {0}.OrgId),(SELECT V.FullName FROM VCard V WHERE V.VCardId = {0}.VCardId)))",
			//    DbType.String,
			//    QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			//this.Fields.Add(new QField("PrjClientId", "PrjClientId",
			//    "(ISNULL({0}.OrgId,-{0}.VCardId))",
			//    DbType.Int32,
			//    QFieldUsingType.Abstract));

			//this.Dictionary.Add(new QDictionary(this.Fields["PrjClientId"], this.Fields["PrjClient"],
			//    "SELECT (O.OrgId) AS Id , O.OrgName as Value FROM Organizations O UNION SELECT (-V.VCardId) AS Id , V.FullName as Value FROM VCard V"));

			// 2007-10-17 New Field is assigned with TimeTracking
			this.Fields.Add(new QField("PrjTaskTime", LocRM.GetString("TaskTime"), "TaskTime", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("PrjTotalMinutes", LocRM.GetString("TotalMinutes"), "TotalMinutes", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));
			this.Fields.Add(new QField("PrjTotalApproved", LocRM.GetString("TotalApproved"), "TotalApproved", DbType.Time, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			///////////////////////////
			// 2008-10-20 New Clients
			// Organization OrgUid
			this.Fields.Add(new QField("PrjOrg", LocRM.GetString("PrjOrganization"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));
			this.Fields.Add(new QField("PrjOrgUid", "PrjOrgUid", "OrganizationId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Organization", "OrgUid", "OrganizationId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjOrgUid"], this.Fields["PrjOrg"],
				"SELECT DISTINCT OrganizationId as Id, Name as Value FROM cls_Organization ORDER BY Value"));

			// Contact ContactUid
			this.Fields.Add(new QField("PrjContact", LocRM.GetString("PrjContact"), "FullName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));
			this.Fields.Add(new QField("PrjContactUid", "PrjContactUid", "ContactId", DbType.Guid, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "cls_Contact", "ContactUid", "ContactId")));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjContactUid"], this.Fields["PrjContact"],
				"SELECT DISTINCT ContactId as Id, FullName as Value FROM cls_Contact ORDER BY Value"));

			// Client
			this.Fields.Add(new QField("PrjClient", LocRM.GetString("PrjClient"), "Name", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
							new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "ProjectClients", "ProjectId", "ProjectId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Fields.Add(new QField("PrjClientUid", "ClientId", "ClientId", DbType.Guid, QFieldUsingType.Abstract,
							new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable, "ProjectClients", "ProjectId", "ProjectId"),
					new QFieldJoinRelation(this.OwnerTable, "Clients", "ClientId", "ClientId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["PrjClientUid"], this.Fields["PrjClient"],
				"SELECT DISTINCT ClientId as Id, Name as Value FROM Clients ORDER BY Value"));
			///////////////////////////

			// 2008-11-12
			this.Fields.Add(new QField("PrjGoals", LocRM.GetString("PrjGoals"), "Goals", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter, false));
			this.Fields.Add(new QField("PrjScope", LocRM.GetString("PrjScope"), "Scope", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter, false));
			this.Fields.Add(new QField("PrjDeliverables", LocRM.GetString("PrjDeliverables"), "Deliverables", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter, false));
			this.Fields.Add(new QField("PrjDescription", LocRM.GetString("PrjDescription"), "Description", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter, false));

			// 2009-08-24
			this.Fields.Add(new QField("PrjCode", LocRM.GetString("PrjCode"), "ProjectCode", DbType.String, QFieldUsingType.Field | QFieldUsingType.Filter | QFieldUsingType.Sort, false));

			//Description

			// Meta Fields
			MetaClass mcProjects = MetaClass.Load("Projects");

			if (mcProjects.ChildClasses.Count > 0)
			{
				QMetaLoader.LoadMetaField(this, mcProjects.ChildClasses[_ExtensionsId]);

				for (int ExIndex = 0; ExIndex < mcProjects.ChildClasses.Count; ExIndex++)
				{
					if (ExIndex != _ExtensionsId && mcProjects.ChildClasses[ExIndex].UserMetaFields.Count > 0)
						QMetaLoader.LoadMetaField(this, mcProjects.ChildClasses[ExIndex]);
				}

				if (_ExtensionsId == 0)
				{
					for (int ExIndex = 1; ExIndex < mcProjects.ChildClasses.Count; ExIndex++)
					{
						if (mcProjects.ChildClasses[ExIndex].UserMetaFields.Count > 0)
						{
							QProject newExtension = new QProject(ExIndex);
							this._Extensions.Add(newExtension);
						}
					}
				}
			}


		}

	}
}
