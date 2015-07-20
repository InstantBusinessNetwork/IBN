using System;
using System.Data;
using System.Resources;

using Mediachase.SQLQueryCreator;

namespace Mediachase.IBN.Business.Reports
{
	/// <summary>
	/// Summary description for QDirectory.
	/// </summary>
	public class QDirectory : QObject
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.IBN.Business.Resources.QObjectsResource", typeof(QCalendarEntries).Assembly);

		public QDirectory()
		{
		}

		protected override void LoadScheme()
		{
			OwnerTable = "USERS";

			//key
			this.Fields.Add(new QField("UserId", LocRM.GetString("UserId"), "PrincipalId", DbType.Int32, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort, true));

			//First Name
			this.Fields.Add(new QField("UserFirstName", LocRM.GetString("UserFirstName"), "FirstName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			//Last Name
			this.Fields.Add(new QField("UserLastName", LocRM.GetString("UserLastName"), "LastName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			//E-Mail
			this.Fields.Add(new QField("UserEmail", LocRM.GetString("UserEmail"), "Email", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			//Phone
			this.Fields.Add(new QField("UserPhone", LocRM.GetString("UserPhone"), "Phone", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USER_DETAILS", "PrincipalId", "UserId"), true));

			//Fax
			this.Fields.Add(new QField("UserFax", LocRM.GetString("UserFax"), "Fax", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USER_DETAILS", "PrincipalId", "UserId"), true));

			//Mobile
			this.Fields.Add(new QField("UserMobile", LocRM.GetString("UserMobile"), "Mobile", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USER_DETAILS", "PrincipalId", "UserId"), true));

			//Job Title 
			this.Fields.Add(new QField("UserJobTitle", LocRM.GetString("UserJobTitle"), "Position", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USER_DETAILS", "PrincipalId", "UserId"), true));

			//Department
			this.Fields.Add(new QField("UserDepartment", LocRM.GetString("UserDepartment"), "Department", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USER_DETAILS", "PrincipalId", "UserId"), true));

			//Location
			this.Fields.Add(new QField("UserLocation", LocRM.GetString("UserLocation"), "Location", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort,
				new QFieldJoinRelation(this.OwnerTable, "USER_DETAILS", "PrincipalId", "UserId"), true));

			//UserActivity
			this.Fields.Add(new QField("UserActivity", LocRM.GetString("UserActivity"),
				"(CASE {0}.Activity WHEN 1 THEN N'" + LocRM.GetString("UserActivityInactive") + "' WHEN 2 THEN N'" + LocRM.GetString("UserActivityPending") + "' WHEN 3 THEN N'" + LocRM.GetString("UserActivityActive") + "' END)",
				DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter | QFieldUsingType.Sort));

			this.Fields.Add(new QField("UserActivityId", "UserActivityId", "Activity", DbType.Int32, QFieldUsingType.Abstract));

			this.Dictionary.Add(new QDictionary(this.Fields["UserActivityId"], this.Fields["UserActivity"],
				"SELECT 1 As Id, N'" + LocRM.GetString("UserActivityInactive") + "' As Value" +
				" UNION " +
				"SELECT 2 As Id, N'" + LocRM.GetString("UserActivityPending") + "' As Value" +
				" UNION " +
				"SELECT 3 As Id, N'" + LocRM.GetString("UserActivityActive") + "' As Value"
				));


			//All secure groups
			this.Fields.Add(new QField("UserSecureGroups", LocRM.GetString("UserSecureGroups"), "GroupName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"dbo.[USER_GROUP]","PrincipalId","UserId"),
					new QFieldJoinRelation("dbo.[USER_GROUP]","dbo.[GROUPS]","GroupId","PrincipalId")
				}));

			this.Fields.Add(new QField("UserSecureGroupsId", "UserSecureGroupsId", "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"dbo.[USER_GROUP]","PrincipalId","UserId"),
					new QFieldJoinRelation("dbo.[USER_GROUP]","dbo.[GROUPS]","GroupId","PrincipalId")
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["UserSecureGroupsId"], this.Fields["UserSecureGroups"],
				"SELECT PrincipalId As Id, GroupName As Value FROM GROUPS"));

			//Only custom Groups
			this.Fields.Add(new QField("UserCustomGroups", LocRM.GetString("UserCustomGroups"), "GroupName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"USER_GROUP","PrincipalId","UserId"),
					new QFieldJoinRelation("USER_GROUP","GROUPS","GroupId","PrincipalId", 
						new SimpleFilterCondition(new QField("PrincipalId"),"9",SimpleFilterType.Great))
				}));

			this.Fields.Add(new QField("UserCustomGroupsId", "UserCustomGroupsId", "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"USER_GROUP","PrincipalId","UserId"),
					new QFieldJoinRelation("USER_GROUP","GROUPS","GroupId","PrincipalId", 
						new SimpleFilterCondition(new QField("PrincipalId"),"9",SimpleFilterType.Great))
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["UserCustomGroupsId"], this.Fields["UserCustomGroups"],
					"SELECT PrincipalId As Id, GroupName As Value FROM GROUPS WHERE PrincipalId > 9"));


			//Only Roles
			this.Fields.Add(new QField("UserRoles", LocRM.GetString("UserRoles"), "GroupName", DbType.String, QFieldUsingType.Field | QFieldUsingType.Grouping | QFieldUsingType.Filter,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"[USER_GROUP]","PrincipalId","UserId"),
					new QFieldJoinRelation("[USER_GROUP]","[GROUPS]","GroupId","PrincipalId", 
						new SimpleFilterCondition(new QField("PrincipalId"),"((SELECT PrincipalID FROM GROUPS WHERE PrincipalId < 8 AND PrincipalId != 1 AND PrincipalId != 6))",SimpleFilterType.In))
				}));

			this.Fields.Add(new QField("UserRolesId", "UserRolesId", "PrincipalId", DbType.Int32, QFieldUsingType.Abstract,
				new QFieldJoinRelation[]
				{
					new QFieldJoinRelation(this.OwnerTable,"[USER_GROUP]","PrincipalId","UserId"),
					new QFieldJoinRelation("[USER_GROUP]","[GROUPS]","GroupId","PrincipalId", 
						new SimpleFilterCondition(new QField("PrincipalId"),"((SELECT PrincipalID FROM GROUPS WHERE PrincipalId < 8 AND PrincipalId != 1 AND PrincipalId != 6))",SimpleFilterType.In))
				}));

			this.Dictionary.Add(new QDictionary(this.Fields["UserRolesId"], this.Fields["UserRoles"],
					"SELECT PrincipalId As Id, GroupName As Value FROM GROUPS WHERE PrincipalId < 8 AND PrincipalId != 1 AND PrincipalId != 6"));


			QMetaLoader.LoadMetaField(this, "UsersEx");
			//Im Groups
		}
	}
}
