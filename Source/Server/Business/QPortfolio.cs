using System;
using System.Data;
using System.Resources;

using Mediachase.SQLQueryCreator;


namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QPortfolio.
	/// </summary>
	public class QPortfolio : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		protected override void LoadScheme()
		{
			OwnerTable = "PROJECT_GROUPS";

			this.Fields.Add(new QField("PortfolioId", LocRM.GetString("PortfolioId"), "ProjectGroupId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));
			this.Fields.Add(new QField("PortfolioTitle", LocRM.GetString("PortfolioTitle"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));


			this.Fields.Add(new QField("PortfolioCreator", LocRM.GetString("PortfolioCreator"), "{0}.LastName + ' ' + {0}.FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Fields.Add(new QField("PortfolioCreatorId", LocRM.GetString("PortfolioCreatorId"), "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation(this.OwnerTable, "USERS", "CreatorId", "PrincipalId")));
			this.Dictionary.Add(new QDictionary(this.Fields["PortfolioCreatorId"], this.Fields["PortfolioCreator"], "SELECT DISTINCT PrincipalId as Id, (LastName + ' ' + FirstName) as Value FROM USERS WHERE PrincipalId IN (SELECT CreatorId FROM PROJECT_GROUPS) ORDER BY [Value]"));

			/*			this.Fields.Add(new QField("PortfolioLastEditor",LocRM.GetString("PortfolioLastEditor"), "{0}.FirstName + ' ' + {0}.LastName", DbType.String,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort,
							new QFieldJoinRelation(this.OwnerTable,"USERS","LastEditorId","PrincipalId")));
						this.Fields.Add(new QField("PortfolioLastEditorId",LocRM.GetString("PortfolioLastEditorId"), "LastEditorId", DbType.Int32, QFieldUsingType.Abstract,
							new QFieldJoinRelation(this.OwnerTable,"USERS","LastEditorId","PrincipalId")));
						this.Dictionary.Add(new QDictionary(this.Fields["PortfolioCreatorId"],this.Fields["PortfolioCreator"],"SELECT DISTINCT PrincipalId as Id, (FirstName +' ' + LastName) as Value FROM USERS WHERE PrincipalId IN (SELECT LastEditorId FROM PROJECT_GROUPS)"));
			*/
			this.Fields.Add(new QField("PortfolioCreationDate", LocRM.GetString("PortfolioCreationDate"), "CreationDate", DbType.DateTime, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));
			//			this.Fields.Add(new QField("PortfolioLastSavedDate",LocRM.GetString("PortfolioLastSavedDate"), "LastSavedDate",DbType.DateTime,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort));

			// Projects
			this.Fields.Add(new QField("PortfolioProject", LocRM.GetString("PortfolioProject"), "Title", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation[]{
											new QFieldJoinRelation(this.OwnerTable,"PROJECT_GROUP","ProjectGroupId","ProjectGroupId"),
											new QFieldJoinRelation(this.OwnerTable,"PROJECTS","ProjectId","ProjectId")
										}
					));
			this.Fields.Add(new QField("PortfolioProjectId", LocRM.GetString("PortfolioProjectId"), "ProjectId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Sort,
				new QFieldJoinRelation[]{
											new QFieldJoinRelation(this.OwnerTable,"PROJECT_GROUP","ProjectGroupId","ProjectGroupId"),
											new QFieldJoinRelation(this.OwnerTable,"PROJECTS","ProjectId","ProjectId")
										}
				));
			this.Dictionary.Add(new QDictionary(this.Fields["PortfolioProjectId"], this.Fields["PortfolioProject"], "SELECT ProjectId as Id, Title as [Value] FROM PROJECTS ORDER BY [Value]"));

			// Project Status
			//			this.Fields.Add(new QField("PortfolioProjectStatus",LocRM.GetString("PortfolioProjectStatus"),"StatusName",DbType.String ,QFieldUsingType.Field|QFieldUsingType.Grouping|QFieldUsingType.Filter|QFieldUsingType.Sort,
			//				new QFieldJoinRelation[]{
			//											new QFieldJoinRelation(this.OwnerTable,"PROJECT_GROUP","ProjectGroupId","ProjectGroupId"),
			//											new QFieldJoinRelation(this.OwnerTable,"PROJECTS","ProjectId","ProjectId"),
			//											new QFieldJoinRelation(this.OwnerTable,"PROJECT_STATUS_LANGUAGE","StatusId","StatusId","LanguageId")
			//										}
			//				));
			//
			//			this.Fields.Add(new QField("PortfolioProjectStatusId","PortfolioProjectStatusId","StatusId",DbType.Int32 ,QFieldUsingType.Abstract,
			//				new QFieldJoinRelation[]{
			//											new QFieldJoinRelation(this.OwnerTable,"PROJECT_GROUP","ProjectGroupId","ProjectGroupId"),
			//											new QFieldJoinRelation(this.OwnerTable,"PROJECTS","ProjectId","ProjectId"),
			//											new QFieldJoinRelation(this.OwnerTable,"PROJECT_STATUS_LANGUAGE","StatusId","StatusId","LanguageId")
			//										}
			//				));
			//			this.Dictionary.Add(new QDictionary(this.Fields["PortfolioProjectStatusId"],this.Fields["PortfolioProjectStatus"],"SELECT StatusId as Id, StatusName as Value FROM PROJECT_STATUS_LANGUAGE WHERE LanguageId = {0}"));

			// Total Project Count
			this.Fields.Add(new QField("PortfolioTotalProjectCount", LocRM.GetString("PortfolioTotalProjectCount"), "(SELECT COUNT(*) FROM PROJECT_GROUP WHERE  ProjectGroupId = {0}.ProjectGroupId)", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			// Active Project Count
			this.Fields.Add(new QField("PortfolioActiveProjectCount", LocRM.GetString("PortfolioActiveProjectCount"), "(SELECT COUNT(*) FROM PROJECT_GROUP PG INNER JOIN PROJECTS P ON P.ProjectId = PG.ProjectId INNER JOIN PROJECT_STATUS PS ON P.StatusId = PS.StatusId WHERE PS.IsActive = 1 AND ProjectGroupId = {0}.ProjectGroupId)", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			// PortfolioFinanceTarget
			this.Fields.Add(new QField("PortfolioFinanceTarget", LocRM.GetString("PortfolioFinanceTarget"), "(SELECT SUM(A.TCur+A.TSub) FROM PROJECT_GROUP PG INNER JOIN ACCOUNTS A ON A.ProjectId = PG.ProjectId WHERE A.OutlineLevel = 1 AND ProjectGroupId = {0}.ProjectGroupId)", DbType.Decimal, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			// PortfolioFinanceEstimate
			this.Fields.Add(new QField("PortfolioFinanceEstimate", LocRM.GetString("PortfolioFinanceEstimate"), "(SELECT SUM(A.ECur+A.ESub) FROM PROJECT_GROUP PG INNER JOIN ACCOUNTS A ON A.ProjectId = PG.ProjectId WHERE A.OutlineLevel = 1 AND ProjectGroupId = {0}.ProjectGroupId)", DbType.Decimal, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			// PortfolioFinanceActual
			this.Fields.Add(new QField("PortfolioFinanceActual", LocRM.GetString("PortfolioFinanceActual"), "(SELECT SUM(A.ACur+A.ASub) FROM PROJECT_GROUP PG INNER JOIN ACCOUNTS A ON A.ProjectId = PG.ProjectId WHERE A.OutlineLevel = 1 AND ProjectGroupId = {0}.ProjectGroupId)", DbType.Decimal, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			//Meta Fields
			QMetaLoader.LoadMetaField(this, "PortfolioEx");
		}

	}
}
